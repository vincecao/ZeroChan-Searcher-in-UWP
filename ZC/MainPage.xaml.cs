using System;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ZC
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
            {
                var statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }

            base.OnNavigatedTo(e);
            myFrame.Navigate(typeof(Zerochan_IndexPage));

        }

        private void Togglebutton_Click(object sender, RoutedEventArgs e)
        {
            //yiYan();
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;

            FooterPanel.Visibility = Visibility.Visible;


        }

        private void Splitter_PaneClosed(SplitView sender, object args)
        {
            FooterPanel.Visibility = Visibility.Collapsed;
        }

        public void yiYan()
        {
            try
            {
                //StringBuilder yiYan = await Zerochan_tools.getLinkContant("http://api.hitokoto.cn/?c=f&text=1");
                //oneWord.Text = yiYan.ToString() + "\n\n\n\t   —— 一言";
                oneWord.Text = "";
            }
            catch (Exception)
            {
                oneWord.Text = "网络有点小问题呢\nThe Network Is Missing\nインターネット接続できません";
            }
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StarListBoxItem.IsSelected)
            {
                myFrame.Navigate(typeof(StarPage));
                headerTitle.Text = "Favorite(beta)";
            }
            else if(IndexListBoxItem.IsSelected)
            {
                myFrame.Navigate(typeof(Zerochan_IndexPage));
                headerTitle.Text = "ZeroChan";
            }
            else
            {
                myFrame.Navigate(typeof(Zerochan_IndexPage));
                headerTitle.Text = "ZeroChan";
            }
        }
    }
}
