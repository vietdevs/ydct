using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public enum VAI_TRO_THAM_XET
        {
            [EnumDisplayString("Chuyên viên thụ lý")]
            CHUYEN_VIEN_THU_LY = 1,
            [EnumDisplayString("Chuyên viên phối hợp")]
            CHUYEN_VIEN_PHOI_HOP = 2,
            [EnumDisplayString("Chuyên gia thẩm định")]
            CHUYEN_GIA_THAM_XET = 3,
            [EnumDisplayString("Tổ trưởng chuyên gia")]
            TO_TRUONG_CHUYEN_GIA = 4,
            [EnumDisplayString("Trưởng phòng")]
            TRUONG_PHONG = 5,
            [EnumDisplayString("Lãnh đạo phòng")]
            LANH_DAO_PHONG = 6
        }

        public enum DON_VI_XU_LY
        {
            [EnumDisplayString("Doanh nghiệp")]
            DOANH_NGHIEP = 1
            ,
            [EnumDisplayString("Một cửa tiếp nhận")]
            MOT_CUA_TIEP_NHAN = 3
            ,
            [EnumDisplayString("Kế toán")]
            KE_TOAN = 2
            ,
            [EnumDisplayString("Một cửa phân công")]
            MOT_CUA_PHAN_CONG = 31
            ,
            [EnumDisplayString("Phòng ban phân công")]
            PHONG_BAN_PHAN_CONG = 4
            ,
            [EnumDisplayString("Trưởng phòng")]
            TRUONG_PHONG = 5
            ,
            [EnumDisplayString("Lãnh đạo cục")]
            LANH_DAO_CUC = 6
            ,
            [EnumDisplayString("Lãnh đạo bộ")]
            LANH_DAO_BO = 7
            ,
            [EnumDisplayString("Văn thư")]
            VAN_THU = 8
            ,
            [EnumDisplayString("Thanh tra")]
            THANH_TRA = 9
            ,
            [EnumDisplayString("Chuyên viên thẩm định")]
            CHUYEN_VIEN_THAM_XET = 10
            ,
            [EnumDisplayString("Chuyên viên phối hợp thẩm định")]
            CHUYEN_VIEN_PHOI_HOP_THAM_XET = 11
            ,
            [EnumDisplayString("Chuyên viên thẩm định & tổng hợp")]
            CHUYEN_VIEN_THAM_XET_TONG_HOP = 12
            ,
            [EnumDisplayString("Phó phòng")]
            PHO_PHONG = 13
            ,
            [EnumDisplayString("Chuyên gia")]
            CHUYEN_GIA = 14
            ,
            [EnumDisplayString("Tổ trưởng chuyên gia")]
            TO_TRUONG_CHUYEN_GIA = 15
            ,
            [EnumDisplayString("Hội đồng thẩm định")]
            HOI_DONG_THAM_DINH = 16
            ,
            [EnumDisplayString("Trưởng đoàn thanh tra")]
            TRUONG_DOAN_THANH_TRA = 17
        }

        public enum PHONG_BAN_ID
        {
            [EnumDisplayString("Phòng chuyên môn")]
            PHONG_CHUYEN_MON = 1,
        }
        public enum VAI_TRO_THAM_DINH
        {
            [EnumDisplayString("Trường đoàn")]
            TRUONG_DOAN = 0,
            [EnumDisplayString("Thư Ký")]
            THU_KY = 1,
            [EnumDisplayString("Thành Viên")]
            THANH_VIEN = 2
        }
    }
}
