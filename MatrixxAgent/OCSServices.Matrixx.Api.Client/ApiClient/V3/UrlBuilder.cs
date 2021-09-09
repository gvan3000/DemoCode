using System;
using System.Linq;
using System.Text.RegularExpressions;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3
{
    public class UrlBuilder : IDisposable
    {
        private readonly string _baseAddress;
        private readonly string _version;
        public UrlBuilder(string baseAddress, string version)
        {
            _baseAddress = baseAddress;
            _version = version;
        }

        public string BuildGetUrl(IQueryParameters queryParameters)
        {
            return BuildParameterizedUrl(queryParameters);
        }

        public string BuildPutUrl(IQueryParameters queryParameters)
        {
            return BuildParameterizedUrl(queryParameters);
        }
        public string BuildDeleteUrl(IQueryParameters queryParameters)
        {
            return BuildParameterizedUrl(queryParameters);
        }

        public string BuildPostUrl<T>()
        {
            var typeOfQueryParameters = typeof(T);
            var apiMethodInfo = (ApiMethodInfoAttribute)Attribute.GetCustomAttribute(typeOfQueryParameters, typeof(ApiMethodInfoAttribute));
            if (apiMethodInfo == null)
            {
                throw new Exception("Could not build the url for the API call.");
            }

            return FormatUrl(apiMethodInfo.UrlTemplate);
        }

        private string BuildParameterizedUrl(IQueryParameters queryParameters)
        {
            var typeOfQueryParameters = queryParameters.GetType();
            var apiMethodInfo = (ApiMethodInfoAttribute)Attribute.GetCustomAttribute(typeOfQueryParameters, typeof(ApiMethodInfoAttribute));
            if (apiMethodInfo == null)
            {
                throw new Exception("Could not build the url for the API call.");
            }


            return FormatUrl(apiMethodInfo.UrlTemplate, queryParameters, typeOfQueryParameters);
        }

        private string FormatUrl(string urlTemplate, IQueryParameters queryParameters, Type typeOfQueryParameters)
        {
            var properties =
                from prop in typeOfQueryParameters.GetProperties()
                from att in prop.GetCustomAttributes(typeof(UrlTemplateParameterAttribute), true)
                where prop.GetValue(queryParameters) != null
                select new
                {
                    Key = ((UrlTemplateParameterAttribute)att).Name,
                    Value = ""+prop.GetValue(queryParameters)
                };

            return FormatUrl(properties.Aggregate(urlTemplate, (current, property) => Regex.Replace(current, $"#{property.Key}#", property.Value)));
        }

        private string FormatUrl(string urlTemplate)
        {
            return $"{EnsureTrailingSlash(_baseAddress)}{EnsureTrailingSlash(_version)}{RemoveLeadingSlash(urlTemplate)}";
        }

        private static string RemoveLeadingSlash(string value)
        {
            return value.First() == '/' ? value.Remove(0, 1) : value;
        }

        private static string EnsureTrailingSlash(string value)
        {
            return value.Last() != '/' ? $"{value}/" : value;
        }
        #region disposable
        bool _disposed;

        ~UrlBuilder()
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