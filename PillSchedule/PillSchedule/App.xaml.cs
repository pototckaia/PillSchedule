using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.LocalNotification;
using PillSchedule.Page;

namespace PillSchedule
{
    public partial class App : Application
    {
        private bool sleep = false;
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());

            NotificationCenter.Current.NotificationReceived += (eventArgs) =>
            {
                if (!sleep)
                {
                    CreateNotificationPage(eventArgs.Request.NotificationId);
                }
            };
            
            NotificationCenter.Current.NotificationTapped += (eventArgs) =>
            {
                CreateNotificationPage(eventArgs.Request.NotificationId);
            };
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            sleep = true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            sleep = false;
        }

        void CreateNotificationPage(int notificationId)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MainPage.Navigation.PushModalAsync(new NotificationPage(notificationId));
            });
        }
    }
}
