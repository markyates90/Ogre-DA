namespace OgreDA.DataAccess;

public class DBConnectionInfo
{
    public string ProviderName { get; set; }
    public string ConnectionString { get; set; }

    public DBConnectionInfo(string connectionString, string provider)
    {
        ConnectionString = connectionString;
        ProviderName = provider;
    }
}