using DeepReader.Apis.Options;
using DeepReader.Apis.Other;
using DeepReader.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeepReader.Apis.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        private readonly IStorageAdapter _storage;

        private ApiOptions _apiOptions;

        public BlocksController(IStorageAdapter storage, IOptionsMonitor<ApiOptions> apiOptionsMonitor)
        {
            _apiOptions = apiOptionsMonitor.CurrentValue;
            apiOptionsMonitor.OnChange(OnApiOptionsChanged);

            _storage = storage;
        }

        private void OnApiOptionsChanged(ApiOptions newOptions)
        {
            _apiOptions = newOptions;
        }

        [HttpGet("block/{block_num}")]
        public async Task<IActionResult> GetBlock(uint block_num)
        {
            var (found, block) = await _storage.GetBlockAsync(block_num);
            if (found)
                return Ok(block);
            return NotFound();
        }

        [HttpGet("block_with_traces/{block_num}")]
        public async Task<IActionResult> GetBlockWithTraces(uint block_num)
        {
            var (found, block) = await _storage.GetBlockAsync(block_num, true);
            if (found)
                return Ok(block);
            return NotFound();
        }

        [HttpGet("block_with_traces_and_actions/{block_num}&{deserialize_actions}")]
        public async Task<IActionResult> GetBlockWithTracesAndActions(uint block_num, bool deserialize_actions = false)
        {
            var (found, block) = await _storage.GetBlockAsync(block_num, true, true);
            if (found)
            {
                if (deserialize_actions)
                {
                    await Parallel.ForEachAsync(block.Transactions, async (transaction, _) =>
                    {
                        await ActionTraceDeserializer.DeserializeActions(transaction.ActionTraces, _storage);
                    });
                }
                return Ok(block);
            }
            return NotFound();
        }
    }
}
