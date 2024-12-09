using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Synapse.ProcessOrders
{
    public class ProcessOrderService(HttpClient apiClient, ILogger logger)
    {
        private readonly HttpClient _apiClient = apiClient;
        private readonly ILogger _logger = logger;

         public Order ProcessOrder(Order order)
        {
            Item[] items = order.Items;
            foreach (Item item in items)
            {
                if (IsItemDelivered(item))
                {
                    SendAlertMessage(item, order.OrderId.ToString());
                    IncrementDeliveryNotification(item);
                }
            }
            return order;
        }

        private static bool IsItemDelivered(Item item)
        {
            return item.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase);
        }

        private async void SendAlertMessage(Item item, string orderId)
        {
            {
                string alertApiUrl = "https://alert-api.com/alerts";
                AlertData alertData = new()
                {
                    Message = $"Alert for delivered item: Order {orderId}, Item: {item.Description}, " +
                              $"Delivery Notifications: {item.DeliveryNotification}"
                };
                
                StringContent content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _apiClient.PostAsync(alertApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Alert sent for delivered item: {item.Description}", item.Description);
                }
                else
                {
                    _logger.LogError("Failed to send alert for delivered item: {item.Description}", item.Description);
                    //TODO: Need to talk with PO about how to handle error in this situation
                    //Do we want to move forward and Increment Delivery Notification if this alert fails to send?
                }
            }
        }

        private static void IncrementDeliveryNotification(Item item)
        {
            item.DeliveryNotification++;
        }
    }
}