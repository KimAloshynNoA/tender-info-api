using TenderInfoAPI.Models;

namespace TenderInfoAPI.Repositories;

public interface ITenderRepository
{
    Task<IEnumerable<Tender>?> GetTendersAsync(int page);
    Task<Tender?> GetTenderByIdAsync(string id);
    Task<int> GetPublicApiPageSizeAsync();
}
