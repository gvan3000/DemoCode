using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public class AddOnContractTranslator : IAddOnContractTranslator
    {
        public AddOnListModel Translate(AddOnsContract input, List<BusinessUnitAddOnMatrixxResourceContract> resourceContracts)
        {
            if (input == null)
            {
                return null;
            }

            AddOnListModel output = new AddOnListModel();
            output.AddOns = new List<AddOnModel>();
            AddOnModel addOn = null;
            foreach (var item in input.AddOnContracts)
            {
                if (item.Definitions == null)
                {
                    continue;
                }

                foreach (var definition in item.Definitions)
                {
                    addOn = new AddOnModel()
                    {
                        Id = item.Id,
                        Type = item.AddOnType.Type,
                        StartDate = item.PurchaseDate,
                        EndDate = EndDateTranlate(item),
                        Amount = definition?.BundleAmount,
                        ResourceId = resourceContracts.FirstOrDefault(x => x.AddOnDefinitionId == definition.Id)?.ResourceId
                    };

                    output.AddOns.Add(addOn);
                }

            }

            return output;
        }

        private DateTime? EndDateTranlate(AddOnContract input)
        {
            if (input.AddOnType.Type != "CASH" && input.EndDate.Type == "EXACT")
            {
                return input.EndDate.ExactDateTime;
            }

            if (input.AddOnType.Type == "CASH")
            {
                return input.PurchaseDate.GetValueOrDefault()
                    .AddDays(input.Duration.Days.GetValueOrDefault())
                    .AddYears(input.Duration.Years.GetValueOrDefault())
                    .AddDays(input.Duration.Weeks.GetValueOrDefault() * 7)
                    .AddMonths(input.Duration.Months.GetValueOrDefault())
                    .AddHours(input.Duration.Hours.GetValueOrDefault());
            }

            if (input.AddOnType.Type != "CASH" && input.EndDate.Type == "DURATION")
            {
                return input.EndDate.ExactDateTime.Value
                    .AddDays(input.EndDate.Days.GetValueOrDefault())
                    .AddYears(input.EndDate.Years.GetValueOrDefault())
                    .AddDays(input.EndDate.Weeks.GetValueOrDefault() * 7)
                    .AddMonths(input.EndDate.Months.GetValueOrDefault())
                    .AddHours(input.EndDate.Hours.GetValueOrDefault());
            }

            return null;
        }
    }
}



