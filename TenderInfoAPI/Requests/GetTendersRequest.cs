﻿using System.ComponentModel;
using TenderInfoAPI.Enums;
using TenderInfoAPI.Requests.Abstracts;

namespace TenderInfoAPI.Requests;

public record GetTendersRequest : IPaginatedRequest
{
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public OrderByField OrderBy { get; init; } = OrderByField.None;
    public OrderDirection OrderDirection { get; init; } = OrderDirection.Ascending;
    [DefaultValue(1)]
    public int Page { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
}
