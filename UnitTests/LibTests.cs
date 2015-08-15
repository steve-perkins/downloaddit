namespace UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DownloadditLib;
    using System.Collections.Generic;

    [TestClass]
    public class LibTests
    {
        [TestMethod]
        public void TestRetrieveTextFromHttp()
        {
            string text = RedditUtils.RetrieveTextFromHttp("https://github.com/robots.txt");
            string firstLine = text.Substring(0, text.IndexOf('\n'));
            Assert.AreEqual("# If you would like to crawl GitHub contact us at support@github.com.", firstLine);
        }

        [TestMethod]
        public void TestRetrieveRedditPage()
        {
            List<string> imageUrls, albumUrls;
            string nextPageUrl;

            RedditUtils.RetrievePage("https://www.reddit.com/user/downloaddit/submitted.json", out imageUrls, out albumUrls, out nextPageUrl);
            
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

            Assert.IsNotNull(nextPageUrl);
        }
    }
}
