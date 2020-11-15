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
        public enum TRANG_THAI_LIEN_THONG
        {
            [EnumDisplayString("Đã liên thông, không thành công")]
            DA_LIEN_THONG_KHONG_THANH_CONG = 1,
            [EnumDisplayString("Đã liên thông, thành công")]
            DA_LIEN_THONG_THANH_CONG = 2,
        }

        public enum TRANG_THAI_LIEN_THONG_REQUEST
        {
            [EnumDisplayString("Chưa có tìa khoản liên thông")]
            CHUA_CO_TAI_KHOAN = 1,
            [EnumDisplayString("Không thành công")]
            KHONG_THANH_CONG = 2,
            [EnumDisplayString("Thành công")]
            THANH_CONG = 3,
            [EnumDisplayString("Không có hồ sơ cần liên thông")]
            KHONG_CO_HO_SO_HOAC_DA_LIEN_THONG = 4,
        }
    }
}
