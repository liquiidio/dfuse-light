using DeepReader.Storage;
using DeepReader.Types.FlattenedTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeepReader.Apis.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        private readonly IStorageAdapter _storage;

        public BlocksController(IStorageAdapter storage)
        {
            _storage = storage;
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
