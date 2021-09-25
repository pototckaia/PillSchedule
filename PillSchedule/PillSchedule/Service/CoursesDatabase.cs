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
      db.Insert(course);
      foreach (var r in receptions)
      {
        db.Insert(new Reception
        {
          Id = 0,
          CourseId = course.Id,
          Time = r
        });
      }
      return course.Id;
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

    public List<Reception> GetCourseReceptions(int courseId)
    {
      return db.Table<Reception>().Where(x => x.CourseId == courseId).ToList();
    }

    public List<TimeSpan> GetCourseReceptionsTimeSpan(int courseId)
    {
      var receptions = GetCourseReceptions(courseId);
      var res = new List<TimeSpan>();
      foreach (var r in receptions)
      {
        res.Add(r.Time);
      }
      return res;
    }

    public List<CourseInstance> GetCourseInstance(DateTime date)
    {
      var d = date - date.TimeOfDay;
      var courses = db.Table<Course>().Where(course => course.StartDate <= d).ToList();
      var courseInstance = new List<CourseInstance>();
      foreach (var course in courses)
      {
        bool courseTakeInDay = false;
        int daysPassed = (int)Math.Round((d - course.StartDate).TotalDays); 
        switch (course.FreqType)
        {
          case eCourseFreqType.Everyday:
            courseTakeInDay = true;
            break;
          case eCourseFreqType.Nday:
            if (daysPassed % (course.Freq + 1) == 0)
            {
              courseTakeInDay = true;
              daysPassed = daysPassed / (course.Freq + 1);
            }
            break;
        }
        switch (course.DurationType)
        {
          case eCourseDurationType.Regular:
            courseTakeInDay &= true;
            break;
          case eCourseDurationType.Days:
            courseTakeInDay &= daysPassed < course.Duration;
            break;
          case eCourseDurationType.Receptions:
            courseTakeInDay &= daysPassed * course.FreqInDay < course.Duration;
            break;
        }

        if (courseTakeInDay)
        {
          var receptions = GetCourseReceptions(course.Id);
          var requiredRecipes = course.Duration - daysPassed * course.FreqInDay;
          foreach (var r in receptions)
          {
            if (course.DurationType == eCourseDurationType.Receptions && requiredRecipes <= 0)
            {
              break;
            }
            --requiredRecipes;
            courseInstance.Add(new CourseInstance
            {
              CourseId = course.Id,
              ReceptionId = r.Id,
              ReceptionTime = r.Time,
              CourseName = course.Name,
              CourseType = course.Type,
              CourseDosage = course.Dosage,
              CourseDosageType = course.DosageType,
              CourseFoodDependence = course.FoodDependence
            });
          }
        }
      }
      return courseInstance;
    }
  }
}
