using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _db;

        public RedisController(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        [HttpGet("get/{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _db.StringGetAsync(key);
            return Ok(value.ToString());
        }

        [HttpPost("set")]
        public async Task<IActionResult> Set([FromQuery] string key, [FromQuery] string value)
        {
            await _db.StringSetAsync(key, value);
            return Ok("Saved");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] string key, [FromQuery] string newValue)
        {
            // Check if the key exists first
            bool exists = await _db.KeyExistsAsync(key);

            if (!exists)
                return NotFound($"Key '{key}' does not exist.");

            // Overwrite the existing value
            await _db.StringSetAsync(key, newValue);
            return Ok($"Key '{key}' updated.");
        }

        [HttpDelete("delete/{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            bool exists = await _db.KeyExistsAsync(key);

            if (!exists)
                return NotFound($"Key '{key}' does not exist.");

            bool deleted = await _db.KeyDeleteAsync(key);

            if (deleted)
                return Ok($"Key '{key}' deleted.");
            else
                return StatusCode(500, "Failed to delete the key.");
        }
    }
}
