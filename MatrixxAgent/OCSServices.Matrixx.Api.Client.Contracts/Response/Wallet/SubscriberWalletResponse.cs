using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Wallet;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Wallet
{
    [MatrixxContract(Name = "MtxResponseWallet")]
    public class SubscriberWalletResponse : MatrixxObject
    {
        [MatrixxContractMember(Name = "BalanceArray")]
        public WalletInfoCollection WalletInfoList { get; set; }

        [MatrixxContractMember(Name = "BalanceArray")]
        public WalletInfoPeriodicCollection WalletInfoPeriodicList { get; set; }

        [MatrixxContractMember(Name = "Result")]
        public int? Result { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }

        [MatrixxContractMember(Name = "BillingCycle")]
        public WalletInfoBillingCycleCollection WalletInfoBillingCycleList { get; set; }

    }
}