using FourtitudeAssessment.Api.Models;
using FourtitudeAssessment.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FourtitudeAssessment.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("submittrxmessage")]
        public IActionResult SubmitTransaction(SubmitTransactionRequest request)
        {
            var response = _transactionService.ProcessTransaction(request);

            return Ok(response);
        }
    }
}
