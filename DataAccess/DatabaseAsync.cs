using System.Data;
using System.Data.Common;

namespace OgreDA.DataAccess
{

	public partial class Database
	{
		public async Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CommandBehavior behavior = CommandBehavior.Default, DbTransaction? transaction = null)
		{
			bool openedConnection = false;
			fixUpParameterizedStatementForProvider(command);
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			else
			{
				if (command.Connection == null)
				{
					DbConnection connection = OpenConnection(false);
					command.Connection = connection;
					openedConnection = true;
				}
				else if (command.Connection.State == ConnectionState.Closed)
				{
					command.Connection.Open();
					openedConnection = true;
				}
			}
			try
			{
				CommandBehavior b = openedConnection ? CommandBehavior.CloseConnection | behavior : behavior;
				return await command.ExecuteReaderAsync(b);
			}
			catch (Exception ex)
			{
				DataAccessException dax = new DataAccessException("Could not execute the ExecuteReader command.", ex);
				dax.Data.Add("Provider", _connectionString.ProviderName);
				dax.Data.Add("Connection String", _connectionString.ConnectionString);
				dax.Data.Add("Command Text", command.CommandText);
				throw dax;
			}
		}
		public Task ExecuteReaderAsync(DbCommand command, Action<DbDataReader> body, CommandBehavior behavior = CommandBehavior.Default, DbTransaction? transaction = null)
		{
			bool needToOpenConnection = command.Connection == null || command.Connection.State != ConnectionState.Open;
			if (needToOpenConnection)
			{
				return executeReaderAndManageConnectionAsync(command, body, behavior);
			}
			else
			{
				return executeReaderAsync(command, body, behavior, transaction);
			}
		}
		public Task<T> ExecuteReaderAsync<T>(DbCommand command, Func<DbDataReader, T> body, CommandBehavior behavior = CommandBehavior.Default, DbTransaction? transaction = null)
		{
			bool needToOpenConnection = command.Connection == null || command.Connection.State != ConnectionState.Open;
			if (needToOpenConnection)
			{
				return executeReaderAndManageConnectionAsync(command, body, behavior);
			}
			else
			{
				return executeReaderAsync(command, body, behavior, transaction);
			}
		}
		private async Task executeReaderAsync(DbCommand command, Action<DbDataReader> body, CommandBehavior behavior, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			using (DbDataReader reader = await ExecuteReaderAsync(command, behavior))
			{
				await Task.Run(() => body(reader));
			}
		}
		private async Task executeReaderAndManageConnectionAsync(DbCommand command, Action<DbDataReader> body, CommandBehavior behavior)
		{
			using (DbConnection connection = OpenConnection(false))
			{
				command.Connection = connection;
				using (DbDataReader reader = await ExecuteReaderAsync(command, behavior))
				{
					await Task.Run(() => body(reader));
				}
			}
		}
		private async Task<T> executeReaderAsync<T>(DbCommand command, Func<DbDataReader, T> body, CommandBehavior behavior, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			T result;
			using (DbDataReader reader = await ExecuteReaderAsync(command, behavior))
			{
				result = await Task.Run<T>(() => body(reader));
			}
			return result;
		}
		private async Task<T> executeReaderAndManageConnectionAsync<T>(DbCommand command, Func<DbDataReader, T> body, CommandBehavior behavior)
		{
			T result;
			using (DbConnection connection = OpenConnection(false))
			{
				command.Connection = connection;
				using (DbDataReader reader = await ExecuteReaderAsync(command, behavior))
				{
					result = await Task.Run(() => body(reader));
				}
			}
			return result;
		}
		public async Task<object?> ExecuteScalarAsync(DbCommand command)
		{
			object? result;
			bool connectionOpened = false;
			fixUpParameterizedStatementForProvider(command);
			// need to create connection
			if (command.Connection == null)
			{
				using (DbConnection connection = OpenConnection(false))
				{
					command.Connection = connection;
					try
					{
						result = await command.ExecuteScalarAsync();
					}
					catch (Exception ex)
					{
						DataAccessException dax = new DataAccessException("Could not execute the ExecuteScalar command.", ex);
						dax.Data.Add("Provider", _connectionString.ProviderName);
						dax.Data.Add("Connection String", _connectionString.ConnectionString);
						dax.Data.Add("Command Text", command.CommandText);
						throw dax;
					}
					finally
					{
						connection.Close();
						command.Connection = null;
					}
				}
				return result;
			}
			// use existing connection, but make sure it is open
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				connectionOpened = true;
			}
			try
			{
				result = await command.ExecuteScalarAsync();
			}
			catch (Exception ex)
			{
				DataAccessException dax = new DataAccessException("Could not execute non-query command.", ex);
				dax.Data.Add("Provider", _connectionString.ProviderName);
				dax.Data.Add("Connection String", _connectionString.ConnectionString);
				dax.Data.Add("Command Text", command.CommandText);
				throw dax;
			}
			finally
			{
				if (connectionOpened)
				{
					command.Connection.Close();
				}
			}

			return result;
		}
		public async Task<T?> ExecuteScalarAsync<T>(DbCommand command)
		{
			object? result = await ExecuteScalarAsync(command);
			try
			{
                if (result != null)
				    return (T)result;
                return default(T);
			}
			catch { }
            if (result != null)
    			return (T)Convert.ChangeType(result, typeof(T));
            return default(T);
		}
		public async Task<int> ExecuteNonQueryAsync(DbCommand command)
		{
			int result;
			bool connectionOpened = false;
			fixUpParameterizedStatementForProvider(command);
			// need to create connection
			if (command.Connection == null)
			{
				using (DbConnection connection = OpenConnection(false))
				{
					command.Connection = connection;
					try
					{
						result = await command.ExecuteNonQueryAsync();
					}
					catch (Exception ex)
					{
						DataAccessException dax = new DataAccessException("Could not execute non-query command.", ex);
						dax.Data.Add("Provider", _connectionString.ProviderName);
						dax.Data.Add("Connection String", _connectionString.ConnectionString);
						dax.Data.Add("Command Text", command.CommandText);
						throw dax;
					}
					finally
					{
						connection.Close();
						command.Connection = null;
					}
				}
				return result;
			}
			// use existing connection, but make sure it is open
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				connectionOpened = true;
			}
			try
			{
				result = await command.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				DataAccessException dax = new DataAccessException("Could not execute non-query command.", ex);
				dax.Data.Add("Provider", _connectionString.ProviderName);
				dax.Data.Add("Connection String", _connectionString.ConnectionString);
				dax.Data.Add("Command Text", command.CommandText);
				throw dax;
			}
			finally
			{
				if (connectionOpened)
				{
					command.Connection.Close();
				}
			}

			return result;
		}
	}
}
