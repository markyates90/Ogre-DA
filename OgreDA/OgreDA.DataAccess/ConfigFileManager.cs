using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace OgreDA.DataAccess
{
	public class ConfigFileManager
	{
		public static NameValueCollection AppSettings
		{
			get
			{
				return ConfigurationManager.AppSettings;
			}
		}
		public static ConnectionStringSettings GetConnectionString(string name)
		{
			return ConfigurationManager.ConnectionStrings[name];
		}

	}
}
