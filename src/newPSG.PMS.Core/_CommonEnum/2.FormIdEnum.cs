using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public enum FORM_ID
        {
            //Module công bố sản phẩm bắt đầu từ 0...
            [EnumDisplayString("Form dang_ky_ho_so")]
            FORM_DANG_KY_HO_SO = 1
            ,
            [EnumDisplayString("Form ke_toan_xac_nhan_thanh_toan")]
            FORM_KE_TOAN_XAC_NHAN_THANH_TOAN = 11
            ,
            [EnumDisplayString("Form mot_cua_ra_soat")]
            FORM_MOT_CUA_RA_SOAT = 12
            ,
            [EnumDisplayString("Form mot_cua_phan_cong")]
            FORM_MOT_CUA_PHAN_CONG = 2
            ,
            [EnumDisplayString("Form phong_ban_phan_cong")]
            FORM_PHONG_BAN_PHAN_CONG = 21
            ,           
            [EnumDisplayString("Form tham_xet_ho_so")]
            FORM_THAM_XET_HO_SO = 3
            ,
            [EnumDisplayString("Form pho_phong_duyet")]
            FORM_PHO_PHONG_DUYET = 31
            ,
            [EnumDisplayString("Form chuyen_gia_tham_dinh")]
            FORM_CHUYEN_GIA_THAM_DINH = 32
            ,
            [EnumDisplayString("Form hoi_dong_tham_dinh")]
            FORM_HOI_DONG_THAM_DINH = 33
            ,
            [EnumDisplayString("Form tong_hop_tham_xet_ho_so")]
            FORM_TONG_HOP_THAM_XET_HO_SO = 34
            ,           
            [EnumDisplayString("Form truong_phong_duyet")]
            FORM_TRUONG_PHONG_DUYET = 4
            ,
            [EnumDisplayString("Form lanh_dao_cuc_duyet")]
            FORM_LANH_DAO_CUC_DUYET = 5
            ,
            [EnumDisplayString("Form lanh_dao_cuc_duyet_cong_van")]
            FORM_LANH_DAO_CUC_DUYET_CONG_VAN = 51
            ,
            [EnumDisplayString("Form lanh_dao_bo_duyet")]
            FORM_LANH_DAO_BO_DUYET = 6
            ,
            [EnumDisplayString("Form van_thu_duyet")]
            FORM_VAN_THU_DUYET = 7
            ,
            [EnumDisplayString("Form van_thu_duyet_cong_van")]
            FORM_VAN_THU_DUYET_CONG_VAN = 71
            ,
            [EnumDisplayString("Form tra_cuu_ho_so")]
            FORM_TRA_CUU_HO_SO = 9,
            [EnumDisplayString("Form thong_ke_chung")]
            FORM_THONG_KE_CHUNG = 91,
            [EnumDisplayString("Form thong_ke_admin")]
            FORM_THONG_KE_ADMIN = 92,
            [EnumDisplayString("Form thong_ke_mot_cua")]
            FORM_THONG_KE_MOT_CUA = 93,
            [EnumDisplayString("Form thong_ke_van_thu")]
            FORM_THONG_KE_VAN_THU = 94,


            FORM_THAM_DINH_HO_SO_TT37 = 371,
            FORM_TONG_HOP_THAM_DINH_TT37 = 372,
            FORM_TRUONG_PHONG_DUYET_THAM_DINH_TT37 = 373,
            FORM_LANH_DAO_CUC_DUYET_THAM_DINH_TT37 = 374,
            FORM_VAN_THU_DUYET_THAM_DINH_TT37 = 375,

            //Thủ tục 59 

            FORM_CHUYEN_VIEN_TONG_HOP_THAM_DINH = 301,
            FORM_TRUONG_PHONG_KET_LUAN_THAM_DINH = 302,
            FORM_LANH_DAO_CUC_DUYET_THAM_DINH = 303,
            FORM_VAN_THU_DUYET_THAM_DINH = 304,
            FORM_THU_HOI_GIAY_CHUNG_NHAN = 305,



        }

        #region 1. ENUM DANG_KY_HO_SO

        public enum LOAI_TAI_LIEU_DINH_KEM
        {
            [EnumDisplayString("Phiếu kiểm nghiệm ATTP")]
            PHIEU_KIEM_NGHIEM = 1,
            [EnumDisplayString("Bản tiêu chuẩn / quy chuẩn kỹ thuật hoặc SPEC")]
            BAN_TIEU_CHUAN = 2,
            [EnumDisplayString("Chứng nhận cơ sở đủ điều kiện/GMP hoặc CFS/HC")]
            CHUNG_NHAN_CO_SO_DU_DIEU_KIEN = 3,
            [EnumDisplayString("Tài liệu chứng minh")]
            TAI_LIEU_CHUNG_MINH = 4,
            [EnumDisplayString("Giấy chứng nhận lưu hành tự do")]
            GIAY_CHUNG_NHAN_LUU_HANH_TU_DO = 5, 
            [EnumDisplayString("Tài liệu khác")]
            TAI_LIEU_KHAC = 6,
        }

        public enum TRANG_THAI_QUA_HAN
        {
            [EnumDisplayString("Chưa quá hạn")]
            CHUA_QUA_HAN = 1
            ,
            [EnumDisplayString("Sắp tới hạn (ít hơn 5 ngày)")]
            SAP_TOI_HAN = 2
            ,
            [EnumDisplayString("Đã quá hạn")]
            DA_QUA_HAN = 3
            ,
        }

        public enum IS_NHAP_KHAU
        {
            [EnumDisplayString("Hàng nhập khẩu")]
            HANG_NHAP_KHAU = 1
           ,
            [EnumDisplayString("Hàng nội địa")]
            HANG_NOI_DIA = 2
           ,
        }

        public enum SAP_XEP_HO_SO
        {
            [EnumDisplayString("Ngày nộp giảm dần")]
            NGAY_NOP_GIAM_DAN = 1
           ,
            [EnumDisplayString("Ngày nộp tăng dần")]
            NGAY_NOP_TANG_DAN = 2
           ,
        }

        public enum TRANG_THAI_TRA_CUU
        {
            [EnumDisplayString("Chờ kế toán xác nhận")]
            CHO_KE_TOAN_XAC_NHAN = 0
           ,
            [EnumDisplayString("Mới nộp")]
            MOI_NOP = 1
           ,
            [EnumDisplayString("Đã phân công chờ thẩm định")]
            DA_PHAN_CONG_CHO_THAM_DINH = 2
           ,
            [EnumDisplayString("Đã có kết luận, chờ lãnh đạo phòng xem xét")]
            DA_CO_KET_LUAN_CHO_LANH_DAO_PHONG_XEM_XET = 3
           ,
            [EnumDisplayString("Chuyển chuyên viên soạn lại công văn")]
            LANH_DAO_PHONG_CHUYEN_CHUYEN_VIEN_SOAN_LAI_CONG_VAN = 4
           ,
            [EnumDisplayString("Chờ xin ý kiến lãnh đạo Cục")]
            CHO_XIN_Y_KIEN_LANH_DAO_CUC = 5
           ,
            [EnumDisplayString("Chờ lãnh đạo Cục ký giấy tiếp nhận")]
            CHO_LANH_DAO_CUC_KY_GIAY_TIEP_NHAN = 6
           ,
            [EnumDisplayString("Chờ văn thư đóng dấu công văn bổ sung")]
            CHO_VAN_THU_DONG_DAU_CONG_VAN = 7
           ,
            [EnumDisplayString("Chờ văn thư đóng dấu giấy tiếp nhận")]
            CHO_VAN_THU_DONG_DAU_GIAY_TIEP_NHAN = 8
           ,
            [EnumDisplayString("Đã trả giấy tiếp nhận")]
            DA_TRA_GIAY_TIEP_NHAN = 9
           ,
            [EnumDisplayString("Đã trả công văn và doanh nghiệp chưa bổ sung")]
            DA_TRA_CONG_VAN_VA_DOANH_NGHIEP_CHUA_BO_SUNG = 10
           ,
            [EnumDisplayString("Doanh nghiệp đã bổ sung, chuyên viên chưa xử lý")]
            DOANH_NGHIEP_DA_BO_SUNG_CHUYEN_VIEN_CHUA_XU_LY = 11
           ,
        }
        #endregion

        public enum TT37_LOAI_TAI_LIEU_DINH_KEM
        {
            [EnumDisplayString("Bản sao văn bằng chuyên môn")]
            VAN_BANG_CHUYEN_MON = 1,
            [EnumDisplayString("Giấy xác nhận quá trình thực hành")]
            QUA_TRINH_THUC_HANH = 2,
            [EnumDisplayString("Giấy chứng nhận đủ điều kiện sức khỏe")]
            DU_DIEU_KIEN_SUC_KHOE = 3,
            [EnumDisplayString("Phiếu lý lịch tư pháp")]
            LY_LICH_TU_PHAP = 4,
            [EnumDisplayString("Ảnh 4x6")]
            ANH_4X6 = 5,
        }
    }
}
