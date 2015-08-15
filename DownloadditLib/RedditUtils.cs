namespace DownloadditLib
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public class RedditUtils
    {
        public static string RetrieveTextFromHttp(string url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static void RetrievePage(string url, out List<string> imageUrls, out List<string> albumUrls, out string nextPageUrl)
        {
            imageUrls = new List<string>();
            albumUrls = new List<string>();
            string json = RetrieveTextFromHttp(url);
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
    }

    public class RedditElement
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
            //public RedditMedia Media { get; set; }
            public List<RedditElement> Children { get; set; }  // Theoretically recursive, but only two-levels in practice... the root page, and an object per post

            // This class is only necessary if we care about displaying image thumbnails
            //public class RedditMedia
            //{
            //    public string Type { get; set; }
            //    public Embedded Oembed { get; set; }

            //    public class Embedded
            //    {
            //        public string Title { get; set; }  // As displayed on Imgur
            //        public string ProviderUrl { get; set; }
            //        public int ThumbnailHeight { get; set; }
            //        public int ThumbnailWidth { get; set; }
            //        public string ThumbnailUrl { get; set; }
            //    }
            //}
        }
    }
}
