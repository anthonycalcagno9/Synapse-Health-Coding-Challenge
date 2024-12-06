using System.Text.Json;

namespace Synapse.FetchOrders
{
    class FetchOrderService(IApiClient apiClient)
    {
        public IApiClient _apiClient = apiClient;

        public async Task<OrderDTO> FetchMedicalEquipmentOrders()
        {
            {
                string ordersApiUrl = "https://orders-api.com/orders";
                var response = await _apiClient.GetAsync(ordersApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string ordersData = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OrderDTO>(ordersData);
                }
                else
                {
                    Console.WriteLine("Failed to fetch orders from API.");
                    return new OrderDTO {Orders = []};
                }
            }
        }
    }
}