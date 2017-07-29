using System;
using EasyZMq.Infrastructure;
using NetMQ;
using NetMQ.Sockets;

namespace EasyZMq.Sockets.Intermediary
{
    internal class IntermediarySocket : IIntermediarySocket
    {
        private readonly IAddressBinder _frontendAddressBinder;
        private readonly IAddressBinder _backendAddressBinder;
        private XSubscriberSocket _frontendSocket;
        private XPublisherSocket _backendSocket;
        private NetMQPoller _poller;
        private Proxy _proxy;

        public IntermediarySocket(IAddressBinder frontendAddressBinder, IAddressBinder backendAddressBinder,
            XSubscriberSocket frontendSocket, XPublisherSocket backendSocket)
        {
            _frontendAddressBinder = frontendAddressBinder;
            _backendAddressBinder = backendAddressBinder;
            _frontendSocket = frontendSocket;
            _backendSocket = backendSocket;

            _frontendAddressBinder.ConnectOrBindAddress(_frontendSocket);
            _backendAddressBinder.ConnectOrBindAddress(_backendSocket);

            _poller = new NetMQPoller { _frontendSocket, _backendSocket };
            _proxy = new Proxy(frontendSocket, backendSocket, poller: _poller);
        }

        public Uri FrontendUri => _frontendAddressBinder.Uri;
        public Uri BackendUri => _backendAddressBinder.Uri;

        public void Start()
        {
            if (_disposedValue) throw new ObjectDisposedException("IntermediarySocket");

            _poller.Run();
            _proxy.Start();
        }

        private bool _disposedValue;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _proxy.Stop();
                    _proxy = null;

                    _poller.Stop();
                    _poller.Dispose();
                    _poller = null;

                    _backendSocket.Dispose();
                    _backendSocket = null;

                    _frontendSocket.Dispose();
                    _frontendSocket = null;

                    NetMQConfig.Cleanup();
                }

                _disposedValue = true;
            }
        }
    }
}