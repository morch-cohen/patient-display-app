using System.Windows;
using PatientDisplay.Services;
using PatientDisplay.ViewModels;
using PatientDisplay.Views;

namespace PatientDisplay;

public partial class App : Application
{
    public ApiService ApiService { get; } = new ApiService();
    public WidgetViewModel WidgetViewModel { get; } = new WidgetViewModel();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Start with the overlay widget
        var widget = new WidgetWindow
        {
            DataContext = WidgetViewModel
        };
        MainWindow = widget;
        widget.Show();
    }
}
