using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;

namespace TaskManagementAPI.Blazor.Services
{
    /// <summary>
    /// Implementation of IApiClient that handles HTTP communication with the backend API
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILocalStorageService localStorage, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _logger = logger;
        }

        /// <summary>
        /// Set the JWT token in localStorage (for persistence)
        /// </summary>
        public async Task SetTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                await _localStorage.RemoveItemAsync("authToken");
                _logger.LogInformation("JWT token cleared from storage");
                return;
            }

            await _localStorage.SetItemAsync("authToken", token);
            _logger.LogInformation("JWT token set in API client");
        }

        /// <summary>
        /// Clear the JWT token from localStorage
        /// </summary>
        public async Task ClearTokenAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _logger.LogInformation("JWT token cleared from API client");
        }

        /// <summary>
        /// Get the token from localStorage and add it to the request
        /// </summary>
        private async Task<HttpRequestMessage> CreateRequestWithAuthAsync(HttpMethod method, string endpoint)
        {
            var request = new HttpRequestMessage(method, endpoint);
            
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            return request;
        }

        /// <summary>
        /// Make a GET request and deserialize response to type T
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                _logger.LogInformation($"GET request to {endpoint}");
                
                var request = await CreateRequestWithAuthAsync(HttpMethod.Get, endpoint);
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogErrorResponse(response);
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET request failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Make a POST request with a JSON body
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, object body)
        {
            try
            {
                _logger.LogInformation($"POST request to {endpoint}");
                
                var request = await CreateRequestWithAuthAsync(HttpMethod.Post, endpoint);
                request.Content = JsonContent.Create(body);
                
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogErrorResponse(response);
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST request failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Make a PUT request with a JSON body
        /// </summary>
        public async Task<T> PutAsync<T>(string endpoint, object body)
        {
            try
            {
                _logger.LogInformation($"PUT request to {endpoint}");
                
                var request = await CreateRequestWithAuthAsync(HttpMethod.Put, endpoint);
                request.Content = JsonContent.Create(body);
                
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogErrorResponse(response);
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PUT request failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Make a PATCH request with a JSON body
        /// </summary>
        public async Task<T> PatchAsync<T>(string endpoint, object body)
        {
            try
            {
                _logger.LogInformation($"PATCH request to {endpoint}");
                
                var request = await CreateRequestWithAuthAsync(HttpMethod.Patch, endpoint);
                request.Content = JsonContent.Create(body);
                
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogErrorResponse(response);
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PATCH request failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Make a DELETE request
        /// </summary>
        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                _logger.LogInformation($"DELETE request to {endpoint}");
                
                var request = await CreateRequestWithAuthAsync(HttpMethod.Delete, endpoint);
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogErrorResponse(response);
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE request failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Private helper: Log error responses
        /// </summary>
        private void LogErrorResponse(HttpResponseMessage response)
        {
            _logger.LogError($"API Error - Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }
    }
}
