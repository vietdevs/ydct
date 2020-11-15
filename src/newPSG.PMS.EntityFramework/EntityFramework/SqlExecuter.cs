using Abp.Dependency;
using Abp.EntityFramework;
using newPSG.PMS.Data;
using newPSG.PMS.EntityFramework;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityFramework
{
    public class SqlExecuter : ISqlExecuter, ITransientDependency
    {
        private readonly IDbContextProvider<PMSDbContext> _dbContextProvider;
        public SqlExecuter(IDbContextProvider<PMSDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;            
        }

        public int Execute(string query, params object[] parameters)
        {
            return _dbContextProvider.GetDbContext().Database.ExecuteSqlCommand(query, parameters);
        }

        public List<K> Query<K>(string query, params object[] parameters)
        {
                return _dbContextProvider.GetDbContext().Database.SqlQuery<K>(query, parameters).ToList();
        }

        public K QueryFirst<K>(string query, params object[] parameters)
        {
            return _dbContextProvider.GetDbContext().Database.SqlQuery<K>(query, parameters).FirstOrDefault();
        }

        public Task<List<K>> QueryAsync<K>(string query, params object[] parameters)
        {
            return _dbContextProvider.GetDbContext().Database.SqlQuery<K>(query, parameters).ToListAsync();
        }

        public Task<K> QueryFirstAsync<K>(string query, params object[] parameters)
        {
            return _dbContextProvider.GetDbContext().Database.SqlQuery<K>(query, parameters).FirstOrDefaultAsync();
        }

        public K GetNextSeqVal<K>(string sequenceName)
        {
            string query = $"SELECT {sequenceName}.NEXTVAL FROM DUAL";
            return QueryFirst<K>(query, new OracleParameter[] { });
        }
    }
}
