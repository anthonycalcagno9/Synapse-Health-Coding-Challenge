using System.Dynamic;
using System.Text.Json;
using Newtonsoft.Json.Linq;


public interface IApiClient
{
    Task<HttpResponseMessage> GetAsync(string url);
    Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
}

public class OrderDTO
{
    public  required Order[] Orders {get; set;}
}

public class Order
{
    public int OrderId {get; set;}
    public required Item[] Items {get; set;}
}

public class Item
{
    public required string Status {get; set;}
    public int DeliveryNotification {get; set;}
    public required string Description {get; set;}

}

public class MockApiClient: IApiClient
{
    public Task<HttpResponseMessage> GetAsync(string url)
    {
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);

        //create items
        Item item1 = new() {Status = "Delivered", DeliveryNotification = 0, Description = "This item is a wheelchair"};
        Item item2 = new(){Status = "Pending", DeliveryNotification = 0, Description = "This item is a hospital bed"};

        //create order
        Order order = new() {Items = [item1, item2], OrderId = 101};

        //create orderDTO
        OrderDTO orders = new OrderDTO 
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

        //print orders json
        Console.WriteLine($"Serialized Json = {ordersJson}");

        //set content as serialized json
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(orders));

        return Task.FromResult(httpResponseMessage);
    }

    public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);

        //create items
        Item item1 = new() {Status = "Delivered", DeliveryNotification = 0, Description = "This item is a wheelchair"};
        Item item2 = new(){Status = "Pending", DeliveryNotification = 0, Description = "This item is a hospital bed"};

        //create order
        Order order = new() {Items = [item1, item2], OrderId = 101};

        //create orderDTO
        OrderDTO orders = new OrderDTO 
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

        //print orders json
        Console.WriteLine($"Serialized Json = {ordersJson}");

        //set content as serialized json
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(orders));

        return Task.FromResult(httpResponseMessage);
    }
}
