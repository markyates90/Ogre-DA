namespace OgreDA.Orm.Query;

public class SqlSelectClause
{
    public string ColumnClause { get; set; }
    public string FromClause { get; set; }
    public string WhereClause { get; set; }
    public string OrderByClause { get; set; }
}

public static class SqlSelectClauseExtensions
{
    public static string ToCommandText(this SqlSelectClause selectClause)
    {
        return $"SELECT {selectClause.ColumnClause} " +
            $"FROM {selectClause.FromClause} " +
            $"WHERE {selectClause.WhereClause}" +
            $"ORDER BY {selectClause.OrderByClause}";
    }
    public static string ToCountCommand(this SqlSelectClause selectClause)
    {
        return $"SELECT COUNT(*) " +
            $"FROM {selectClause.FromClause} " +
            $"WHERE {selectClause.WhereClause}";
    }
}


