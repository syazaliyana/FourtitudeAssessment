using System.Text.Json.Serialization;

namespace FourtitudeAssessment.Api.Models
{
    public class SubmitTransactionResponse
    {
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("totalamount")]
        public long? TotalAmount { get; set; }

        [JsonPropertyName("totaldiscount")]
        public long? TotalDiscount {  get; set; }
        
        [JsonPropertyName("finalamount")]
        public long? FinalAmount { get; set; }

        [JsonPropertyName("resultmessage")]
        public string? ResultMessage { get; set; } = string.Empty;
    }
}
