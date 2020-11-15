using newPSG.PMS.EntityFramework;

namespace newPSG.PMS.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly PMSDbContext _context;

        public InitialHostDbBuilder(PMSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
