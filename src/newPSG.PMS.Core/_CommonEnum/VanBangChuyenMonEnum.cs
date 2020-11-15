using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public enum VAN_BANG_CHUYEN_MON
        {
            [EnumDisplayString("Bác sỹ, y sỹ")]
            BAC_SY_Y_SY = 1,
            [EnumDisplayString("Điều dưỡng viên")]
            DIEU_DUONG_VIEN = 2,
            [EnumDisplayString("Hộ sinh viên")]
            HO_SINH_VIEN = 3,
            [EnumDisplayString("Kỹ thuật viên")]
            KY_THUAT_VIEN = 4,
            [EnumDisplayString("Lương y")]
            LUONG_Y = 5,
            [EnumDisplayString("Ngươi có bài thuốc gia chuyên hoắc có pp chữa bệnh gia chuyền")]
            NGUOI_CO_BAI_THUOC_GIA_CHUYEN = 6,
        }
    }
}
