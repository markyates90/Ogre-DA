using System.Data.Common;
using OgreDA.DataAccess;

namespace OgreDA.Orm;

public static class DatabaseExtensions
{
    public static async Task<List<T>> QueryAsync<T>(this Database db, DbCommand command) where T : class, new()
    {
        List<T> result = new List<T>();
        await db.ExecuteReaderAsync(command, (reader) =>
        {
            DataReaderPropertySetters<T> setters = new DataReaderPropertySetters<T>(reader);
            while (reader.Read())
            {
                result.Add(reader.MapToObject(setters));
            }
        } );

        return result;
    }
}