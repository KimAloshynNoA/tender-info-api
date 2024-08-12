using Newtonsoft.Json;
using TenderInfoAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace TenderInfoAPI.Repositories;

public class TenderRepository : ITenderRepository
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TenderRepository> _logger;

    public TenderRepository(HttpClient httpClient, IMemoryCache cache, ILogger<TenderRepository> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<IEnumerable<Tender>?> GetTendersAsync(int page)
    {
        try
        {
            var tendersApiResponse = await GetAsync<TendersApiResponse>($"tenders?page={page}");
            return tendersApiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenders for page {Page}", page);
            throw;
        }
    }

    public async Task<Tender?> GetTenderByIdAsync(string id)
    {
        try
        {
            return await GetAsync<Tender>($"tenders/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching a tender by id {Id}", id);
            throw;
        }
    }

    public async Task<int> GetPublicApiPageSizeAsync()
    {
        const string cacheKey = "PublicApiPageSize";
        const int expTimeHours = 24;
        const int page = 1;

        try
        {
            if (!_cache.TryGetValue(cacheKey, out int publicApiPageSize))
            {
                var tendersApiResponse = await GetAsync<TendersApiResponse>($"tenders?page={page}");
                publicApiPageSize = tendersApiResponse != null ? tendersApiResponse.PageSize : 0;

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expTimeHours)
                };

                _cache.Set(cacheKey, publicApiPageSize, cacheOptions);
            }

            return publicApiPageSize;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public API page size");
            throw;
        }
    }

    private async Task<TResponse?> GetAsync<TResponse>(string endpoint) where TResponse : class
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching data from endpoint {Endpoint}", endpoint);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching data from endpoint {Endpoint}", endpoint);
            throw;
        }
    }
}
