using NetMQ;
using NUnit.Framework;

namespace EasyZMq.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeTearDown]
        public void TearDown()
        {
            NetMQConfig.Cleanup(false);
        }
    }
}
