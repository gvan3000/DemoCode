using System;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public class BaseProxy : IDisposable
    {
        #region disposable
        protected bool _disposed;

        ~BaseProxy()
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

            _disposed = true;
        }
        #endregion    
    }
}