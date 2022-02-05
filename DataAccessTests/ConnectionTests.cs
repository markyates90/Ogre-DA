using System;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;

namespace OgreDA.DataAccess.Tests;

[TestClass]
public class ConnectionTests: TestBase
{
    /// <summary>
    ///A test for OpenConnection
    ///</summary>
    [TestMethod()]
    public void OpenConnectionTest()
    {
        Database target = new Database(TestDBConnection);
        DbConnection actual = target.OpenConnection();
        Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
        actual.Close();
    }

    /// <summary>
    ///A test for OpenConnection
    ///</summary>
    [TestMethod()]
    public void OpenConnectionTestDefaultConnection()
    {
        Database target = new Database(TestDBConnection);
        DbConnection expected = target.CreateConnection(true);
        DbConnection actual = target.OpenConnection(true);
        Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
        Assert.AreSame(expected, actual, "Should be the same connection object.");
        actual.Close();
    }

    /// <summary>
    ///A test for OpenConnection
    ///</summary>
    [TestMethod()]
    public void OpenConnectionTestNotDefaultConnection()
    {
        Database target = new Database(TestDBConnection);
        DbConnection notexpected = target.CreateConnection();
        DbConnection actual = target.OpenConnection(false);
        Assert.AreEqual(System.Data.ConnectionState.Open, actual.State);
        Assert.AreNotSame(notexpected, actual, "Should not be the same connection object.");
        actual.Close();
    }
}
