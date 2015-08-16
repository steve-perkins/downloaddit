namespace DownloadditLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class ImgurUtils
    {
        public static List<string> ParseAlbum(Uri albumUrl)
        {
            HashSet<string> uniqueImageUrls = new HashSet<string>();
            string xhtml = HttpUtils.RetrieveTextFromHttp(albumUrl);
            Regex regex = new Regex("\"//i.imgur.com/[^\"]+\"");
            MatchCollection matches = regex.Matches(xhtml);
            foreach (Match match in matches)
            {
                // Format URL's
                string url = match.Value;
                url = url.Trim('"');
                url = "http:" + url;

                // Eliminate small thumbnails
                char lastCharBeforeDot = url[url.LastIndexOf('.') - 1];
                if (lastCharBeforeDot == 's')
                {
                    continue;
                }

                // Eliminate badly-formatted images (e.g. from JavaScript)
                if (url.Remove(0, 19).IndexOf('.') < 1)
                {
                    continue;
                }

                uniqueImageUrls.Add(url);
            }
            return uniqueImageUrls.ToList<string>();
        }

    }
}
