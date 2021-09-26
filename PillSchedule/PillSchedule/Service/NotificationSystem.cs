using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.LocalNotification;

namespace PillSchedule.Service
{
    class NotificationSystem
    {
        private static NotificationSystem _instance;

        private readonly TimeSpan repeatInterval = new TimeSpan(0, 10, 0);
        private readonly int repeatCount = 3;
        public static NotificationSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationSystem();
                }
                return _instance;
            }
        }

        public void onCreateCourse(Course course, List<TimeSpan> receptions)
        {
            var notification = CreateNotification(course, receptions);
            if (isCourseComplete(course, notification))
            {
                return;
            }
            CoursesDatabase.Instance.CreateNotification(notification);
            CreateNotificationCenter(notification);
        }

        public void onDeleteCourse(int courseId)
        {
            var notification = CoursesDatabase.Instance.GetNotificationByCourseId(courseId);
            NotificationCenter.Current.Cancel(notification.Id);
            CoursesDatabase.Instance.DeleteNotification(notification.Id);
        }

        public void onUpdateCourse(Course course, List<TimeSpan> receptions)
        {
            onDeleteCourse(course.Id);
            onCreateCourse(course, receptions);
        }

        public void onReceiveNotification(Notification notification)
        {
            NotificationCenter.Current.Cancel(notification.Id);
            var course = CoursesDatabase.Instance.GetCourse(notification.CourseId);
            if (course == null)
                return;
            var newNotification = CreateNextNotification(course, notification);
            if (!isCourseComplete(course, newNotification))
            {
                CoursesDatabase.Instance.UpdateNotification(newNotification);
                CreateNotificationCenter(newNotification);
            }
        }

        private Notification CreateNotification(Course course, List<TimeSpan> receptions)
        {
            var nowDate = DateTime.Now.Date;
            var nowTime = DateTime.Now.TimeOfDay;
            var clearStartDate = course.StartDate.Date;
            if (clearStartDate > nowDate)
            {
                return new Notification()
                {
                    CourseId = course.Id,
                    Date = clearStartDate,
                    Time = receptions.Min(),
                    DaysPassed = 0,
                    ReceptionsPassed = 0,
                    ReceptionInDayIndex = 0
                };
            }
            int daysPassed = (int)Math.Round((nowDate - clearStartDate).TotalDays);
            var date = nowDate;
            if (course.FreqType == eCourseFreqType.Nday)
            {
                int mod = daysPassed % (course.Freq + 1);
                int days = daysPassed / (course.Freq + 1);
                if (mod == 0)
                {
                    daysPassed = days;
                }
                else
                {
                    daysPassed = days + 1;
                    date = date.AddDays(course.Freq + 1 - mod);
                }
            }
            var receptionsOrdered = receptions.OrderBy(r => r).ToList();
            var receptionInDayIndex = 0;
            var time = receptionsOrdered[0];
            if (date == nowDate)
            {
                if (receptionsOrdered[receptionsOrdered.Count - 1] < nowTime)
                {
                    ++daysPassed;
                    date = date.AddDays(course.FreqType == eCourseFreqType.Nday ? course.Freq + 1 : 1);
                }
                else
                {
                    for (int i = 1; i < receptionsOrdered.Count; ++i)
                    {
                        if (receptionsOrdered[i - 1] <= nowTime && receptionsOrdered[i] > nowTime)
                        {
                            receptionInDayIndex = i - 1;
                            time = receptionsOrdered[receptionInDayIndex];
                            break;
                        }
                    }
                }
            }
            return new Notification()
            {
                CourseId = course.Id,
                Date = date,
                Time = time,
                DaysPassed = daysPassed,
                ReceptionInDayIndex = receptionInDayIndex,
                ReceptionsPassed = daysPassed * course.FreqInDay + receptionInDayIndex
            };
        }

        private bool isCourseComplete(Course course, Notification notification)
        {
            if (course.DurationType == eCourseDurationType.Days)
            {
                return notification.DaysPassed >= course.Duration;
            }
            else if (course.DurationType == eCourseDurationType.Receptions)
            {
                return notification.ReceptionsPassed >= course.Duration;
            }
            return false;
        }

        private void CreateNotificationCenter(Notification notification)
        {
            NotificationCenter.Current.Show(new NotificationRequest()
            {
                NotificationId = notification.Id,
                Title = "Прием лекарств",
                Description = "Примите таблетки",
                Schedule = new NotificationRequestSchedule()
                {
                    Repeats = NotificationRepeat.TimeInterval,
                    NotifyTime = notification.Date + notification.Time,
                    NotifyRepeatInterval = repeatInterval,
                    NotifyAutoCancelTime = notification.Date + notification.Time + repeatInterval * repeatCount,
                },
                Android = new AndroidOptions()
                {
                    Priority = NotificationPriority.High,
                }
            });
        }

        private Notification CreateNextNotification(Course course, Notification notification)
        {
            ++notification.ReceptionsPassed;
            ++notification.ReceptionInDayIndex;
            if (course.FreqInDay == notification.ReceptionInDayIndex)
            {
                ++notification.DaysPassed;
                notification.ReceptionInDayIndex = 0;
                notification.Date = notification.Date.AddDays(course.FreqType == eCourseFreqType.Nday ? course.Freq + 1 : 1);
            }
            var receptions = CoursesDatabase.Instance.GetCourseReceptions(course.Id).OrderBy(r => r.Time).ToList();
            notification.Time = receptions[notification.ReceptionInDayIndex].Time;
            return notification;
        }
    }
}
