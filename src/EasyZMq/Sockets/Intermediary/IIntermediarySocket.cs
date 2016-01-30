using System;

namespace EasyZMq.Sockets.Intermediary
{
    public interface IIntermediarySocket : IDisposable
    {
        Uri FrontendUri { get; }
        Uri BackendUri { get; }
        void Start();
    }
}
