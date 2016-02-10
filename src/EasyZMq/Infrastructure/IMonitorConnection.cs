using System;

namespace EasyZMq.Infrastructure
{
    public interface IMonitorConnection
    {
        event Action Connected;
        event Action Disconnected;
        event Action ConnectRetried;
    }
}
