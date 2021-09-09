using OTAServices.Business.Entities.SimManagement;
using System.Collections.Generic;

namespace OTAServices.Business.Interfaces.Repository
{
    public interface ISimProfileSponsorRepository
    {
        List<SimProfileSponsor> GetSimProfileSponsorList(List<int> simProfilesIds);
   
 }
}
