using RestfulAPI.TeleenaServiceReferences.AddressService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal
{
    public class LoadAddressData
    {
        public List<AddressContract> Addresses { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }
}
