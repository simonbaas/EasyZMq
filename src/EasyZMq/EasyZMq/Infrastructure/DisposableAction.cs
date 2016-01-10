using System;

namespace EasyZMq.Infrastructure
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        public DisposableAction(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            _action = action;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _action();
            }
        }
    }
}
