using FourtitudeAssessment.Api.Models;

namespace FourtitudeAssessment.Api.Services
{
    public interface ITransactionService
    {
        SubmitTransactionResponse ProcessTransaction(SubmitTransactionRequest request);
    }
}
