using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OgreDA.Orm;
public class ObjectMap<T> where T : new()
{
    protected T? sourceObject;
    private Type sourceType;

    private string? tableName;

    public Dictionary<string, PropertyInfo> AliasToPropertyMap { get; set; }
    public Dictionary<string, string> PropertyToColumnMap { get; set; }

    public T SourceObject
    {
        get
        {
            if (sourceObject == null)
            {
                sourceObject = new T();
            }
            return sourceObject;
        }
    }
    
    public ObjectMap()
    {
        sourceType = typeof(T);
        AliasToPropertyMap = new Dictionary<string, PropertyInfo>();
        PropertyToColumnMap = new Dictionary<string, string>();
        configureMapping();
    }
    public ObjectMap(T ObjectToMap)
    {
        sourceObject = ObjectToMap;
        sourceType = ObjectToMap!.GetType();
        AliasToPropertyMap = new Dictionary<string, PropertyInfo>();
        PropertyToColumnMap = new Dictionary<string, string>();
        configureMapping();
    }

    protected void configureMapping()
    {
        getTableName();

        List<PropertyInfo> properties = sourceType.GetTypeInfo().GetProperties( BindingFlags.Public | BindingFlags.Instance ).Where( p => p.CanWrite ).ToList();
        foreach (PropertyInfo property in properties)
        {
            mapProperty(property);
        }
    }

    private void getTableName()
    {
        TableAttribute? tableAttribute = sourceType.GetTypeInfo().GetCustomAttribute<TableAttribute>(true);
        if (tableAttribute != null)
        {
            tableName = tableAttribute.TableName;
        }
    }
    private void mapProperty(PropertyInfo property)
    {
        AliasAttribute? aliasAttribute = property.GetCustomAttribute<AliasAttribute>(true);
        string columnName = aliasAttribute != null ? aliasAttribute.Alias : property.Name.ToUpper();
        AliasToPropertyMap.Add(columnName, property);
        PropertyToColumnMap.Add(property.Name, columnName);
    }
}
