using System;
namespace OgreDA.Orm;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class AliasAttribute : Attribute
{
    public string Alias { get; }
    public AliasAttribute(string alias)
    {
        Alias = alias;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class TableAttribute : Attribute
{
    public string TableName { get; }
    // This is a positional argument
    public TableAttribute(string tableName)
    {
        TableName = tableName;
    }
}