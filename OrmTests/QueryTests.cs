using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OgreDA.DataAccess;
using OgreDA.Orm;

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
}