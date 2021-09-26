using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Linq;
using PillSchedule.Service;

namespace PillSchedule
{
    public class TimePickerDate
    {
        public event Action onTimeChange = delegate { };
        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set
            {
                _time = value;
                onTimeChange();
            }
        }
    }

    class CourseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command SaveCourseCommand { get; set; }
        public Command DeleteCourseCommand { get; set; }

        private Course _course;
        private ObservableCollection<TimePickerDate> _receptionTime;
        private bool _isCreateMode;
        private bool _isCourseFreqEnable;
        private bool _isCourseDurationEnable;
        private bool _isLockCommand;
        private Dictionary<string, string> _validationErrors;
        private INavigation _navigation;

        public CourseViewModel(INavigation navigation, Course course = null)
        {
            SaveCourseCommand = new Command(onSaveCourse);
            DeleteCourseCommand = new Command(onDeleteCourse);
            _validationErrors = new Dictionary<string, string>();
            _receptionTime = new ObservableCollection<TimePickerDate>();
            _isLockCommand = false;
            _navigation = navigation;

            var times = new List<TimeSpan>();
            if (course != null)
            {
                _course = course;
                times = CoursesDatabase.Instance.GetCourseReceptionsTimeSpan(course.Id);
            }
            else
            {
                _course = new Course
                {
                    Type = eCourseType.Capsule,
                    Dosage = 1,
                    DosageType = eDosageType.Piece,
                    FreqInDay = 1,
                    Freq = 0,
                    FreqType = eCourseFreqType.Everyday,
                    FoodDependence = eFoodDependence.None,
                    Duration = 0,
                    DurationType = eCourseDurationType.Regular,
                    StartDate = DateTime.Now - DateTime.Now.TimeOfDay,
                };
                for (int i = 0; i < _course.FreqInDay; ++i)
                {
                    times.Add(new TimeSpan(0, 0, 0));
                }
            }
            isCreateMode = course == null;
            isCourseFreqEnable = _course.FreqType != eCourseFreqType.Everyday;
            isCourseDurationEnable = _course.DurationType != eCourseDurationType.Regular;
            foreach (var time in times)
            {
                var pickerDate = new TimePickerDate { Time = time };
                pickerDate.onTimeChange += onReceptionTimeChange;
                CourseReceptionTime.Add(pickerDate);
            }
        }

        public bool isCreateMode
        {
            get => _isCreateMode;
            set
            {
                if (value != _isCreateMode)
                {
                    _isCreateMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool isEditMode { get => !_isCreateMode; }

        public bool isCourseFreqEnable
        {
            get => _isCourseFreqEnable;
            set
            {
                if (value != _isCourseFreqEnable)
                {
                    _isCourseFreqEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool isCourseDurationEnable
        {
            get => _isCourseDurationEnable;
            set
            {
                if (value != _isCourseDurationEnable)
                {
                    _isCourseDurationEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CourseName
        {
            get => _course.Name;
            set
            {
                if (value != null && _course.Name != value)
                {
                    _course.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CourseType
        {
            get => (int)_course.Type;
            set
            {
                if (value != (int)_course.Type)
                {
                    _course.Type = (eCourseType)value;
                    OnPropertyChanged();
                }
            }
        }

        public string CourseDosage
        {
            get => _course.Dosage.ToString();
            set
            {
                if (double.TryParse(value, out double dosage))
                {
                    if (!_course.Dosage.Equals(dosage))
                    {
                        _course.Dosage = dosage;
                        OnPropertyChanged();
                        if (dosage <= 0)
                        {
                            RaiseErrorsChanged("Доза должна быть больше нуля");
                        }
                        else
                        {
                            RemoveErrorsChanged();
                        }

                    }
                }
                else
                {
                    _course.Dosage = -1;
                    RaiseErrorsChanged("Введите дозу");
                }
            }
        }

        public int CourseDosageType
        {
            get => (int)_course.DosageType;
            set
            {
                if (value != (int)_course.DosageType)
                {
                    _course.DosageType = (eDosageType)value;
                    OnPropertyChanged();
                }
            }
        }

        public string CourseFreqInDay
        {
            get => _course.FreqInDay.ToString();
            set
            {
                if (int.TryParse(value, out int freqInDay))
                {
                    if (_course.FreqInDay != freqInDay)
                    {
                        _course.FreqInDay = freqInDay;
                        UpdateReceptionTime();
                        OnPropertyChanged();
                        if (_course.FreqInDay <= 0)
                        {
                            RaiseErrorsChanged("Приемов в день должно быть больше нуля");
                        }
                        else if (_course.FreqInDay > 15)
                        {
                            RaiseErrorsChanged("Приемов не может быть больше 15");
                        }
                        else
                        {
                            RemoveErrorsChanged();
                        }
                    }
                }
                else
                {
                    _course.FreqInDay = -1;
                    RaiseErrorsChanged("Введите кол-во приемов в день");
                    UpdateReceptionTime();
                }
            }
        }

        public string CourseFreq

        {
            get => _course.Freq.ToString();
            set
            {
                if (int.TryParse(value, out int freq))
                {
                    if (_course.Freq != freq)
                    {
                        _course.Freq = freq;
                        OnPropertyChanged();
                    }
                    if (_course.FreqType == eCourseFreqType.Nday && _course.Freq <= 0)
                    {
                        RaiseErrorsChanged("Частота приема должно быть больше нуля");
                    }
                    else
                    {
                        RemoveErrorsChanged();
                    }
                }
                else if (_course.FreqType == eCourseFreqType.Nday)
                {
                    RaiseErrorsChanged("Введите частоту приема");
                }

            }
        }

        public int CourseFreqType
        {
            get => (int)_course.FreqType;
            set
            {
                if (value != (int)_course.FreqType)
                {
                    _course.FreqType = (eCourseFreqType)value;
                    OnPropertyChanged();
                    isCourseFreqEnable = _course.FreqType != eCourseFreqType.Everyday;
                    CourseFreq = isCourseFreqEnable ? _course.Freq.ToString() : "0";

                    if (!isCourseFreqEnable)
                    {
                        RemoveErrorsChanged("CourseFreq");
                    }
                }
            }
        }

        public int CourseFoodDependence
        {
            get => (int)_course.FoodDependence;
            set
            {
                if (value != (int)_course.FoodDependence)
                {
                    _course.FoodDependence = (eFoodDependence)value;
                    OnPropertyChanged();
                }
            }
        }

        public string CourseDuration
        {
            get => _course.Duration.ToString();
            set
            {
                if (int.TryParse(value, out int duration))
                {
                    if (_course.Duration != duration)
                    {
                        _course.Duration = duration;
                        OnPropertyChanged();
                    }
                    if (_course.DurationType != eCourseDurationType.Regular && _course.Duration <= 0)
                    {
                        RaiseErrorsChanged("Продолжительность приема должно быть больше нуля");
                    }
                    else
                    {
                        RemoveErrorsChanged();
                    }
                }
                else if (_course.DurationType != eCourseDurationType.Regular)
                {
                    RaiseErrorsChanged("Введите продолжительность приема");
                }
            }
        }

        public int CourseDurationType
        {
            get => (int)_course.DurationType;
            set
            {
                if (value != (int)_course.DurationType)
                {
                    _course.DurationType = (eCourseDurationType)value;
                    OnPropertyChanged();
                    isCourseDurationEnable = _course.DurationType != eCourseDurationType.Regular;
                    CourseDuration = isCourseDurationEnable ? _course.Duration.ToString() : "0";
                    if (!isCourseDurationEnable)
                    {
                        RemoveErrorsChanged("CourseDuration");
                    }
                }
            }
        }

        public DateTime CourseStartDate
        {
            get => _course.StartDate;
            set
            {
                if (!value.Equals(_course.StartDate))
                {
                    _course.StartDate = value.Date;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TimePickerDate> CourseReceptionTime
        {
            get => _receptionTime;
            set
            {
                if (!value.Equals(_receptionTime))
                {
                    _receptionTime = value;
                    OnPropertyChanged();
                }
                onReceptionTimeChange();
            }
        }

        void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public bool HasErrors { get => _validationErrors.Count > 0; }

        public string GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
                return null;
            return _validationErrors[propertyName];
        }

        private void RaiseErrorsChanged(string errorMessage, [CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(errorMessage))
            {
                _validationErrors[propertyName] = errorMessage;
            }
        }

        private void RemoveErrorsChanged([CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName) && _validationErrors.ContainsKey(propertyName))
            {
                _validationErrors.Remove(propertyName);
            }
        }

        private void onSaveCourse()
        {
            if (_isLockCommand)
            {
                return;
            }

            if (isValidCourse())
            {
                _isLockCommand = true;
                var db = CoursesDatabase.Instance;
                if (isCreateMode)
                {
                    db.CreateCourse(_course, ReceptionTimeList);
                    NotificationSystem.Instance.onCreateCourse(_course, ReceptionTimeList);
                }
                else
                {
                    db.UpdateCourse(_course, ReceptionTimeList);
                    NotificationSystem.Instance.onUpdateCourse(_course, ReceptionTimeList);
                }
                _navigation.PopAsync();
            }
        }

        private void onDeleteCourse()
        {
            if (_isLockCommand)
            {
                return;
            }

            if (!isCreateMode)
            {
                _isLockCommand = true;
                NotificationSystem.Instance.onDeleteCourse(_course.Id);
                CoursesDatabase.Instance.DeleteCourse(_course.Id);
                _navigation.PopAsync();
            }
        }

        private bool isValidCourse()
        {
            if (HasErrors || _course.Dosage <= 0)
                return false;
            if (_course.FreqInDay <= 0 || _receptionTime.Count() != _course.FreqInDay)
                return false;
            if (ReceptionTimeList.Distinct().Count() != _receptionTime.Count())
                return false;
            if (_course.FreqType == eCourseFreqType.Nday && _course.Freq <= 0)
                return false;
            if (_course.DurationType != eCourseDurationType.Regular && _course.Duration <= 0)
                return false;
            return true;
        }

        private void UpdateReceptionTime()
        {
            if (_course.FreqInDay == _receptionTime.Count)
            {
                return;
            }
            var isRemove = _course.FreqInDay < _receptionTime.Count;
            var count = Math.Abs(Math.Max(_course.FreqInDay, 0) - _receptionTime.Count);
            for (int i = 0; i < count; ++i)
            {
                if (isRemove)
                {
                    _receptionTime.RemoveAt(0);
                }
                else
                {
                    var pickerDate = new TimePickerDate { Time = new TimeSpan(0, 0, 0) };
                    pickerDate.onTimeChange += onReceptionTimeChange;
                    _receptionTime.Add(pickerDate);
                }
            }
            CourseReceptionTime = new ObservableCollection<TimePickerDate>(_receptionTime);
        }

        private List<TimeSpan> ReceptionTimeList
        {
            get
            {
                var result = new List<TimeSpan>();
                foreach (var picker in CourseReceptionTime)
                {
                    result.Add(picker.Time);
                }
                return result;
            }
        }

        private void onReceptionTimeChange()
        {
            if (ReceptionTimeList.Distinct().Count() != _receptionTime.Count())
            {
                RaiseErrorsChanged("Времена приема должны быть уникальны", "CourseReceptionTime");
            }
            else
            {
                RemoveErrorsChanged("CourseReceptionTime");
            }
        }
    }
}
