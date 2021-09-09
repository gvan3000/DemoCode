using System;
using OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies;

namespace OCSServices.Matrixx.Api.Client.ApiClient
{
    public interface IClient : IDisposable
    {
        IDeviceProxy DeviceProxy { get; }

        ISubscriberProxy SubscriberProxy { get; }

        IMultiProxy MultiProxy { get; }

        IBalanceProxy BalanceProxy { get; }

        IGroupProxy GroupProxy { get; }

        IOfferProxy OfferProxy { get; }

        IWalletProxy WalletProxy { get; }
    }

    public class Client : IClient
    {
        bool _disposed;

        ~Client()
        {
            Dispose(false);
        }

        private IDeviceProxy _deviceProxy;
        public IDeviceProxy DeviceProxy => _deviceProxy ?? (_deviceProxy = new DeviceProxy());

        private ISubscriberProxy _subscriberProxy;
        public ISubscriberProxy SubscriberProxy => _subscriberProxy ?? (_subscriberProxy = new SubscriberProxy());

        private IMultiProxy _multiProxy;
        public IMultiProxy MultiProxy => _multiProxy ?? (_multiProxy = new MultiProxy());

        private IBalanceProxy _balanceProxy;
        public IBalanceProxy BalanceProxy => _balanceProxy ?? (_balanceProxy = new BalanceProxy());

        private IGroupProxy _groupProxy;
        public IGroupProxy GroupProxy => _groupProxy ?? (_groupProxy = new GroupProxy());

        private IOfferProxy _offerProxy;
        public IOfferProxy OfferProxy => _offerProxy ?? (_offerProxy = new OfferProxy());

        private IWalletProxy _walletProxy;
        public IWalletProxy WalletProxy => _walletProxy ?? (_walletProxy = new WalletProxy());

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
                _deviceProxy?.Dispose();
                _subscriberProxy?.Dispose();
                _groupProxy?.Dispose();
                _balanceProxy?.Dispose();
                _multiProxy?.Dispose();
                _offerProxy?.Dispose();
                _walletProxy?.Dispose();
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
    }
}
