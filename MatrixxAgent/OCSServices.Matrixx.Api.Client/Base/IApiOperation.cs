using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;

namespace OCSServices.Matrixx.Api.Client.Base
{
    internal interface IApiOperation { }
    internal interface IGetApiOperation<TResult> : IApiOperation where TResult : new()
    {
        Task<TResult> Execute(IQueryParameters parameters);
    }

    internal interface IPostApiOperation<TPayload, TResult> : IApiOperation 
        where TPayload : new()
        where TResult : new()
    {
        Task<TResult> Execute(TPayload payLoad);
    }
}
