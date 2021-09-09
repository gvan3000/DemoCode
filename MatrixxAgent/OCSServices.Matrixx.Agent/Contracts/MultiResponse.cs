using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts
{
    public class MultiResponse : BasicResponse
    {
        public List<BasicResponse> RepsonseList
        {
            get;
            set;
        } 
    }
}