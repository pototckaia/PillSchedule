using System;
using SQLite;

namespace PillSchedule
{
  [Table("Reception")]
  public class Reception
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int CourseId { get; set; }
    public TimeSpan Time { get; set; }
  }
}
