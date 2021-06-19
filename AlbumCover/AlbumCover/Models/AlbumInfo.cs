using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TagLib;

namespace AlbumCover.Models
{
    public class AlbumInfo
    {
        // 파일 위치
        private string _path;
        public string path { get { return _path; } set { _path = value; } }

        // 음악 이름
        private string _song;
        public string song { get { return _song; } set { _song = value; } }

        // 아티스트 이름
        private string _artist;
        public string artist { get { return _artist; } set { _artist = value; } }

        // 트랙 번호
        private int _trackNum;
        public int trackNum { get { return _trackNum; } set { _trackNum = value; } }

        // 장르
        private string _genre;
        public string genre { get { return _genre; } set { _genre = value; } }

        // 앨범 커버 이미지
        private IPicture _coverArt;
        public IPicture covertArt {
            get { return _coverArt; }
            set {
                _coverArt = value;
                if (_coverArt != null)
                {
                    coverImg = changeImage(value);
                }
            }
        }

        private BitmapImage _coverImg;
        public BitmapImage coverImg { get { return _coverImg; } set { _coverImg = value; } }

        // IPicture -> BitmapImage
        private BitmapImage changeImage(IPicture picture)
        {
            MemoryStream ms = new MemoryStream(picture.Data.Data);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            return bitmap;
        }
    }
}
