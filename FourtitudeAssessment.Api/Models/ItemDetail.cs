using System.Text.Json.Serialization;

namespace FourtitudeAssessment.Api.Models
{
    public class ItemDetail
    {
        [JsonPropertyName("partneritemref")]
        public string PartnerItemRef { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("qty")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitprice")]
        public long UnitPrice { get; set; }
    }
}