using System;
using ASME.WebEnd.Entities;
using ASME.WebEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASME.WebEnd.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionLedger _transactionLedger;

        public TransactionsController(ITransactionLedger transactionLedger)
        {
            _transactionLedger = transactionLedger;
        }
        
        [HttpGet("{from}/{till}")]
        public ActionResult<Transaction[]> Get(DateTimeOffset from, DateTimeOffset till)
        {
            try
            {
                return Ok(_transactionLedger.FindTransactions(from, till));
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}