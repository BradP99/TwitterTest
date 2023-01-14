namespace Server.Models
{
    public class TopHashTags
    {
        public string? Tag { get; set; }
        public int Count { get; set; }
    }

    public class StreamInfo
    {
        public int TweetCount { get; set; }
        public List<TopHashTags> TopTags { get; set; } = new(10);
    }
}
