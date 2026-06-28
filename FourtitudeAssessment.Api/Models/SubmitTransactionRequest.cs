using System.Text.Json.Serialization;

namespace FourtitudeAssessment.Api.Models
{
    public class SubmitTransactionRequest
    {
        [JsonPropertyName("partnerkey")]
        public string PartnerKey { get; set; } = string.Empty;

        [JsonPropertyName("partnerrefno")]
        public string PartnerRefNo { get; set; } = string.Empty;

        [JsonPropertyName("partnerpassword")]
        public string PartnerPassword {  get; set; } = string.Empty;

        [JsonPropertyName("totalamount")]
        public long TotalAmount { get; set; }

        [JsonPropertyName("items")]
        public List<ItemDetail> Items { get; set; } = new();

        [JsonPropertyName("timestamp")]
        public string TimeStamp { get; set; } = string.Empty;

        [JsonPropertyName("sig")]
        public string Signature {  get; set; } = string.Empty;
    }
}
