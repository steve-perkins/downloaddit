namespace DownloadditLib
{
    using System;
    using System.IO;
    using System.Net;

    public class HttpUtils
    {
        public static string RetrieveTextFromHttp(Uri url)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (WebException e)
            {
                Console.WriteLine("There was an error retrieving URL [{0}]:  {1}", url == null ? "null" : url.ToString(), e);
                return "";
            }
        }

        public static void DownloadBinaryFile(Uri url, string targetDirectory)
        {
            // Reject URL's that are null, or contain no path following the domain name
            if (url == null || "/".Equals(url.PathAndQuery)) return;

            // Remove any query parameters
            string path = url.PathAndQuery;
            int questionMarkIndex = path.IndexOf('?');
            if (questionMarkIndex > -1)
            {
                path = path.Substring(0, questionMarkIndex);
            }

            // Remove any hashtags
            int hashIndex = path.IndexOf('#');
            if (hashIndex > -1)
            {
                path = path.Substring(0, hashIndex);
            }

            // Trim any paths prior to the file name
            string fileName = path.Substring(path.LastIndexOf('/') + 1);

            // Download and save file
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            byte[] buffer = new byte[32768];
            using (Stream input = response.GetResponseStream())
            {
                string targetFile = Path.Combine(targetDirectory, fileName);
                using (FileStream output = new FileStream(targetFile, FileMode.Create))
                {
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}
