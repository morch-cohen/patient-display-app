using System.Windows;
using System.Windows.Input;
using PatientDisplay.Models;
using PatientDisplay.Services;
using PatientDisplay.ViewModels;

namespace PatientDisplay.Views;

public partial class DashboardWindow : Window
{
    private readonly DashboardViewModel _viewModel;

    public DashboardWindow()
    {
        InitializeComponent();
        var apiService = ((App)Application.Current).ApiService;
        _viewModel = new DashboardViewModel(apiService);
        DataContext = _viewModel;

        if (Properties.Settings.Default.DashboardWidth > 0)
        {
            Width = Properties.Settings.Default.DashboardWidth;
            Height = Properties.Settings.Default.DashboardHeight;
        }
        else
        {
            Width = 500;
            Height = 640;
        }

        DataObject.AddPastingHandler(SearchBox, SearchBox_Pasting);
        Loaded += async (_, _) => await _viewModel.LoadPatientsAsync();
    }

    protected override void OnActivated(System.EventArgs e)
    {
        base.OnActivated(e);
        if (Application.Current.MainWindow is WidgetWindow widget)
        {
            widget.Topmost = false;
            widget.Topmost = true;
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        Properties.Settings.Default.DashboardWidth = Width;
        Properties.Settings.Default.DashboardHeight = Height;
        Properties.Settings.Default.Save();
        base.OnClosed(e);
    }

    private void SearchBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Allow only letters and digits
        foreach (char c in e.Text)
        {
            if (!char.IsLetterOrDigit(c))
            {
                e.Handled = true;
                break;
            }
        }
    }

    private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Explicitly block space bar
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
    }

    private void SearchBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            string text = (string)e.DataObject.GetData(DataFormats.Text);
            foreach (char c in text)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    e.CancelCommand();
                    break;
                }
            }
        }
    }

    private void PatientCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is System.Windows.Controls.Border border && border.Tag is PatientModel patient)
        {
            // Check if a window for this patient is already open
            foreach (Window window in Application.Current.Windows)
            {
                if (window is PatientDetailWindow existingDetailWindow && 
                    existingDetailWindow.DataContext is PatientDetailViewModel vm && 
                    vm.Patient.Id == patient.Id)
                {
                    // Window is already open, bring it to front
                    if (existingDetailWindow.WindowState == WindowState.Minimized)
                        existingDetailWindow.WindowState = WindowState.Normal;
                    
                    existingDetailWindow.Activate();
                    return;
                }
            }

            var detailWindow = new PatientDetailWindow(patient);

            // Position the detail window next to the dashboard
            detailWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            detailWindow.Top = this.Top;
            
            // Try placing it to the right
            double desiredLeft = this.Left + this.ActualWidth + 20;
            
            // If it goes off-screen to the right, place it to the left instead
            if (desiredLeft + detailWindow.Width > SystemParameters.WorkArea.Right)
            {
                desiredLeft = this.Left - detailWindow.Width - 20;
            }
            
            detailWindow.Left = desiredLeft;

            detailWindow.Show();
        }
    }
}
