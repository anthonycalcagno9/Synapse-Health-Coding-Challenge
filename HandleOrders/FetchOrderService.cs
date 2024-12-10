using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Synapse.FetchOrders
{
    public class FetchOrderService(HttpClient apiClient, ILogger logger)
    {
        public HttpClient _apiClient = apiClient;
        public ILogger _logger = logger;
        private const string FetchOrderApiUrl = "https://orders-api.com/orders";

        public async Task<OrderDTO> FetchMedicalEquipmentOrders()
        {
            {
                HttpResponseMessage response = await _apiClient.GetAsync(FetchOrderApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully fetched orders from API");
                    string ordersData = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OrderDTO>(ordersData);
                }
                else
                {
                    _logger.LogError("Failed to fetch orders from API.");
                    return new OrderDTO {Orders = []};
                }
            }
        }
    }
}