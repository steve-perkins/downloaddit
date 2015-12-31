namespace DownloadditWPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

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

        /// <summary>
        /// Event handler for the "File->Exit" menu option.  Terminates the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event handler for the "File->About" menu option.  Displays descriptive text about the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(
@"Downloaddit, version 1.0
by Steve Perkins
http://steveperkins.com
https://github.com/steve-perkins/downloaddit"
    , "About");
        }

        /// <summary>
        /// Event handler for the "Find Images" button.  Fetches image URL's based on the field values in 
        /// the first tab, and uses that information to set field values in the second tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindImagesButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = DownloadImagesTabItem;

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
            int pageCount = 1;
            for ( ; pageCount <= maxPages && nextPageCursor != null; pageCount++)
            {
                Console.WriteLine("Processing page {0} for {1} {2}", pageCount, type.ToString().ToLower(), entity);
                Uri redditUrl = RedditUtils.BuildUrl(entity, type, "".Equals(nextPageCursor) ? null : nextPageCursor);
                RedditUtils.ParsePage(redditUrl, ref imageUrls, ref albumUrls, out nextPageCursor);
                if (nextPageCursor != null) Console.WriteLine("nextPageCursor == {0}", nextPageCursor);
            }
            Console.WriteLine("Parsed {0} image URL's and {1} album URL's from {2} pages", imageUrls.Count, albumUrls.Count, pageCount);

            // Parse out image URL's from the albums
            var imagesFromAlbums = albumUrls.SelectMany(albumUrl => ImgurUtils.ParseAlbum(new Uri(albumUrl)));
            imageUrls.UnionWith(imagesFromAlbums);
            Console.WriteLine("There are {0} total images after extracting {1} from albums", imageUrls.Count, imagesFromAlbums.ToList().Count);

            PageCountLabel.Content = pageCount;
            ImagesCountLabel.Content = imageUrls.Count;
            ImagesDataGrid.ItemsSource = imageUrls.Select(imageUrl => new ImgurUrl { URL = imageUrl });
        }

        /// <summary>
        /// Event handler to add row numbers to the main data grid when its contents are updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagesDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        /// <summary>
        /// Event handler to select a destination folder when the Browse button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (System.Windows.Forms.DialogResult.OK == dialogResult)
            {
                DestinationTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Event handler to download images when the Save button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string destination = DestinationTextBox.Text;
            foreach (ImgurUrl item in ImagesDataGrid.ItemsSource)
            {
                HttpUtils.DownloadBinaryFile(new Uri(item.URL), destination);
            }
            Console.WriteLine("Downloads complete!");
        }
    }

    /// <summary>
    /// Inner class to store image URL strings, for use by the main data grid.
    /// </summary>
    public class ImgurUrl
    {
        public String URL { get; set; }
    }
}
