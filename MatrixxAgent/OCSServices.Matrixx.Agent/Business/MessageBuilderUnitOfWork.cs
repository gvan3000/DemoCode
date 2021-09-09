using OCSServices.Matrixx.Agent.Business.Interfaces;
using System;

namespace OCSServices.Matrixx.Agent.Business
{
    public class MessageBuilderUnitOfWork : IMessageBuilderUnitOfWork, IDisposable
    {
        private Subscriber _subscriber;
        private Offer _offer;
        private Balance _balance;
        private Group _group;
        private Device _device;
        private Threshold _threshold;
        private Wallet _wallet;

        public IGroup Group => _group ?? (_group = new Group());
        public IOffer Offer => _offer ?? (_offer = new Offer());
        public IBalance Balance => _balance ?? (_balance = new Balance());
        public ISubscriber Subscriber => _subscriber ?? (_subscriber = new Subscriber());
        public IDevice Device => _device ?? (_device = new Device());

        public IThreshold Threshold => _threshold ?? (_threshold = new Threshold());

        public IWallet Wallet => _wallet ?? (_wallet = new Wallet());

        #region disposable
        bool _disposed;

        ~MessageBuilderUnitOfWork()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_subscriber != null)
                    _subscriber.Dispose();
                if (_balance != null)
                    _balance.Dispose();
                if (_group != null)
                    _group.Dispose();
                if (_offer != null)
                    _offer.Dispose();
                if (_threshold != null)
                    _threshold.Dispose();

                _device?.Dispose();
            }
            _disposed = true;
        }

        #endregion
    }
}
