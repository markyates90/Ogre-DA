using System.Data;
using System.Data.Common;
using System.Reflection;

namespace OgreDA.DataAccess;

public static class DbCommandExtensions
{
    /// <summary>
    /// Enlist a command within a transaction
    /// </summary>
    /// <param name="command"></param>
    /// <param name="transaction"></param>
    public static void EnlistTransaction(this DbCommand command, DbTransaction? transaction)
    {
        if (transaction != null)
        {
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
        }
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    public static void AddParameter(this DbCommand command, string name, object value)
    {
        addParameter(command, name, null, 0, ParameterDirection.Input, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    /// <param name="direction">The Parameter direction.</param>
    public static void AddParameter(this DbCommand command, string name, object value, ParameterDirection direction)
    {
        addParameter(command, name, null, 0, direction, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    /// <param name="dbType">The Parameter data type</param>
    /// <param name="size">Size if appropriate for the type</param>
    public static void AddParameter(this DbCommand command, string name, object value, DbType dbType, int size = 0)
    {
        addParameter(command, name, dbType, size, ParameterDirection.Input, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    /// <param name="direction">The Parameter direction.</param>
    /// <param name="dbType">The Parameter data type</param>
    /// <param name="size">Size if appropriate for the type</param>
    public static void AddParameter(this DbCommand command, string name, object value, ParameterDirection direction, DbType dbType, int size = 0)
    {
        addParameter(command, name, dbType, size, direction, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="dbType">The Parameter data type</param>
    /// <param name="size">Size if appropriate for the type</param>
    /// <param name="direction">The Parameter direction.</param>
    /// <param name="nullable"></param>
    /// <param name="sourceColumn"></param>
    /// <param name="value">Parameter Value</param>
    public static void AddParameter(this DbCommand command,
                                                string name,
                                                DbType dbType,
                                                int size,
                                                ParameterDirection direction,
                                                bool nullable,
                                                string sourceColumn,
                                                object value)
    {
        addParameter(command, name, dbType, size, direction, nullable, sourceColumn, value);
    }

    private static void addParameter(DbCommand command,
                                        string name,
                                        DbType? dbType,
                                        int size,
                                        ParameterDirection direction,
                                        bool nullable,
                                        string sourceColumn,
                                        object value)
    {
        DbParameter param = command.CreateParameter();
        param.ParameterName = name;

        if (dbType.HasValue)
        {
            param.DbType = dbType.Value;
        }

        param.Value = value ?? DBNull.Value;

        if (size != 0)
        {
            param.Size = size;
        }
        param.Direction = direction;
        param.IsNullable = nullable;
        param.SourceColumn = sourceColumn;

        command.Parameters.Add(param);
    }

    /// <summary>
    /// Replace non-portable syntax using tokens
    /// </summary>
    /// <param name="sqlString">SQL syntax with tokens</param>
    /// <param name="isOracle">Is Oracle?</param>
    /// <returns></returns>
    public static string ReplaceSqlTokens(this string sqlString, bool isOracle)
    {
        if (string.IsNullOrEmpty(sqlString))
        {
            return sqlString;
        }
        else
        {
            if (isOracle)
            {
                return sqlString.Replace("{DataType.String}", "varchar2")
                                .Replace("{DataType.DateTime}", "Date")
                                .Replace("{DataType.Integer}", "NUMBER")
                                .Replace("{DataType.Bool}", "NUMBER(1,0)")
                                .Replace("{Date.Day(}", "TRUNC(");
            }
            else
            {
                return sqlString.Replace("{DataType.String}", "nvarchar")
                                .Replace("{DataType.DateTime}", "Datetime2")
                                .Replace("{DataType.Integer}", "BigInt")
                                .Replace("{DataType.Bool}", "BIT")
                                .Replace("{Date.Day(}", "CONVERT(DATE,");
            }
        }
    }

    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list that is used for a GUID field..
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    public static void AddGuidParameter(this DbCommand command, string name, Nullable<Guid> value)
    {
        AddGuidParameter(command, name, ParameterDirection.Input, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list that is used for a GUID field.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    /// <param name="direction">The Parameter direction.</param>
    public static void AddGuidParameter(this DbCommand command, string name, Nullable<Guid> value, ParameterDirection direction)
    {
        AddGuidParameter(command, name, direction, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list that is used for a GUID field.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="direction">The Parameter direction.</param>
    /// <param name="nullable"></param>
    /// <param name="sourceColumn"></param>
    /// <param name="value">Parameter Value</param>
    public static void AddGuidParameter(this DbCommand command,
                                                string name,
                                                ParameterDirection direction,
                                                bool nullable,
                                                string sourceColumn,
                                                Nullable<Guid> value)
    {
        DbParameter param = command.CreateParameter();
        param.ParameterName = name;

        if (direction != ParameterDirection.Output)
            param.Value = verifyGuidDbTypeAndValue(command, param, value);
        else
            setGuidDbType(command, param);

        param.Direction = direction;
        param.IsNullable = nullable;
        param.SourceColumn = sourceColumn;

        command.Parameters.Add(param);
    }

    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list that is used for a varbinary(max) or BLOB field.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    public static void AddBinaryParameter(this DbCommand command, string name, byte[] value)
    {
        AddBinaryParameter(command, name, ParameterDirection.Input, true, string.Empty, value);
    }
    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list that is used for a varbinary(max) or BLOB field.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="value">Parameter Value</param>
    /// <param name="direction">The Parameter direction.</param>
    public static void AddBinaryParameter(this DbCommand command, string name, byte[] value, ParameterDirection direction)
    {
        AddBinaryParameter(command, name, direction, true, string.Empty, value);
    }



    /// <summary>
    /// Creates and adds a DbParameter to a DbCommand object's Parameters list that is used for a varbinary(max) or BLOB field.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name">ParameterName</param>
    /// <param name="direction">The Parameter direction.</param>
    /// <param name="nullable"></param>
    /// <param name="sourceColumn"></param>
    /// <param name="value">Parameter Value</param>
    public static void AddBinaryParameter(this DbCommand command,
                                                string name,
                                                ParameterDirection direction,
                                                bool nullable,
                                                string sourceColumn,
                                                byte[] value)
    {
        DbParameter param = command.CreateParameter();
        param.ParameterName = name;

        param.Value = verifyBinaryDbTypeAndValue(command, param, value);

        //This is required because Oracle has a limit of 32K on input parameters
        if (!supportsGuid(command) && direction == ParameterDirection.Input)
            param.Direction = ParameterDirection.InputOutput;
        else
            param.Direction = direction;

        param.IsNullable = nullable;
        param.SourceColumn = sourceColumn;
        command.Parameters.Add(param);
    }
    public static Guid? GetGuidParameterValue(this DbParameterCollection parameters, string parameterName)
    {
        for (int index = 0; index < parameters.Count; ++index)
        {
            DbParameter parameter = parameters[index];
            if (string.Compare(parameter.ParameterName, parameterName, true) != 0)
                continue;

            if (parameter.Value == null || parameter.Value == DBNull.Value)
                return null;

            try
            {
                Guid g = (Guid)parameter.Value;
                return g;
            }
            catch
            {
                //Guid not supported
            }
            try
            {
                Guid g = new Guid((byte[])parameter.Value);
                return g;
            }
            catch (Exception e)
            {
                throw new DataAccessException("Could not convert the data in the parameter to a Guid.", e);
            }

        }
        return null;
    }

    private static object verifyBinaryDbTypeAndValue(DbCommand command, DbParameter param, byte[] value)
    {
        param.DbType = DbType.Binary;
        string typeName = command.GetType().AssemblyQualifiedName??"";
        if (typeName.Contains("Oracle.ManagedDataAccess"))
        {
            typeName = typeName.Replace("OracleCommand", "OracleDbType");
            Type? enumType = Type.GetType(typeName);
            object? enumVal = enumType != null ? Enum.Parse(enumType, "Blob") : null;
            PropertyInfo? prop = param.GetType().GetRuntimeProperty("OracleDbType");
            prop?.SetValue(param, enumVal);
        }

        if (value != null)
        {
            param.Size = value.Length;
            return value;
        }

        return DBNull.Value;
    }

    private static object verifyGuidDbTypeAndValue(DbCommand command, DbParameter param, Nullable<Guid> value)
    {
        if (!value.HasValue)
            return DBNull.Value;

        if (supportsGuid(command))
        {
            param.DbType = DbType.Guid;
            return value.Value;
        }
        else
        {
            param.DbType = DbType.Binary;
            return value.Value.ToByteArray();
        }
    }
    private static void setGuidDbType(DbCommand command, DbParameter param)
    {
        if (supportsGuid(command))
        {
            param.DbType = DbType.Guid;
        }
        else
        {
            param.DbType = DbType.Binary;
            param.Size = 16;
        }
    }

    private static bool supportsGuid(DbCommand command)
    {
        if (command.GetType().FullName!.Contains("SqlCommand"))
            return true;

        if (command.GetType().FullName!.Contains("Oracle"))
            return false;

        if (command.Connection == null)
            return false;

        if (command.Connection.ConnectionString.ToUpper().Contains("ORACLE"))
            return false;

        return true;
    }
}