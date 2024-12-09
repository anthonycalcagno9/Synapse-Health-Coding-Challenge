using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Synapse.ProcessOrders;
using Synapse.Utilities;
using Xunit;

namespace Synapse.HandleOrders.Tests
{
    public class ProcessOrderServiceTests
    {
        [Fact]
        public void ProcessOrder_ItemShouldBeDelivered_ConfirmSendAlertMessageApiWasCalled()
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
            Mock<ILogger<ProcessOrderService>> mockLogger = new();
            ProcessOrderService processOrderService = new(httpClient, mockLogger.Object);

            //Act
            processOrderService.ProcessOrder(UtilityService.GetMockOrderShouldBeDelivered());

            //Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && 
                    req.RequestUri == new Uri("https://alert-api.com/alerts")
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public void ProcessOrder_ItemShouldNotBeDelivered_SendAlertApiIsNeverCalled()
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
            Mock<ILogger<ProcessOrderService>> mockLogger = new();
            ProcessOrderService processOrderService = new(httpClient, mockLogger.Object);

            //Act
            processOrderService.ProcessOrder(UtilityService.GetMockOrderShouldNotBeDelivered());

            //Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public void ProcessOrder_OrderShouldBeDelivered_ItemDeliveryNotificationIsIncremented()
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
            Mock<ILogger<ProcessOrderService>> mockLogger = new();
            ProcessOrderService processOrderService = new(httpClient, mockLogger.Object);
            Order order = UtilityService.GetMockOrderShouldBeDelivered();
            int originalDeliveryNotificationValue = order.Items[0].DeliveryNotification;

            //Act
            Order processedOrder = processOrderService.ProcessOrder(UtilityService.GetMockOrderShouldBeDelivered());

            //Assert
            Assert.Equal(originalDeliveryNotificationValue + 1, processedOrder.Items[0].DeliveryNotification);
        }
    }
}
