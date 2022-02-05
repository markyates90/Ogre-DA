using System;
using System.Configuration;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Tests;

[TestClass]
public class CreationTests : TestBase
{
	/// <summary>
	///A test for Database Constructor
	///</summary>
	[TestMethod()]
	public void DatabaseConstructorTestSql()
	{
		Database target = new Database(TestDBConnection);
		Assert.IsInstanceOfType(target.CreateConnection(),
			typeof(System.Data.SqlClient.SqlConnection),
			"Should be using SqlClient types.");
	}
	/// <summary>
	///A test for Database Constructor
	///</summary>
	[TestMethod()]
	public void DatabaseConstructorTestOleDb()
	{
		Database target = new Database(TestOleDbConnection);
		Assert.IsInstanceOfType(target.CreateConnection(),
			typeof(System.Data.OleDb.OleDbConnection),
			"Should be using OleDb types.");
	}
	/// <summary>
	///A test for Database Constructor
	///</summary>
	[TestMethod()]
	public void DatabaseConstructorTest2()
	{
		Database target = new Database(sqlProvider, sqlCS);
		Assert.IsInstanceOfType(target.CreateConnection(),
			typeof(System.Data.SqlClient.SqlConnection),
			"Should be using SqlClient types.");
	}

	/// <summary>
	///A test for CreateCommand
	///</summary>
	[TestMethod()]
	public void CreateCommandTestSql()
	{
		Database target = new Database(TestDBConnection);
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
		Database target = new Database(TestDBConnection);
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
		Database target = new Database(TestDBConnection);
		DbConnection actual;
		actual = target.CreateCommand(false).Connection;
		Assert.IsNull(actual, "The command object should not have an associated connection.");
	}

	/// <summary>
	///A test for CreateCommand
	///</summary>
	[TestMethod()]
	public void CreateCommandTestOleDb()
	{
		Database target = new Database(TestOleDbConnection);
		Type expected = typeof(System.Data.OleDb.OleDbCommand);
		DbCommand actual;
		actual = target.CreateCommand();
		Assert.IsInstanceOfType(actual, expected, "Should have created a command of type OleDbCommand.");
	}

	/// <summary>
	///A test for CreateCommandBuilder
	///</summary>
	[TestMethod()]
	public void CreateCommandBuilderTestSql()
	{
		Database target = new Database(TestDBConnection);
		Type expected = typeof(System.Data.SqlClient.SqlCommandBuilder);
		DbCommandBuilder actual;
		actual = target.CreateCommandBuilder();
		Assert.IsInstanceOfType(actual, expected, "Should have created a command builder of type SqlCommandBuilder.");
	}

	/// <summary>
	///A test for CreateCommandBuilder
	///</summary>
	[TestMethod()]
	public void CreateCommandBuilderTestOleDb()
	{
		Database target = new Database(TestOleDbConnection);
		Type expected = typeof(System.Data.OleDb.OleDbCommandBuilder);
		DbCommandBuilder actual;
		actual = target.CreateCommandBuilder();
		Assert.IsInstanceOfType(actual, expected, "Should have created a command builder of type OleDbCommandBuilder.");
	}

	/// <summary>
	///A test for CreateConnection
	///</summary>
	[TestMethod()]
	public void CreateConnectionTestSql()
	{
		Database target = new Database(TestDBConnection);
		Type expected = typeof(System.Data.SqlClient.SqlConnection);
		DbConnection actual;
		actual = target.CreateConnection();
		Assert.IsInstanceOfType(actual, expected, "Should have created a Connection of type SqlConnection.");
	}

	/// <summary>
	///A test for CreateConnection
	///</summary>
	[TestMethod()]
	public void CreateConnectionTestOleDb()
	{
		Database target = new Database(TestOleDbConnection);
		Type expected = typeof(System.Data.OleDb.OleDbConnection);
		DbConnection actual;
		actual = target.CreateConnection();
		Assert.IsInstanceOfType(actual, expected, "Should have created a Connection of type OleDbConnection.");
	}
	/// <summary>
	///A test for CreateConnection
	///</summary>
	[TestMethod()]
	public void CreateConnectionDefaultVsNonDefault()
	{
		Database target = new Database(TestDBConnection);
		DbConnection notexpected = target.CreateConnection(false);
		DbConnection actual = target.CreateConnection(true);
		DbConnection expected = target.CreateConnection(true);
		Assert.AreNotSame(notexpected, actual, "Should not be the same connection object");
		Assert.AreSame(expected, actual, "Should be the same connection object.");
	}

	/// <summary>
	///A test for CreateDataAdapter
	///</summary>
	[TestMethod()]
	public void CreateDataAdapterTest()
	{
		Database target = new Database(TestDBConnection);
		Type expected = typeof(System.Data.SqlClient.SqlDataAdapter);
		DbDataAdapter actual;
		actual = target.CreateDataAdapter();
		Assert.IsInstanceOfType(actual, expected, "Should have created a DataAdapter of type SqlDataAdapter.");

		target = new Database(TestOleDbConnection);
		expected = typeof(System.Data.OleDb.OleDbDataAdapter);
		actual = target.CreateDataAdapter();
		Assert.IsInstanceOfType(actual, expected, "Should have created a DataAdapter of type OleDbDataAdapter.");
	}

	/// <summary>
	///A test for CreateParameter
	///</summary>
	[TestMethod()]
	public void CreateParameterTest()
	{
		Database target = new Database(TestDBConnection);
		Type expected = typeof(System.Data.SqlClient.SqlParameter);
		DbParameter actual;
		actual = target.CreateParameter("Test");
		Assert.IsInstanceOfType(actual, expected, "Should have created a parameter of type SqlParameter.");
		Assert.AreEqual<string>("Test", actual.ParameterName);

		target = new Database(TestOleDbConnection);
		expected = typeof(System.Data.OleDb.OleDbParameter);
		actual = target.CreateParameter("Test");
		Assert.IsInstanceOfType(actual, expected, "Should have created a parameter of type OleDbParameter.");
		Assert.AreEqual<string>("Test", actual.ParameterName);
	}
}
