using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatientDisplay.Models;
using PatientDisplay.Services;

namespace PatientDisplay.ViewModels;

public partial class PatientDetailViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCleared))]
    [NotifyPropertyChangedFor(nameof(StatusLabel))]
    private PatientModel _patient = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCleared))]
    [NotifyPropertyChangedFor(nameof(StatusLabel))]
    [NotifyCanExecuteChangedFor(nameof(SaveStatusCommand))]
    private SurgicalStatus _selectedStatus;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleStatusCommand))]
    private bool _isSaving = false;

    [ObservableProperty]
    private string _saveError = string.Empty;

    [ObservableProperty]
    private bool _saveSuccess = false;

    public bool IsCleared => SelectedStatus == SurgicalStatus.Cleared;
    public string StatusLabel => SelectedStatus == SurgicalStatus.Cleared ? "Cleared" : "Pending";

    public PatientDetailViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public void Initialize(PatientModel patient)
    {
        Patient = patient;
        SelectedStatus = patient.SurgicalStatus;
        SaveError = string.Empty;
        SaveSuccess = false;
    }

    private bool CanToggleStatus() => !IsSaving;

    [RelayCommand(CanExecute = nameof(CanToggleStatus))]
    private void ToggleStatus()
    {
        SelectedStatus = SelectedStatus == SurgicalStatus.Cleared
            ? SurgicalStatus.Pending
            : SurgicalStatus.Cleared;
        SaveSuccess = false;
        SaveError = string.Empty;
    }

    private bool CanSaveStatus()
    {
        return Patient != null && SelectedStatus != Patient.SurgicalStatus;
    }

    [RelayCommand(CanExecute = nameof(CanSaveStatus))]
    private async Task SaveStatusAsync()
    {
        IsSaving = true;
        SaveError = string.Empty;
        SaveSuccess = false;

        int maxRetries = 5;
        int currentTry = 0;

        while (currentTry < maxRetries)
        {
            try
            {
                currentTry++;
                var updated = await _apiService.PatchPatientStatusAsync(Patient.Id, SelectedStatus);
                if (updated is not null)
                {
                    // Update the existing reference so DashboardWindow observes the change
                    Patient.SurgicalStatus = updated.SurgicalStatus;
                    SelectedStatus = updated.SurgicalStatus;
                    SaveSuccess = true;
                    
                    // Re-evaluate CanExecute after successful save
                    SaveStatusCommand.NotifyCanExecuteChanged();
                    IsSaving = false;
                    return; // Success!
                }
            }
            catch (Exception ex)
            {
                if (currentTry >= maxRetries)
                {
                    SaveError = $"Failed to save after {maxRetries} attempts: {ex.Message}";
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        IsSaving = false;
    }
}
