using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PillSchedule.ViewModel;

namespace PillSchedule
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {

        MainViewModel vm;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = vm = new MainViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            vm.OnAppearing();
        }
    }
}