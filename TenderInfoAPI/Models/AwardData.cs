using Newtonsoft.Json;

namespace TenderInfoAPI.Models;

public class AwardData
{
    [JsonProperty("suppliers")]
    public List<Supplier> Suppliers { get; set; }
}
