using DeepReader.Apis.Options;
using DeepReader.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DeepReader.Apis.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IStorageAdapter _storage;

        private ApiOptions _apiOptions;

        public TransactionsController(IStorageAdapter storage, IOptionsMonitor<ApiOptions> apiOptionsMonitor)
        {
            _apiOptions = apiOptionsMonitor.CurrentValue;
            apiOptionsMonitor.OnChange(OnApiOptionsChanged);

            _storage = storage;
        }
        private void OnApiOptionsChanged(ApiOptions newOptions)
        {
            _apiOptions = newOptions;
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
