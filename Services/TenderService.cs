using TenderInfoAPI.DTOs;
using TenderInfoAPI.Enums;
using TenderInfoAPI.Models;
using TenderInfoAPI.Requests;
using TenderInfoAPI.Repositories;
using TenderInfoAPI.Requests.Abstracts;
using TenderInfoAPI.Exceptions;

namespace TenderInfoAPI.Services;

public class TenderService: ITenderService
{
    private readonly ITenderRepository _tenderRepository;
    private readonly ILogger<TenderService> _logger;

    public TenderService(ITenderRepository tenderRepository, ILogger<TenderService> logger)
    {
        _tenderRepository = tenderRepository;
        _logger = logger;

    }

    public async Task<IEnumerable<TenderDto>> GetTendersAsync(GetTendersRequest request)
    {
        try
        {
            var allTenders = await FetchAndPaginateTendersAsync(request);
            var filteredTenders = ApplyFiltering(allTenders, request);
            var orderedTenders = ApplyOrdering(filteredTenders, request);
            return orderedTenders.Select(MapToTenderDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting tenders");
            throw;
        }
    }

    public async Task<TenderDto?> GetTenderByIdAsync(string id)
    {
        try
        {
            var tender = await _tenderRepository.GetTenderByIdAsync(id);
            return tender != null ? MapToTenderDto(tender) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting tender by id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TenderDto>?> GetTendersBySupplierAsync(GetTendersBySupplierRequest request)
    {
        try
        {
            var allTenders = await FetchAndPaginateTendersAsync(request);
            var filteredTenders = allTenders
                .Where(t => t.AwardData.Any(ad => ad.Suppliers.Any(s => s.Id == request.SupplierId)))
                .GroupBy(t => t.Id)
                .Select(g => g.First());

            if (!filteredTenders.Any())
                return null;

            return filteredTenders.Select(MapToTenderDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting tenders by supplier");
            throw;
        }
    }

    public TenderDto MapToTenderDto(Tender tender)
    {
        return new TenderDto
        {
            Id = tender.Id,
            Date = tender.Date,
            Title = tender.Title,
            Description = tender.Description,
            AmountEur = tender.AmountEur,
            Suppliers = tender.AwardData?.SelectMany(ad => ad.Suppliers).Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList()
        };
    }

    private async Task<IEnumerable<Tender>> FetchAndPaginateTendersAsync<TRequest>(TRequest request) 
        where TRequest : IPaginatedRequest
    {
        const int maxPages = 100;
        int publicApiPageSize = await _tenderRepository.GetPublicApiPageSizeAsync();
        int maxElements = maxPages * publicApiPageSize;

        int tendersToSkip = (request.Page - 1) * request.PageSize;
        int skipWithinFetchedTenders = tendersToSkip % publicApiPageSize;

        int startPage = (tendersToSkip / publicApiPageSize) + 1;
        if (startPage > maxPages)
        {
            throw new InvalidPageRangeException(
                $"The first element of requested data exceeds the limit of {maxElements} available. You have to choose values for " +
                $"PageSize and Page parameters, so PageSize * Page is <= {maxElements}");
        }

        int endPage = (int)Math.Ceiling((tendersToSkip + request.PageSize) / (double)publicApiPageSize);
        endPage = endPage <= maxPages ? endPage : maxPages;  // The amount of data should be limited to the first 100 pages of the source API.

        var allTenders = new List<Tender>();

        for (int i = startPage; i <= endPage; i++)
        {
            var tenders = await _tenderRepository.GetTendersAsync(i);
            if (tenders != null)
            {
                allTenders.AddRange(tenders);
            }
        }

        return allTenders.Skip(skipWithinFetchedTenders).Take(request.PageSize);
    }

    private static IEnumerable<Tender> ApplyFiltering(IEnumerable<Tender> tenders, GetTendersRequest request)
    {
        if (request.MinPrice.HasValue) tenders = tenders.Where(t => t.AmountEur >= request.MinPrice.Value);
        if (request.MaxPrice.HasValue) tenders = tenders.Where(t => t.AmountEur <= request.MaxPrice.Value);
        if (request.StartDate.HasValue) tenders = tenders.Where(t => t.Date >= request.StartDate.Value);
        if (request.EndDate.HasValue) tenders = tenders.Where(t => t.Date <= request.EndDate.Value);

        return tenders;
    }

    private static IEnumerable<Tender> ApplyOrdering(IEnumerable<Tender> tenders, GetTendersRequest request)
    {
        return request.OrderBy switch
        {
            OrderByField.Price => request.OrderDirection == OrderDirection.Ascending
                ? tenders.OrderBy(t => t.AmountEur)
                : tenders.OrderByDescending(t => t.AmountEur),
            OrderByField.Date => request.OrderDirection == OrderDirection.Ascending
                ? tenders.OrderBy(t => t.Date)
                : tenders.OrderByDescending(t => t.Date),
            _ => tenders
        };
    }
}
