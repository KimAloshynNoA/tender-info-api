using Moq;
using TenderInfoAPI.Enums;
using TenderInfoAPI.Models;
using TenderInfoAPI.Requests;
using TenderInfoAPI.Services;
using TenderInfoAPI.Exceptions;
using TenderInfoAPI.Repositories;
using Microsoft.Extensions.Logging;

namespace TendersInfoApi.Tests.Services;

public class TenderServiceTests
{
    private readonly Mock<ITenderRepository> _tenderRepositoryMock;
    private readonly Mock<ILogger<TenderService>> _loggerMock;
    private readonly TenderService _tenderService;

    public TenderServiceTests()
    {
        _tenderRepositoryMock = new Mock<ITenderRepository>();
        _loggerMock = new Mock<ILogger<TenderService>>();
        _tenderService = new TenderService(_tenderRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetTendersAsync_ReturnsFilteredAndOrderedTenders()
    {
        // Arrange
        var request = new GetTendersRequest
        {
            MinPrice = 100,
            MaxPrice = 1000,
            OrderBy = OrderByField.Price,
            OrderDirection = OrderDirection.Ascending,
            Page = 1,
            PageSize = 10
        };

        var tenders = new List<Tender>
    {
        new Tender { Id = "1", AmountEur = 200 },
        new Tender { Id = "2", AmountEur = 800 },
        new Tender { Id = "3", AmountEur = 500 }
    };

        _tenderRepositoryMock.Setup(repo => repo.GetTendersAsync(It.IsAny<int>()))
                             .ReturnsAsync(tenders);

        _tenderRepositoryMock.Setup(repo => repo.GetPublicApiPageSizeAsync())
                             .ReturnsAsync(100);

        // Act
        var result = await _tenderService.GetTendersAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal("1", result.First().Id);
        _tenderRepositoryMock.Verify(repo => repo.GetTendersAsync(It.IsAny<int>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task GetTendersAsync_LogsError_WhenExceptionThrown()
    {
        // Arrange
        var request = new GetTendersRequest();
        var exception = new Exception("Test exception");

        _tenderRepositoryMock.Setup(repo => repo.GetPublicApiPageSizeAsync())
                             .ReturnsAsync(1);

        _tenderRepositoryMock.Setup(repo => repo.GetTendersAsync(It.IsAny<int>()))
                             .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tenderService.GetTendersAsync(request));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while getting tenders")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTenderByIdAsync_ReturnsTenderDto_WhenTenderExists()
    {
        // Arrange
        var tenderId = "123";
        var tender = new Tender { Id = tenderId, Title = "Tender 1" };

        _tenderRepositoryMock.Setup(repo => repo.GetTenderByIdAsync(tenderId))
                             .ReturnsAsync(tender);

        // Act
        var result = await _tenderService.GetTenderByIdAsync(tenderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tenderId, result.Id);
        _tenderRepositoryMock.Verify(repo => repo.GetTenderByIdAsync(tenderId), Times.Once);
    }

    [Fact]
    public async Task GetTenderByIdAsync_ReturnsNull_WhenTenderDoesNotExist()
    {
        // Arrange
        var tenderId = "123";
        _tenderRepositoryMock.Setup(repo => repo.GetTenderByIdAsync(tenderId))
                             .ReturnsAsync((Tender)null);

        // Act
        var result = await _tenderService.GetTenderByIdAsync(tenderId);

        // Assert
        Assert.Null(result);
        _tenderRepositoryMock.Verify(repo => repo.GetTenderByIdAsync(tenderId), Times.Once);
    }

    [Fact]
    public async Task GetTendersBySupplierAsync_ReturnsTendersFilteredBySupplier()
    {
        // Arrange
        var request = new GetTendersBySupplierRequest { SupplierId = "supplier-123", Page = 1, PageSize = 10 };

        var tenders = new List<Tender>
    {
        new Tender
        {
            Id = "1",
            AwardData = new List<AwardData>
            {
                new AwardData
                {
                    Suppliers = new List<Supplier>
                    {
                        new Supplier { Id = "supplier-123", Name = "Supplier 1" }
                    }
                }
            }
        },
        new Tender
        {
            Id = "2",
            AwardData = new List<AwardData>
            {
                new AwardData
                {
                    Suppliers = new List<Supplier>
                    {
                        new Supplier { Id = "supplier-456", Name = "Supplier 2" }
                    }
                }
            }
        }
    };

        _tenderRepositoryMock.Setup(repo => repo.GetTendersAsync(It.IsAny<int>()))
                             .ReturnsAsync(tenders);

        _tenderRepositoryMock.Setup(repo => repo.GetPublicApiPageSizeAsync())
                             .ReturnsAsync(100);

        // Act
        var result = await _tenderService.GetTendersBySupplierAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("1", result.First().Id);
    }

    [Fact]
    public async Task GetTendersBySupplierAsync_LogsError_WhenExceptionThrown()
    {
        // Arrange
        var request = new GetTendersBySupplierRequest { SupplierId = "supplier-123" };
        var exception = new Exception("Test exception");

        _tenderRepositoryMock.Setup(repo => repo.GetTendersAsync(It.IsAny<int>()))
                             .ThrowsAsync(exception);

        _tenderRepositoryMock.Setup(repo => repo.GetPublicApiPageSizeAsync())
                             .ReturnsAsync(1);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tenderService.GetTendersBySupplierAsync(request));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while getting tenders by supplier")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task FetchAndPaginateTendersAsync_ThrowsInvalidPageRangeException_WhenStartPageExceedsMax()
    {
        // Arrange
        var request = new GetTendersRequest { Page = 200, PageSize = 10 };

        _tenderRepositoryMock.Setup(repo => repo.GetPublicApiPageSizeAsync())
                             .ReturnsAsync(10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPageRangeException>(() => _tenderService.GetTendersAsync(request));
    }
}