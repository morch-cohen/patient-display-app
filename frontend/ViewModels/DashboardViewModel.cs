using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatientDisplay.Models;
using PatientDisplay.Services;

namespace PatientDisplay.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private CancellationTokenSource? _debounceCts;

    [ObservableProperty]
    private ObservableCollection<PatientModel> _patients = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _totalPages = 0;

    [ObservableProperty]
    private int _totalItems = 0;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isSearchEnabled = false;

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
                TriggerDebouncedSearch();
        }
    }

    public DashboardViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    private void TriggerDebouncedSearch()
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        Task.Delay(350, token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                CurrentPage = 1;
                _ = LoadPatientsAsync();
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    [RelayCommand]
    public async Task LoadPatientsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        IsSearchEnabled = false; // Disable search while connecting/loading

        int maxRetries = 5;
        int currentTry = 0;

        while (currentTry < maxRetries)
        {
            try
            {
                currentTry++;
                var result = await _apiService.GetPatientsAsync(SearchQuery, CurrentPage, 10);
                if (result is not null)
                {
                    Patients = new ObservableCollection<PatientModel>(result.Items);
                    TotalPages = result.TotalPages;
                    TotalItems = result.TotalItems;
                    CurrentPage = result.CurrentPage;

                    ((App)System.Windows.Application.Current).WidgetViewModel.UpdatePendingState(result.Items);
                    
                    IsSearchEnabled = true;
                    IsLoading = false;
                    return; // Success!
                }
                else if (currentTry >= maxRetries)
                {
                    ErrorMessage = "Server returned no data after multiple attempts.";
                }
            }
            catch (Exception ex)
            {
                if (currentTry >= maxRetries)
                {
                    ErrorMessage = $"Connection failed: {ex.Message}";
                }
                else
                {
                    // Wait 1.5s between retries to make it visible to user
                    await Task.Delay(1500);
                }
            }
        }

        IsLoading = false;
        // Search remains disabled if we failed to connect
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPage()
    {
        CurrentPage++;
        await LoadPatientsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPage()
    {
        CurrentPage--;
        await LoadPatientsAsync();
    }

    private bool CanGoNext() => CurrentPage < TotalPages;
    private bool CanGoPrevious() => CurrentPage > 1;
}
