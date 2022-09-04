using AutoFixture;
using AutoMapper;
using BoDi;
using CommentImitationProject.DAL;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;
using CommentImitationProject.Services;
using CommentImitationProject.Services.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace CommentImitationProject.Tests.Behavior.Steps;

[Binding]
public class GetCommentByIdSteps
{
    private Comment _successComment;
    private Guid _id;

    private ICommentService _commentService;
    private Mock<IUnitOfWork> mockUnitOfWork;
    private Mock<IMapper> mockMapper;

    private readonly IObjectContainer _container;

    public GetCommentByIdSteps(IObjectContainer container)
    {
        _container = container;
    }

    [BeforeScenario]
    public void CreateCommentService()
    {
        mockUnitOfWork = new Mock<IUnitOfWork>();
        mockMapper = new Mock<IMapper>();

        _commentService = new CommentService(mockUnitOfWork.Object, mockMapper.Object);

        _container.RegisterInstanceAs(_commentService);
    }

    [Given(@"Guid id is empty\.")]
    public void GivenGuidIdIsEmpty()
    {
        _id = Guid.Empty;
    }

    [Given(@"Guid id not empty\.")]
    public void GivenIdNotEmpty()
    {
        _id = Guid.NewGuid();
    }

    [Then(@"ArgumentException should be thrown\.")]
    public void ThenArgumentExceptionShouldBeThrown()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _commentService.GetById(_id));
    }

    [Given(@"Something wrong with database\.")]
    public void GivenSomethingWrongWithDatabaseConnection()
    {
        mockUnitOfWork.Setup(x => x.Comments.GetById(_id)).ThrowsAsync(new Exception());
    }

    [Then(@"Exception should be thrown\.")]
    public void ThenDatabaseExceptionShouldBeThrown()
    {
        Assert.ThrowsAsync<Exception>(() => _commentService.GetById(_id));
    }

    [Given(@"Comment with this id not exists\.")]
    public void GivenCommentWithThisIdNotExists()
    {
        mockUnitOfWork.Setup(x => x.Comments.GetById(_id)).ReturnsAsync((Comment)null!);
    }

    [Then(@"NullReferenceException should be thrown\.")]
    public void ThenArgumentNullExceptionShouldBeThrown()
    {
        Assert.ThrowsAsync<NullReferenceException>(() => _commentService.GetById(_id));
    }

    [Given(@"Comment with this id exists\.")]
    public void GivenCommentWithThisIdExists()
    {
        _successComment = new Fixture().Build<Comment>()
            .With(x => x.Author,
                new Fixture().Build<User>()
                    .Without(x => x.Comments)
                    .Without(x => x.Posts)
                    .Create())
            .With(x => x.Post,
                new Fixture().Build<Post>()
                    .Without(x => x.Author)
                    .Without(x => x.Comments)
                    .Create())
            .Create();
    }

    [Then(@"Should return mapped comment\.")]
    public async Task ThenShouldReturnMappedComment()
    {
        mockUnitOfWork.Setup(x => x.Comments.GetById(_id)).ReturnsAsync(_successComment);

        var expectedResult = new CommentDto
        {
            Author = _successComment.Author,
            Id = _successComment.Id,
            Post = _successComment.Post,
            Text = _successComment.Text,
            LastEditDate = _successComment.LastEditDate
        };

        mockMapper.Setup(x => x.Map<CommentDto>(_successComment))
            .Returns(expectedResult);

        var result = await _commentService.GetById(_id);

        mockUnitOfWork.Verify(x => x.Comments.GetById(_id), Times.Once);
        result.Should().BeOfType<CommentDto>();
        result.Should().Be(expectedResult);
    }
}