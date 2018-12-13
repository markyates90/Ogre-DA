using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace OgreDA.DataAccess
{
	public class DbProviderFactories
	{
		public static DbProviderFactory GetFactory(string providerName)
		{
			return System.Data.SqlClient.SqlClientFactory.Instance;
		}
	}
}
