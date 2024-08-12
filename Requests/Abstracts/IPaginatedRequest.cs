using MediatR;
using TenderInfoAPI.DTOs;

namespace TenderInfoAPI.Requests.Abstracts
{
    public interface IPaginatedRequest : IRequest<IEnumerable<TenderDto>>
    {
        int Page { get; set; }
        int PageSize { get; set; }
    }
}
