using Newtonsoft.Json;

namespace TenderInfoAPI.Models;

public class TendersApiResponse
{
    [JsonProperty("page_count")]
    public int PageCount { get; set; }

    [JsonProperty("page_number")]
    public int PageNumber { get; set; }

    [JsonProperty("page_size")]
    public int PageSize { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("data")]
    public List<Tender> Data { get; set; }
}
