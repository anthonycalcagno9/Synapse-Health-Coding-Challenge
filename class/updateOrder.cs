using System.Text.Json;

namespace Synapse.UpdateOrder
{
    public class UpdateOrderService(IApiClient apiClient)
    {
        public IApiClient _apiClient = apiClient;

        public async Task SendAlertAndUpdateOrder(Order order)
        {
            {
                string updateApiUrl = "https://update-api.com/update";
                var content = new StringContent(JsonSerializer.Serialize(order), System.Text.Encoding.UTF8, "application/json");
                var response = await _apiClient.PostAsync(updateApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Updated order sent for processing: OrderId {order.OrderId}");
                }
                else
                {
                    Console.WriteLine($"Failed to send updated order for processing: OrderId {order.OrderId}");
                }
            }
        }
    }
}