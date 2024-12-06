using Newtonsoft.Json.Linq;

namespace Synapse.ProcessOrders
{
    class ProcessOrderService(IApiClient apiClient)
    {
        public IApiClient _apiClient = apiClient;

         public Order ProcessOrder(Order order)
        {
            var items = order.Items;
            foreach (var item in items)
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

        private void SendAlertMessage(Item item, string orderId)
        {
            {
                string alertApiUrl = "https://alert-api.com/alerts";
                var alertData = new
                {
                    Message = $"Alert for delivered item: Order {orderId}, Item: {item.Description}, " +
                              $"Delivery Notifications: {item.DeliveryNotification}"
                };
                var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
                var response = _apiClient.PostAsync(alertApiUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Alert sent for delivered item: {item.Description}");
                }
                else
                {
                    Console.WriteLine($"Failed to send alert for delivered item: {item.Description}");
                }
            }
        }

        private static void IncrementDeliveryNotification(Item item)
        {
            item.DeliveryNotification++;
        }
    }
}