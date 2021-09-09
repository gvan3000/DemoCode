using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using Polly;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.ApiClient.Base
{
    public abstract class BaseApiOperation : IDisposable
    {
        private ILog _logger;
        protected ILog Logger => _logger ?? (_logger = LogManager.GetLogger(GetType()));

        private string _version;
        protected string Version
        {
            get
            {
                if (!string.IsNullOrEmpty(_version)) return _version;
                var type = GetType();
                var attribute = Attribute.GetCustomAttribute(type, typeof(ApiVersionAttribute));
                if (attribute != null)
                {
                    _version = ((ApiVersionAttribute)attribute).Version.ToString();
                }
                else
                {
                    var error = $"ApiVersion not found for {type.Name}";
                    Logger.Error(error);
                    throw new Exception(error);
                }
                return _version;
            }
        }

        private static MatrixxXmlSerializer _serializer;
        protected MatrixxXmlSerializer Serializer => _serializer ?? (_serializer = new MatrixxXmlSerializer());

        protected string BaseUrl { get; set; }

        private readonly UrlBuilder _urlBuilder;
        private static HttpClient _httpClient;

        private readonly Policy _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) });

        [Obsolete]
        protected BaseApiOperation(string baseUrlKey)
        {
            BaseUrl = ConfigurationManager.AppSettings[baseUrlKey];
            _urlBuilder = new UrlBuilder(BaseUrl, Version);
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _httpClient.Timeout = TimeSpan.FromMinutes(1);
        }
        /// <summary>
        /// Static constructor for initialization of HttpClient And MatrixxXmlSerializer
        /// </summary>
        static BaseApiOperation()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _httpClient.Timeout = TimeSpan.FromMinutes(1);
            _serializer = new MatrixxXmlSerializer();
        }
        /// <summary>
        /// Parameterized constructor recieves endpoint and on the basis of it sets urlbuilder
        /// </summary>
        /// <param name="endpoint"></param>
        protected BaseApiOperation(Endpoint endpoint)
        {
            var version = endpoint.EndpointAttributes.Where(x => x.EndpointAttributeTypeId == (int)EndpointAttributeType.Version).FirstOrDefault().Value;
            _urlBuilder = new UrlBuilder(endpoint.EndpointUrl, version);                       
        }

        public async Task<TResult> Get<TResult>(IQueryParameters queryParameters) where TResult : new()
        {
            var result = new TResult();
            var response = new HttpResponseMessage();
            var httpResponse = string.Empty;
            var url = _urlBuilder.BuildGetUrl(queryParameters);
            Logger.Debug($"GET {url}");

            await _retryPolicy.ExecuteAsync(async () =>
            {
                response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            });
            if (response.IsSuccessStatusCode)
            {
                Logger.Debug($"HTTP Response: {response}");
                httpResponse = await response.Content.ReadAsStringAsync();
                result = Serializer.Deserialize<TResult>(httpResponse);
            }
            else
            {
                Logger.Error($"Error sending {nameof(Get)} to Matrixx: Response = {httpResponse}");
            }
            return result;
        }

        public async Task<string> Put<TInput>(IQueryParameters queryParameters, TInput input) where TInput : new()
        {
            var response = new HttpResponseMessage();
            var httpResponse = string.Empty;
            var url = _urlBuilder.BuildPutUrl(queryParameters);
            var payload = Serializer.Serialize(input);
            Logger.Debug($"PUT {url}");
            Logger.Debug($"Payload: {payload}");

            await _retryPolicy.ExecuteAsync(async () =>
            {
                response = await _httpClient.PutAsync(url, new StringContent(payload, Encoding.UTF8, "application/xml")).ConfigureAwait(false);
            });
            if (response.IsSuccessStatusCode)
            {
                httpResponse = await response.Content.ReadAsStringAsync();
                Logger.Debug($"HTTP Response: {httpResponse}");
            }
            else
            {
                Logger.Error($"Error sending {nameof(Put)} to Matrixx: Response = {httpResponse}");
            }
            return httpResponse;
        }

        public async Task<TResult> Delete<TResult>(IQueryParameters queryParameters) where TResult : new()
        {
            var result = new TResult();
            var response = new HttpResponseMessage();
            var httpResponse = string.Empty;
            var url = _urlBuilder.BuildDeleteUrl(queryParameters);
            Logger.Debug($"DELETE {url}");

            await _retryPolicy.ExecuteAsync(async () =>
            {
                response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
            });
            if (response.IsSuccessStatusCode)
            {
                httpResponse = await response.Content.ReadAsStringAsync();
                Logger.Debug($"HTTP Response: {httpResponse}");
                result = Serializer.Deserialize<TResult>(httpResponse);
            }
            else
            {
                Logger.Error($"Error sending {nameof(Delete)} to Matrixx: Response = {httpResponse}");
            }
            return result;
        }

        public async Task<TResult> Post<TInput, TResult>(TInput input) where TInput : new() where TResult : new()
        {
            var result = new TResult();
            var response = new HttpResponseMessage();
            var httpResponse = string.Empty;
            var url = _urlBuilder.BuildPostUrl<TInput>();
            var payload = Serializer.Serialize(input);
            Logger.Debug($"POST {url}");
            Logger.Debug($"Payload: {payload}");

            await _retryPolicy.ExecuteAsync(async () =>
            {
                response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/xml")).ConfigureAwait(false);
            });
            if (response.IsSuccessStatusCode)
            {
                httpResponse = await response.Content.ReadAsStringAsync();
                Logger.Debug($"HTTP Response: {httpResponse}");
                result = Serializer.Deserialize<TResult>(httpResponse);
            }
            else
            {
                Logger.Error($"Error sending {nameof(Post)} to Matrixx: Response = {httpResponse}");
            }
            return result;
        }

        #region IDisposable
        private bool _disposed;

        ~BaseApiOperation()
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
                _urlBuilder?.Dispose();
            }
            _disposed = true;
        }
        #endregion
    }
}