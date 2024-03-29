using System;
namespace OgreDA.Orm.Tests;

public class UnitTestModel
{
    public string? StringCol { get; set; }
    public int? IntCol { get; set; }
    public double? DoubleCol { get; set; }
    public DateTime? DateCol { get; set; }
    public DateTime? DateTimeCol { get; set; }
    public string? LongTextCol { get; set; }

    public UnitTestModel() {}
}

public class QueryTestModel
{
    public int QueryTestID { get; set; }
    public string NameCol { get; set; }
    public int ValueCol { get; set; }
}
