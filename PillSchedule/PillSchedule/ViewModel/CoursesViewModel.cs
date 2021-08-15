using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Linq;

namespace PillSchedule.ViewModel
{
  class CoursesViewModel: INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<Course> Courses { get; }

    public CoursesViewModel()
    {
      Courses = new ObservableCollection<Course>();
      LoadCourses();
    }

    void LoadCourses()
    {
      Courses.Clear();
      foreach (var courses in CoursesDatabase.Instance.GetCourses())
      {
        Courses.Add(courses);
      }
    }

  }
}
