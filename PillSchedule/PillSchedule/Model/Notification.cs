using System;
using SQLite;

namespace PillSchedule
{
    [Table("Notification")]
    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime Date { get; set; }
        public int DaysPassed { get; set; }
        public int ReceptionsPassed { get; set; }
        public int ReceptionInDayIndex { get; set; }
    }
}
