using Newtonsoft.Json;

namespace TenderInfoAPI.Models;

public class Tender
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("awarded_value_eur")]
    public decimal AmountEur { get; set; }

    [JsonProperty("awarded")]
    public List<AwardData> AwardData { get; set; }
}