using OTAServices.Business.Entities.OTACampaignDeleteImsi;
using OTAServices.Business.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTAServices.DataCore.Repositories.OtaDb
{
    public class DeleteIMSICallbackRepository : IDeleteIMSICallbackRepository
    {
        #region [ Private fields ]

        private readonly OtaDbContext _dbContext;

        #endregion

        #region [ Constructor ]

        public DeleteIMSICallbackRepository(OtaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region [ IDeleteIMSICallbackRepository implementation ]
        public void Add(DeleteIMSICallback deleteIMSICallback)
        {
            _dbContext.AddEntity(deleteIMSICallback);
        }

        public DeleteIMSICallback GetByImsiAndOasisRequestId(string imsi, int oasisRequestId)
        {
            return _dbContext.DeleteIMSICallback.FirstOrDefault(x => x.IMSI == imsi && x.OasisRequestId == oasisRequestId);
        }

        public void Update(DeleteIMSICallback deleteIMSICallback)
        {
            _dbContext.UpdateEntity(deleteIMSICallback);
        }

        #endregion
    }
}
