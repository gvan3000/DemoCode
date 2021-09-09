namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn
{
    public interface IAvailableMsisdnFactory
    {
        IAvailableMsisdnStrategy GetStrategy(AvailableMsisdnProviderRequest request);
    }
}
