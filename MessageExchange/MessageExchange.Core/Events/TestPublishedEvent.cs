
namespace MessageExchange.Core.Events
{
    public class TestPublishedEvent
    {
        public readonly string TestField;

        public TestPublishedEvent(string testField)
        {
            TestField = testField;
        }
    }
}
