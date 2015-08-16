namespace DownloadditLib
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RedditUtils
    {
        public static void ParsePage(
            Uri url,
            out List<string> imageUrls,
            out List<string> albumUrls,
            out string nextPageUrl
        )
        {
            imageUrls = new List<string>();
            albumUrls = new List<string>();

            string json = HttpUtils.RetrieveTextFromHttp(url);
            RedditElement redditElement = JsonConvert.DeserializeObject<RedditElement>(json);
            foreach (RedditElement post in redditElement.Data.Children)
            {
                string domain = post.Data.Domain;
                if (domain.ToLower().EndsWith("imgur.com"))
                {
                    string urlTail = post.Data.Url.Remove(0, post.Data.Url.LastIndexOf('/'));
                    if (urlTail.Contains('.'))
                    {
                        imageUrls.Add(post.Data.Url);
                    }
                    else
                    {
                        albumUrls.Add(post.Data.Url);
                    }
                }
            }

            nextPageUrl = redditElement.Data.After;
        }

        public static Uri BuildUrl(string entity, RedditEntity entityType, string cursor = null)
        {
            if (entity == null) return null;

            string url = "https://www.reddit.com";
            if (RedditEntity.Subreddit.Equals(entityType))
            {
                url += "/r/" + entity + ".json";
            }
            else
            {
                url += "/user/" + entity + "/submitted.json";
            }
            if (cursor != null)
            {
                url += "?after=" + cursor;
            }
            return new Uri(url);
        }
    }

    public enum RedditEntity { User, Subreddit }

    class RedditElement
    {
        public string Kind { get; set; }
        public RedditData Data { get; set; }

        public class RedditData
        {
            public string Domain { get; set; }  // e.g. "imgur.com"
            public string Subreddit { get; set; }
            public string Title { get; set; }  // As displayed on Reddit
            public string Url { get; set; }  // Will contain a file extention (e.g. ".jpg") if it's an individual file, or else it's an album
            public string Before { get; set; }  // Cursor for the previous page (or null if at the beginning)
            public string After { get; set; }  // Cursor for the next page (or null if at the end) 
            public List<RedditElement> Children { get; set; }  // Theoretically recursive, but only two-levels in practice... the root page, and an object per post
        }
    }
}
