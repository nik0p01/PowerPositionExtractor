using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Axpo;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using PowerPositionWorkerService.Services;

using Xunit;

namespace PowerPositionWorkerService.Tests
{
    public class PowerServiceClientTests
    {
        [Fact]
        public async Task GetTradesAsync_ReturnsTrades_WhenServiceReturnsTrades()
        {
            // Arrange
            var logger = new NullLogger<PowerTradesServiceClient>();

            var mockService = new Mock<IPowerService>();
            var date = new DateTime(2025, 1, 1);

            var sampleTrades = new[] { PowerTrade.Create(date, 24) };
            mockService
                .Setup(s => s.GetTradesAsync(date))
                .ReturnsAsync(sampleTrades);

            var client = new PowerTradesServiceClient(logger, mockService.Object);

            // Act
            var trades = await client.GetTradesAsync(date);

            // Assert
            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.Equal(sampleTrades.Length, trades.Count());
        }

        [Fact]
        public async Task GetTradesAsync_ReturnsEmpty_WhenServiceReturnsNull()
        {
            // Arrange
            var logger = new NullLogger<PowerTradesServiceClient>();

            var mockService = new Mock<IPowerService>();
            var date = new DateTime(2025, 1, 1);

            var sampleTrades = new[] { PowerTrade.Create(date, 24) };
            mockService
                .Setup(s => s.GetTradesAsync(date))
                .ReturnsAsync((IEnumerable<PowerTrade>)null);

            var client = new PowerTradesServiceClient(logger, mockService.Object);

            // Act
            var trades = await client.GetTradesAsync(date);

            // Assert
            Assert.NotNull(trades);
            Assert.Empty(trades);
        }
    }
}
