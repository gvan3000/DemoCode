using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    public class APNsResponseModel
    {
        [JsonProperty("apns")]
        public List<APNResponseDetail> Apns { get; set; }

        [JsonProperty("default_apn")]
        public Guid? DefaultApn { get; set; }
    }
}
