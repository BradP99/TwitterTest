using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Server.Controllers;
using Server.Models;
using Shouldly;
using test;
using Xunit;

namespace Test
{
    public class TwitterStreamTest
    {
        [Fact]
        public void TestStream()
        {
            var configuration = new ConfigurationBuilder().
                                AddJsonFile("appsettings.Development.json").
                                Build();
            ListLogger logger = new();
            MemoryCache cache = new(new MemoryCacheOptions());
            PollingService pollingService = new(cache, configuration);
            pollingService.StartAsync(new CancellationToken());

            Thread.Sleep(10000);
            pollingService.StopAsync(new CancellationToken());

            var controller = new TwitterFeedController(logger, cache);
            var result = controller.Get();
            result.ShouldNotBeNull();

            OkObjectResult? okResult = result as OkObjectResult;
            okResult.ShouldNotBeNull();
            okResult.StatusCode.ShouldBe(200);

            StreamInfo? streamInfo = okResult.Value as StreamInfo;
            streamInfo.ShouldNotBeNull();
            
        }
    }
}