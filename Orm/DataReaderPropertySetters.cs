using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using OgreDA.DataAccess;

namespace OgreDA.Orm;
public struct PropertySetter<T>
{
	public string columnName;
	public Action<T, DbDataReader> setterFunc;
}

public class DataReaderPropertySetters<T> where T : new()
{
	const int BOOLTRUEVALUE = 1;
	private List<PropertySetter<T>> setters;
	public List<PropertySetter<T>> SetterFunctions
	{ 
		get
		{
			return setters;
		} 
	}
	protected ObjectMap<T> objectMap;

	public DataReaderPropertySetters( DbDataReader dr )
	{
		objectMap = new ObjectMap<T>();
		setters = new List<PropertySetter<T>>();
		getPropertySetters( dr );
	}
	public DataReaderPropertySetters( DbDataReader dr, ObjectMap<T> map )
	{
		objectMap = map;
		setters = new List<PropertySetter<T>>();
		getPropertySetters( dr );
	}

	protected void getPropertySetters( DbDataReader dr )
	{
		// build up list of mapping functions for column to property
		string columnName;
		for ( var i = 0; i < dr.FieldCount; i++ )
		{
			columnName = dr.GetName( i ).ToUpper();
			if ( !objectMap.AliasToPropertyMap.Keys.Any(n => n == columnName) )
			{
				continue;
			}

			PropertySetter<T> setter;
			setter.setterFunc = getSetterMethod( dr, i, objectMap.AliasToPropertyMap[columnName] );
			setter.columnName = columnName;
			setters.Add( setter );
		}
	}

	private Action<T, DbDataReader> getSetterMethod(DbDataReader dr, int ordinal, PropertyInfo property )
	{
		Type fieldType = dr.GetFieldType(ordinal);

		switch ( fieldType.Name.ToLower() )
		{
			case "string":
				return stringSetter(property, ordinal);
			case "int16":
				return smallIntSetter(property, ordinal);
			case "int32":
				return intSetter(property, ordinal);
			case "double":
				return doubleSetter(property, ordinal);
			case "decimal":
				return decimalSetter(property, ordinal);
			case "datetime":
				return dateTimeSetter(property, ordinal);
			case "byte[]":
				return binarySetter(property, ordinal);
			case "guid":
				return guidSetter(property, ordinal);
			case "byte":
				string typeName = dr.GetDataTypeName(ordinal);
				if ( string.IsNullOrEmpty( typeName ) )
					return defaultSetter(property, ordinal);

				switch ( typeName.ToLower() )
				{
					case "smallint":
					case "tinyint":
						return smallIntFromByteSetter(property, ordinal);
					default:
					return defaultSetter(property, ordinal);
				}
			default:
				return defaultSetter(property, ordinal);
		}
	}

