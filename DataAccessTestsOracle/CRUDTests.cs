using System;
using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Oracle.Tests
{
	[TestClass]
	public class CRUDTests : TestBase
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
		[TestMethod()]
		public void ExecuteNonQueryTestOracleOleDb()
		{
			Database target = new Database(TestOleDbConnection);
			DbCommand command = target.CreateCommand();
			setInsertCommand(command, "ExecuteNonQueryTestOracle");
			int expected = 1;
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual, "A record should have been inserted.");

			command = target.CreateCommand();
			command.CommandText = getDeleteCommand();
			command.AddParameter("stringval", "ExecuteNonQueryTestOracle");
			actual = target.ExecuteNonQuery(command);
			Assert.IsTrue(actual > 0, "At least one record should have been deleted.");
		}

		[TestMethod()]
		public void ExecuteNonQueryTestOracleODPManaged()
		{
			Database target = new Database(TestDBConnection);
			DbCommand command = target.CreateCommand();
			setInsertCommand(command, "ExecuteNonQueryTestOracle");
			int expected = 1;
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual, "A record should have been inserted.");

			command = target.CreateCommand();
			command.CommandText = getDeleteCommand();
			command.AddParameter("string", "ExecuteNonQueryTestOracle");
			actual = target.ExecuteNonQuery(command);
			Assert.IsTrue(actual > 0, "At least one record should have been deleted.");
		}
		[TestMethod()]
		public void ExecuteReaderTestOracleOleDb()
		{
			Database target = new Database(TestOleDbConnection);
			DbCommand command = target.CreateCommand();
			command.CommandText = "SELECT * FROM UnitTest";
			DbDataReader reader = target.ExecuteReader(command);
			Assert.IsTrue(reader.HasRows, "Query should have returned some rows.");
			reader.Close();
			command.Connection.Close();
		}
		[TestMethod()]
		public void ExecuteReaderTestOracleODPManaged()
		{
			Database target = new Database(TestDBConnection);
			DbCommand command = target.CreateCommand();
			command.CommandText = "SELECT * FROM UnitTest";
			DbDataReader reader = target.ExecuteReader(command);
			Assert.IsTrue(reader.HasRows, "Query should have returned some rows.");
			reader.Close();
			command.Connection.Close();
		}
		[TestMethod()]
		public void ExecuteScalarTestOracle()
		{
			Database target = new Database(TestOleDbConnection);
			// prep
			DbCommand cmd = target.CreateCommand();
			setInsertCommand(cmd, "ExecuteScalarTestOracle");
			target.ExecuteNonQuery(cmd);
			// test
			DbCommand command = target.CreateCommand();
			command.CommandText = "SELECT COUNT(*) FROM UnitTest WHERE StringCol=@stringval";
			command.AddParameter("stringval", "ExecuteScalarTestOracle");
			object actual = target.ExecuteScalar(command);
			Assert.IsInstanceOfType(actual, typeof(decimal), "A decimal should be returned from query.");
			Assert.IsTrue((decimal)actual > 0, "Count should be at least 1.");

			// clean-up
			command.CommandText = getDeleteCommand();
			target.ExecuteNonQuery(command);
		}
		[TestMethod()]
		public void ExecuteScalarTestOracleODPManaged()
		{
			Database target = new Database(TestDBConnection);
			// prep
			DbCommand cmd = target.CreateCommand();
			setInsertCommand(cmd, "ExecuteScalarTestOracle");
			target.ExecuteNonQuery(cmd);
			// test
			DbCommand command = target.CreateCommand();
			command.CommandText = "SELECT COUNT(*) FROM UnitTest WHERE StringCol=@string";
			command.AddParameter("string", "ExecuteScalarTestOracle");
			object actual = target.ExecuteScalar(command);
			Assert.IsInstanceOfType(actual, typeof(decimal), "A decimal should be returned from query.");
			Assert.IsTrue((decimal)actual > 0, "Count should be at least 1.");

			// clean-up
			command.CommandText = getDeleteCommand();
			target.ExecuteNonQuery(command);
		}
	}
}
