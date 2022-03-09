namespace OgreDA.Orm.Query;

public class SqlSelectClause
{
    public string? ColumnClause { get; set; }
    public string? FromClause { get; set; }
    public string? WhereClause { get; set; }
    public string? OrderByClause { get; set; }
}

public static class SqlSelectClauseExtensions
{
    public static string ToCommandText(this SqlSelectClause selectClause)
    {
        if (string.IsNullOrEmpty(selectClause.ColumnClause) || string.IsNullOrEmpty(selectClause.FromClause))
        {
            throw new ArgumentException("ColumnClause and FromClause must be set to have a valid Select Statement.");
        }
        return $"SELECT {selectClause.ColumnClause}" +
            $" FROM {selectClause.FromClause} " +
            (string.IsNullOrEmpty(selectClause.WhereClause) ? "" : $" WHERE {selectClause.WhereClause}") +
            (string.IsNullOrEmpty(selectClause.OrderByClause) ? "" : $" ORDER BY {selectClause.OrderByClause}");
    }
    public static string ToCountCommand(this SqlSelectClause selectClause)
    {
        if (string.IsNullOrEmpty(selectClause.ColumnClause) || string.IsNullOrEmpty(selectClause.FromClause))
        {
            throw new ArgumentException("ColumnClause and FromClause must be set to have a valid Select Statement.");
        }
        return $"SELECT COUNT(*)" +
            $" FROM {selectClause.FromClause} " +
            (string.IsNullOrEmpty(selectClause.WhereClause) ? "" : $" WHERE {selectClause.WhereClause}");
    }
}


