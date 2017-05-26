using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace ZC
{

    public sealed partial class StarPage : Page
    {

        private ObservableCollection<MyImage> data;

        public StarPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DoShowImage();
        }

        private void myAdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            App._id = data.IndexOf(e.ClickedItem as MyImage);
            App.imageSource = (e.ClickedItem as MyImage).ImageUrl;
            this.Frame.Navigate(typeof(fullScreenImage));
        }

        

        private async void DoShowImage()
        {
            data = new ObservableCollection<MyImage>();
            
            var counts = 0;
            try
            {
                ObservableCollection<string> imageList = await LoadFromJsonAsync("myfavconfig");
                counts = imageList.Count;

                if (counts == 0)
                {
                    EmpytTipsTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    EmpytTipsTextBlock.Visibility = Visibility.Collapsed;
                }

                for (int i = 0; i < counts; i++)
                {
                    
                    try
                    {
                        data.Add(new MyImage()
                        {
                            //ImageUrl = "ms-appx:///Assets/sample/1.jpg"
                            //ImageUrl = "ms-appdata:///local/Save/Image/" + App.imageArr[i] + ".jpg",
                            ImageUrl = imageList[i],
                        });

                    }
                    catch
                    {

                    }
                }
            }
            catch
            {
                
            }
            myAdaptiveGridView.ItemsSource = data;



        }

        class MyImage
        {
            public string ImageUrl { get; set; }

        }

        //load id list json
        public async Task<ObservableCollection<string>> LoadFromJsonAsync(string JsonFile)
        {          
            string JsonString = await DeserializeFileAsync(JsonFile);
            if (JsonString != null)
                return JsonConvert.DeserializeObject<ObservableCollection<string>>(JsonString);
            return null;
        }

        //read json
        public async Task<string> DeserializeFileAsync(string JsonFile)
        {
            try
            {
                StorageFolder storageFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync("Fav", CreationCollisionOption.OpenIfExists);
                //StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Fav", CreationCollisionOption.OpenIfExists);
                StorageFile localFile = await storageFolder.GetFileAsync(JsonFile + ".json");
                return await FileIO.ReadTextAsync(localFile);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private void myAdaptiveGridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

            //var index = myAdaptiveGridView.Items.IndexOf(myAdaptiveGridView.SelectedItem);
            var imageSource = (((FrameworkElement)e.OriginalSource).DataContext as MyImage).ImageUrl;
            DeleteAsync(imageSource);

            data.Remove(((FrameworkElement)e.OriginalSource).DataContext as MyImage);
            myAdaptiveGridView.ItemsSource = data;

        }

        private async void DeleteAsync(string imageSource)
        {
            try
            {
                ObservableCollection<string> imageList = await LoadFromJsonAsync("myfavconfig");
                imageList.Remove(imageSource);

                Zerochan_IndexPage zi = new Zerochan_IndexPage();
                zi.saveJsonFile(imageList);
            }
            catch
            {

            }
        }
    }
}
