using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public enum TENANT_TYPE
        {
            [EnumDisplayString("Doanh nghiệp")]
            DOANH_NGHIEP = 1,
            [EnumDisplayString("HCC Cục")]
            HCC_CUC = 2,
            [EnumDisplayString("HCC Chi cục")]
            HCC_CHI_CUC = 3
        }
        public enum THU_TUC_ID
        {
            [EnumDisplayString("TT-99")]
            THU_TUC_99 = 99,
            [EnumDisplayString("TT-98")]
            THU_TUC_98 = 98,

            #region Danh sách thủ tục
            [EnumDisplayString("TT-01")]
            THU_TUC_01 = 1,
            [EnumDisplayString("TT-02")]
            THU_TUC_02 = 2,
            [EnumDisplayString("TT-03")]
            THU_TUC_03 = 3,
            [EnumDisplayString("TT-04")]
            THU_TUC_04 = 4,
            [EnumDisplayString("TT-05")]
            THU_TUC_05 = 5,
            [EnumDisplayString("TT-06")]
            THU_TUC_06 = 6,
            [EnumDisplayString("TT-07")]
            THU_TUC_07 = 7,
            [EnumDisplayString("TT-08")]
            THU_TUC_08 = 8,
            [EnumDisplayString("TT-09")]
            THU_TUC_09 = 9,
            [EnumDisplayString("TT-10")]
            THU_TUC_10 = 10,
            [EnumDisplayString("TT-11")]
            THU_TUC_11 = 11,
            [EnumDisplayString("TT-12")]
            THU_TUC_12 = 12,
            [EnumDisplayString("TT-13")]
            THU_TUC_13 = 13,
            [EnumDisplayString("TT-14")]
            THU_TUC_14 = 14,
            [EnumDisplayString("TT-15")]
            THU_TUC_15 = 15,
            [EnumDisplayString("TT-16")]
            THU_TUC_16 = 16,
            [EnumDisplayString("TT-17")]
            THU_TUC_17 = 17,
            [EnumDisplayString("TT-18")]
            THU_TUC_18 = 18,
            [EnumDisplayString("TT-19")]
            THU_TUC_19 = 19,
            [EnumDisplayString("TT-20")]
            THU_TUC_20 = 20,
            [EnumDisplayString("TT-21")]
            THU_TUC_21 = 21,
            [EnumDisplayString("TT-22")]
            THU_TUC_22 = 22,
            [EnumDisplayString("TT-23")]
            THU_TUC_23 = 23,
            [EnumDisplayString("TT-24")]
            THU_TUC_24 = 24,
            [EnumDisplayString("TT-25")]
            THU_TUC_25 = 25,
            [EnumDisplayString("TT-26")]
            THU_TUC_26 = 26,
            [EnumDisplayString("TT-27")]
            THU_TUC_27 = 27,
            [EnumDisplayString("TT-28")]
            THU_TUC_28 = 28,
            [EnumDisplayString("TT-29")]
            THU_TUC_29 = 29,
            [EnumDisplayString("TT-30")]
            THU_TUC_30 = 30,
            [EnumDisplayString("TT-31")]
            THU_TUC_31 = 31,
            [EnumDisplayString("TT-32")]
            THU_TUC_32 = 32,
            [EnumDisplayString("TT-33")]
            THU_TUC_33 = 33,
            [EnumDisplayString("TT-34")]
            THU_TUC_34 = 34,
            [EnumDisplayString("TT-35")]
            THU_TUC_35 = 35,
            [EnumDisplayString("TT-36")]
            THU_TUC_36 = 36,
            [EnumDisplayString("TT-37")]
            THU_TUC_37 = 37,
            [EnumDisplayString("TT-38")]
            THU_TUC_38 = 38,
            [EnumDisplayString("TT-39")]
            THU_TUC_39 = 39,
            [EnumDisplayString("TT-40")]
            THU_TUC_40 = 40,
            [EnumDisplayString("TT-41")]
            THU_TUC_41 = 41,
            [EnumDisplayString("TT-42")]
            THU_TUC_42 = 42,
            [EnumDisplayString("TT-43")]
            THU_TUC_43 = 43,
            [EnumDisplayString("TT-44")]
            THU_TUC_44 = 44,
            [EnumDisplayString("TT-45")]
            THU_TUC_45 = 45,
            [EnumDisplayString("TT-46")]
            THU_TUC_46 = 46,
            [EnumDisplayString("TT-47")]
            THU_TUC_47 = 47,
            [EnumDisplayString("TT-48")]
            THU_TUC_48 = 48,
            [EnumDisplayString("TT-49")]
            THU_TUC_49 = 49,
            [EnumDisplayString("TT-50")]
            THU_TUC_50 = 50,
            [EnumDisplayString("TT-51")]
            THU_TUC_51 = 51,
            [EnumDisplayString("TT-52")]
            THU_TUC_52 = 52,
            [EnumDisplayString("TT-53")]
            THU_TUC_53 = 53,
            [EnumDisplayString("TT-54")]
            THU_TUC_54 = 54,
            [EnumDisplayString("TT-55")]
            THU_TUC_55 = 55,
            [EnumDisplayString("TT-56")]
            THU_TUC_56 = 56,
            [EnumDisplayString("TT-59")]
            THU_TUC_59 = 59
            #endregion
        }
        public class MA_THU_TUC
        {
            public static string THU_TUC_99 = "THUTUC99";
            public static string THU_TUC_98 = "THUTUC98";

            public static string THU_TUC_01 = "THUTUC01";
            public static string THU_TUC_02 = "THUTUC02";
            public static string THU_TUC_03 = "THUTUC03";
            public static string THU_TUC_04 = "THUTUC04";
            public static string THU_TUC_05 = "THUTUC05";
            public static string THU_TUC_06 = "THUTUC06";
            public static string THU_TUC_07 = "THUTUC07";
            public static string THU_TUC_08 = "THUTUC08";
            public static string THU_TUC_09 = "THUTUC09";
            public static string THU_TUC_10 = "THUTUC10";

            public static string THU_TUC_37 = "THUTUC37";
        }
        public enum QUI_TRINH_THAM_DINH
        {
            [EnumDisplayString("Qui trình 1 chuyên viên thẩm định")]
            QT_1_CHUYEN_VIEN = 1,
            [EnumDisplayString("Qui trình 2 chuyên viên thẩm định")]
            QT_2_CHUYEN_VIEN = 2
        }
        public enum LUONG_XU_LY
        {
            [EnumDisplayString("Luồng hồ sơ")]
            LUONG_HO_SO = 1,
            [EnumDisplayString("Luồng thực địa")]
            LUONG_THUC_DIA = 2
        }

        #region Chữ ký
        public enum LOAI_CHU_KY
        {
            [EnumDisplayString("Chữ ký")]
            CHU_KY = 1,
            [EnumDisplayString("Dấu của Cục")]
            DAU_CUA_CUC = 2
        }
        #endregion

        #region TT05
        public enum LOAI_TAI_LIEU_TT05
        {
            [EnumDisplayString("Chứng nhận đăng ký kinh doanh")]
            CHUNG_NHAN_DANG_KY_KINH_DOANH = 1,
            [EnumDisplayString("Danh sách người tập huấn ATTP")]
            DANH_SACH_NGUOI_TAP_HUAN = 2,
            [EnumDisplayString("Danh sách giấy khám sức khỏe")]
            DANH_SACH_KHAM_SUC_KHOE = 3,
            [EnumDisplayString("Bản thuyết minh CSVC")]
            BAN_THUYET_MINH = 4,
            [EnumDisplayString("Tài liệu khác")]
            TAI_LIEU_KHAC = 5
        }
        public enum SAN_XUAT_KINH_DOANH_TT05
        {
            [EnumDisplayString("Sản xuất")]
            SAN_XUAT = 1,
            [EnumDisplayString("Kinh doanh")]
            KINH_DOANH = 2,
            [EnumDisplayString("Sản xuất và kinh doanh")]
            SAN_XUAT_VA_KINH_DOANH = 3,
        }
        #endregion

        #region TT04
        public enum LOAI_TAI_LIEU_TT04
        {
            [EnumDisplayString("Chứng nhận đăng ký kinh doanh")]
            CHUNG_NHAN_DANG_KY_KINH_DOANH = 1,
            [EnumDisplayString("Danh sách người tập huấn ATTP")]
            DANH_SACH_NGUOI_TAP_HUAN = 2,
            [EnumDisplayString("Danh sách giấy khám sức khỏe")]
            DANH_SACH_KHAM_SUC_KHOE = 3,
            [EnumDisplayString("Bản thuyết minh CSVC")]
            BAN_THUYET_MINH = 4,
            [EnumDisplayString("Tài liệu khác")]
            TAI_LIEU_KHAC = 5
        }
        public enum SAN_XUAT_KINH_DOANH_TT04
        {
            [EnumDisplayString("Sản xuất")]
            SAN_XUAT = 1,
            [EnumDisplayString("Kinh doanh")]
            KINH_DOANH = 2,
            [EnumDisplayString("Sản xuất và kinh doanh")]
            SAN_XUAT_VA_KINH_DOANH = 3,
        }
        #endregion

        #region TT03
        public enum LOAI_TEP_DINH_KEM_TT03
        {
            [EnumDisplayString("Giấy công bố sản phẩm")]
            GIAY_CONG_BO = 1,
            [EnumDisplayString("Giấy chứng nhận đăng ký kinh doanh")]
            GIAY_CHUNG_NHAN_KINH_DOANH = 2,
            [EnumDisplayString("File video")]
            FILE_MP4 = 3,
            [EnumDisplayString("File audio")]
            FILE_MP3 = 4,
            [EnumDisplayString("Tờ rơi, apphich, poster, sản phẩm in, ...")]
            TO_ROI = 5,
            [EnumDisplayString("Tài liệu khác")]
            TAI_LIEU_KHAC = 6
        }
        public enum DANG_TAI_LIEU_TT03
        {
            [EnumDisplayString("Giấy")]
            GIAY = 1,
            [EnumDisplayString("Biểu")]
            BIEU = 2,
            [EnumDisplayString(".mp4")]
            VIDEO = 3,
            [EnumDisplayString(".mp3")]
            AUDIO = 4
        }
        public enum PHO_PHONG_THAM_XET
        {
            [EnumDisplayString("Phó phòng")]
            LANH_DAO = 0,
            [EnumDisplayString("Chuyên viên thụ lý")]
            THU_LY = 1,
            [EnumDisplayString("Chuyên viên phối hợp")]
            PHOI_HOP = 2,
        }
        #endregion

        public enum LUONG_XU_LY_TT37
        {
            [EnumDisplayString("Luồng thẩm xét")]
            LUONG_RA_SOAT = 1,
            [EnumDisplayString("Luồng thẩm đinh")]
            LUONG_THAM_DINH = 2
        }

        #region TT59
        public enum LUONG_XU_LY_TT59
        {
            [EnumDisplayString("Luồng thẩm xét")]
            LUONG_THAM_XET = 1,
            [EnumDisplayString("Luồng thẩm đinh")]
            LUONG_THAM_DINH= 2
        }
        #endregion


        public enum THU_TUC_TEXT  // Sử dụng cho thanh toán and hiển thị tên thủ tục khi gửi mail
        {
            [EnumDisplayString("Đăng ký cấp chứng chỉ hành nghề")]
            DANG_KY_CAP_CCHN = 37,
        }
    }
}
