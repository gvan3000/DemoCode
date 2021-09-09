using OCSServices.Matrixx.Agent.Contracts.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IWallet
    {
        MultiRequest GetQueryWalletMultiRequest(GetWalletRequest request);
        WalletQueryRequest GetQueryWalletRequest(GetWalletRequest request);
        GroupWalletQueryRequest GetQueryGroupWalletRequest(GetGroupWalletRequest request);
    }
}
