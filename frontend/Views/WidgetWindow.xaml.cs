using System;
using System.Windows;
using System.Windows.Input;
using PatientDisplay.ViewModels;

namespace PatientDisplay.Views;

public partial class WidgetWindow : Window
{
    private DashboardWindow? _dashboardWindow;
    private readonly WidgetViewModel _viewModel;

    public WidgetWindow()
    {
        InitializeComponent();
        
        // Use the ViewModel provided by App.xaml.cs if it's already set
        _viewModel = (WidgetViewModel)(DataContext ?? ((App)Application.Current).WidgetViewModel);
        DataContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        // Position widget near bottom-right of work area or load from settings
        if (Properties.Settings.Default.WidgetLeft >= 0)
        {
            Left = Properties.Settings.Default.WidgetLeft;
            Top  = Properties.Settings.Default.WidgetTop;
        }
        else
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 24;
            Top  = workArea.Bottom - Height - 24;
        }
    }

    private Point _startPoint;
    private bool _isDragging;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (e.ChangedButton == MouseButton.Left)
        {
            _startPoint = e.GetPosition(this);
            _isDragging = false;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            Point position = e.GetPosition(this);
            if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                _isDragging = true;
                this.DragMove();
                ClampToWorkArea();
            }
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        if (!_isDragging && e.ChangedButton == MouseButton.Left)
        {
            if (DataContext is WidgetViewModel vm && vm.ToggleDashboardCommand.CanExecute(null))
            {
                vm.ToggleDashboardCommand.Execute(null);
            }
        }
        _isDragging = false;
    }

    private void ClampToWorkArea()
    {
        var workArea = SystemParameters.WorkArea;
        Left = Math.Max(workArea.Left, Math.Min(Left, workArea.Right  - Width));
        Top  = Math.Max(workArea.Top,  Math.Min(Top,  workArea.Bottom - Height));
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(WidgetViewModel.IsDashboardOpen)) return;

        if (_viewModel.IsDashboardOpen)
            ShowDashboard();
        else
            HideDashboard();
    }

    private void ShowDashboard()
    {
        if (_dashboardWindow is null || !_dashboardWindow.IsLoaded)
        {
            _dashboardWindow = new DashboardWindow();
            _dashboardWindow.Closed += (_, _) =>
            {
                _viewModel.IsDashboardOpen = false;
                _dashboardWindow = null;
            };
        }
        _dashboardWindow.Show();
        _dashboardWindow.Activate();
    }

    private void HideDashboard()
    {
        _dashboardWindow?.Hide();
    }

    protected override void OnClosed(EventArgs e)
    {
        Properties.Settings.Default.WidgetLeft = Left;
        Properties.Settings.Default.WidgetTop  = Top;
        Properties.Settings.Default.Save();
        base.OnClosed(e);
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        Topmost = true;
    }
}
