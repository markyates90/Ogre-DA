using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;

namespace OgreDA.DataAccess
{
	public class ConfigFileManager
	{
		public static string SettingsFileName = "appsettings.json";
		private static object lockObj = new object();
		private static IConfigurationRoot configuration;
		private static NameValueCollection settings;

		private static void buildConfig()
		{
			if (configuration == null)
			{
				lock (lockObj)
				{
					if (configuration == null)
					{
						IConfigurationBuilder builder = new ConfigurationBuilder()
							.SetBasePath(Directory.GetCurrentDirectory())
							.AddJsonFile(SettingsFileName)
							.AddJsonFile($"appsettings.Development.json", optional: true);

						configuration = builder.Build();

						IConfigurationSection appSettingsSection = configuration.GetSection("AppSettings");
						settings = new NameValueCollection();
						if (appSettingsSection != null)
						{
							foreach (IConfigurationSection setting in appSettingsSection.GetChildren())
							{
								settings.Add(setting.Key, setting.Value);
							}
						}
					}
				}
			}
		}
		public static string GetSetting(string key)
		{
			buildConfig();
			return configuration[$"AppSettings:{key}"];
		}

		public static NameValueCollection AppSettings
		{
			get
			{
				buildConfig();
				return settings;
			}
		}
		public static ConnectionStringSettings GetConnectionString(string name)
		{
			buildConfig();
			var csSetting = configuration.GetSection($"ConnectionStrings:{name}");
			if (csSetting == null) return null;

			if (csSetting.Value != null) return new ConnectionStringSettings(name, null, csSetting.Value);

			ConnectionStringSettings cs = new ConnectionStringSettings();
			csSetting.Bind(cs);
			return cs;

			return configuration.GetSection($"ConnectionStrings:{name}").Get<ConnectionStringSettings>();
		}
	}
}
