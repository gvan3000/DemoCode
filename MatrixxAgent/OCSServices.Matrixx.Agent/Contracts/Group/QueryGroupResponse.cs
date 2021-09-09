using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Group
{
    public class QueryGroupResponse
    {
        public string CustomReference { get; set; }
        public string Name { get; set; }
        public string ObjectId { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public int SubscriberCount { get; set; }
        public int AdminCount { get; set; }
        public int AdminCursor { get; set; }
        public int SubscriberMemberCount { get; set; }
        public long SubscriberMemberCursor { get; set; }
        public int GroupMemberCount { get; set; }
        public int GroupMemberCursor { get; set; }
        public int NotificationPreference { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

    }
}
