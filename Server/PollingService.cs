using Microsoft.Extensions.Caching.Memory;
using Server.Models;
using Tweetinvi;
using Tweetinvi.Models;

public class PollingService : BackgroundService
{
    private const string key = "TwitterStream";
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public PollingService(IMemoryCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string? twitterAPIKey = _configuration["AppSettings:TwitterAPIKey"];
        string? twitterSecretKey = _configuration["AppSettings:TwitterSecretKey"];
        string? twitterBearerToken = _configuration["AppSettings:TwitterBearerToken"];

        if (twitterAPIKey != null && twitterSecretKey != null && twitterBearerToken != null)
        {
            var appCredentials = new ConsumerOnlyCredentials(twitterAPIKey, twitterSecretKey)
            {
                BearerToken = twitterBearerToken // bearer token is optional in some cases
            };

            var appClient = new TwitterClient(appCredentials);

            var sampleStream = appClient.StreamsV2.CreateSampleStream();
            sampleStream.TweetReceived += (sender, eventArgs) =>
            {
                string tweet = eventArgs.Tweet.Text;
                var hashTags = eventArgs.Tweet.Entities.Hashtags;

                StreamInfo? info = _cache.Get<StreamInfo>(key);

                if (info != null)
                {
                    info.TweetCount++;
                    if (hashTags != null)
                    {
                        foreach (var tag in hashTags)
                        {
                            TopHashTags? exists = info.TopTags.Find(m => m.Tag == tag.Tag);
                            if (exists != null)
                            {
                                exists.Count++;
                            }
                            else
                            {
                                info.TopTags.Add(new TopHashTags { Tag = tag.Tag, Count = 1 });
                            }
                        }
                        info.TopTags = info.TopTags.OrderByDescending(k => k.Count).ToList();
                    }
                    _cache.Set(key, info);
                }
                else
                {
                    info = new StreamInfo { TweetCount = 1 };

                    if (hashTags != null)
                    {
                        foreach (var tag in hashTags)
                        {
                            info.TopTags.Add(new TopHashTags { Tag = tag.Tag, Count = 1 });
                        }
                    }
                    _cache.Set(key, info);
                }
            };
            await sampleStream.StartAsync();
        }
    }
}