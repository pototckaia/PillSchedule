using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PillSchedule
{
  public partial class CoursePage : ContentPage
  {
    CourseViewModel viewModel;

    public CoursePage()
    {
      InitializeComponent();
      BindingContext = viewModel = new CourseViewModel();

      if (viewModel.isEditMode)
      {
        var toolbarItemHome = new ToolbarItem
        {
          Text = "Удалить",
          Command = viewModel.DeleteCourseCommand
        };
        ToolbarItems.Add(toolbarItemHome);
      }
    }

    private void createErrorEffect(string error, Label errorLabel, Entry form)
    {
      createErrorMessage(error, errorLabel);
      form.TextColor = error != null ? Color.Red : Color.Default;
    }

    private void createErrorMessage(string error, Label errorLabel)
    {
      errorLabel.IsVisible = error != null;
      if (error != null)
      {
        errorLabel.Text = error;
      }
    }

    private void CourseDosageEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
      createErrorEffect(viewModel.GetErrors("CourseDosage"), CourseDosageEntryErrorMsg, CourseDosageEntry);
    }

    private void CourseFreqInDayEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
      createErrorEffect(viewModel.GetErrors("CourseFreqInDay"), CourseFreqInDayEntryErrorMsg, CourseFreqInDayEntry);
      createErrorMessage(viewModel.GetErrors("CourseReceptionTime"), CourseReceptionTimeErrorMsg);
    }

    private void CourseFreqEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
      createErrorEffect(viewModel.GetErrors("CourseFreq"), CourseFreqEntryErrorMsg, CourseFreqEntry);
    }

    private void CourseFreqTypePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (viewModel != null)
        createErrorEffect(viewModel.GetErrors("CourseFreq"), CourseFreqEntryErrorMsg, CourseFreqEntry);
    }

    private void CourseDurationEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
      createErrorEffect(viewModel.GetErrors("CourseDuration"), CourseDurationEntryErrorMsg, CourseDurationEntry);

    }

    private void CourseDurationTypePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (viewModel != null)
        createErrorEffect(viewModel.GetErrors("CourseDuration"), CourseDurationEntryErrorMsg, CourseDurationEntry);

    }

    private void TimePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      createErrorMessage(viewModel.GetErrors("CourseReceptionTime"), CourseReceptionTimeErrorMsg);
    }
  }

}