	private Action<T, DbDataReader> stringSetter( PropertyInfo pi, int ordinal )
	{
		return ( result, reader ) =>
		{
			string? value = reader.SafeGetString(ordinal);
			pi.SetValue(result, value, null);
		};
	}
	private Action<T, DbDataReader> intSetter( PropertyInfo pi, int ordinal )
	{
		if (pi.IsEnumProperty())
		{
			return setEnumFromInt( pi, ordinal );
		}

		var isValueDbNull = setNullIfDbNull( pi, ordinal );
		if ( pi.IsBoolProperty() )
		{
			return ( result, reader ) =>
			{
				if ( isValueDbNull( result, reader ) )
					return;
				int fieldValue = reader.GetInt32(ordinal);
				bool value = fieldValue == BOOLTRUEVALUE;
				pi.SetValue(result, value, null);
			};
		}

		return ( result, reader ) =>
		{
			if ( isValueDbNull( result, reader ) )
				return;
			int fieldValue = reader.GetInt32(ordinal);
			pi.SetValue(result, fieldValue, null);;
		};
	}
	private Action<T, DbDataReader> setEnumFromInt( PropertyInfo pi, int ordinal )
	{
		Type? enumType = null;
		if ( pi.PropertyType.GetTypeInfo().BaseType == typeof( Enum ) )
		{
			enumType = pi.PropertyType;
		}
		else
		{
			enumType = pi.PropertyType.GetTypeInfo().GetGenericArguments()[0];
		}

		if ( enumType != null )
		{
			var isValueDbNull = setNullIfDbNull( pi, ordinal );
			return ( result, reader ) =>
			{
				
				if ( !isValueDbNull( result, reader ) )
				{
					int fieldValue = reader.GetInt32(ordinal);
					Func<object, T> cast = (o) => {return (T)o;};
					MethodInfo castMethod = cast.GetMethodInfo().MakeGenericMethod( enumType );
					object? enumValue = castMethod.Invoke( this, new object[] { fieldValue } );
					pi.SetValue(result, enumValue, null);
				}
			};
		}
		throw new InvalidOperationException($"Expected Enum propery type, got {pi.PropertyType.GetTypeInfo()}");
	}
	private Action<T, DbDataReader> smallIntSetter( PropertyInfo pi, int ordinal )
	{
		if (pi.IsEnumProperty())
		{
			return setEnumFromInt16( pi, ordinal );
		}

		var isValueDbNull = setNullIfDbNull( pi, ordinal );
		if (pi.IsBoolProperty())
		{
			return ( result, reader ) =>
			{
				if (!isValueDbNull(result, reader))
				{
					short fieldValue = reader.GetInt16(ordinal);
					bool value = fieldValue == BOOLTRUEVALUE;
					pi.SetValue(result, value, null);
				}
			};
		}

		return (result, reader) =>
		{
			if (!isValueDbNull(result, reader))
			{
				short fieldValue = reader.GetInt16(ordinal);
				pi.SetValue(result, fieldValue, null);
			}
		};
	}
	private Action<T, DbDataReader> setEnumFromInt16( PropertyInfo pi, int ordinal )
	{
		Type? enumType = null;
		if ( pi.PropertyType.GetTypeInfo().BaseType == typeof( Enum ) )
		{
			enumType = pi.PropertyType;
		}
		else
		{
			enumType = pi.PropertyType.GetTypeInfo().GetGenericArguments()[0];
		}

		if (enumType != null)
		{
			var isValueDbNull = setNullIfDbNull( pi, ordinal );
			return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					short fieldValue = reader.GetInt16(ordinal);
					Func<object, T> cast = (o) => {return (T)o;};
					MethodInfo castMethod = cast.GetMethodInfo().MakeGenericMethod( enumType );
					object? enumValue = castMethod.Invoke( this, new object[] { fieldValue } );
					pi.SetValue(result, enumValue, null);
				}
			};
		}
		throw new InvalidOperationException($"Expected Enum propery type, got {pi.PropertyType.GetTypeInfo()}");
	}

	private Action<T, DbDataReader> doubleSetter( PropertyInfo pi, int ordinal )
	{
		var isValueDbNull = setNullIfDbNull( pi, ordinal );
		if (pi.IsIntProperty())
		{
			return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					double value = reader.GetDouble(ordinal);
					pi.SetValue(result, Convert.ToInt32(value), null);
				}
			};
		}
		else
		{
			return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					double value = reader.GetDouble(ordinal);
					pi.SetValue(result, value, null);
				}
			};
		}
	}
	private Action<T, DbDataReader> decimalSetter( PropertyInfo pi, int ordinal )
	{
		var isValueDbNull = setNullIfDbNull( pi, ordinal );

		if (pi.IsIntProperty())
		{
			return (result, reader) =>
				{
					if (!isValueDbNull( result, reader ))
					{
						decimal value = reader.GetDecimal(ordinal);
						pi.SetValue(result, Convert.ToInt32( value ), null);
					}
				};
		}
		else if (pi.IsBoolProperty())
		{
			return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					decimal fieldValue = reader.GetDecimal(ordinal);
					int intVal = Convert.ToInt32( fieldValue );
					bool value = intVal == BOOLTRUEVALUE;
					pi.SetValue(result, value, null);
				}
			};
		}
		else if (pi.IsDoubleProperty())
		{
			return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					decimal value = reader.GetDecimal(ordinal);
					pi.SetValue(result, Convert.ToDouble(value), null);
				}
			};
		}
		else
		{
			return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					decimal value = reader.GetDecimal(ordinal);
					pi.SetValue(result, value, null);
				}
			};
		}
	}
	private Action<T, DbDataReader> dateTimeSetter( PropertyInfo pi, int ordinal )
	{
		var isValueDbNull = setNullIfDbNull( pi, ordinal );
		return ( result, reader ) =>
			{
				if (!isValueDbNull(result, reader))
				{
					DateTime value = reader.GetDateTime(ordinal);
					pi.SetValue(result, value, null);
				}
			};
	}
	private Action<T, DbDataReader> binarySetter( PropertyInfo pi, int ordinal )
	{
		if (pi.IsGuidProperty())
		{
			return guidSetter(pi, ordinal);
		}
		else if (pi.PropertyType == typeof(byte[]))
		{
			return (result, reader) =>
			{
				byte[]? value = reader.GetBinary(ordinal);
				pi.SetValue(result, value, null);
			};
		}
		else
		{
			throw new InvalidCastException($"Could not convert binary to {pi.PropertyType.FullName}.");
		}
	}
	private Action<T, DbDataReader> smallIntFromByteSetter( PropertyInfo pi, int ordinal )
	{
		var isValueDbNull = setNullIfDbNull( pi, ordinal );
		return (result, reader) =>
		{
			if (!isValueDbNull(result, reader))
			{
				Int16 value = reader.GetByte(ordinal);
				pi.SetValue(result, value, null);
			}
		};
	}
	private Action<T, DbDataReader> guidSetter(PropertyInfo pi, int ordinal)
	{
		var isValueDbNull = setNullIfDbNull(pi, ordinal);
		return (result, reader) =>
			{
				if (!isValueDbNull(result, reader))
				{
					Guid value = reader.SafeGetGuid(ordinal);
					pi.SetValue(result, value, null);
				}
			};
	}
	private Action<T, DbDataReader> defaultSetter(PropertyInfo pi, int ordinal)
	{
		var isValueDbNull = setNullIfDbNull(pi, ordinal);
		return ( result, reader ) =>
			{
				if (!isValueDbNull(result, reader))
				{
					pi.SetValue(result, reader.GetValue(ordinal), null);
				}
			};
	}

	private Func<T, DbDataReader, bool> setNullIfDbNull(PropertyInfo pi, int ordinal)
	{
		if (pi.IsPropertyNullable())
		{
			return (result, reader) =>
			{
				if (reader.IsDBNull(ordinal))
				{
					pi.SetValue(result, null, null);
					return true;
				}
				return false;
			};
		}

		return (result, reader) => reader.IsDBNull(ordinal);
	}
}
