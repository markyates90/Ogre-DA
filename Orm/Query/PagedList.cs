using System.Collections.Generic;
namespace OgreDA.Orm.Query;

public class PagedList<T> where T : class, new()
{
  public List<T> Items { get; set; }
  public int TotalCount { get; set; }
  public int Take { get; set; }
  public int Skip { get; set; }

  public PagedList()
  {
    Items = new List<T>();
  }
}