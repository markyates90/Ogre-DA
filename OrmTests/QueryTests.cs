using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;
using OgreDA.Orm;
using OgreDA.Orm.Query;

namespace OgreDA.Orm.Tests;

[TestClass]
public class QueryTests : TestBase
{
    [TestMethod]
    public async Task TestMethod1()
    {
        Database db = new Database(TestDBConnection);
        DbCommand command = db.CreateCommand();
        command.CommandText = "SELECT * FROM UnitTest";
        List<UnitTestModel> result = await db.QueryAsync<UnitTestModel>(command);
        Assert.IsTrue(result.Count > 0);
        Assert.AreEqual(result[0].IntCol, 1024);
    }
    
    [TestMethod]
    public async Task SelectTest()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .Where("ValueCol > @valuecol")
            .OrderBy("NameCol")
            .AddParameter("valuecol", 5)
            .ToListAsync<QueryTestModel>();

        Assert.AreEqual(15, result.Count);
        Assert.AreEqual("Eight", result[0].NameCol);
    }
    [TestMethod]
    public async Task SelectTest2()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .Where("ValueCol > @valuecol")
            .AddParameter("valuecol", 5)
            .ToListAsync<QueryTestModel>();

        Assert.AreEqual(15, result.Count);
        Assert.AreEqual("Six", result[0].NameCol);
    }
    [TestMethod]
    public async Task SelectTest3()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .ToListAsync<QueryTestModel>();

        Assert.AreEqual(20, result.Count);
        Assert.AreEqual("One", result[0].NameCol);
    }
    [TestMethod]
    public async Task SelectTest4()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .OrderBy("NameCol")
            .ToListAsync<QueryTestModel>();

        Assert.AreEqual(20, result.Count);
        Assert.AreEqual("Eight", result[0].NameCol);
    }
    [TestMethod]
    public async Task PagingTest()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .Where("ValueCol > @valuecol")
            .OrderBy("NameCol")
            .AddParameter("valuecol", 5)
            .ToPagedListAsync<QueryTestModel>(5, 10);

        Assert.AreEqual(5, result.Items.Count);
        Assert.AreEqual(15, result.TotalCount);
        Assert.AreEqual("Sixteen", result.Items[0].NameCol);
    }
    [TestMethod]
    public async Task PagingTest2()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .Where("ValueCol > @valuecol")
            .OrderBy("NameCol")
            .AddParameter("valuecol", 5)
            .ToPagedListAsync<QueryTestModel>(10, 0);

        Assert.AreEqual(10, result.Items.Count);
        Assert.AreEqual(15, result.TotalCount);
        Assert.AreEqual("Eight", result.Items[0].NameCol);
    }
    [TestMethod]
    public async Task PagingTest3()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .OrderBy("NameCol")
            .ToPagedListAsync<QueryTestModel>(5, 15);

        Assert.AreEqual(5, result.Items.Count);
        Assert.AreEqual(20, result.TotalCount);
        Assert.AreEqual("Thirteen", result.Items[0].NameCol);
    }
    [TestMethod]
    public async Task PagingTest4()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .ToPagedListAsync<QueryTestModel>(5, 5);

        Assert.AreEqual(5, result.Items.Count);
        Assert.AreEqual(20, result.TotalCount);
        Assert.AreEqual("Six", result.Items[0].NameCol);
    }
    [TestMethod]
    public async Task PagingTest5()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .Where("ValueCol > @valuecol")
            .OrderBy("NameCol")
            .AddParameter("valuecol", 5)
            .ToPagedListAsync<QueryTestModel>(5, 20);

        Assert.AreEqual(0, result.Items.Count);
        Assert.AreEqual(15, result.TotalCount);
    }
    [TestMethod]
    public async Task PagingTest6()
    {
        Database db = new Database(TestDBConnection);
        var result = await db.Select("QueryTestID, NameCol, ValueCol")
            .From("QueryTest")
            .OrderBy("NameCol")
            .AddParameter("valuecol", 5)
            .ToPagedListAsync<QueryTestModel>(5, 18);

        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(20, result.TotalCount);
    }
}