using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SamletInfo.Models;

public class ApiClient
{
    private readonly HttpClient _client = new HttpClient();

    public ApiClient()
    {
        _client.BaseAddress = new Uri("http://localhost:5099");
    }

    public async Task<List<Booking>> GetBookings()
    {
        var response = await _client.GetStringAsync("/api/bookings");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<Booking>>(response, options);
    }

    public async Task<bool> AddBooking(Booking newBooking)
    {
        var response = await _client.PostAsJsonAsync("/api/bookings", newBooking);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Room>> GetRooms()
    {
        var response = await _client.GetStringAsync("/api/rooms");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<Room>>(response, options);
    }

    public async Task<bool> DeleteBooking(int bookingId)
    {
        var response = await _client.DeleteAsync($"/api/bookings/{bookingId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateServiceTask(ServiceTask task)
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", task);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ServiceTask>> GetAllServiceTasks()
    {
        var response = await _client.GetFromJsonAsync<List<ServiceTask>>("/api/tasks");
        return response ?? new List<ServiceTask>();
    }
    public async Task<List<TaskTemplate>> GetTaskTemplates()
    {
        var response = await _client.GetFromJsonAsync<List<TaskTemplate>>("/api/tasktemplates");
        return response ?? new List<TaskTemplate>();
    }


    public async Task<List<TaskTemplate>> GetTaskTemplatesByType(string type)
    {
        var response = await _client.GetFromJsonAsync<List<TaskTemplate>>($"/api/templates?type={type}");
        return response ?? new List<TaskTemplate>();
    }
}
