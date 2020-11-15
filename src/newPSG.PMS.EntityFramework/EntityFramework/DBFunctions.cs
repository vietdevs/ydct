using EntityFramework.Functions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS
{
    public static class DBFunctions
    {
        [Function(FunctionType.ComposableScalarValuedFunction, "f_LocDauLowerCaseDB", Schema = "dbo")]
        [return: Parameter(DbType = "nvarchar")]
        public static string LocDauLowerCaseDB([Parameter(DbType = "nvarchar")] this string value) => Function.CallNotSupported<string>();
    }
}
