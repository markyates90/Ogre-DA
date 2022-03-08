using System.Collections.Generic;
using System.Data.Common;
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
    public Task<List<T>> ToListAsync<T>() where T : class, new();
    public Task<PagedList<T>> ToPagedListAsync<T>(int take, int skip) where T : class, new();
    public IQueryResult AddParameter(string name, object value);
}

public class FluentSelect : IFrom, IWhere, IOrderBy, IQueryResult
{
    private SqlSelectClause sqlSelect;
    private Database database;
    private DbCommand command;

    public FluentSelect(Database db, string columnsClause)
    {
        database = db;
        command = db.CreateCommand();
        sqlSelect = new SqlSelectClause();
        sqlSelect.ColumnClause = columnsClause;
    }

    public IWhere From(string fromClause)
    {
        sqlSelect.FromClause = fromClause;
        return this;
    }

    public IOrderBy Where(string whereClause)
    {
        sqlSelect.WhereClause = whereClause;
        return this;
    }

    public IQueryResult OrderBy(string orderByClause)
    {
        sqlSelect.OrderByClause = orderByClause;
        return this;
    }

    public IQueryResult AddParameter(string name, object value)
    {
        command.AddParameter(name, value);
        return this;
    }

    public async Task<List<T>> ToListAsync<T>() where T : class, new()
    {
        command.CommandText = sqlSelect.ToCommandText();
        return await database.QueryAsync<T>(command);
    }
    public async Task<PagedList<T>> ToPagedListAsync<T>(int take, int skip) where T : class, new()
    {
        PagedList<T> result = new PagedList<T>() {Take=take, Skip=skip};
        command.CommandText = sqlSelect.ToCommandText() + $" OFFSET ({skip}) ROWS FETCH NEXT {take} ROWS ONLY";
        Task<List<T>> listTask = database.QueryAsync<T>(command);
        command.CommandText = sqlSelect.ToCountCommand();
        result.TotalCount = await database.ExecuteScalarAsync<int>(command);
        result.Items = await listTask;

        return result;
    }
}

public static class DatabaseExtensions
{
    public static IFrom Select(this Database database, string columnsClause)
    {
        return new FluentSelect(database, columnsClause);
    }
}