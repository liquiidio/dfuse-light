﻿using DeepReader.Apis.Options;
using DeepReader.Apis.Other;
using DeepReader.Storage;
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

        [HttpGet("transaction_with_actions/{transaction_id}&{deserialize_actions}")]
        public async Task<IActionResult> GetTransactionWithActions(string transaction_id, bool deserialize_actions = false)
        {
            var (found, transaction) = await _storage.GetTransactionAsync(transaction_id, true);
            if (found)
            {
                if (deserialize_actions)
                {
                    await ActionTraceDeserializer.DeserializeActions(transaction.ActionTraces, _storage);
                }
                return Ok(transaction);
            }

            return NotFound();
        }
    }
}
