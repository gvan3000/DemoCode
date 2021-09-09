using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OTAServices.Business.Entities.LeaseRequest;

namespace OTAServices.DataCore.Repositories.ProvisioningDb.Configurations
{
    public class SubscriberListLeaseRequestConfiguration : IEntityTypeConfiguration<SubscriberListLeaseRequest>
    {
        public void Configure(EntityTypeBuilder<SubscriberListLeaseRequest> builder)
        {
            builder.ToTable("LeaseRequest", "ota");
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.SubscriberListId).HasColumnName("SubscriberListId");
            builder.Property(x => x.CampaignId).HasColumnName("CampaignId");
            builder.Property(x => x.LeaseRequests).HasColumnName("LeaseRequests");
        }
    }
}
