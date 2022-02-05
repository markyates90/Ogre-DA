using System.Data;
using System.Data.Common;

namespace OgreDA.DataAccess
{
	/// <summary>
	/// ADO.Net facade to simplify database access.
	/// </summary>
	public partial class Database
	{
		/// <summary>
		/// Creates a DbDataAdapter object using the appropriate classes for the provider.
		/// </summary>
		/// <returns>A reference to the created DbDataAdapter object.</returns>
		public DbDataAdapter? CreateDataAdapter()
		{
			return providerFactory.CreateDataAdapter();
		}
		/// <summary>
		/// Creates a DbCommandBuilder object using the appropriate classes for the provider.
		/// </summary>
		/// <returns>A reference to the created DbCommandBuilder object.</returns>
		public DbCommandBuilder? CreateCommandBuilder()
		{
			return providerFactory.CreateCommandBuilder();
		}
		/// <summary>
		/// Executes the Fill method on the data adapter to add or refresh rows in the dataset.
		/// 
		/// Handles the creation of the connection, if it is not provided on the command.
		/// Fill will handle the opening/closing of the connection, if it is needed.
		/// </summary>
		/// <exception cref="OgreDA.DataAccess.DataAccessException">
		/// Thrown if the select command is not provided.
		/// </exception>
		/// <param name="adapter">The DbDataAdapter object with the select command to execute.</param>
		/// <param name="ds">The DataSet object to be filled.</param>
		public void FillDataAdapter(DbDataAdapter adapter, DataSet ds)
		{
			if (adapter.SelectCommand == null)
			{
				throw new DataAccessException("Could not fill adapter without a command.", new ArgumentNullException("adapter.SelectCommand"));
			}
			if (adapter.SelectCommand.Connection == null)
			{
				adapter.SelectCommand.Connection = CreateConnection();
			}
			adapter.Fill(ds);
		}
	}
}
