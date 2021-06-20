using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumCover.Models
{
    public class SongInfo : AlbumInfo
    {
        // 음악 이름
        private string _song;
        public string song { get => _song; set { _song = value; } }

        // 아티스트 이름
        private string _artist;
        public string artist { get => _artist; set { _artist = value; } }

        // 트랙 번호
        private uint _trackNum;
        public uint trackNum { get => _trackNum; set { _trackNum = value; } }

        // 장르
        private string _genre;
        public string genre { get => _genre; set { _genre = value; } }

        // 년도
        private uint _year;
        public uint year { get => _year; set { _year = value; } }
    }
}
