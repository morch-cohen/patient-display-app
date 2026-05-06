using System.Windows;
using System.Windows.Input;
using PatientDisplay.Models;
using PatientDisplay.Services;
using PatientDisplay.ViewModels;

namespace PatientDisplay.Views;

public partial class PatientDetailWindow : Window
{
    public PatientDetailWindow(PatientModel patient)
    {
        InitializeComponent();
        var apiService = ((App)Application.Current).ApiService;
        var viewModel = new PatientDetailViewModel(apiService);
        viewModel.Initialize(patient);
        DataContext = viewModel;
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

    private Point _startPoint;
    private bool _isDragging;

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(this);
        _isDragging = false;
        base.OnPreviewMouseLeftButtonDown(e);
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        base.OnPreviewMouseMove(e);
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            if (e.OriginalSource is System.Windows.FrameworkElement el)
            {
                if (el.TemplatedParent is System.Windows.Controls.Primitives.ScrollBar || 
                    el is System.Windows.Controls.Primitives.Thumb || 
                    el is System.Windows.Controls.Button ||
                    el.TemplatedParent is System.Windows.Controls.Button)
                {
                    return;
                }
            }

            Point position = e.GetPosition(this);
            if (System.Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                System.Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                _isDragging = true;
                this.DragMove();
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
