using System.Reflection;

namespace OgreDA.Orm;

public static class PropertyInfoExtentions
{
    public static bool IsEnumProperty(this PropertyInfo propInfo)
    {
        return propInfo.PropertyType.GetTypeInfo().BaseType == typeof(Enum) ||
				(propInfo.PropertyType.GetTypeInfo().IsGenericType 
                && propInfo.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>)
				&& propInfo.PropertyType.GetTypeInfo().GetGenericArguments()[0].GetTypeInfo().BaseType == typeof(Enum));
    }

    public static bool IsBoolProperty(this PropertyInfo propInfo)
    {
        return propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?);
    }
    public static bool IsIntProperty(this PropertyInfo propInfo)
    {
        return propInfo.PropertyType == typeof(int) || propInfo.PropertyType == typeof(int?);
    }
    public static bool IsDoubleProperty(this PropertyInfo propInfo)
    {
        return propInfo.PropertyType == typeof(double) || propInfo.PropertyType == typeof(double?);
    }
    public static bool IsGuidProperty(this PropertyInfo propInfo)
    {
        return propInfo.PropertyType == typeof(Guid) || propInfo.PropertyType == typeof(Guid?);
    }
    public static bool IsPropertyNullable(this PropertyInfo propInfo)
    {
        return !propInfo.PropertyType.GetTypeInfo().IsValueType || 
            (propInfo.PropertyType.GetTypeInfo().IsGenericType 
            && propInfo.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof( Nullable<> ));
    }
}