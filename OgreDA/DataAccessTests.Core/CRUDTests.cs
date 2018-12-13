using System;
using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace DataAccessTests
{
	[TestClass]
	public class CRUDTests
	{
		private void setInsertCommand(DbCommand command, string testName)
		{
			command.CommandText = "INSERT INTO UnitTest (StringCol, IntCol, DoubleCol, DateCol, DateTimeCol, LongTextCol) "
				+ "VALUES (@stringval, @intval, @doubleval, @dateval, @datetimeval, @longtextval)";
			command.AddParameter("stringval", testName);
			command.AddParameter("intval", 1024);
			command.AddParameter("doubleval", 80.14);
			command.AddParameter("dateval", DateTime.Today);
			command.AddParameter("datetimeval", DateTime.Now);
			command.AddParameter("longtextval", string.Concat(Enumerable.Repeat("This is a long text. ", 1000)));
		}
		private string getDeleteCommand()
		{
			return "DELETE FROM UnitTest WHERE StringCol=@stringval";
		}
		/// <summary>
		///A test for ExecuteNonQuery
		///</summary>
		[TestMethod()]
		public void ExecuteNonQueryTest()
		{
			Database target = new Database("TestDB");
			DbConnection connection = target.OpenConnection();
			DbCommand command = target.CreateCommand();
			setInsertCommand(command, "ExecuteNonQueryTest");

			int expected = 1;
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual, "A record should have been inserted.");

			command.CommandText = getDeleteCommand();
			actual = target.ExecuteNonQuery(command);
			Assert.IsTrue(actual > 0, "At least one record should have been deleted.");
			Assert.AreEqual(System.Data.ConnectionState.Open, connection.State, "The connection should still be open.");
			connection.Close();
		}

		/// <summary>
		///A test for ExecuteNonQuery
		///</summary>
		[TestMethod()]
		public void ExecuteNonQueryTestDefaultConnectionNotOpen()
		{
			Database target = new Database("TestDB");
			DbConnection connection = target.CreateConnection();
			DbCommand command = target.CreateCommand();
			setInsertCommand(command, "ExecuteNonQueryTestDefaultConnectionNotOpen");

			int expected = 1;
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual, "A record should have been inserted.");

			command.CommandText = getDeleteCommand();
			actual = target.ExecuteNonQuery(command);
			Assert.IsTrue(actual > 0, "At least one record should have been deleted.");
			Assert.AreEqual(System.Data.ConnectionState.Closed, connection.State, "The connection should be closed.");
		}

		/// <summary>
		///A test for ExecuteNonQuery
		///</summary>
		[TestMethod()]
		public void ExecuteNonQueryTestDefaultConnectionNotCreated()
		{
			Database target = new Database("TestDB");
			DbCommand command = target.CreateCommand();
			setInsertCommand(command, "ExecuteNonQueryTestDefaultConnectionNotCreated");
			int expected = 1;
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual, "A record should have been inserted.");
			Assert.IsNull(command.Connection);

			command.CommandText = getDeleteCommand();
			actual = target.ExecuteNonQuery(command);
			Assert.IsTrue(actual > 0, "At least one record should have been deleted.");
			Assert.IsNull(command.Connection);
		}

		/// <summary>
		///A test for ExecuteNonQuery
		///</summary>
		[TestMethod()]
		public void ExecuteNonQueryTestNonDefaultConnection()
		{
			Database target = new Database("TestDB");
			DbCommand command = target.CreateCommand(false);
			setInsertCommand(command, "ExecuteNonQueryTestNonDefaultConnection");

			int expected = 1;
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual, "A record should have been inserted.");
			Assert.IsNull(command.Connection);

			command.CommandText = getDeleteCommand();
			actual = target.ExecuteNonQuery(command);
			Assert.IsTrue(actual > 0, "At least one record should have been deleted.");
			Assert.IsNull(command.Connection);
		}

		/// <summary>
		///A test for ExecuteReader
		///</summary>
		[TestMethod()]
		public void ExecuteReaderTest()
		{
			Database target = new Database("TestDB");
			DbCommand command = target.CreateCommand();
			command.CommandText = "SELECT * FROM UnitTest";
			DbDataReader reader = target.ExecuteReader(command);
			Assert.IsTrue(reader.HasRows, "Query should have returned some rows.");
			reader.Close();
			command.Connection.Close();
		}
		[TestMethod()]
		public void ExecuteScalarTest()
		{
			Database target = new Database("TestDB");
			// prep
			DbCommand cmd = target.CreateCommand();
			setInsertCommand(cmd, "ExecuteScalarTest");
			target.ExecuteNonQuery(cmd);
			// test
			DbCommand command = target.CreateCommand();
			command.CommandText = "SELECT COUNT(*) FROM UnitTest WHERE StringCol=@stringval";
			command.AddParameter("stringval", "ExecuteScalarTest");
			object actual = target.ExecuteScalar(command);
			Assert.IsInstanceOfType(actual, typeof(int), "An integer should be returned from query.");
			Assert.IsTrue((int)actual > 0, "Count should be at least 1.");

			// clean-up
			command.CommandText = getDeleteCommand();
			target.ExecuteNonQuery(command);
		}
	}
}
