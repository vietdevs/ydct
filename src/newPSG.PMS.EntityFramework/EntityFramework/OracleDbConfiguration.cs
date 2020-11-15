using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.EntityFramework;
using System.Data.Entity;
using Oracle.ManagedDataAccess.Client;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Common;

namespace newPSG.PMS.EntityFramework
{
    public class OracleDbConfiguration : DbConfiguration
    {
        public OracleDbConfiguration()
        {
            SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
        }
    }

}