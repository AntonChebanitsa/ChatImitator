using TechTalk.SpecFlow;

namespace CommentImitationProject.Tests.Behavior.Steps;

[Binding]
public class GetCommentByIdSteps
{
    [Given(@"Guid id is empty\.")]
    public void GivenGuidIdIsEmpty()
    {
        ScenarioContext.StepIsPending();
    }

    [Then(@"ArgumentException should be thrown\.")]
    public void ThenArgumentExceptionShouldBeThrown()
    {
        ScenarioContext.StepIsPending();
    }
}