using MediatR;
using TenderInfoAPI.DTOs;
using TenderInfoAPI.Requests.Abstracts;

namespace TenderInfoAPI.Requests;

public record GetTendersBySupplierRequest : IPaginatedRequest
{
    public required string SupplierId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
};