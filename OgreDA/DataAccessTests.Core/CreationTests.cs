using System;
using System.Configuration;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace DataAccessTests
{
	[TestClass]
	public class CreationTests
	{
		/// <summary>
		///A test for Database Constructor
		///</summary>
		[TestMethod()]
		public void DatabaseConstructorTestSql()
		{
			ConnectionStringSettings connectionString = ConfigFileManager.GetConnectionString("TestDB");
			Database target = new Database(connectionString);
			Assert.IsInstanceOfType(target.CreateConnection(),
				typeof(System.Data.SqlClient.SqlConnection),
				"Should be using SqlClient types.");
		}

		/// <summary>
		///A test for Database Constructor
		///</summary>
		[TestMethod()]
		public void DatabaseConstructorTest1Sql()
		{
			string connectionName = "TestDB";
			Database target = new Database(connectionName);
			Assert.IsInstanceOfType(target.CreateConnection(),
				typeof(System.Data.SqlClient.SqlConnection),
				"Should be using SqlClient types.");
		}


		/// <summary>
		///A test for Database Constructor
		///</summary>
		[TestMethod()]
		public void DatabaseConstructorTest2()
		{
			ConnectionStringSettings connectionString = ConfigFileManager.GetConnectionString("TestDB");
			Database target = new Database(connectionString.ProviderName, connectionString.ConnectionString);
			Assert.IsInstanceOfType(target.CreateConnection(),
				typeof(System.Data.SqlClient.SqlConnection),
				"Should be using SqlClient types.");
		}

		///// <summary>
		/////A test for Database Constructor
		/////</summary>
		//[TestMethod()]
		//[ExpectedException(typeof(DataAccessException))]
		//public void DatabaseConstructorTestMissingConfigSetting()
		//{
		//	string connectionString = "UnknownSetting";
		//	Database target = new Database(connectionString);
		//}

		/// <summary>
		///A test for CreateCommand
		///</summary>
		[TestMethod()]
		public void CreateCommandTestSql()
		{
			Database target = new Database("TestDB");
			Type expected = typeof(System.Data.SqlClient.SqlCommand);
			DbCommand actual;
			actual = target.CreateCommand();
			Assert.IsInstanceOfType(actual, expected, "Should have created a command of type SqlCommand.");
		}

		/// <summary>
		///A test for CreateCommand
		///</summary>
		[TestMethod()]
		public void CreateCommandTestDefaultConnection()
		{
			Database target = new Database("TestDB");
			DbConnection expected = target.CreateConnection(true);
			DbConnection actual;
			actual = target.CreateCommand(true).Connection;
			Assert.AreSame(expected, actual, "The command object should be associated with the default connection.");
		}

		/// <summary>
		///A test for CreateCommand
		///</summary>
		[TestMethod()]
		public void CreateCommandTestNotDefaultConnection()
		{
			Database target = new Database("TestDB");
			DbConnection actual;
			actual = target.CreateCommand(false).Connection;
			Assert.IsNull(actual, "The command object should not have an associated connection.");
		}

		/// <summary>
		///A test for CreateCommandBuilder
		///</summary>
		[TestMethod()]
		public void CreateCommandBuilderTestSql()
		{
			Database target = new Database("TestDB");
			Type expected = typeof(System.Data.SqlClient.SqlCommandBuilder);
			DbCommandBuilder actual;
			actual = target.CreateCommandBuilder();
			Assert.IsInstanceOfType(actual, expected, "Should have created a command builder of type SqlCommandBuilder.");
		}

		/// <summary>
		///A test for CreateConnection
		///</summary>
		[TestMethod()]
		public void CreateConnectionTestSql()
		{
			Database target = new Database("TestDB");
			Type expected = typeof(System.Data.SqlClient.SqlConnection);
			DbConnection actual;
			actual = target.CreateConnection();
			Assert.IsInstanceOfType(actual, expected, "Should have created a Connection of type SqlConnection.");
		}

		/// <summary>
		///A test for CreateConnection
		///</summary>
		[TestMethod()]
		public void CreateConnectionDefaultVsNonDefault()
		{
			Database target = new Database("TestDB");
			DbConnection notexpected = target.CreateConnection(false);
			DbConnection actual = target.CreateConnection(true);
			DbConnection expected = target.CreateConnection(true);
			Assert.AreNotSame(notexpected, actual, "Should not be the same connection object");
			Assert.AreSame(expected, actual, "Should be the same connection object.");
		}

	}
}
