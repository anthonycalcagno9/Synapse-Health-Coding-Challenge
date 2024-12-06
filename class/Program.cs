using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Synapse.FetchOrders;
using Synapse.ProcessOrders;
using Synapse.UpdateOrder;

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

            FetchOrderService orderService = new(apiClient);

            ProcessOrderService processOrderService = new(apiClient);

            UpdateOrderService updateOrderService = new(apiClient);

            var medicalEquipmentOrders = await orderService.FetchMedicalEquipmentOrders();
            foreach (var order in medicalEquipmentOrders.Orders)
            {
                var updatedOrder = processOrderService.ProcessOrder(order);
                updateOrderService.SendAlertAndUpdateOrder(updatedOrder).GetAwaiter().GetResult();
            }

            Console.WriteLine("Results sent to relevant APIs.");
			return 0;
        }
    }
}


