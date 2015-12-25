namespace DownloadditWPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using DownloadditLib;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(
@"Downloaddit, version 1.0
by Steve Perkins
http://steveperkins.com
https://github.com/steve-perkins/downloaddit"
    , "About");
        }

        private void FindImagesButton_Click(object sender, RoutedEventArgs e)
        {
            
            // TODO:  Refactor, and make this asynchronous so it doesn't block the main UI thread

            // Get user input from XAML fields
            string entity = EntityTextBox.Text;
            RedditEntity type = (UserEntityType.IsChecked == true) ? RedditEntity.User : RedditEntity.Subreddit;
            int maxPages;
            if (!int.TryParse(MaxPagesTextBox.Text, out maxPages)) maxPages = 1;

            // Parse Imgur image and album URL's for the given Reddit user or subuser, stopping when we 
            // either run out of pages or else hit the maximum page count
            HashSet<string> imageUrls = new HashSet<string>();
            HashSet<string> albumUrls = new HashSet<string>();
            string nextPageCursor = "";
            for (int index = 0; index < maxPages && nextPageCursor != null; index++)
            {
                Console.WriteLine("Processing page {0} for {1} {2}", index + 1, type.ToString().ToLower(), entity);
                Uri redditUrl = RedditUtils.BuildUrl(entity, type, "".Equals(nextPageCursor) ? null : nextPageCursor);
                RedditUtils.ParsePage(redditUrl, ref imageUrls, ref albumUrls, out nextPageCursor);
                if (nextPageCursor != null) Console.WriteLine("nextPageCursor == {0}", nextPageCursor);
            }
            Console.WriteLine("Found {0} image URL's and {1} album URL's", imageUrls.Count, albumUrls.Count);

            // Parse out image URL's from the albums
            var imagesFromAlbums = albumUrls.SelectMany(albumUrl => ImgurUtils.ParseAlbum(new Uri(albumUrl)));
            imageUrls.UnionWith(imagesFromAlbums);
            Console.WriteLine("There are {0} total images after extracting {1} from albums", imageUrls.Count, imagesFromAlbums.ToList().Count);

            // TODO:  Populate the data grid and other fields on the second tab
        }
    }
}
