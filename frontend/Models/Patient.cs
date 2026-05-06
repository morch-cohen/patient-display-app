using System.Collections.Generic;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PatientDisplay.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SurgicalStatus
{
    Cleared,
    Pending
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Male,
    Female
}

public partial class PatientModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("gender")]
    public Gender Gender { get; set; }

    [JsonPropertyName("diagnosis")]
    public string Diagnosis { get; set; } = string.Empty;

    private SurgicalStatus _surgicalStatus;

    [JsonPropertyName("surgical_status")]
    public SurgicalStatus SurgicalStatus
    {
        get => _surgicalStatus;
        set => SetProperty(ref _surgicalStatus, value);
    }

    [JsonPropertyName("clinical_summary")]
    public string ClinicalSummary { get; set; } = string.Empty;
}

public class PaginatedPatients
{
    [JsonPropertyName("items")]
    public List<PatientModel> Items { get; set; } = new();

    [JsonPropertyName("total_items")]
    public int TotalItems { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }
}

public class PatientUpdateRequest
{
    [JsonPropertyName("surgical_status")]
    public string SurgicalStatus { get; set; } = string.Empty;
}
