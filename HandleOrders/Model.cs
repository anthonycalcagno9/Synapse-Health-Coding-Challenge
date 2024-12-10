public class OrderDTO
{
    public required Order[] Orders {get; set;}
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

public class AlertData
{
    public required string Message {get; set;}
}

