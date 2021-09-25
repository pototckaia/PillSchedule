using System;
using SQLite;

namespace PillSchedule
{
  public class CourseInstance
  {
    public int CourseId { get; set; }
    public int ReceptionId { get; set; }
    public TimeSpan ReceptionTime { get; set; }
    public string CourseName { get; set; }
    public eCourseType CourseType { get; set; }
    public double CourseDosage { get; set; }
    public eDosageType CourseDosageType { get; set; }
    public eFoodDependence CourseFoodDependence { get; set; }
  }
}
