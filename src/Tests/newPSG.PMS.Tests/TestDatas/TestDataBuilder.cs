using EntityFramework.DynamicFilters;
using newPSG.PMS.EntityFramework;

namespace newPSG.PMS.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly PMSDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(PMSDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();

            _context.SaveChanges();
        }
    }
}
