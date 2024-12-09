using System.Text.Json;

namespace Synapse.Utilities
{
    public class UtilityService
    {
        public static HttpResponseMessage GetMockOrderResponseMessage()
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

        public static OrderDTO GetMockOrderDTO()
        {
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

            return orders;
        }

        public static Order GetMockOrderShouldBeDelivered()
        {
            //create items
            Item item1 = new() {Status = "Delivered", DeliveryNotification = 0, Description = "This item is a wheelchair"};
            Item item2 = new(){Status = "Pending", DeliveryNotification = 0, Description = "This item is a hospital bed"};

            //create order
            Order order = new() {Items = [item1, item2], OrderId = 101};

            return order;
        }

        public static Order GetMockOrderShouldNotBeDelivered()
        {
            //create items
            Item item1 = new() {Status = "Pending", DeliveryNotification = 0, Description = "This item is a wheelchair"};
            Item item2 = new(){Status = "Pending", DeliveryNotification = 0, Description = "This item is a hospital bed"};

            //create order
            Order order = new() {Items = [item1, item2], OrderId = 101};

            return order;
        }
    }
}