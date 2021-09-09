using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    public class BusinessUnitPatchModel
    {
        private string _name;
        private string _customerId;
        private Guid? _personId;
        private string _wholesalePriceplan;
        private Proposition[] _propositions;
        private Guid[] _addOnIds;

        [JsonProperty("name")]
        [MaxLength(50)]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                IsNameSet = true;
            }
        }

        [JsonIgnore]
        public bool IsNameSet { get; set; }

        [JsonProperty("customer_id")]
        public string CustomerId
        {
            get { return _customerId; }
            set
            {
                _customerId = value;
                IsCustomerIdSet = true;
            }
        }

        [JsonIgnore]
        public bool IsCustomerIdSet { get; set; }

        [JsonProperty("person_id")]
        public Guid? PersonId
        {
            get { return _personId; }
            set
            {
                _personId = value;
                IsPersonIdSet = true;
            }
        }

        [JsonIgnore]
        public bool IsPersonIdSet { get; set; }

        [JsonProperty("wholesale_priceplan")]
        public string WholesalePriceplan
        {
            get { return _wholesalePriceplan; }
            set
            {
                _wholesalePriceplan = value;
                IsWholesalePriceplanSet = true;
            }
        }

        [JsonIgnore]
        public bool IsWholesalePriceplanSet { get; set; }

        [JsonIgnore]
        public bool IsPropositionsSet { get; set; }

        [JsonProperty("propositions")]
        public Proposition[] Propositions
        {
            get { return _propositions; }
            set
            {
                _propositions = value;
                IsPropositionsSet = true;
            }
        }

        [JsonIgnore]
        public bool IsAddOnIdsSet { get; set; }

        [JsonProperty("add_ons")]
        public Guid[] AddOnIds
        {
            get { return _addOnIds; }
            set
            {
                _addOnIds = value;
                IsAddOnIdsSet = true;
            }
        }

        [JsonProperty("contract_number")]
        public string ContractNumber { get; set; }
    }
}
