using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ZC
{
    public class Zerochan_Picture
    {
        public string _id { get; set; }
        private string _url;
        public string _smallImageUrl;
        public string _middleImageUrl;
        public string _largeImageUrl;
        private BitmapImage _smallImage;
        private BitmapImage _middleImage;
        private BitmapImage _largeImage;

        public Zerochan_Picture(string _url)
        {
            this._url = _url;
            this._smallImageUrl = _url;
            this._middleImageUrl = _smallImageUrl.Replace("240", "600").Replace("s3", "s1");
            this._largeImageUrl = _smallImageUrl.Replace("240", "full").Replace("s3", "static");
            this._smallImage = setBitmapImage(1);
            this._middleImage = setBitmapImage(2);
            this._largeImage = setBitmapImage(3);
            this._id = _url.Substring(_url.IndexOf(".240.") + 5).Replace(".jpg", "");

        }

        private BitmapImage setBitmapImage(int _size)
        {
            if (_size == 1)
            {
                return new BitmapImage(new Uri(_smallImageUrl, UriKind.Absolute));
            }else if (_size == 2)
            {
                return new BitmapImage(new Uri(_middleImageUrl, UriKind.Absolute));
            }else if (_size == 3)
            {
                return new BitmapImage(new Uri(_largeImageUrl, UriKind.Absolute));
            }
            else
            {
                return null;
            }
        }

        public async Task<ArrayList> getTags()
        {
            return await Zerochan_tools.getPicTags("http://www.zerochan.net/" + _id);
        }

        public BitmapImage getImage(int _size)
        {
            if(_size ==1)
            {
                return _smallImage;
            }
            else if(_size == 2)
            {
                return _middleImage;
            }
            else
            {
                return _largeImage;
            }
        }

        public string getId()
        {
            return _id;
        }

        public string getUrl()
        {
            return _url;
        }
    }
}
