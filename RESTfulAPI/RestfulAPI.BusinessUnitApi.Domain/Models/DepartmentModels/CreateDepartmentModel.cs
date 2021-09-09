using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels
{
    /// <summary>
    /// Create Department Model
    /// </summary>
    public class CreateDepartmentModel
    {
        /// <summary>
        /// Name of the department
        /// </summary>
        [JsonProperty("name")]
        [Required]
        [StringLength(50)]

        public string Name { get; set; }

        /// <summary>
        /// Cost centre id of the department
        /// </summary>
        [JsonProperty("cost_center")]
        [StringLength(50)]
        public string CostCenter { get; set; }
    }
}
