using System;

namespace EasyZMq.Sockets
{
    public interface IMonitorConnection
    {
        event Action Connected;
        event Action Disconnected;
        event Action ConnectRetried;
    }
}
