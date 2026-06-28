using FourtitudeAssessment.Api.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace FourtitudeAssessment.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly List<AllowedPartner> _allowedPartners =
        [
            new AllowedPartner{
                PartnerKey="FAKEGOOGLE",
                PartnerPassword="FAKEPASSWORD1234"
            },

            new AllowedPartner{
                PartnerKey = "FAKEPEOPLE",
                PartnerPassword = "FAKEPASSWORD4578"
            }
        ];

        public SubmitTransactionResponse ProcessTransaction(SubmitTransactionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PartnerKey))
            {
                return Fail("Partner Key is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PartnerRefNo))
            {
                return Fail("Partner Ref No is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PartnerPassword))
            {
                return Fail("Partner Password is required.");
            }

            if (request.TotalAmount <=0)
            {
                return Fail("Total amount must be greater than 0.");
            }

            if (request.Items != null && request.Items.Count > 0)
            {
                long calculatedTotalAmount = 0;

                foreach (var item in request.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.PartnerItemRef))
                        return Fail("Partner Item Ref is required.");

                    if (string.IsNullOrWhiteSpace(item.Name))
                        return Fail("Name is required");

                    if (item.Quantity <= 0 || item.Quantity > 5)
                        return Fail("Quantity must be greater than 0 AND must not exceed 5.");

                    if (item.UnitPrice <= 0)
                        return Fail("Unit Price must be greater than 0.");

                    calculatedTotalAmount += item.Quantity * item.UnitPrice;
                }

                if (calculatedTotalAmount != request.TotalAmount)
                {
                    return Fail("Invalid Total Amount.");
                }
            }

            if (!DateTime.TryParseExact(request.TimeStamp, 
                                        "yyyy-MM-ddTHH:mm:ss.fffffffZ", 
                                        CultureInfo.InvariantCulture, 
                                        DateTimeStyles.RoundtripKind,
                                        out DateTime parsedTimeStamp))
            {
                return Fail("Invalid timestamp format.");
            }

            TimeSpan difference = DateTime.UtcNow - parsedTimeStamp;
            if (Math.Abs(difference.TotalMinutes) > 5)
            {
                return Fail("Expired.");
            }

            bool partnerFound = false;
            string encodedPassword = "";
            long totalDiscount = 0;
            long finalAmount = 0;

            foreach (var partner in _allowedPartners)
            {
                if (request.PartnerKey == partner.PartnerKey)
                {
                    partnerFound = true;

                    encodedPassword = EncodeBase64(partner.PartnerPassword);
                    if (encodedPassword != request.PartnerPassword)
                    {
                        return Fail("Access Denied!");
                    }

                    string generatedSignature = GenerateSignature(parsedTimeStamp.ToString("yyyyMMddHHmmss"), request.PartnerKey, request.PartnerRefNo, request.TotalAmount, encodedPassword);
                    if (generatedSignature != request.Signature)
                    {
                        return Fail("Access Denied!");
                    }

                    double discountPercentage = CalculateDiscountPercentage(request.TotalAmount);
                    totalDiscount = (long)(request.TotalAmount * discountPercentage);

                    finalAmount = request.TotalAmount - totalDiscount;

                    break;
                }
            }

            if (!partnerFound)
            {
                return Fail("Access Denied!");
            }


            return new SubmitTransactionResponse
            {
                Result = 1,
                TotalAmount = request.TotalAmount,
                TotalDiscount = totalDiscount,
                FinalAmount = finalAmount
            };
        }

        private SubmitTransactionResponse Fail(string message)
        {
            return new SubmitTransactionResponse
            {
                Result = 0,
                ResultMessage = message
            };
        }

        private string EncodeBase64(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private string GenerateSignature(string timestamp, string partnerKey, string partnerRefNo, long totalAmount, string encodedPassword)
        {
            string rawData = timestamp + partnerKey + partnerRefNo + totalAmount + encodedPassword;

            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

            string hexaHash = Convert.ToHexStringLower(hashBytes);
            string signatureBase64 = EncodeBase64(hexaHash);
            return signatureBase64;
        }

        private double CalculateDiscountPercentage(long totalAmount)
        {
            double discountPercentage = 0;

            //base discount
            if (totalAmount < 20000)
            {
                discountPercentage = 0.0;
            }
            else if (totalAmount >= 20000 && totalAmount <= 50000)
            {
                discountPercentage = 0.05;
            }
            else if (totalAmount >= 50100 && totalAmount <= 80000)
            {
                discountPercentage = 0.07;
            }
            else if (totalAmount >= 80100 && totalAmount <= 120000)
            {
                discountPercentage = 0.1;
            }
            else if (totalAmount > 120000)
            {
                discountPercentage = 0.15;
            }

            //conditional discount
            if (IsPrime(totalAmount) && totalAmount > 50000)
            {
                discountPercentage += 0.08;
            }
            if ((totalAmount % 10 == 5) && totalAmount > 90000)
            {
                discountPercentage += 0.10;
            }

            //cap on max 20% discount
            if (discountPercentage > 0.20)
            {
                discountPercentage = 0.20;
            }

            return discountPercentage;
        }

        private bool IsPrime(long number)
        {
            if (number <= 1)
            {
                return false;
            }

            for (long i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
