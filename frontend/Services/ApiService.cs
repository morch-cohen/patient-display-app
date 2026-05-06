using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PatientDisplay.Models;

namespace PatientDisplay.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public ApiService(string baseUrl = "http://localhost:8000")
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    /// <summary>GET /patients — Returns a paginated list, optionally filtered by search.</summary>
    public async Task<PaginatedPatients?> GetPatientsAsync(string? search, int page = 1, int size = 10)
    {
        var url = $"/patients?page={page}&size={size}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaginatedPatients>(_jsonOptions);
    }

    /// <summary>GET /patients/{id} — Returns detailed info for a single patient.</summary>
    public async Task<PatientModel?> GetPatientAsync(string id)
    {
        var response = await _httpClient.GetAsync($"/patients/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatientModel>(_jsonOptions);
    }

    /// <summary>PATCH /patients/{id} — Updates only the surgical_status field.</summary>
    public async Task<PatientModel?> PatchPatientStatusAsync(string id, SurgicalStatus status)
    {
        var body = new PatientUpdateRequest { SurgicalStatus = status.ToString() };
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/patients/{id}", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatientModel>(_jsonOptions);
    }
}
