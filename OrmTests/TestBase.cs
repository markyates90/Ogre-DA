using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using OgreDA.DataAccess;

namespace OgreDA.Orm.Tests;

[TestClass]
public class TestBase
{
    protected const string sqlProvider = "System.Data.SqlClient";
    protected const string sqlCS = "Data Source=(local);Initial Catalog=TestDB;Persist Security Info=True;User ID=TestUser;Password=testusrPWD!";
   
    protected DBConnectionInfo TestDBConnection = new (sqlCS, sqlProvider);

    [AssemblyInitialize()]
    public static void InitializeTests(TestContext context)
    {
        DbProviderFactories.RegisterFactory(sqlProvider, typeof(System.Data.SqlClient.SqlClientFactory));
    }
}