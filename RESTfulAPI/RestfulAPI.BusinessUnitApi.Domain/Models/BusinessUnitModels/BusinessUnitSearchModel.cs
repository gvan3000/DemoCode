using Newtonsoft.Json;
using RestfulAPI.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels
{
    public class BusinessUnitSearchModel: IValidatableObject
    {
        #region Private Fields

        private string _hasSharedWallet;
        private string _end_user_subscription;

        #endregion

        /// <summary>
        /// Business unit name filter
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Customer Id filter
        /// </summary>
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Has shared wallet filter
        /// </summary>
        [JsonProperty("has_shared_wallet")]
        public string HasSharedWallet
        {
            get { return _hasSharedWallet; }
            set { _hasSharedWallet = value; IsHasSharedWalletSet = true; }
        }

        /// <summary>
        /// End user subscription filter
        /// </summary>
        [JsonProperty("end_user_subscription")]
        public string EndUserSubscription
        {
            get { return _end_user_subscription; }
            set { _end_user_subscription = value; IsEndUserSubscriptionSet = true; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool isNameSet = !string.IsNullOrEmpty(Name);
            bool isCustomerIdSet = !string.IsNullOrEmpty(CustomerId);

            int filterCount = FilterCounter(isNameSet, isCustomerIdSet, IsHasSharedWalletSet, IsEndUserSubscriptionSet);

            bool validationPassed = false;

            if (IsHasSharedWalletSet)
                validationPassed = IsBoolParamValid(HasSharedWallet) && filterCount == 1;

            if (IsEndUserSubscriptionSet)
                validationPassed = IsBoolParamValid(EndUserSubscription) && filterCount == 1;

            if (isNameSet || isCustomerIdSet)
                validationPassed = filterCount == 1;

            yield return validationPassed
                ? ValidationResult.Success
                : new ValidationResult("Search criteria is not valid. Valid examples: \"has_shared_wallet:<bool>\", \"end_user_subscription:<bool>\", \"name:<search busines unit name>\", \"customer_id:<search customer id>\".");
        }

        private int FilterCounter(params bool[] booleanFilters)
        {
            return booleanFilters.Count(b => b);
        }

        private bool IsBoolParamValid(string filterParam)
        {
            var isValid = TruthfulnessConstants.TruthfulnessValues.Any(t => t.ToLower() == filterParam.ToLower());
                        
            return isValid;
        }

        [JsonIgnore]
        public bool IsHasSharedWalletSet { get; private set; }

        [JsonIgnore]
        public bool IsEndUserSubscriptionSet { get; set; }
    }
}
