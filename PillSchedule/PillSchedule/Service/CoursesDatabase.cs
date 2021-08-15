using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using SQLite;

namespace PillSchedule
{
  class CoursesDatabase
  {
    private static CoursesDatabase _instance;
    private readonly SQLiteConnection db;

    public static CoursesDatabase Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new CoursesDatabase();
        }
        return _instance;
      }
    }

    private CoursesDatabase()
    {
      var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Personal),
        "courses.db");
      db = new SQLiteConnection(path);
      createTables();
    }

    private void createTables()
    {
      db.CreateTable<Course>();
      db.CreateTable<Reception>();
    }

    public int CreateCourse(Course course, List<TimeSpan> receptions)
    {
      var id = db.Insert(course);
      foreach (var r in receptions)
      {
        db.Insert(new Reception
        {
          Id = 0,
          CourseId = id,
          Time = r
        });
      }
      return id;
    }

    public void UpdateCourse(Course course, List<TimeSpan> receptions)
    {
      db.Update(course);
      db.Table<Reception>().Where(x => x.CourseId == course.Id).Delete();
      foreach (var r in receptions)
      {
        db.Insert(new Reception
        {
          Id = 0,
          CourseId = course.Id,
          Time = r
        });
      }
    }

    public void DeleteCourse(int id)
    {
      db.Delete<Course>(id);
      db.Table<Reception>().Where(x => x.CourseId == id).Delete();
    }

    public Course GetCourse(int id)
    {
      return db.Get<Course>(id);
    }

    public List<Course> GetCourses()
    {
      return db.Table<Course>().ToList();
    }

    public List<TimeSpan> GetCourseReceptions(int courseId)
    {
      var receptions = db.Table<Reception>().Where(x => x.CourseId == courseId).ToList();
      var res = new List<TimeSpan>();
      foreach (var r in receptions)
      {
        res.Add(r.Time);
      }
      return res;
    }
  }
}
