using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PillSchedule.ViewModel;

namespace PillSchedule
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MainPage : TabbedPage
  {

    CoursesViewModel courses;

    public MainPage()
    {
      InitializeComponent();
      BindingContext = courses = new CoursesViewModel(Navigation);
    }

    protected override void OnAppearing()
    {
      courses.OnAppearing();
    }
  }
}