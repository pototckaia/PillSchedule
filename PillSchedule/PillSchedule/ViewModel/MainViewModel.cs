using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Linq;

namespace PillSchedule.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Course> Courses { get; }
        public ObservableCollection<CourseInstance> CoursesInstance { get; }
        public Command CreateCourseCommand { get; set; }
        public Command<Course> TapCourseCommand { get; set; }

        private INavigation _navigation;
        private bool _isRefreshingСontent = false;
        private DateTime _selectedData = DateTime.Now;

        public MainViewModel(INavigation navigation)
        {
            Courses = new ObservableCollection<Course>();
            CoursesInstance = new ObservableCollection<CourseInstance>();
            TapCourseCommand = new Command<Course>(onTapCourse);
            CreateCourseCommand = new Command(onCreateCourse);
            _navigation = navigation;
            LoadCourses();
            LoadCourseInstance();
        }

        public void OnAppearing()
        {
            if (_isRefreshingСontent)
            {
                _isRefreshingСontent = false;
                LoadCourses();
                LoadCourseInstance();
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedData;
            set
            {
                _selectedData = value;
                LoadCourseInstance();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDate"));
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

        private void LoadCourseInstance()
        {
            CoursesInstance.Clear();
            foreach (var courseInstance in CoursesDatabase.Instance.GetCourseInstance(SelectedDate))
            {
                CoursesInstance.Add(courseInstance);
            }
        }

        private void onCreateCourse()
        {
            _isRefreshingСontent = true;
            _navigation.PushAsync(new CoursePage());
        }

        private void onTapCourse(Course course)
        {
            if (course != null)
            {
                _isRefreshingСontent = true;
                _navigation.PushAsync(new CoursePage(course));
            }
        }
    }
}
