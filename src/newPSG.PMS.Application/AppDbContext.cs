using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("Default"){

        }
    }
}
