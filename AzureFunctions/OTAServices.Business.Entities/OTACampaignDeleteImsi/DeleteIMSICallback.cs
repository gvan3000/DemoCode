using System;

namespace OTAServices.Business.Entities.OTACampaignDeleteImsi
{
    public class DeleteIMSICallback
    {
        public int Id { get; set; }
        public string IMSI { get; set; }
        public int OasisRequestId { get; set; }
        public string Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? ModifiedBy { get; set; }



}
}
