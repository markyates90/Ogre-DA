using System;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Oracle.Tests
{
	[TestClass]
	public class ConnectionTests : TestBase
	{
		[TestMethod()]
		public void OpenConnectionTestOracle()
		{
			Database target = new Database(TestDBConnection);
			DbConnection actual = target.OpenConnection();
			Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
			actual.Close();
		}
		[TestMethod()]
		public void OpenConnectionTestOracleClient()
		{
			Database target = new Database(TestOleDbConnection);
			DbConnection actual = target.OpenConnection();
			Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
			actual.Close();
		}
	}
}
