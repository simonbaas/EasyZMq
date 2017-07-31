using System;
using NetMQ;

namespace EasyZMq.Tests
{
    public class TestsFixture : IDisposable
    {
        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
        }
    }
}
