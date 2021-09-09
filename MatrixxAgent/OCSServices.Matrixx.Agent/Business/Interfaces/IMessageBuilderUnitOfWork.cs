using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IMessageBuilderUnitOfWork
    {
        ISubscriber Subscriber { get; }
        IOffer Offer { get; }
        IBalance Balance { get; }
        IGroup Group { get; }
        IDevice Device { get; }
        IThreshold Threshold { get; }
        IWallet Wallet { get; }

        void Dispose();
    }
}
