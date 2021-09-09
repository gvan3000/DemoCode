using log4net;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace OTAServices.Bussines.Functions.IntegrationTests
{
    static class TestHelper
    {
        public static void InitializeEventFlowRepo()
        {
            ILoggerRepository efRepo = null;
            var repos = LogManager.GetAllRepositories();
            foreach (var repo in repos)
            {
                if (repo.Name.Equals("EventFlowRepo"))
                {
                    efRepo = repo;
                    break;
                }
            }

            if (efRepo == null)
            {
                efRepo = (Hierarchy)LogManager.CreateRepository("EventFlowRepo");
            }
        }
    }
}
