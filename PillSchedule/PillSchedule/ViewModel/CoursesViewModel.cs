using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Linq;

namespace PillSchedule.ViewModel
{
  class CoursesViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<Course> Courses { get; }
    public Command CreateCourseCommand { get; set; }
    public Command<Course> TapCourseCommand { get; set; }
    public Command RefreshCoursesCommand { get; set; }

    private INavigation _navigation;
    private bool _isRefreshingCourses = false;

    public CoursesViewModel(INavigation navigation)
    {
      Courses = new ObservableCollection<Course>();
      TapCourseCommand = new Command<Course>(onTapCourse);
      CreateCourseCommand = new Command(onCreateCourse);
      RefreshCoursesCommand = new Command(onRefreshCourses);
      _navigation = navigation;
      LoadCourses();
    }

    public bool IsRefreshingCourses
    {
      get => _isRefreshingCourses;
      set
      {
        if (value != _isRefreshingCourses)
        {
          _isRefreshingCourses = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshingCourses"));
        }
      }
    }

    private void LoadCourses()
    {
      Courses.Clear();
      foreach (var courses in CoursesDatabase.Instance.GetCourses())
      {
        Courses.Add(courses);
      }
    }

    private void onCreateCourse()
    {
      _navigation.PushAsync(new CoursePage());
    }

    private void onTapCourse(Course course)
    {
      if (course != null)
      {
        _navigation.PushAsync(new CoursePage(course));
      }
    }

    private void onRefreshCourses()
    {
      LoadCourses();
      IsRefreshingCourses = false;
    }

    public void OnAppearing()
    {
      IsRefreshingCourses = true;
      LoadCourses();
      IsRefreshingCourses = false;
    }
  }
}
