using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PatientDisplay.ViewModels;

public partial class WidgetViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isDashboardOpen = false;

    [ObservableProperty]
    private bool _hasPendingPatients = false;

    [RelayCommand]
    private void ToggleDashboard()
    {
        IsDashboardOpen = !IsDashboardOpen;
    }

    public void UpdatePendingState(System.Collections.Generic.IEnumerable<PatientDisplay.Models.PatientModel> patients)
    {
        HasPendingPatients = System.Linq.Enumerable.Any(patients, p => p.SurgicalStatus == PatientDisplay.Models.SurgicalStatus.Pending);
    }
}
