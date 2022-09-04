using AutoFixture;
using AutoMapper;
using CommentImitationProject.DAL;
using Moq;

namespace CommentImitationProject.UnitTests.Services
{
    public class ServicesTestFixture
    {
        protected readonly Mock<IUnitOfWork> MockUnitOfWork;
        protected readonly Mock<IMapper> MockMapper;
        protected readonly Fixture Fixture;

        protected ServicesTestFixture()
        {
            MockMapper = new Mock<IMapper>();
            MockUnitOfWork = new Mock<IUnitOfWork>();
            Fixture = new Fixture();
        }
    }
}