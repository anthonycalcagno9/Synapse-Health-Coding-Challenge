using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Synapse.FetchOrders;
using Synapse.Utilities;

public class FetchOrderServiceTests
{
    [Fact]
        public async Task FetchOrder_CallsOrdersApi_ReturnsSuccessAndAnOrderDTO()
        {
            //Arrange
            Mock<HttpMessageHandler> mockHttpMessageHandler = new();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(UtilityService.GetMockOrderResponseMessage());
            
            HttpClient httpClient = new(mockHttpMessageHandler.Object);
            Mock<ILogger<FetchOrderService>> mockLogger = new();
            FetchOrderService fetchOrderService = new(httpClient, mockLogger.Object);
            OrderDTO expectedResponse = UtilityService.GetMockOrderDTO();

            //Act
            OrderDTO fetchOrderResponse = await fetchOrderService.FetchMedicalEquipmentOrders();

            //Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && 
                    req.RequestUri == new Uri("https://orders-api.com/orders")
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            Assert.Equal(JsonSerializer.Serialize(expectedResponse), JsonSerializer.Serialize(fetchOrderResponse));
        }

        [Fact]
        public async Task FetchOrder_CallsOrdersApi_ReturnsFailureAndAnEmptyOrderDTO()
        {
            //Arrange
            Mock<HttpMessageHandler> mockHttpMessageHandler = new();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
            
            HttpClient httpClient = new(mockHttpMessageHandler.Object);
            Mock<ILogger<FetchOrderService>> mockLogger = new();
            FetchOrderService fetchOrderService = new(httpClient, mockLogger.Object);
            OrderDTO expectedResponse = new() { Orders = []};

            //Act
            OrderDTO fetchOrderResponse = await fetchOrderService.FetchMedicalEquipmentOrders();

            //Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && 
                    req.RequestUri == new Uri("https://orders-api.com/orders")
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            Assert.Equal(JsonSerializer.Serialize(expectedResponse), JsonSerializer.Serialize(fetchOrderResponse));
        }
}