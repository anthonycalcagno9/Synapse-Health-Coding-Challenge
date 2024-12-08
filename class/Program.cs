using System.Text.Json;
using Moq;
using Moq.Protected;
using Synapse.FetchOrders;
using Synapse.ProcessOrders;
using Synapse.UpdateOrder;
using Microsoft.Extensions.Logging;

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
            if (args.Length < 1)
            {
                Console.WriteLine("Need to input env in run command: dotnet run dev");
                System.Environment.Exit(0);
            }

            //Logger setup
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger<Program>();

            HttpClient apiClient = new();

            //getting env from args, if dev then we want to mock the api calls
            bool isDev = args[0] == "dev";
            if (isDev)
            {
                var mockApiService = new Mock<HttpMessageHandler>();
                mockApiService.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()).ReturnsAsync(GetMockOrderResponseMessage());
                apiClient = new(mockApiService.Object);
            }


            FetchOrderService orderService = new(apiClient, factory.CreateLogger<FetchOrderService>());
            ProcessOrderService processOrderService = new(apiClient, factory.CreateLogger<ProcessOrderService>());
            UpdateOrderService updateOrderService = new(apiClient, factory.CreateLogger<UpdateOrderService>());

            OrderDTO medicalEquipmentOrders = await orderService.FetchMedicalEquipmentOrders();
            foreach (Order order in medicalEquipmentOrders.Orders)
            {
                Order updatedOrder = processOrderService.ProcessOrder(order);
                await updateOrderService.SendAlertAndUpdateOrder(updatedOrder);
            }

            logger.LogInformation("Results sent to relevant APIs.");
			return 0;
        }

        private static HttpResponseMessage GetMockOrderResponseMessage()
        {
            HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);

            //create items
            Item item1 = new() {Status = "Delivered", DeliveryNotification = 0, Description = "This item is a wheelchair"};
            Item item2 = new(){Status = "Pending", DeliveryNotification = 0, Description = "This item is a hospital bed"};

            //create order
            Order order = new() {Items = [item1, item2], OrderId = 101};

            //create orderDTO
            OrderDTO orders = new()
            {
                Orders = 
                [
                    new Order
                    {
                        OrderId = 101,
                        Items = [item1, item2]
                    },
                    new Order
                    {
                        OrderId = 102,
                        Items = [item1, item2]
                    }
                ]
            };

            //serialize orders to JSON
            string ordersJson = JsonSerializer.Serialize(orders);

            //set content as serialized json
            httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(orders));

            return httpResponseMessage;
        }
    }
}


