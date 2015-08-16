namespace UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DownloadditLib;
    using System.Collections.Generic;
    using System;
    using System.IO;

    [TestClass]
    public class LibTests
    {
        [TestMethod]
        public void TestRetrieveTextFromHttp()
        {
            string text = HttpUtils.RetrieveTextFromHttp(new Uri("https://github.com/robots.txt"));
            string firstLine = text.Substring(0, text.IndexOf('\n'));
            Assert.AreEqual("# If you would like to crawl GitHub contact us at support@github.com.", firstLine);
        }

        [TestMethod]
        public void TestBuildUrl()
        {
            Uri shouldBeNull = RedditUtils.BuildUrl(null, RedditEntity.Subreddit);
            Assert.IsNull(shouldBeNull);

            Uri subreddit = RedditUtils.BuildUrl("downloaddit", RedditEntity.Subreddit);
            Assert.AreEqual(new Uri("https://www.reddit.com/r/downloaddit.json"), subreddit);

            Uri user = RedditUtils.BuildUrl("downloaddit", RedditEntity.User);
            Assert.AreEqual(new Uri("https://www.reddit.com/user/downloaddit/submitted.json"), user);

            Uri cursor = RedditUtils.BuildUrl("downloaddit", RedditEntity.User, "testCursor");
            Assert.AreEqual(new Uri("https://www.reddit.com/user/downloaddit/submitted.json?after=testCursor"), cursor);
        }

        [TestMethod]
        public void TestRedditParsePage()
        {
            List<string> imageUrls, albumUrls; 
            string nextPageCursor;

            Uri redditUrl = RedditUtils.BuildUrl("downloaddit", RedditEntity.User);
            RedditUtils.ParsePage(redditUrl, out imageUrls, out albumUrls, out nextPageCursor);
            
            Assert.AreEqual(7, imageUrls.Count);
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/S4C7aXQ.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/ZpfAWXh.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/7uhgGPV.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/p9rEApr.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/vK86CfT.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/QxzTjP1.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/0DIjbFw.jpg"));

            Assert.AreEqual(2, albumUrls.Count);
            Assert.IsTrue(albumUrls.Contains("http://imgur.com/gallery/iC1HR"));
            Assert.IsTrue(albumUrls.Contains("http://imgur.com/gallery/qLp4E"));

            Assert.IsNotNull(nextPageCursor);

            redditUrl = RedditUtils.BuildUrl("downloaddit", RedditEntity.User, nextPageCursor);
            RedditUtils.ParsePage(redditUrl, out imageUrls, out albumUrls, out nextPageCursor);

            Assert.AreEqual(3, imageUrls.Count);
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/zpCdbSo.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/NAjPBsa.jpg"));
            Assert.IsTrue(imageUrls.Contains("https://i.imgur.com/7gnUdfU.jpg"));

            Assert.AreEqual(2, albumUrls.Count);
            Assert.IsTrue(albumUrls.Contains("http://imgur.com/a/Rjjn2"));
            Assert.IsTrue(albumUrls.Contains("http://imgur.com/gallery/w32RT/new"));

            Assert.IsNull(nextPageCursor);
        }

        [TestMethod]
        public void TestImgurParseAlbum()
        {
            List<string> imageUrls = ImgurUtils.ParseAlbum(new Uri("http://imgur.com/gallery/qLp4E"));
            Assert.AreEqual(10, imageUrls.Count);
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/zUDXxa9.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/S4em4Pr.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/3YxxHZG.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/tAw2DK1.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/CiBlYyN.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/i5FWfzA.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/zP7P6pg.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/TrYBtzx.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/hCjiuQd.jpg"));
            Assert.IsTrue(imageUrls.Contains("http://i.imgur.com/rvw7Esb.jpg"));
        }

        [TestMethod]
        public void TestDownloadBinaryFile()
        {
            string path = Path.Combine(Path.GetTempPath(), "S4C7aXQ.jpg");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            HttpUtils.DownloadBinaryFile(new Uri("https://i.imgur.com/S4C7aXQ.jpg"), Path.GetTempPath());
            Assert.IsTrue(File.Exists(path));
        }
    }
}
