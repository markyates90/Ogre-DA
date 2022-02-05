using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Oracle.Tests;

[TestClass]
public class TestBase
{
    protected const string oracleProvider = "Oracle.ManagedDataAccess.Client";
    protected const string oleDbProvider = "System.Data.OleDb";
    protected const string oracleCS = "User ID=;Password=;Data Source=TestDB;";
    protected const string oleDbCS = "Provider=OraOleDB.Oracle;chunksize=8000;User ID=;Password=;Data Source=TestDB;DistribTx=0";
   
    protected DBConnectionInfo TestDBConnection = new (oracleCS, oracleProvider);
    protected DBConnectionInfo TestOleDbConnection = new (oleDbCS, oleDbProvider);

    [AssemblyInitialize()]
    public static void InitializeTests(TestContext context)
    {
        DbProviderFactories.RegisterFactory(oracleProvider, typeof(Oracle.ManagedDataAccess.Client.OracleClientFactory));
        DbProviderFactories.RegisterFactory(oleDbProvider, typeof(System.Data.OleDb.OleDbFactory));
    }
}