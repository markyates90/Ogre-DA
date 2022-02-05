using System;
using System.Configuration;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Oracle.Tests
{
	[TestClass]
	public class CreationTests : TestBase
	{
		[TestMethod()]
		public void CreateConnectionTestOracleOleDb()
		{
			Database target = new Database(TestOleDbConnection);
			Type expected = typeof(System.Data.OleDb.OleDbConnection);
			DbConnection actual;
			actual = target.CreateConnection();
			Assert.IsInstanceOfType(actual, expected, "Should have created a Connection of type OleDbConnection.");
		}
		[TestMethod()]
		public void CreateConnectionTestOracleODPManaged()
		{
			Database target = new Database(TestDBConnection);
			string expected = "Oracle.ManagedDataAccess.Client.OracleConnection";
			string actual = target.CreateConnection().GetType().FullName;
			Assert.AreEqual(actual, expected, "Should have created a Connection of type Oracle.ManagedDataAccess.Client.OracleConnection.");
		}
		/// <summary>
		///A test for CreateDataAdapter
		///</summary>
		[TestMethod()]
		public void CreateDataAdapterTest()
		{
			Database target = new Database(TestOleDbConnection);
			Type expected = typeof(System.Data.OleDb.OleDbDataAdapter);
			DbDataAdapter actual = target.CreateDataAdapter();
			Assert.IsInstanceOfType(actual, expected, "Should have created a DataAdapter of type OleDbDataAdapter.");

			target = new Database(TestDBConnection);
			string expectedString = "Oracle.ManagedDataAccess.Client.OracleDataAdapter";
			string actualString = target.CreateDataAdapter().GetType().FullName;
			Assert.AreEqual(actualString, expectedString, "Should have created a DataAdapter of type Oracle.ManagedDataAccess.Client.OracleDataAdapter.");
		}

		/// <summary>
		///A test for CreateParameter
		///</summary>
		[TestMethod()]
		public void CreateParameterTest()
		{
			Database target = new Database(TestOleDbConnection);
			Type expected = typeof(System.Data.OleDb.OleDbParameter);
			DbParameter actual = target.CreateParameter("Test");
			Assert.IsInstanceOfType(actual, expected, "Should have created a parameter of type OleDbParameter.");
			Assert.AreEqual<string>("Test", actual.ParameterName);

			target = new Database(TestDBConnection);
			string expectedString = "Oracle.ManagedDataAccess.Client.OracleParameter";
			string actualString = target.CreateParameter("Test").GetType().FullName;
			Assert.AreEqual(actualString, expectedString, "Should have created a parameter of type Oracle.ManagedDataAccess.Client.OracleParameter.");
			Assert.AreEqual<string>("Test", actual.ParameterName);
		}
	}
}
