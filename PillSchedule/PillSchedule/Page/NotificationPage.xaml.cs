using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PillSchedule.Service;

namespace PillSchedule.Page
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        private Notification _notification;
        private Course _course;

        public NotificationPage(int notificationId)
        {
            InitializeComponent();
            _notification = CoursesDatabase.Instance.GetNotification(notificationId);
            _course = CoursesDatabase.Instance.GetCourse(_notification.CourseId);

            var descriptionText = $"Время для {_course.Name}, ";
            switch (_course.Type)
            {
                case eCourseType.Capsule:
                    descriptionText += "капсула, ";
                    break;
                case eCourseType.Drops:
                    descriptionText += "капли, ";
                    break;
                case eCourseType.Injection:
                    descriptionText += "укол, ";
                    break;
                case eCourseType.Mixture:
                    descriptionText += "микстура, ";
                    break;
                case eCourseType.Ointment:
                    descriptionText += "мазь, ";
                    break;
                case eCourseType.Pill:
                    descriptionText += "таблетка, ";
                    break;
                case eCourseType.Procedure:
                    descriptionText += "процедура, ";
                    break;
            }
            descriptionText += $"доза {_course.Dosage} ";
            if (_course.DosageType == eDosageType.Mg)
            {
                descriptionText += "мг, ";
            }
            else
            {
                descriptionText += "шт, ";
            }
            switch (_course.FoodDependence)
            {
                case eFoodDependence.After:
                    descriptionText += "принимать после еды";
                    break;
                case eFoodDependence.Before:
                    descriptionText += "принимать до еды";
                    break;
                case eFoodDependence.During:
                    descriptionText += "принимать во время еды";
                    break;
                case eFoodDependence.None:
                    descriptionText += "с едой не связано";
                    break;
            }
            DescriptionLabel.Text = descriptionText;

            var dateFormate = _notification.Date.ToString("dd-MMM-yy");
            var timeFormate = _notification.Time.ToString("hh\\:mm");
            DateLabel.Text = $"Время: {dateFormate} {timeFormate}";
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            NotificationSystem.Instance.onReceiveNotification(_notification);
            Navigation.PopModalAsync();
        }
    }
}