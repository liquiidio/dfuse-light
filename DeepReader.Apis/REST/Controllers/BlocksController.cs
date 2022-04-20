using DeepReader.Apis.Options;
using DeepReader.Storage;
using DeepReader.Types.FlattenedTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
    }
}
