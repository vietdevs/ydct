using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        #region FUNCTIONS
        public enum FORM_FUNCTION
        {
            [EnumDisplayString("Chức năng - Xem hồ sơ")]
            XEM_HO_SO = 9
            ,
            [EnumDisplayString("Chức năng - Xem đơn hàng")]
            XEM_BAN_DANG_KY = 91
            ,
            [EnumDisplayString("Chức năng - Xem công văn")]
            XEM_CONG_VAN = 92
            ,
            [EnumDisplayString("Chức năng - Xem giấy tiếp nhận")]
            XEM_GIAY_TIEP_NHAN = 93
            ,
            [EnumDisplayString("Chức năng - Xem thanh toán hồ sơ")]
            XEM_THANH_TOAN_HO_SO = 94
            ,
            [EnumDisplayString("Chức năng - Xem biên bản thẩm định")]
            XEM_BIEN_BAN_THAM_DINH = 95
            ,
            [EnumDisplayString("Chức năng - Xem lý do trả lại")]
            XEM_LY_DO_TRA_LAI = 96
            ,
            [EnumDisplayString("Chức năng - Nộp kết quả kiểm nghiệm khảo nghiệm")]
            NOP_KET_QUA_KIEM_NGHIEM_KHAO_NGHIEM = 81
            ,
            [EnumDisplayString("Chức năng - Nộp hồ sơ sau khi nộp kết quả kiểm nghiệm khảo nghiệm")]
            NOP_HO_SO_SAU_KHI_KIEM_NGHIEM_KHAO_NGHIEM_TOI_VAN_THU = 82
            ,
            //FORM DANG_KY_CONG_BO
            [EnumDisplayString("Chức năng - Sửa hồ sơ")]
            SUA_HO_SO = 11,
            [EnumDisplayString("Chức năng - Lãnh đạo phân công hồ sơ")]
            LANH_DAO_PHAN_CONG_HO_SO = 111
            ,
            [EnumDisplayString("Chức năng - Hủy hồ sơ")]
            HUY_HO_SO = 12
            ,
            [EnumDisplayString("Chức năng - Doanh nghiệp ký số hồ sơ")]
            DOANH_NGHIEP_KY_SO_HO_SO = 13
            ,
            [EnumDisplayString("Chức năng - Doanh nghiệp nộp hồ sơ")]
            DOANH_NGHIEP_NOP_HO_SO = 14
            ,
            [EnumDisplayString("Chức năng - Doanh nghiệp thanh toán")]
            DOANH_NGHIEP_THANH_TOAN = 15
            ,
            [EnumDisplayString("Chức năng - Doanh nghiệp thanh toán lại")]
            DOANH_NGHIEP_THANH_TOAN_LAI = 151
            ,

            [EnumDisplayString("Chức năng - Nộp hồ sơ bổ sung")]
            NOP_HO_SO_BO_SUNG = 16
            ,
            [EnumDisplayString("Chức năng - Tạo bản sao hồ sơ")]
            TAO_BAN_SAO_HO_SO = 17
            ,
            [EnumDisplayString("Chức năng - Doanh nghiệp nộp hồ sơ bị trả lại")]
            NOP_HO_SO_BI_TRA_LAI = 18
            ,
            [EnumDisplayString("Chức năng - Doanh nghiệp upload hồ sơ đã ký")]
            DOANH_NGHIEP_UPLOAD_HOSO_DA_KY = 19
            ,
            [EnumDisplayString("Chức năng - Nộp báo cáo khắc phục")]
            NOP_BAO_CAO_KHAC_PHUC = 111,

            [EnumDisplayString("Chức năng - Nộp hồ sơ để rà soát")]
            NOP_HO_SO_DE_RA_SOAT = 112
            ,
            [EnumDisplayString("Chức năng - Nộp hồ sơ thanh toán thất bại")]
            NOP_HO_SO_THANH_TOAN_THAT_BAI = 123
            ,

            //FORM KE_TOAN_XAC_NHAN_THANH_TOAN
            [EnumDisplayString("Chức năng - Kế toán xác nhận thanh toán")]
            KE_TOAN_XAC_NHAN_THANH_TOAN = 2
            ,
            [EnumDisplayString("Chức năng - Kế toán xác nhận thanh toán")]
            KE_TOAN_PHAN_CONG = 53
            ,

            //FORM MOT_CUA_PHAN_CONG
            [EnumDisplayString("Chức năng - Một cửa rà soát hồ sơ")]
            MOT_CUA_RA_SOAT_HO_SO = 21,

            [EnumDisplayString("Chức năng - Một cửa gửi lãnh đạo")]
            MOT_CUA_GUI_LANH_DAO = 19987
            ,
            [EnumDisplayString("Chức năng - Một cửa phân công hồ sơ")]
            MOT_CUA_PHAN_CONG_HO_SO = 20
            ,
            [EnumDisplayString("Chức năng - Một cửa phân công lại hồ sơ")]
            MOT_CUA_PHAN_CONG_LAI_HO_SO = 22
             ,
            [EnumDisplayString("Chức năng - Một cửa phân công hồ sơ")]
            MOT_CUA_TIEP_NHAN_HO_SO = 23
            ,
            //FORM PHONG_BAN_PHAN_CONG
            [EnumDisplayString("Chức năng - Phòng ban phân công")]
            PHONG_BAN_PHAN_CONG = 25
            ,
            [EnumDisplayString("Chức năng - Phòng ban trả lại hồ sơ")]
            PHONG_BAN_TRA_LAI = 370
            ,
            [EnumDisplayString("Chức năng - Phân công lại hồ sơ chưa xử lý")]
            PHAN_CONG_LAI_HO_SO_CHUA_XU_LY = 26
            ,
            [EnumDisplayString("Chức năng - Phân công lại hồ sơ đã xử lý")]
            PHAN_CONG_LAI_HO_SO_DA_XU_LY = 27
            ,
            [EnumDisplayString("Chức năng - Một cửa trả phiếu tiếp nhận")]
            MOT_CUA_TRA_PHIEU_TIEP_NHAN = 28
            ,

            //FORM THAM_DINH_HO_SO
            [EnumDisplayString("Chức năng - Thẩm xét hồ sơ mới")]
            THAM_XET_HO_SO_MOI = 3
            ,
            [EnumDisplayString("Chức năng - Lập đoàn thẩm định hồ sơ")]
            LAP_DOAN_THAM_DINH_HO_SO = 789654
            ,
            [EnumDisplayString("Chức năng - Thẩm xét hồ sơ bổ sung")]
            THAM_XET_HO_SO_BO_SUNG = 31
            ,
            [EnumDisplayString("Chức năng - Thẩm xét lại")]
            THAM_XET_LAI = 32
            ,
            [EnumDisplayString("Chức năng - Chuyên viên duyệt thẩm xét")]
            CHUYEN_VIEN_DUYET_THAM_XET = 33
            ,
            [EnumDisplayString("Chức năng - Chuyên viên tổng hợp thẩm định")]
            CHUYEN_VIEN_TONG_HOP_THAM_DINH = 34
            ,
            [EnumDisplayString("Chức năng - Chuyên viên tổng hợp thẩm định lại")]
            CHUYEN_VIEN_TONG_HOP_THAM_DINH_LAI = 35
            ,
            [EnumDisplayString("Chức năng - Chuyên viên tổng hợp thẩm định bổ sung")]
            CHUYEN_VIEN_TONG_HOP_THAM_DINH_BO_SUNG = 36
            ,
            [EnumDisplayString("Chức năng - Chuyên viên yêu cầu phân công lại hồ sơ")]
            CHUYEN_VIEN_YEU_CAU_PHAN_CONG_LAI = 37
            ,
            [EnumDisplayString("Chức năng -Chuyên viên gửi hồ sơ duyệt đăng ký")]
            CHUYEN_VIEN_GUI_DUYET_DANG_KY = 49
            ,
            [EnumDisplayString("Chức năng - Chuyên gia duyệt thẩm định")]
            CHUYEN_GIA_THAM_DINH = 50
            ,
            [EnumDisplayString("Chức năng -Hội đồng thẩm định")]
            HOI_DONG_THAM_DINH = 51
            ,

            [EnumDisplayString("Chức năng -Upload kết quả trình ký")]
            CHUYEN_VIEN_UPLOAD_KET_QUA = 3009
            ,

            //FORM PHO_PHONG_DUYET
            [EnumDisplayString("Chức năng - Phó phòng duyệt thẩm xét")]
            PHO_PHONG_DUYET = 8
            ,

            //FORM TRUONG_PHONG_DUYET
            [EnumDisplayString("Chức năng - Trưởng phòng duyệt thẩm xét")]
            TRUONG_PHONG_DUYET = 4
            ,
            [EnumDisplayString("Chức năng - Trưởng phòng ký số")]
            TRUONG_PHONG_KY_SO = 41
            ,
            [EnumDisplayString("Chức năng - Trưởng phòng duyệt thẩm xét lại")]
            TRUONG_PHONG_DUYET_LAI = 43
            ,
            [EnumDisplayString("Chức năng - Trưởng phòng duyệt thẩm định cơ sở")]
            TRUONG_PHONG_DUYET_THAM_DINH = 42
            ,
            [EnumDisplayString("Chức năng - Trưởng phòng duyệt thẩm định lại")]
            TRUONG_PHONG_DUYET_THAM_DINH_LAI = 44
            ,
            //FORM LANH_DAO_CUC_DUYET
            [EnumDisplayString("Chức năng - Lãnh đạo cục duyệt")]
            LANH_DAO_CUC_DUYET = 5
            ,
            [EnumDisplayString("Chức năng - Lãnh đạo cục ký số")]
            LANH_DAO_CUC_KY_SO = 51
            ,
            [EnumDisplayString("Chức năng - Lãnh đạo cục duyệt thẩm định cơ sở")]
            LANH_DAO_CUC_DUYET_THAM_DINH = 52
            ,
            [EnumDisplayString("Chức năng - Lãnh đạo cục duyệt thẩm định của hội đồng")]
            LANH_DAO_CUC_DUYET_THAM_DINH_HOI_DONG = 54
            ,
            //FORM LANH_DAO_BO_DUYET
            [EnumDisplayString("Chức năng - Lãnh đạo bộ duyệt")]
            LANH_DAO_BO_DUYET = 6
            ,
            [EnumDisplayString("Chức năng - Lãnh đạo bộ ký số")]
            LANH_DAO_BO_KY_SO = 61
            ,

            //FORM VAN_THU_DUYET            
            [EnumDisplayString("Chức năng - Văn thư đóng dấu")]
            VAN_THU_DUYET = 7
            ,
            [EnumDisplayString("Chức năng - Văn thư báo cáo công văn có sai sót")]
            VAN_THU_BAO_CAO_CONG_VAN_CO_SAI_SOT = 71
            ,
            //FORM VAN_THU_DUYET            
            [EnumDisplayString("Chức năng - Văn thư đóng dấu")]
            VAN_THU_DONG_DAU = 72,
            [EnumDisplayString("Chức năng - Văn thư duyệt thẩm định")]
            VAN_THU_DUYET_THAM_DINH = 73,
            [EnumDisplayString("Chức năng - Doanh nghiệp nộp hồ sơ kiểm nghiệm khảo nghiệm")]
            DOANH_NGHIEP_NOP_HO_SO_NOP_KET_QUA_KIEM_NGHIEM_KHAO_NGHIEM = 74
            ,
        }
        #endregion
    }
}
