using DeepReader.Apis.Options;
using DeepReader.Apis.Other;
using DeepReader.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

        [HttpGet("action_trace/{global_sequence}&{deserialize_actions}")]
        public async Task<IActionResult> GetActionTrace(uint globalSequence, bool deserialize_actions = false)
        {
            var (found, actionTrace) = await _storage.GetActionTraceAsync(globalSequence);
            if (found)
            {
                if (deserialize_actions)
                {
                    await ActionTraceDeserializer.DeserializeAction(actionTrace, _storage);
                }
                return Ok(actionTrace);
            }
            return NotFound();
        }
    }
}
