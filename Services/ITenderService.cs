using TenderInfoAPI.DTOs;
using TenderInfoAPI.Requests;

namespace TenderInfoAPI.Services;

public interface ITenderService
{
    Task<IEnumerable<TenderDto>> GetTendersAsync(GetTendersRequest request);
    Task<TenderDto?> GetTenderByIdAsync(string id);
    Task<IEnumerable<TenderDto>?> GetTendersBySupplierAsync(GetTendersBySupplierRequest request);
}
