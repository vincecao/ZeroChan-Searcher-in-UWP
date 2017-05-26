using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZC
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class fullScreenImage : Page
    {
        private static string JsonFile;
        private ObservableCollection<string> TextList = new ObservableCollection<string>();

        public fullScreenImage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var imageSource = App.imageSource;
            FullScreenImage.Source = new BitmapImage(new Uri(imageSource));
            HeaderGrid.Visibility = Visibility.Visible;
            FooterStackPanel.Visibility = Visibility.Visible;
            ShowBtn.Visibility = Visibility.Collapsed;

#pragma warning disable CS0618 // Type or member is obsolete
            scrollViewer.ZoomToFactor(1);
#pragma warning restore CS0618 // Type or member is obsolete

            ImageLinkUrlHyperLinkBtn.Content = "Image Source: zerochan.net/" + substringImageId(imageSource);
            var url = "http://zerochan.net/" + substringImageId(imageSource);
            ImageLinkUrlHyperLinkBtn.NavigateUri = new Uri(url);


        }

        private string substringImageId(string largeImageSource)
        {
            var imageId = Zerochan_tools.trimStr(new System.Text.StringBuilder(largeImageSource), "full.", ".jpg");
            return imageId.ToString();
        }

        private void back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }

        }

        private void HideBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HeaderGrid.Visibility = Visibility.Collapsed;
            FooterStackPanel.Visibility = Visibility.Collapsed;
            ShowBtn.Visibility = Visibility.Visible;
        }

        private void ShowBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HeaderGrid.Visibility = Visibility.Visible;
            FooterStackPanel.Visibility = Visibility.Visible;
            ShowBtn.Visibility = Visibility.Collapsed;
        }

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
                try
                {
                var sourceString = App.imageSource;
                    await SaveImage(sourceString.ToString());

                    DownloadBtn.Content = "Okay~";
                    System.Diagnostics.Debug.WriteLine("Output Success");
                }
                catch (Exception)
                {
                    DownloadBtn.Content = "Output Error!";
                    System.Diagnostics.Debug.WriteLine("Error");
                    //System.Diagnostics.Debug.WriteLine(zc_pic.getUrl().ToString());
                }
            }

        public static async Task SaveImage(String URL)
        {
            try
            {
                //-< show Image as Thumbnail >-
                StorageFolder downloadsPath = KnownFolders.PicturesLibrary;
                StorageFolder zerochanFolder = await downloadsPath.CreateFolderAsync("Zerochan", CreationCollisionOption.OpenIfExists);
                StorageFile saveImage = await zerochanFolder.CreateFileAsync(URL.Substring(27), CreationCollisionOption.GenerateUniqueName);

                String abc = zerochanFolder.Path;

                HttpClient client = new HttpClient();

                byte[] responseBytes = await client.GetByteArrayAsync(URL);
                var stream = await saveImage.OpenAsync(FileAccessMode.ReadWrite);

                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    DataWriter writer = new DataWriter(outputStream);
                    writer.WriteBytes(responseBytes);
                    await writer.StoreAsync();
                    await outputStream.FlushAsync();
                }
                stream.Dispose();
            }

            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
