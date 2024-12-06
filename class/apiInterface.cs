using System.Text.Json;
using Newtonsoft.Json.Linq;


public interface IApiClient
{
    Task<HttpResponseMessage> GetAsync(string url);
    Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
}

public class OrderDTO : JObject
{
    public int OrderId {get; set;}
    public required Item[] Items {get; set;}
}

public class Item : JObject
{
    public required string Status;
}

public class MockApiClient: IApiClient
{
    public Task<HttpResponseMessage> GetAsync(string url)
    {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        Item item1 = new Item {Status = "Delivered"};
        Item item2 = new Item {Status = "tacoBell"};
        OrderDTO order = new OrderDTO {Items = [item1, item2], OrderId = 101};
        OrderDTO[] responseContent = [order];
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(responseContent));

        Console.WriteLine(JsonSerializer.Serialize(order.OrderId));
        Console.WriteLine("ResponseContent = ", responseContent);
        Console.WriteLine(httpResponseMessage.Content);


        return Task.FromResult(httpResponseMessage);
    }

    public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        Item item1 = new Item {Status = "Delivered"};
        Item item2 = new Item {Status = "tacoBell"};
        OrderDTO order = new OrderDTO {Items = [item1, item2], OrderId = 101};
        OrderDTO[] responseContent = [order];
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(responseContent));


        return Task.FromResult(httpResponseMessage);
    }
}
