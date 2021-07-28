using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DigitalPalette.Models
{
    public class ColorChip
    {
        // 이름
        private string _name = string.Empty;
        public string name { get => _name; set { _name = value; } }

        // 저장 위치
        private string _path = string.Empty;
        public string path { get => _path; set { _path = value; } }

        // show index Color
        private ColorInfo _idxColor = new ColorInfo();
        public ColorInfo idxColor { get => _idxColor; set { _idxColor = value; } }

        // Colors
        private List<ColorInfo> _colors = new List<ColorInfo>();

        public List<ColorInfo> colors { get => _colors; set { _colors = value; } }

    }
}
