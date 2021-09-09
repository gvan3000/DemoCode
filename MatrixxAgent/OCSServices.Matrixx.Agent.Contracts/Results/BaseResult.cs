using OCSServices.Matrixx.Agent.Business;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class BaseResult
    {
        public int Result { get; set; }

        public string ResultText { get; set; }

        public string ActionName { get; set; }

        public BaseResult()
        {
            History = new List<SendReceivedHistory>();
        }

        public bool IsSuccess
        {
            get
            {
                return new Result(Result).IsSuccess;                
            }
        }

        public List<SendReceivedHistory> History { get; set; }
    }
}