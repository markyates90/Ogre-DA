using System;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace DataAccessTests
{
	[TestClass]
	public class ConnectionTests
	{
		[TestMethod()]
		public void OpenConnectionTestOracle()
		{
			Database target = new Database("TestDBOracle");
			DbConnection actual = target.OpenConnection();
			Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
			actual.Close();
		}
		[TestMethod()]
		public void OpenConnectionTestOracleClient()
		{
			Database target = new Database("TestDBOracleClient");
			DbConnection actual = target.OpenConnection();
			Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
			actual.Close();
		}
		[TestMethod()]
		public void OpenConnectionTestOracleODPManaged()
		{
			Database target = new Database("TestDBOracleODPManaged");
			DbConnection actual = target.OpenConnection();
			Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
			actual.Close();
		}
	}
}
