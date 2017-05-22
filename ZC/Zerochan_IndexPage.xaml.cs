using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ZC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Zerochan_IndexPage : Page
    {
        private static ArrayList ZcPicList;
        private static int URLIndex;
        private int i = 1;
        private static String sort, quality;

        public Zerochan_IndexPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //var MainPage = new MainPage();
            //MainPage.yiYan();

            try
            {
                ZcPicList = await Zerochan_tools.GetPages(0, 3, null, null, null);
            }
            catch (Exception)
            {

            }

            showImage();

        }

        public void showImage()
        {

            try
            {
                if (SmallDisplayImage.Source == null && MiddleDisplayImage.Source == null && LargeDisplayImage.Source == null)
                {
                    loadImageFromZcPicList(0);
                }

                var old = ctlList.Items.Count;
                for (int i = 0; i < ZcPicList.Count - old; i++)
                {
                    Zerochan_Picture zc = (Zerochan_Picture)ZcPicList[old + i];
                    Image ctlImage = new Image()
                    {
                        Source = zc.getImage(1),
                        Height = 50
                    };
                    ctlImage.Width = 2 * ctlImage.Height;
                    ctlImage.Stretch = Stretch.UniformToFill;

                    ctlList.Items.Add(ctlImage);
                }

            }
            catch (Exception)
            {
                SmallDisplayImage.Source = null;
                //MiddleDisplayImage.Source = null;
                LargeDisplayImage.Source = null;
                MiddleDisplayImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/404.png"));
                DownloadBtn.IsEnabled = false;
            }

        }

        private void loadImageFromZcPicList(int index)
        {
            Zerochan_Picture zc_pic = (Zerochan_Picture)ZcPicList[index];
            SmallDisplayImage.Source = zc_pic.getImage(1);
            MiddleDisplayImage.Source = zc_pic.getImage(2);
            LargeDisplayImage.Source = zc_pic.getImage(3);
        }

        private async void zerochan_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ZcPicList.Clear();
                ctlList.Items.Clear();
                DownloadBtn.IsEnabled = true;
                DownloadBtn.Content = "Download";
            }
            catch (Exception)
            {

            }

            i = 1;

            if (sortCb.SelectedIndex == 0)
            {
                sort = "&s=id";
            }
            else if (sortCb.SelectedIndex == 1)
            {
                sort = "&s=fav";
            }
            else if (sortCb.SelectedIndex == 2)
            {
                sort = "&s=random";
            }

            if (qualityCb.SelectedIndex == 0)
            {
                quality = "?d=0";
            }
            else if (qualityCb.SelectedIndex == 1)
            {
                quality = "?d=1";
            }
            else if (qualityCb.SelectedIndex == 2)
            {
                quality = "?d=2";
            }

            try
            {
                if (searchTextBox.Text != "")
                {
                    String correct = await Zerochan_tools.getCorrectUrl(searchTextBox.Text);
                    searchTextBox.Text = correct.Replace("http://www.zerochan.net/", "").Replace("+", " ");

                    ArrayList newlist = await Zerochan_tools.GetPages(0, 3, searchTextBox.Text, quality, sort);
                    for (int i = 0; i < newlist.Count; i++)
                    {
                        ZcPicList.Add(newlist[i]);
                    }
                }
                else
                {
                    ArrayList newlist = await Zerochan_tools.GetPages(0, 3, null, null, null);
                    for (int i = 0; i < newlist.Count; i++)
                    {
                        ZcPicList.Add(newlist[i]);
                    }
                }

                loadImageFromZcPicList(0);

                showImage();

            }
            catch (Exception)
            {
                SmallDisplayImage.Source = null;
                //MiddleDisplayImage.Source = null;
                LargeDisplayImage.Source = null;
                MiddleDisplayImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/404.png"));
                DownloadBtn.IsEnabled = false;
            }

        }

        private void ctlListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DownloadBtn.Content = "Download";

            try
            {
                ShowListViewItem();//instead

                //Image temp = (Image)ctlList.Items[URLIndex];
                //if (temp.Source != null)
                //{
                //    loadImageFromZcPicList(URLIndex);
                //}

            }
            catch (Exception)
            {

            }

        }

        private void ShowListViewItem()
        {
            URLIndex = ctlList.Items.IndexOf(ctlList.SelectedItem);
            loadImageFromZcPicList(URLIndex);
        }

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {

            if (ctlList.SelectedIndex == -1)
            {
                ctlList.SelectedIndex = 0;
            }

            if (ctlList.SelectionMode == ListViewSelectionMode.Single)
            {
                URLIndex = ctlList.Items.IndexOf(ctlList.SelectedItem);
                Zerochan_Picture zc_pic = (Zerochan_Picture)ZcPicList[URLIndex];

                try
                {
                    await SaveImage(zc_pic._largeImageUrl);

                    DownloadBtn.Content = "Okay~";
                    System.Diagnostics.Debug.WriteLine("Output Success");
                }
                catch (Exception)
                {
                    DownloadBtn.Content = "Output Error!";
                    System.Diagnostics.Debug.WriteLine("Error");
                    System.Diagnostics.Debug.WriteLine(zc_pic.getUrl().ToString());
                }
            }
            else
            {
                for (int i = 0; i < ctlList.SelectedItems.Count; i++)
                {
                    URLIndex = ctlList.Items.IndexOf(ctlList.SelectedItems[i]);
                    Zerochan_Picture zc_pic = (Zerochan_Picture)ZcPicList[URLIndex];

                    try
                    {

                        await SaveImage(zc_pic._largeImageUrl);
                        System.Diagnostics.Debug.WriteLine("Output Success");
                        DownloadBtn.Content = "Okay~ " + i;
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Error");
                        System.Diagnostics.Debug.WriteLine(zc_pic._largeImageUrl);
                        DownloadBtn.Content = "Output Error!";
                    }
                }
            }
            ctlList.SelectionMode = ListViewSelectionMode.Single;
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

        private void ctlListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ctlList.SelectionMode == ListViewSelectionMode.Multiple)
            {
                ctlList.SelectionMode = ListViewSelectionMode.Single;

            }
            if (ctlList.SelectionMode == ListViewSelectionMode.Single)
            {
                ctlList.SelectionMode = ListViewSelectionMode.Multiple;
            }

        }

        private void ctlListViewItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (ctlList.SelectionMode == ListViewSelectionMode.Multiple)
            {
                ctlList.SelectionMode = ListViewSelectionMode.Single;

            }
            if (ctlList.SelectionMode == ListViewSelectionMode.Single)
            {
                ctlList.SelectionMode = ListViewSelectionMode.Multiple;
            }
        }

        private void DisplayImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ctlList.SelectionMode == ListViewSelectionMode.Single)
            {

                listPanel.Visibility = Visibility.Collapsed;
                TopBarPanel.Visibility = Visibility.Collapsed;
                DisplayImagePack.Visibility = Visibility.Collapsed;

                //FullScreenScrollViewer = new ScrollViewer();
                FullScreenDisplayImage.Source = LargeDisplayImage.Source;

                FullScreenScrollViewer.Visibility = Visibility.Visible;
                backBtn.Visibility = Visibility.Visible;

                prevBtn.Visibility = Visibility.Collapsed;
                nextBtn.Visibility = Visibility.Collapsed;

                FullScreenScrollViewer.ZoomToFactor(1);
            }


            if (ctlList.SelectionMode == ListViewSelectionMode.Multiple)
            {
                ctlList.SelectionMode = ListViewSelectionMode.Single;
            }

        }

        private void Img_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            //initialpoint = e.Position;
        }

        private void Img_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //this.Transform.TranslateX += e.Delta.Translation.X;
            //this.Transform.TranslateY += e.Delta.Translation.Y;

            Point currentpoint = e.Position;
            if (e.Cumulative.Translation.X <= -50)//500 is the threshold value, where you want to trigger the swipe right event
            {
                //System.Diagnostics.Debug.WriteLine("Swipe Right");
                //System.Diagnostics.Debug.WriteLine(e.Cumulative.Translation.X);
                e.Complete();
                nextBtn_Click(null, e);
            }
            else if (e.Cumulative.Translation.X >= 50)
            {
                //System.Diagnostics.Debug.WriteLine("Swipe Left");
                //System.Diagnostics.Debug.WriteLine(e.Cumulative.Translation.X);
                e.Complete();
                prevBtn_Click(null, e);
            }
        }

        private void Img_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // reset the Opacity
            //this.LargeDisplayImage.Opacity = 1;
        }

        private void Imginfo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Imginfo.Visibility = Visibility.Collapsed;
        }

        private void searchTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if ((Char)e.Key != (char)13)
            {
                return;
            }
            else
            {
                zerochan_btn_Click(sender, e);
            }
        }

        private void scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (scrollViewer.HorizontalOffset >= scrollViewer.ScrollableWidth)
            {
                doChangePage();
            }
        }

        private async void doChangePage()
        {
            try
            {
                i++;

                if (i > 3)
                {
                    if (searchTextBox.Text != "")
                    {
                        ArrayList newlist = await Zerochan_tools.GetPages(i - 1, 1, searchTextBox.Text, quality, sort);
                        for (int i = 0; i < newlist.Count; i++)
                        {
                            ZcPicList.Add(newlist[i]);
                        }
                        showImage();
                    }
                    else
                    {
                        ArrayList newlist = await Zerochan_tools.GetPages(i - 1, 1, null, null, null);
                        for (int i = 0; i < newlist.Count; i++)
                        {
                            ZcPicList.Add(newlist[i]);
                        }
                        showImage();
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        //forver2
        private void InfoSwitch_Click(object sender, RoutedEventArgs e)
        {
            InfoSplit.IsPaneOpen = !InfoSplit.IsPaneOpen;
            showTags();
            System.Diagnostics.Debug.WriteLine("get info success");
        }

        public async void showTags()
        {
            tagsList.Items.Clear();
            try
            {
                URLIndex = ctlList.Items.IndexOf(ctlList.SelectedItem);

                Zerochan_Picture zc_pic = (Zerochan_Picture)ZcPicList[URLIndex];
                ArrayList tagsArr = await zc_pic.getTags();
                var id = zc_pic.getId();

                urlBlock.Content = "Source link: http://www.zerochan.net/" + id;
                urlBlock.NavigateUri = new Uri("http://www.zerochan.net/" + id);

                for (int i = 0; i < tagsArr.Count; i++)
                {
                    tagsList.Items.Add(tagsArr[i]); //标签的网址是：Zerochan_tools.returnTagUrl(arr[i])
                }
            }
            catch (Exception)
            {
                try
                {
                    Zerochan_Picture zc_pic = (Zerochan_Picture)ZcPicList[0];
                    ArrayList tagsArr = await zc_pic.getTags();
                    var id = zc_pic.getId();

                    urlBlock.Content = "Source link: http://www.zerochan.net/" + id;
                    urlBlock.NavigateUri = new Uri("http://www.zerochan.net/" + id);

                    for (int i = 0; i < tagsArr.Count; i++)
                    {
                        tagsList.Items.Add(tagsArr[i]); //标签的网址是：Zerochan_tools.returnTagUrl(arr[i])
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void tagsList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                searchTextBox.Text = tagsList.SelectedItem.ToString();
                zerochan_btn_Click(sender, e);
            }
            catch (Exception)
            {

            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            InfoSplit.IsPaneOpen = !InfoSplit.IsPaneOpen;
        }

        private async void FullScreenScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                scrollViewer.ZoomToFactor(1);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else if (scrollViewer.ZoomFactor == 1)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                scrollViewer.ZoomToFactor(2);
#pragma warning restore CS0618 // Type or member is obsolete

                var dispatcher = Window.Current.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                    scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
#pragma warning restore CS0618 // Type or member is obsolete
                });
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayImagePack.Visibility == Visibility.Collapsed)
            {
                listPanel.Visibility = Visibility.Visible;
                TopBarPanel.Visibility = Visibility.Visible;
                DisplayImagePack.Visibility = Visibility.Visible;
                FullScreenScrollViewer.Visibility = Visibility.Collapsed;
                backBtn.Visibility = Visibility.Collapsed;

                prevBtn.Visibility = Visibility.Visible;
                nextBtn.Visibility = Visibility.Visible;
            }
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = ctlList.Items.IndexOf(ctlList.SelectedItem) - 1;


            if (a < 1)
            {
                prevBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                prevBtn.Visibility = Visibility.Visible;
            }

            try
            {
                ctlList.SelectedIndex = a;

                Image temp = (Image)ctlList.Items[a];
                if (temp.Source != null && a >= 0)
                {
                    loadImageFromZcPicList(a);
                }

                nextBtn.Visibility = Visibility.Visible;
            }
            catch
            {

            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            var b = ctlList.Items.IndexOf(ctlList.SelectedItem) + 1;

            if (b >= ctlList.Items.Count - 1)
            {
                doChangePage();
            }

            try
            {
                ctlList.SelectedIndex = b;

                Image temp = (Image)ctlList.Items[b];
                if (temp.Source != null && b >= 0)
                {
                    loadImageFromZcPicList(b);

                    prevBtn.Visibility = Visibility.Visible;

                }


            }
            catch
            {

            }
        }
    }
}
