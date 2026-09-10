using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using SharedModels.Models;
using System.Threading.Tasks;

namespace ServiceAppMaui
{
    public class ApiClientMaui
    {
        private readonly HttpClient _client = new HttpClient();

        public ApiClientMaui()
        {
            _client.BaseAddress = new Uri("http://localhost:5099"); 
        }

        public async Task<List<ServiceTask>> GetTasksAsync(string roleType)
        {
            return await _client.GetFromJsonAsync<List<ServiceTask>>($"/api/tasks?type={roleType}")
                ?? new List<ServiceTask>();
        }

        public async Task<bool> UpdateTaskAsync(ServiceTask task)
        {
            var response = await _client.PutAsJsonAsync($"/api/tasks/{task.Id}", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            var response = await _client.PostAsJsonAsync($"/api/bookings", booking);
            return response.IsSuccessStatusCode;
        }
    }
}
