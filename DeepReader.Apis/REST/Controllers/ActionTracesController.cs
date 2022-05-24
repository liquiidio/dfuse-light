using DeepReader.Apis.Options;
using DeepReader.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeepReader.Apis.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionTracesController : ControllerBase
    {
        private readonly IStorageAdapter _storage;

        private ApiOptions _apiOptions;

        public ActionTracesController(IStorageAdapter storage, IOptionsMonitor<ApiOptions> apiOptionsMonitor)
        {
            _apiOptions = apiOptionsMonitor.CurrentValue;
            apiOptionsMonitor.OnChange(OnApiOptionsChanged);

            _storage = storage;
        }

        private void OnApiOptionsChanged(ApiOptions newOptions)
        {
            _apiOptions = newOptions;
        }

        [HttpGet("action_trace/{global_sequence}")]
        public async Task<IActionResult> GetActionTrace(uint globalSequence)
        {
            var (found, actionTrace) = await _storage.GetActionTraceAsync(globalSequence);
            if (found)
                return Ok(actionTrace);
            return NotFound();
        }
    }
}
