using Microsoft.AspNetCore.Mvc;
using Tweetinvi.Models;
using Tweetinvi;
using Microsoft.Extensions.Caching.Memory;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitterFeedController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public TwitterFeedController(ILogger logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet(Name = "GetStreamData")]
        public IActionResult Get()
        {
            try
            {
                StreamInfo? sInfo = _cache.Get<StreamInfo>("TwitterStream");
                return new OkObjectResult(new StreamInfo { TweetCount = sInfo?.TweetCount ?? 0, TopTags = sInfo?.TopTags?.Take(10)?.ToList() });
            }
            catch
            {
                return new ObjectResult(new { Error = true, Message = "Error getting stream info" });
            }
        }
    }
}