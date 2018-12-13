using System;
using System.Data;
using System.Data.Common;

namespace OgreDA.DataAccess
{
	public partial class Database
	{
	
		private bool handleProviderSpecificParameters(DbCommand command)
		{
			return false;
		}

	}
}