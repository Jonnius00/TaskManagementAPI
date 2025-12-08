using System.Net.Http.Headers;

namespace TaskManagementAPI.Blazor.Services
{
    /// <summary>
    /// Interface for API client that handles all HTTP communication with the backend API
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Set the JWT token for authenticated requests
        /// </summary>
        Task SetTokenAsync(string token);

        /// <summary>
        /// Clear the JWT token
        /// </summary>
        Task ClearTokenAsync();

        /// <summary>
        /// Make a GET request to the API
        /// </summary>
        Task<T> GetAsync<T>(string endpoint);

        /// <summary>
        /// Make a POST request to the API
        /// </summary>
        Task<T> PostAsync<T>(string endpoint, object body);

        /// <summary>
        /// Make a PUT request to the API
        /// </summary>
        Task<T> PutAsync<T>(string endpoint, object body);

        /// <summary>
        /// Make a PATCH request to the API
        /// </summary>
        Task<T> PatchAsync<T>(string endpoint, object body);

        /// <summary>
        /// Make a DELETE request to the API
        /// </summary>
        Task DeleteAsync(string endpoint);
    }
}
