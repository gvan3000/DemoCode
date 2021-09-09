using Newtonsoft.Json;
using RestfulAPI.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels
{
    public class AvailableMSISDNSearchModel : IValidatableObject
    {
        #region Private Fields

        private string _status;

        #endregion

        /// <summary>
        /// Msisdns status filter; example: q=status:available
        /// </summary>
        [JsonProperty("status")]
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool isStatusSet = !string.IsNullOrEmpty(Status);

            bool validationPassed = !isStatusSet || IsStatusValid(Status);

            yield return validationPassed
                ? ValidationResult.Success
                : new ValidationResult($"Query expression is not valid. Only valid status is {MobileStatusConstants.Available}.");
        }

        private bool IsStatusValid(string status)
        {
            return status.Equals(Constants.MobileStatusConstants.Available, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
