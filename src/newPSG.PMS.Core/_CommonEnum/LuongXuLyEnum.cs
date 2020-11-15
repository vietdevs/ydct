using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public enum LOAI_FILE
        {
            [EnumDisplayString("mp4")]
            MP4 = 1,
            [EnumDisplayString("mp3")]
            MP3 = 2,
            [EnumDisplayString("pdf")]
            PDF = 3,
        }
    }
}
