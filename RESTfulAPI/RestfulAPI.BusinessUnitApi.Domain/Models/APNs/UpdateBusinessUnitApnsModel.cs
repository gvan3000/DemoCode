using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.APNs
{
    public class UpdateBusinessUnitApnsModel
    {
        [JsonProperty("apns")]
        public List<APNRequestDetail> Apns { get; set; }
        [JsonProperty("default_apn")]
        public Guid? DefaultApn { get; set; }
    }
}
