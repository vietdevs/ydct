using System.Data.SqlClient;
using Shouldly;
using Xunit;

namespace newPSG.PMS.Tests.General
{
    public class ConnectionString_Tests
    {
        [Fact]
        public void SqlConnectionStringBuilder_Test()
        {
            var csb = new SqlConnectionStringBuilder("Server=localhost; Database=PMS; Trusted_Connection=True;");
            csb["Database"].ShouldBe("PMS");
        }
    }
}
