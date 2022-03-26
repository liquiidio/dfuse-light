using DeepReader.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeepReader.Apis.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IStorageAdapter _storage;

        public TransactionsController(IStorageAdapter storage)
        {
            _storage = storage;
        }

        [HttpGet("transaction/{transaction_id}")]
        public async Task<IActionResult> GetTransaction(string transaction_id)
        {
            var (found, transaction) = await _storage.GetTransactionAsync(transaction_id);
            if (found)
                return Ok(transaction);
            return NotFound();
        }
    }
}
