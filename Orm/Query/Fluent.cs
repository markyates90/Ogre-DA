using OgreDA.DataAccess;

namespace OgreDA.Orm.Query;

public interface IFrom
{
    public IWhere From(string fromClause);
}

public interface IWhere : IQueryResult
{
    public IOrderBy Where(string whereClause);
}

public interface IOrderBy :IQueryResult
{
    public IQueryResult OrderBy(string orderByClause);
}

public interface IQueryResult
{
    public List<T> ToList<T>();
}

public class FluentSelect : IFrom, IWhere, IOrderBy, IQueryResult
{
    private SqlSelectClause command;
    private Database database;

    public FluentSelect(Database db, string columnsClause)
    {
        database = db;
        command = new SqlSelectClause();
        command.ColumnClause = columnsClause;
    }

    public IWhere From(string fromClause)
    {
        command.FromClause = fromClause;
        return this;
    }

    public IOrderBy Where(string whereClause)
    {
        command.WhereClause = whereClause;
        return this;
    }

    public IQueryResult OrderBy(string orderByClause)
    {
        command.OrderByClause = orderByClause;
        return this;
    }

    public List<T> ToList<T>()
    {
        return new List<T>();
    }
}

public static class DatabaseExtensions
{
    public static IFrom Select(this Database database, string columnsClause)
    {
        return new FluentSelect(database, columnsClause);
    }
}