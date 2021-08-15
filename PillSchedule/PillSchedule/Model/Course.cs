using System;
using SQLite;

namespace PillSchedule
{
  public enum eCourseType
  {
    Pill,
    Capsule,
    Drops,
    Mixture,
    Ointment,
    Injection,
    Procedure,
  }

  public enum eDosageType
  {
    Piece,
    Mg
  }

  public enum eCourseFreqType
  {
    Everyday,
    Nday,
  }

  public enum eFoodDependence
  {
    None,
    Before,
    During,
    After
  }

  public enum eCourseDurationType
  {
    Regular,
    Days,
    Receptions,
  }

  [Table("Course")]
  public class Course
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public eCourseType Type { get; set; }
    public double Dosage { get; set; }
    public eDosageType DosageType { get; set; }
    public int FreqInDay { get; set; }
    public int Freq { get; set; }
    public eCourseFreqType FreqType { get; set; }
    public eFoodDependence FoodDependence { get; set; }
    public int Duration { get; set; }
    public eCourseDurationType DurationType { get; set; }
    public DateTime StartDate { get; set; }
  }
}
