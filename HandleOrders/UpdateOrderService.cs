using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Synapse.UpdateOrder
{
    public class UpdateOrderService(HttpClient apiClient, ILogger logger)
    {
        public HttpClient _apiClient = apiClient;
        public ILogger _logger = logger;
        private const string UpdateApiUrl = "https://update-api.com/update";

        public async Task SendAlertAndUpdateOrder(Order order)
        {
            {
                StringContent content = new(JsonSerializer.Serialize(order), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _apiClient.PostAsync(UpdateApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Updated order sent for processing: OrderId {order.OrderId}", order.OrderId);
                }
                else
                {
                    _logger.LogError("Failed to send updated order for processing: OrderId {order.OrderId}", order.OrderId);
                }
            }
        }
    }
}