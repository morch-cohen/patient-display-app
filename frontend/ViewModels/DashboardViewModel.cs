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

        try
        {
            var result = await _apiService.GetPatientsAsync(SearchQuery, CurrentPage, 10);
            if (result is not null)
            {
                Patients = new ObservableCollection<PatientModel>(result.Items);
                TotalPages = result.TotalPages;
                TotalItems = result.TotalItems;
                CurrentPage = result.CurrentPage;

                ((App)System.Windows.Application.Current).WidgetViewModel.UpdatePendingState(result.Items);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load patients: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
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
