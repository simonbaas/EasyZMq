using System;

namespace EasyZMq.Sockets.Intermediary
{
    public interface IIntermediarySocket : IStartableSocket, IDisposable
    {
        Uri FrontendUri { get; }
        Uri BackendUri { get; }
    }
}
