using System;

namespace EasyZMq.Infrastructure
{
    internal interface IMonitorConnection
    {
        event Action Connected;
        event Action Disconnected;
        event Action ConnectRetried;
    }
}
