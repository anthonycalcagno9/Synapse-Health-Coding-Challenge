using System.Text.Json;
using Moq;
using Moq.Protected;
using Synapse.FetchOrders;
using Synapse.ProcessOrders;
using Synapse.UpdateOrder;
using Synapse.Utilities;
using Microsoft.Extensions.Logging;

namespace Synapse.HandleOrders
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
                Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync", 
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(UtilityService.GetMockOrderResponseMessage());
                apiClient = new(mockHttpMessageHandler.Object);
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
    }
}


