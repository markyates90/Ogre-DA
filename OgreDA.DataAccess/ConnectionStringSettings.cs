namespace System.Configuration
{
	public class ConnectionStringSettings
	{
		public string Name { get; set; }
		public string ProviderName { get; set; }
		public string ConnectionString { get; set; }

		public ConnectionStringSettings() { }
		public ConnectionStringSettings(string name, string connectionString, string provider)
		{
			Name = name;
			ConnectionString = connectionString;
			ProviderName = provider;
		}
	}
}
