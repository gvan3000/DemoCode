using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.MobileService;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators
{
    public class MsisdnContractTranslator : ITranslate<MsisdnContract[], AvailableMSISDNResponseModel>
    {
        public AvailableMSISDNResponseModel Translate(MsisdnContract[] input)
        {
            if (input == null || input.Length < 1)
                return null;

            AvailableMSISDNResponseModel output = new AvailableMSISDNResponseModel();
            output.Msisdns = new List<MsisdnModel>();

            foreach (var item in input)
            {
                var msisdn = Translate(item);
                output.Msisdns.Add(msisdn);
            }

            output.Paging = new PagingModel()
            {
                TotalResults = input.FirstOrDefault() != null ? input.FirstOrDefault().TotalResults : 0
            };

            return output;
        }

        private MsisdnModel Translate(MsisdnContract input)
        {
            if (input == null)
                return null;

            MsisdnModel output = new MsisdnModel()
            {
                Country = string.IsNullOrEmpty(input.CountryISO2Code) ? input.CountryName : input.CountryISO2Code,
                Msisdn = input.Msisdn,
                Status = input.NumberStatusCode,
                ProductId = input.ProductId,
                IsVirtual = input.IsVirtual
            };

            return output;
        }
    }
}
