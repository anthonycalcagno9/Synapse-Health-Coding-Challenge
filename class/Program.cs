using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Synapse.OrdersExample
{
    /// <summary>
    /// I Get a list of orders from the API
    /// I check if the order is in a delviered state, If yes then send a delivery alert and add one to deliveryNotification
    /// I then update the order.   
    /// </summary>
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Start of App");

            IApiClient apiClient = new MockApiClient();

            var medicalEquipmentOrders = await FetchMedicalEquipmentOrders(apiClient);
            foreach (var order in medicalEquipmentOrders.Orders)
            {
                var updatedOrder = ProcessOrder(order, apiClient);
                SendAlertAndUpdateOrder(updatedOrder, apiClient).GetAwaiter().GetResult();
            }

            Console.WriteLine("Results sent to relevant APIs.");
			return 0;
        }

        static async Task<OrderDTO> FetchMedicalEquipmentOrders(IApiClient apiClient)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string ordersApiUrl = "https://orders-api.com/orders";
                var response = await apiClient.GetAsync(ordersApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response);
                    string ordersData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("===============");
                    Console.WriteLine(ordersData);
                    Console.WriteLine("================");
                    //return JObject.Parse(ordersData).ToObject<OrderDTO>();
                    return JsonSerializer.Deserialize<OrderDTO>(ordersData);
                }
                else
                {
                    Console.WriteLine("Failed to fetch orders from API.");
                    return new OrderDTO {Orders = []};
                }
            }
        }

        static Order ProcessOrder(Order order, IApiClient apiClient)
        {
            var items = order.Items;
            foreach (var item in items)
            {
                if (IsItemDelivered(item))
                {
                    SendAlertMessage(item, order.OrderId.ToString(), apiClient);
                    IncrementDeliveryNotification(item);
                }
            }

            return order;
        }

        static bool IsItemDelivered(Item item)
        {
            return item.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Delivery alert
        /// </summary>
        /// <param name="orderId">The order id for the alert</param>
        static void SendAlertMessage(Item item, string orderId, IApiClient apiClient)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string alertApiUrl = "https://alert-api.com/alerts";
                var alertData = new
                {
                    Message = $"Alert for delivered item: Order {orderId}, Item: {item.Description}, " +
                              $"Delivery Notifications: {item.DeliveryNotification}"
                };
                var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
                var response = apiClient.PostAsync(alertApiUrl, content).Result;

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

        static void IncrementDeliveryNotification(Item item)
        {
            item.DeliveryNotification++;
        }

        static async Task SendAlertAndUpdateOrder(Order order, IApiClient apiClient)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string updateApiUrl = "https://update-api.com/update";
                var content = new StringContent(JsonSerializer.Serialize(order), System.Text.Encoding.UTF8, "application/json");
                var response = await apiClient.PostAsync(updateApiUrl, content);

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


