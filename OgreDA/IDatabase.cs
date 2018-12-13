using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace OgreDA.DataAccess
{
	public interface IDatabaseWriter
	{
		int ExecuteNonQuery(DbCommand command);
		int ExecuteNonQuery(DbCommand command, DbTransaction transaction);
		Task<int> ExecuteNonQueryAsync(DbCommand command);
		object ExecuteScalar(DbCommand command);
		object ExecuteScalar(DbCommand command, DbTransaction transaction);
		T ExecuteScalar<T>(DbCommand command);
		Task<object> ExecuteScalarAsync(DbCommand command);
		Task<T> ExecuteScalarAsync<T>(DbCommand command);
	}
}
