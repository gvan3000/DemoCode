using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;
using api = OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Wallet : BaseMessageBuilder, IWallet
    {
        public MultiRequest GetQueryWalletMultiRequest(GetWalletRequest request)
        {
            var result = new MultiRequest
            {
                RequestCollection = new RequestCollection
                {
                    Values = new List<MatrixxObject>
                    {
                        new api.WalletQueryRequest()
                        {
                            SearchData = new SubscriberSearchData
                            {
                                SearchCollection = new SearchCollection
                                {
                                    //ExternalId = request.ProductId.ToString().ToUpper()
                                    AccessNumber = request.MsIsdn
                                }
                            }
                        }
                    }
                }
            };

            return result;
        }

        public WalletQueryRequest GetQueryWalletRequest(GetWalletRequest request)
        {
            var result = new api.WalletQueryRequest()
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        AccessNumber = request.MsIsdn
                    }
                }
            };
            return result;
        }

        public GroupWalletQueryRequest GetQueryGroupWalletRequest(GetGroupWalletRequest request)
        {
            var result = new api.GroupWalletQueryRequest()
            {
                SearchData = new GroupSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.ExternalId
                    }
                }
            };
            return result;
        }
    }
}
