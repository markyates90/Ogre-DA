using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Tests;

[TestClass]
public class TestBase
{
    protected const string sqlProvider = "System.Data.SqlClient";
    protected const string oleDbProvider = "System.Data.OleDb";
    protected const string sqlCS = "Data Source=(local);Initial Catalog=TestDB;Persist Security Info=True;User ID=TestUser;Password=testusrPWD!";
    protected const string oleDbCS = "Provider=SQLOLEDB.1;Data Source=(local);Initial Catalog=TestDB;Persist Security Info=True;User ID=TestUser;Password=testusrPWD!";
   
    protected DBConnectionInfo TestDBConnection = new (sqlCS, sqlProvider);
    protected DBConnectionInfo TestOleDbConnection = new (oleDbCS, oleDbProvider);

    [AssemblyInitialize()]
    public static void InitializeTests(TestContext context)
    {
        DbProviderFactories.RegisterFactory(sqlProvider, typeof(System.Data.SqlClient.SqlClientFactory));
        DbProviderFactories.RegisterFactory(oleDbProvider, typeof(System.Data.OleDb.OleDbFactory));
    }
}