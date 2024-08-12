using MediatR;
using System.ComponentModel;
using TenderInfoAPI.DTOs;
using TenderInfoAPI.Requests.Abstracts;

namespace TenderInfoAPI.Requests;

public record GetTendersBySupplierRequest : IPaginatedRequest
{
    public required string SupplierId { get; set; }
    [DefaultValue(1)]
    public int Page { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
};