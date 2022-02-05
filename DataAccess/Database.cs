using System.Data;
using System.Data.Common;

namespace OgreDA.DataAccess
{
	/// <summary>
	/// ADO.Net facade to simplify database access.
	/// </summary>
	public partial class Database : IDatabaseWriter
	{
		public static int DefaultCommandTimeout = 30;
		private DbProviderFactory providerFactory;
		private DBConnectionInfo _connectionString;
		private DBMS dbEngine;
		private DbConnection? _connection;

		public string Provider
		{
			get { return _connectionString.ProviderName; }
		}
		public DBMS DBEngine
		{
			get { return dbEngine; }
		}
		/// <summary>
		/// Creates an instance of the database facade using the database connection information passed.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if connectionString is null.
		/// </exception>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown when the connection contains an unrecognized provider.
		/// </exception>
		/// <param name="connectionString">Connection String to use to connect to the database.</param>
		public Database(DBConnectionInfo connectionString)
		{
			if (connectionString == null || string.IsNullOrEmpty(connectionString.ConnectionString))
			{
				throw new ArgumentNullException("connectionString cannot be null or empty.");
			}
			_connectionString = connectionString;
			providerFactory = setupProvider(connectionString);
		}
		/// <summary>
		/// Creates an instance of the database facade using the provider and connection string passed.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if connectionString is null or empty.
		/// </exception>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown when the providerName contains an unrecognized provider.
		/// </exception>
		/// <param name="providerName">The provider to use. Can be System.Data.SqlClient, System.Data.OleDb or
		/// the invariant name of a DbProviderFactories setting in the config file.</param>
		/// <param name="connectionString">The connection string to use.</param>
		public Database(string providerName, string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString cannot be null or empty.");
			}
			_connectionString = new DBConnectionInfo(connectionString, providerName);
			providerFactory = setupProvider(_connectionString);
		}
		/// <summary>
		/// Creates a connection to the database. This class will hold onto a reference to the connection
		/// object if useDefaultConnection is true. The other "Create" methods can then use this reference
		/// to the connection by also passing true for useDefaultConnection.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown with the class instance has no database connection information. Use a constructor that passes
		/// the connection information.
		/// </exception>
		/// <param name="useDefaultConnection">Instructs the class instance to hold a reference to the connection
		/// object for use by other calls to methods on the class.</param>
		/// <returns>A reference to the created connection object.</returns>
		public DbConnection CreateConnection(bool useDefaultConnection = false)
		{
			// verify that a connection string exists
			if (_connectionString == null || string.IsNullOrEmpty(_connectionString.ConnectionString))
			{
				throw new DataAccessException("Cannot create a connection because a database connection string was not provided.");
			}
			if (useDefaultConnection && _connection != null)
			{
				return _connection;
			}
			else
			{
				DbConnection? connection = providerFactory.CreateConnection();
				if (connection != null)
				{
					connection.ConnectionString = _connectionString.ConnectionString;
					if (useDefaultConnection)
					{
						_connection = connection;
					}
					return connection;
				}
				throw new DataAccessException("Connection could not be created.");
			}
		}
		/// <summary>
		/// Opens a connection to the database. If using the default connection on the class instance 
		/// and the connection has not yet been created, the default connection will be created before 
		/// attempting to open the connection. If not using the default connection, a new connection
		/// object will be created and opened.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the database connection cannot be opened.
		/// </exception>
		/// <param name="useDefaultConnection">Indicates whether the default connection object associated with the
		/// class instance should be used or if a new connection should be created.</param>
		/// <returns>A reference to the connection that was opened.</returns>
		public DbConnection OpenConnection(bool useDefaultConnection = false)
		{
			DbConnection connection = CreateConnection(useDefaultConnection);
			try
			{
				connection.Open();
			}
			catch (Exception ex)
			{
				DataAccessException dax = new DataAccessException("The database connection could not be opened.", ex);
				dax.Data.Add("Provider", _connectionString.ProviderName);
				dax.Data.Add("Connection String", _connectionString.ConnectionString);
				throw dax;
			}
			return connection;
		}
		/// <summary>
		/// Creates a DbCommand object using the appropriate classes for the provider.
		/// </summary>
		/// <param name="useDefaultConnection">Indicates whether the default connection object associated with the
		/// class instance should be used. If false, the Command's Connection property should be set manually. If it 
		/// is not set, a temporary connection object will automatically be created when a command is executed.</param>
		/// <param name="commandTimeout">Wait time for a command before teminating and generating an error, in seconds. </param>
		/// <returns>A reference to the created DbCommand object.</returns>
		public DbCommand CreateCommand(bool useDefaultConnection = false, int commandTimeout = -1)
		{
			DbCommand? command = providerFactory.CreateCommand();
			if (command != null)
			{
				if (useDefaultConnection)
				{
					command.Connection = _connection;
				}

				if (commandTimeout >= 0)
				{
					command.CommandTimeout = commandTimeout;
				}
				else if (command.CommandTimeout > 0)
				{
					command.CommandTimeout = DefaultCommandTimeout;
				}
				return command;
			}
			throw new DataAccessException("Command could not be created.");
		}
		/// <summary>
		/// Creates a DbCommand object using the appropriate classes for the provider and sets its CommandType
		/// to StoredProcedure.
		/// </summary>
		/// <param name="storedProcName">The name of the stored procedure to associate with the command.</param>
		/// <param name="useDefaultConnection">Indicates whether the default connection object associated with the
		/// class instance should be used. If false, the Command's Connection property should be set manually. If it 
		/// is not set, a temporary connection object will automatically be created when a command is executed.</param>
		/// <param name="commandTimeout">Wait time for a command before teminating and generating an error, in seconds. </param>
		/// <returns>A reference to the created DbCommand object.</returns>
		public DbCommand GetStoredProcCommand(string storedProcName, bool useDefaultConnection = false, int commandTimeout = -1)
		{
			DbCommand command = CreateCommand(commandTimeout: commandTimeout);
			command.CommandText = storedProcName;
			command.CommandType = System.Data.CommandType.StoredProcedure;
			if (useDefaultConnection)
			{
				command.Connection = _connection;
			}
			return command;
		}
		/// <summary>
		/// Creates a DbParameter object using the appropriate classes for the provider and sets is ParameterName
		/// property to the passed name. As an alternative, use the AddParameter extension methods that have been
		/// added to DbCommand.
		/// </summary>
		/// <param name="name">The name to use for the ParameterName property.</param>
		/// <returns>A reference to the created DbParameter object.</returns>
		public DbParameter CreateParameter(string name)
		{
			DbParameter? param = providerFactory.CreateParameter();
			if (param != null)
			{
				param.ParameterName = name;
				return param;
			}
			throw new DataAccessException("Could not create Parameter");
		}
		/// <summary>
		/// Executes a Non-Query command using the passed DbCommand object on the database.
		/// 
		/// An attempt will be made to adjust an parameterized commands to match the syntax required
		/// by the provider. Parameterized commands should be defined using named parameters as supported
		/// by SQL server.
		/// 
		/// If a connection is not associated with the Command, a connection will be created.
		/// If the associated connection is not open, the connection will be opened and closed 
		/// automatically.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the database connection cannot be opened or if the query fails.
		/// </exception>
		/// <param name="command">The DbCommand object with the command to execute.</param>
		/// <returns>The result of the Non-Query command; typically the number of rows affected by the command.</returns>
		public int ExecuteNonQuery(DbCommand command)
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
						result = command.ExecuteNonQuery();
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
				result = command.ExecuteNonQuery();
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
		/// <summary>
		/// Executes a Non-Query command using the passed DbCommand object on the database. The command will
		/// participate in the passed transaction.
		/// 
		/// An attempt will be made to adjust an parameterized commands to match the syntax required
		/// by the provider. Parameterized commands should be defined using named parameters as supported
		/// by SQL server.
		/// 
		/// A connection must already be associated with the command (either manually or by using the default
		/// connection when the command was created) and the connection must already be open.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the query fails.
		/// </exception>
		/// <param name="command">The DbCommand object with the command to execute.</param>
		/// <param name="transaction">The DbTransaction object for the transaction.</param>
		/// <returns>The result of the Non-Query command; typically the number of rows affected by the command.</returns>
		public int ExecuteNonQuery(DbCommand command, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			fixUpParameterizedStatementForProvider(command);
			try
			{
				return command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				DataAccessException dax = new DataAccessException("Could not execute non-query command.", ex);
				dax.Data.Add("Provider", _connectionString.ProviderName);
				dax.Data.Add("Connection String", _connectionString.ConnectionString);
				dax.Data.Add("Command Text", command.CommandText);
				throw dax;
			}
		}
		/// <summary>
		/// Executes an ExecuteReader command using the passed DbCommand object on the database.
		/// 
		/// An attempt will be made to adjust an parameterized commands to match the syntax required
		/// by the provider. Parameterized commands should be defined using named parameters as supported
		/// by SQL server.
		/// 
		/// If a connection is not associated with the Command, a connection will be created.
		/// If the associated connection is not open, the connection will be opened. It is the 
		/// callers responsibility to close the DbDataReader and the connection when done with
		/// the reader.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the database connection cannot be opened or if the query fails.
		/// </exception>
		/// <param name="command">The DbCommand object with the command to execute.</param>
		/// <returns>A reference to a DbDataReader to read the results of the query.</returns>
		public DbDataReader ExecuteReader(DbCommand command)
		{
			return ExecuteReader(command, CommandBehavior.Default);
		}
		public DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior)
		{
			bool openedConnection = false;
			fixUpParameterizedStatementForProvider(command);
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
			try
			{
				return openedConnection ? command.ExecuteReader(CommandBehavior.CloseConnection | behavior) : command.ExecuteReader(behavior);
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
		/// <summary>
		/// Executes an ExecuteReader command using the passed DbCommand object on the database.
		/// 
		/// An attempt will be made to adjust an parameterized commands to match the syntax required
		/// by the provider. Parameterized commands should be defined using named parameters as supported
		/// by SQL server.
		/// 
		/// A connection must already be associated with the command (either manually or by using the default
		/// connection when the command was created) and the connection must already be open. It is the 
		/// callers responsibility to close the DbDataReader and the connection when done with
		/// the reader.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the query fails.
		/// </exception>
		/// <param name="command">The DbCommand object with the command to execute.</param>
		/// <param name="transaction">The DbTransaction object for the transaction.</param>
		/// <returns>A reference to a DbDataReader to read the results of the query.</returns>
		public DbDataReader ExecuteReader(DbCommand command, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			fixUpParameterizedStatementForProvider(command);
			try
			{
				return command.ExecuteReader();
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
		public void ExecuteReader(DbCommand command, Action<DbDataReader> body, DbTransaction? transaction = null)
		{
			ExecuteReader(command, body, CommandBehavior.Default, transaction);
		}
		public void ExecuteReader(DbCommand command, Action<DbDataReader> body, CommandBehavior behavior, DbTransaction? transaction = null)
		{
			bool needToOpenConnection = command.Connection == null || command.Connection.State != ConnectionState.Open;
			if (needToOpenConnection)
			{
				executeReaderAndManageConnection(command, body, behavior);
			}
			else
			{
				executeReader(command, body, behavior, transaction);
			}
		}
		public T ExecuteReader<T>(DbCommand command, Func<DbDataReader, T> body, DbTransaction? transaction = null)
		{
			return ExecuteReader<T>(command, body, CommandBehavior.Default, transaction);
		}
		public T ExecuteReader<T>(DbCommand command, Func<DbDataReader, T> body, CommandBehavior behavior, DbTransaction? transaction = null)
		{
			bool needToOpenConnection = command.Connection == null || command.Connection.State != ConnectionState.Open;
			if (needToOpenConnection)
			{
				return executeReaderAndManageConnection(command, body, behavior);
			}
			else
			{
				return executeReader(command, body, behavior, transaction);
			}
		}
		private void executeReader(DbCommand command, Action<DbDataReader> body, CommandBehavior behavior, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			using (DbDataReader reader = ExecuteReader(command, behavior))
			{
				body(reader);
			}
		}
		private void executeReaderAndManageConnection(DbCommand command, Action<DbDataReader> body, CommandBehavior behavior)
		{
			using (DbConnection connection = OpenConnection(false))
			{
				command.Connection = connection;
				using (DbDataReader reader = ExecuteReader(command, behavior))
				{
					body(reader);
				}
			}
		}
		private T executeReader<T>(DbCommand command, Func<DbDataReader, T> body, CommandBehavior behavior, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			using (DbDataReader reader = ExecuteReader(command, behavior))
			{
				return body(reader);
			}
		}
		private T executeReaderAndManageConnection<T>(DbCommand command, Func<DbDataReader, T> body, CommandBehavior behavior)
		{
			using (DbConnection connection = OpenConnection(false))
			{
				command.Connection = connection;
				using (DbDataReader reader = ExecuteReader(command, behavior))
				{
					return body(reader);
				}
			}
		}

		/// <summary>
		/// Executes an ExecuteScalar command using the passed DbCommand object on the database.
		/// 
		/// An attempt will be made to adjust an parameterized commands to match the syntax required
		/// by the provider. Parameterized commands should be defined using named parameters as supported
		/// by SQL server.
		/// 
		/// If a connection is not associated with the Command, a connection will be created.
		/// If the associated connection is not open, the connection will be opened and closed 
		/// automatically.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the database connection cannot be opened or if the query fails.
		/// </exception>
		/// <param name="command">The DbCommand object with the command to execute.</param>
		/// <returns>The value of the first column of the first result of the command.</returns>
		public object? ExecuteScalar(DbCommand command)
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
						result = command.ExecuteScalar();
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
				result = command.ExecuteScalar();
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
		/// <summary>
		/// Executes an ExecuteScalar command using the passed DbCommand object on the database.
		/// 
		/// An attempt will be made to adjust an parameterized commands to match the syntax required
		/// by the provider. Parameterized commands should be defined using named parameters as supported
		/// by SQL server.
		/// 
		/// A connection must already be associated with the command (either manually or by using the default
		/// connection when the command was created) and the connection must already be open.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the query fails.
		/// </exception>
		/// <param name="command">The DbCommand object with the command to execute.</param>
		/// <param name="transaction">The DbTransaction object for the transaction.</param>
		/// <returns>The value of the first column of the first result of the command.</returns>
		public object? ExecuteScalar(DbCommand command, DbTransaction? transaction)
		{
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			fixUpParameterizedStatementForProvider(command);
			try
			{
				return command.ExecuteScalar();
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

		public T? ExecuteScalar<T>(DbCommand command)
		{
			object? result = ExecuteScalar(command);
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

		private DbProviderFactory setupProvider(DBConnectionInfo connectionString)
		{
			if (connectionString == null)
			{
				throw new DataAccessException("Could not setup Db", new ArgumentNullException("connectString"));
			}

			try
			{
				//accepts trailing whitespace and variable casing
				DbProviderFactory? pf = DbProviderFactories.GetFactory(connectionString.ProviderName);
				if (pf == null)
				{
					throw new DataAccessException("A DbProvider Factory was not created.");
				}
				setDbEngine(connectionString, pf);
				return pf;
			}
			catch (ArgumentException)
			{
				throw new DataAccessException("Provider not recognized");
			}
		}
		private void setDbEngine(DBConnectionInfo connectionString, DbProviderFactory pf)
		{
			if (pf.GetType().FullName!.Contains("SqlClientFactory"))
				dbEngine = DBMS.SqlServer;

			else if (pf.GetType().FullName!.Contains("Oracle"))
				dbEngine = DBMS.Oracle;

			else if (connectionString.ConnectionString.ToUpper().Contains("ORACLE"))
				dbEngine = DBMS.Oracle;

			else if (connectionString.ConnectionString.ToUpper().Contains("SQLOLEDB"))
				dbEngine = DBMS.SqlServer;

			else
				dbEngine = DBMS.Other;

		}
		private void fixUpParameterizedStatementForProvider(DbCommand command)
		{
			if (!handleProviderSpecificParameters(command) && command.GetType().FullName!.Contains("Oracle"))
			{
				command.CommandText = command.CommandText.Replace('@', ':');
			}

			if (dbEngine == DBMS.Oracle)
			{
				command.CommandText = command.CommandText.Replace("ISNULL(", "NVL(");
			}
		}
		private bool handleProviderSpecificParameters(DbCommand command)
		{
			if (command.GetType().FullName!.Contains("OleDbCommand"))
			{
				replaceParametersWithPlaceHolder(command);
				return true;
			}
			return false;
		}
		private void replaceParametersWithPlaceHolder(DbCommand command)
		{
			string commandText = command.CommandText;
			// must replace named parameters with ? place holders
			foreach (DbParameter param in command.Parameters)
			{
				string paramName = param.ParameterName;
				if (!paramName.StartsWith("@"))
				{
					paramName = "@" + paramName;
				}
				string pattern = paramName + @"\b";
				commandText = System.Text.RegularExpressions.Regex.Replace(commandText, pattern, "?");
			}
			command.CommandText = commandText;
		}
	}
}
