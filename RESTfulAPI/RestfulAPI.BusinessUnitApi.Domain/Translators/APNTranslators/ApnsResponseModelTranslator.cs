using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    public class ApnsResponseModelTranslator : ITranslate<ApnDetailContract[], APNsResponseModel>
    {
        public APNsResponseModel Translate(ApnDetailContract[] input)
        {
            if (input == null)
            {
                return null;
            }

            var responseModel = new APNsResponseModel() { Apns = new List<APNResponseDetail>() };
            foreach (var apnDetail in input)
            {
                responseModel.Apns.Add(new APNResponseDetail() { Name = apnDetail.Name, Id = apnDetail.ApnSetDetailId });
                if (apnDetail.IsDefault)
                {
                    responseModel.DefaultApn = apnDetail.ApnSetDetailId;
                }
            }

            return responseModel;
        }
    }
}
