using System;
using System.Data.Common;

namespace OgreDA.Orm;
public static class DbDataReaderExtensions
{
	public static T MapToObject<T>(this DbDataReader reader, DataReaderPropertySetters<T> setters) where T : new()
	{
		T result = new T();
		foreach ( PropertySetter<T> setter in setters.SetterFunctions )
		{
			try
			{
				setter.setterFunc( result, reader );
			}
			catch ( Exception ex )
			{
				throw new Exception( "Error setting property for " + setter.columnName, ex );
			}
		}
		return result;
	}
}
