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
        public enum LOAI_FILE_KY
        {
            [EnumDisplayString("Đơn đăng ký")]
            DON_DANG_KY = 1,
            [EnumDisplayString("Công văn")]
            CONG_VAN = 2,
            [EnumDisplayString("Giấy chứng nhận")]
            GIAY_CHUNG_NHAN = 3
        }
        public enum LUONG_XU_LY_TT3
        {
            [EnumDisplayString("Thẩm định cho phép kiểm nghiệm")]
            THAM_DINH_CHO_PHEP_KIEM_NGHIEM = 1,
            [EnumDisplayString("Thẩm định cấp số đăng ký")]
            THAM_DINH_CAP_SO_DANG_KY = 2
        }
    }
}
