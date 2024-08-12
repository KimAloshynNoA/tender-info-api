using Moq;
using System.Net;
using Moq.Protected;
using Newtonsoft.Json;
using TenderInfoAPI.Models;
using TenderInfoAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace TenderInfoApi.Tests.Repositories;

public class TenderRepositoryTests
{
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<ILogger<TenderRepository>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly TenderRepository _tenderRepository;
    private const string testUrl = "https://sometesturl";


    public TenderRepositoryTests()
    {
        _cacheMock = new Mock<IMemoryCache>();
        _loggerMock = new Mock<ILogger<TenderRepository>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(testUrl)
        };

        _tenderRepository = new TenderRepository(_httpClient, _cacheMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetTendersAsync_ReturnsTenders_WhenApiReturnsSuccess()
    {
        // Arrange
        const int page = 1;
        var tendersResponse = new TendersApiResponse
        {
            Data =
            [
                new() { Id = "123", Title = "Tender 1" },
                new() { Id = "456", Title = "Tender 2" }
            ]
        };
        var responseContent = new StringContent(JsonConvert.SerializeObject(tendersResponse));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = responseContent
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);


        var tenderRepository = new TenderRepository(_httpClient, _cacheMock.Object, _loggerMock.Object);

        // Act
        var result = await tenderRepository.GetTendersAsync(page);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetTendersAsync_LogsError_WhenExceptionThrown()
    {
        // Arrange
        const int page = 1;
        var mockHandler = new Mock<HttpMessageHandler>();
        var exception = new HttpRequestException("Error occurred");

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        var tenderRepository = new TenderRepository(_httpClient, _cacheMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => tenderRepository.GetTendersAsync(page));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error fetching tenders for page")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTenderByIdAsync_ReturnsTender_WhenApiReturnsSuccess()
    {
        // Arrange
        const string tenderId = "123";
        var tender = new Tender { Id = tenderId, Title = "Tender 1" };
        var responseContent = new StringContent(JsonConvert.SerializeObject(tender));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = responseContent
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var tenderRepository = new TenderRepository(_httpClient, _cacheMock.Object, _loggerMock.Object);

        // Act
        var result = await tenderRepository.GetTenderByIdAsync(tenderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tenderId, result.Id);
    }

    [Fact]
    public async Task GetTenderByIdAsync_LogsError_WhenExceptionThrown()
    {
        // Arrange
        const string tenderId = "123";
        var exception = new HttpRequestException("Error occurred");

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        var tenderRepository = new TenderRepository(_httpClient, _cacheMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => tenderRepository.GetTenderByIdAsync(tenderId));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error fetching a tender by id")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPublicApiPageSizeAsync_LogsError_WhenExceptionThrown()
    {
        // Arrange
        var exception = new HttpRequestException("Error occurred");

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        var tenderRepository = new TenderRepository(_httpClient, _cacheMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => tenderRepository.GetPublicApiPageSizeAsync());
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error fetching public API page size")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
