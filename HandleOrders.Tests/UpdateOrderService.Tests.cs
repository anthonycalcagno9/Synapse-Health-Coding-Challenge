using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Synapse.UpdateOrder;
using Synapse.Utilities;

public class UpdateOrderServiceTests
{
    [Fact]
        public async Task UpdateOrder_CallsUpdateApi_UpdateApiIsCalledExactlyOnceWithCorrectParameters()
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
            Mock<ILogger<UpdateOrderService>> mockLogger = new();
            UpdateOrderService updateOrderService = new(httpClient, mockLogger.Object);

            //Act
            await updateOrderService.SendAlertAndUpdateOrder(UtilityService.GetMockOrderShouldBeDelivered());

            //Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && 
                    req.RequestUri == new Uri("https://update-api.com/update")
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
}