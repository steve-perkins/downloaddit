namespace DownloadditLib
{
    using System.IO;
    using System.Net;

    public class BaseUtils
    {
        public static string RetrieveTextFromHttp(string url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
