using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        #region 1. FORM_CASE QUẢN LÝ DOANH NGHIỆP
        public enum FORM_CASE_DOANH_NGHIEP
        {
            [EnumDisplayString("Tất cả")]
            ALL = 0,
            [EnumDisplayString("Chưa duyệt")]
            DOANH_NGHIEP_CHUA_DUOC_DUYET = 1,
            [EnumDisplayString("Đã được duyệt")]
            DUOC_DUYET_CHAP_NHAN = 2,
            [EnumDisplayString("Không được duyệt")]
            DUOC_DUYET_KHONG_CHAP_NHAN = 3,
        }
        #endregion

        #region 2. FORM_CASE XỬ LÝ HỒ SƠ

        //DANG_KY_HO_SO
        public enum FORM_CASE_DANG_KY_HO_SO
        {
            [ThucTucEnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [ThucTucEnumDisplayString("Hồ sơ mới",true)]
            HO_SO_MOI = 1
           ,
            [ThucTucEnumDisplayString("Hồ sơ chờ tiếp nhận")]
            HO_SO_CHO_TIEP_NHAN = 2
           ,
            [ThucTucEnumDisplayString("Hồ sơ chờ thanh toán", true)]
            HO_SO_CHO_THANH_TOAN = 3
            ,
            [ThucTucEnumDisplayString("Hồ sơ đã gửi thanh toán")]
            HO_SO_DA_GUI_THANH_TOAN = 4
           ,
            [ThucTucEnumDisplayString("Hồ sơ bị trả lại", true)]
            HO_SO_BI_TRA_LAI = 5
           ,
            [ThucTucEnumDisplayString("Hồ sơ đang thẩm định")]
            HO_SO_DANG_XU_LY = 6
           ,
            [ThucTucEnumDisplayString("Hồ sơ cần bổ sung", true)]
            HO_SO_CAN_BO_SUNG = 7
           ,
            [ThucTucEnumDisplayString("Hồ sơ đã hoàn thành")]
            HO_SO_HOAN_TAT = 8
           ,
            [ThucTucEnumDisplayString("Hồ sơ chờ nộp giấy kiểm nghiệm")]
            HO_SO_CHO_NOP_GIAY_KIEM_NGHIEM = 9
        }
        public enum FORM_CASE_KHAO_NGHIEM
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ mới")]
            HO_SO_MOI = 1
           ,
            [EnumDisplayString("Hồ sơ chờ tiếp nhận")]
            HO_SO_CHO_TIEP_NHAN = 2
           ,
            [EnumDisplayString("Hồ sơ chờ thanh toán")]
            HO_SO_CHO_THANH_TOAN = 3
            ,
            [EnumDisplayString("Hồ sơ đã gửi thanh toán")]
            HO_SO_DA_GUI_THANH_TOAN = 4
           ,
            [EnumDisplayString("Hồ sơ bị trả lại")]
            HO_SO_BI_TRA_LAI = 5
           ,
            [EnumDisplayString("Hồ sơ đang thẩm định")]
            HO_SO_DANG_XU_LY = 6
           ,
            [EnumDisplayString("Hồ sơ cần bổ sung")]
            HO_SO_CAN_BO_SUNG = 7
           ,
            [EnumDisplayString("Hồ sơ đã hoàn thành")]
            HO_SO_HOAN_TAT = 8
        }
        public enum FORM_CASE_CAP_SO_DANG_KY
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
       ,
            [EnumDisplayString("Hồ sơ chờ nộp giấy kiểm nghiệm")]
            HO_SO_CHO_NOP_GIAY_KIEM_NGHIEM = 1
       ,
            [EnumDisplayString("Hồ sơ chờ tiếp nhận")]
            HO_SO_CHO_TIEP_NHAN = 2
       ,
            [EnumDisplayString("Hồ sơ chờ thanh toán")]
            HO_SO_CHO_THANH_TOAN = 3
        ,
            [EnumDisplayString("Hồ sơ đã gửi thanh toán")]
            HO_SO_DA_GUI_THANH_TOAN = 4
       ,
            [EnumDisplayString("Hồ sơ bị trả lại")]
            HO_SO_BI_TRA_LAI = 5
       ,
            [EnumDisplayString("Hồ sơ đang thẩm định")]
            HO_SO_DANG_XU_LY = 6
       ,
            [EnumDisplayString("Hồ sơ cần bổ sung")]
            HO_SO_CAN_BO_SUNG = 7
       ,

            [EnumDisplayString("Hồ sơ đã hoàn thành")]
            HO_SO_HOAN_TAT = 8
        }

        //KE_TOAN_XAC_NHAN_THANH_TOAN
        public enum FORM_CASE_XAC_NHAN_THANH_TOAN
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ chờ xác nhận thanh toán")]
            HO_SO_CHO_XAC_NHAN_THANH_TOAN = 1
           ,
            [EnumDisplayString("Hồ sơ thanh toán thành công")]
            HO_SO_DA_XAC_NHAN_THANH_TOAN_THANH_CONG = 2
           ,
            [EnumDisplayString("Hồ sơ thanh toán thất bại")]
            HO_SO_DA_XAC_NHAN_THANH_TOAN_THAT_BAI = 3
        }

        //MOT_CUA
        public enum FORM_CASE_MOT_CUA_RA_SOAT
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ cần rà soát")]
            HO_SO_CAN_RA_SOAT = 1,
            [EnumDisplayString("Hồ sơ chờ thanh toán")]
            HO_SO_CHO_THANH_TOAN = 2,
            [EnumDisplayString("Hồ sơ bị trả lại")]
            HO_SO_BI_TRA_LAI = 3,
            [EnumDisplayString("Hồ sơ chờ trả giấy tiếp nhận")]
            HO_SO_CHO_TRA_GIAY_TIEP_NHAN = 4,
            [EnumDisplayString("Hồ sơ đã trả giấy tiếp nhận")]
            HO_SO_DA_TRA_GIAY_TIEP_NHAN = 5
        }
        //MOT_CUA 03
        public enum FORM_CASE_MOT_CUA_RA_SOAT_TT03
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ cần rà soát")]
            HO_SO_CAN_RA_SOAT = 1,
            [EnumDisplayString("Hồ sơ chờ thanh toán")]
            HO_SO_CHO_THANH_TOAN = 2,
            [EnumDisplayString("Hồ sơ bị trả lại")]
            HO_SO_BI_TRA_LAI = 3
        }
        public enum FORM_CASE_MOT_CUA_PHAN_CONG
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ nộp mới")]
            HO_SO_NOP_MOI = 1,
            //[EnumDisplayString("Hồ sơ nộp bổ sung")]
            //HO_SO_NOP_BO_SUNG = 2,
            [EnumDisplayString("Hồ sơ đã tiếp nhận")]
            HO_SO_DA_TIEP_NHAN = 2
        }

        //PHONG_BAN_PHAN_CONG
        public enum FORM_CASE_PHONG_BAN_PHAN_CONG
        {

            [EnumDisplayString("Hồ sơ chưa phân công")]
            CHUA_PHAN_CONG = 1
           ,
            [EnumDisplayString("Hồ sơ phân công, chưa xử lý")]
            DA_PHAN_CONG = 2
           ,
            [EnumDisplayString("Hồ sơ phân công, đang xử lý")]
            DA_XU_LY = 3
        }

        //THAM_XET_HO_SO
        public enum FORM_CASE_THAM_XET_HO_SO
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ mới")]
            HO_SO_THAM_XET_MOI = 1
           ,
            [EnumDisplayString("Hồ sơ đã thẩm xét & chờ xử lý")]
            HO_SO_DA_THAM_XET = 2
           ,
            [EnumDisplayString("Hồ sơ bổ sung")]
            HO_SO_THAM_XET_BO_SUNG = 3
           ,
            [EnumDisplayString("Hồ sơ thẩm xét lại")]
            HO_SO_THAM_XET_LAI = 4
           ,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 5
        }

        //THAM_DINH_HO_SO
        public enum FORM_CASE_THAM_DINH_HO_SO
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ hậu kiểm mới")]
            HO_SO_THAM_DINH_MOI = 1
           ,
            [EnumDisplayString("Hồ sơ đã hậu kiểm & chờ xử lý")]
            HO_SO_DA_THAM_DINH = 2
           ,
            [EnumDisplayString("Hồ sơ hậu kiểm lại")]
            HO_SO_THAM_DINH_LAI = 3
           ,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 4
        }

        public enum FORM_CASE_THAM_XET_HO_SO_CAP_SO_DANG_KY
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
         ,
            [EnumDisplayString("Hồ sơ mới")]
            HO_SO_THAM_XET_MOI = 1
         ,
            [EnumDisplayString("Hồ sơ đã thẩm định & chờ xử lý")]
            HO_SO_DA_THAM_XET = 2
         ,
            [EnumDisplayString("Hồ sơ bổ sung")]
            HO_SO_THAM_XET_BO_SUNG = 3
         ,
            [EnumDisplayString("Hồ sơ thẩm định lại")]
            HO_SO_THAM_XET_LAI = 4
         ,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 5
         ,
            [EnumDisplayString("Hồ sơ duyệt cấp số đăng ký")]
            HO_SO_DUYET_CAP_SO_DANG_KY = 6
        }
        public enum FORM_CASE2_THAM_XET_HO_SO
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ thụ lý")]
            VI_TRI_CHUYEN_VIEN_THU_LY = 1
           ,
            [EnumDisplayString("Hồ sơ phối hợp")]
            VI_TRI_CHUYEN_VIEN_PHOI_HOP = 2
        }

        public enum FORM_CASE_DEFAULT
        {
            [EnumDisplayString("Thẩm định cho phép khảo nghiệm")]
            THAM_DINH_KHAO_NGHIEM = 1
           ,
            [EnumDisplayString("Thẩm định cấp số đăng ký")]
            THAM_DINH_CAP_SO_DANG_KY = 2
        }
        public enum FORM_CASE_CHUYEN_VIEN_TONG_HOP
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0,
            [EnumDisplayString("Hồ sơ chờ tổng hợp")]
            HO_SO_CHO_TONG_HOP = 1,
            [EnumDisplayString("Hồ sơ tổng hợp lại")]
            HO_SO_TONG_HOP_LAI = 2,
            [EnumDisplayString("Hồ sơ tổng hợp bổ sung")]
            HO_SO_TONG_HOP_BO_SUNG = 3,
            [EnumDisplayString("Hồ sơ đã tổng hợp")]
            HO_SO_DANG_THEO_DOI = 4,
        }

        //PHO_PHONG_DUYET
        public enum FORM_CASE_PHO_PHONG_DUYET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
         ,
            [EnumDisplayString("Hồ sơ chưa duyệt")]
            HO_SO_CHUA_DUYET = 1
            ,
            //,
            //   [EnumDisplayString("Hồ sơ duyệt lại")]
            //   HO_SO_TRA_LAI = 2
            // ,
            //   [EnumDisplayString("Hồ sơ bổ sung")]
            //   HO_SO_BO_SUNG = 3,
            [EnumDisplayString("Hồ sơ đã duyệt và đang theo dõi")]
            HO_SO_DA_DUYET = 4
        }
        //CHUYEN_GIA_THAM_DINH
        public enum FORM_CASE_CHUYEN_GIA_THAM_DINH
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ chưa duyệt")]
            HO_SO_CHUA_DUYET = 1
           ,
            [EnumDisplayString("Hồ sơ đã duyệt và đang theo dõi")]
            HO_SO_DA_DUYET = 2
        }
        //TO_TRUONG_CHUYEN_GIA
        public enum FORM_CASE_TO_TRUONG_CHUYEN_GIA
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ chưa duyệt")]
            HO_SO_CHUA_DUYET = 1
           ,
            [EnumDisplayString("Hồ sơ đã duyệt và đang theo dõi")]
            HO_SO_DA_DUYET = 2
        }

        public enum FORM_CASE_CHUYEN_GIA_THAM_DINH_TT12
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ chưa duyệt")]
            HO_SO_CHUA_DUYET = 1
           ,
            [EnumDisplayString("Hồ sơ duyệt lại")]
            HO_SO_DUYET_LAI = 2
           ,
            [EnumDisplayString("Hồ sơ đã duyệt và đang theo dõi")]
            HO_SO_DA_DUYET = 3
        }


        //HOI_DONG_THAM_DINH
        public enum FORM_CASE_HOI_DONG_THAM_DINH
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ chưa duyệt")]
            HO_SO_CHUA_DUYET = 1
           ,
            [EnumDisplayString("Hồ sơ đã duyệt và đang theo dõi")]
            HO_SO_DA_DUYET = 2
        }

        //TRUONG_PHONG_DUYET
        public enum FORM_CASE_TRUONG_PHONG_DUYET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ kết luận")]
            HO_SO_CHUA_XU_LY = 1,
            [EnumDisplayString("Hồ sơ kết luận lại")]
            HO_SO_XU_LY_LAI = 2,
            [EnumDisplayString("Hồ sơ trả lại chuyên viên")]
            HO_SO_TRA_LAI = 3,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 4,
        }
        //LANH_DAO_CUC_DUYET
        public enum FORM_CASE_LANH_DAO_CUC_DUYET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ duyệt")]
            HO_SO_CHO_DUYET = 1,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DA_DUYET = 3,
        }

        //LANH_DAO_BO_DUYET
        public enum FORM_CASE_LANH_DAO_BO_DUYET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0
           ,
            [EnumDisplayString("Hồ sơ chưa duyệt")]
            HO_SO_CHUA_DUYET = 1
           ,
            [EnumDisplayString("Hồ sơ đã duyệt và đang theo dõi")]
            HO_SO_DA_DUYET = 2
        }

        //VAN_THU_DUYET
        public enum FORM_CASE_VAN_THU_DUYET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ đóng dấu")]
            HO_SO_CHUA_DUYET = 1,
            [EnumDisplayString("Hồ sơ đã đóng dấu")]
            HO_SO_DA_DUYET = 2
        }

        public enum FORM_CASE_VAN_THU_DUYET_CONG_VAN
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Công văn chờ đóng dấu")]
            CONG_VAN_CHUA_DUYET = 1,
            [EnumDisplayString("Công văn trả lại")]
            CONG_VAN_TRA_LAI = 2,
            [EnumDisplayString("Công văn đã đóng dấu")]
            CONG_VAN_DA_DUYET = 3
        }

        //TRA_CUU_HO_SO
        public enum FORM_CASE_TRA_CUU_HO_SO
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0
            ,
            [EnumDisplayString("Hồ sơ mới")]
            HO_SO_MOI = 1
           ,
            [EnumDisplayString("Hồ sơ đang thẩm định")]
            HO_SO_DANG_XU_LY = 2
           ,
            [EnumDisplayString("Hồ sơ yêu cầu bổ sung")]
            HO_SO_YEU_CAU_BO_SUNG = 3
           ,
            [EnumDisplayString("Hồ sơ hoàn thành")]
            HO_SO_HOAN_THANH = 4
        }

        //THONG_KE_HO_SO_MOT_CUA
        public enum FORM_CASE_THONG_KE_MOT_CUA
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0
            ,
            [EnumDisplayString("Hồ sơ đã tiếp nhận")]
            HO_SO_DA_TIEP_NHAN = 1
           ,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2
        }
        public enum FORM_CASE_THONG_KE_VAN_THU
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0
           ,
            [EnumDisplayString("Hồ sơ đã đóng dấu")]
            HO_SO_DA_DONG_DAU = 1
          ,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2
        }
        public enum FORM_CASE_THONG_KE_ADMIN
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0,
            [EnumDisplayString("Hồ sơ đã tiếp nhận")]
            HO_SO_DA_TIEP_NHAN = 1,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2,
            [EnumDisplayString("Hồ sơ đã giải quyết")]
            HO_SO_DA_GIAI_QUYET = 3,
            [EnumDisplayString("Hồ sơ chờ bổ sung")]
            HO_SO_CHO_BO_SUNG = 4
        }
        #endregion

        #region tt37

        public enum FORM_CASE_THAM_DINH_HO_SO_TT37
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ thẩm định")]
            HO_SO_CHO_THAM_DINH = 1,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 2,
        }

        public enum FORM_CASE_TONG_HOP_THAM_DINH_TT37
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ tổng hợp")]
            HO_SO_CHO_TONG_HOP = 1,
            [EnumDisplayString("Hồ sơ tổng hợp đã lưu")]
            HO_SO_TONG_HOP_DA_LUU = 2,
            [EnumDisplayString("Hồ sơ tổng hợp lại")]
            HO_SO_TONG_HOP_LAI = 3,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 4,
        }

        public enum FORM_CASE_TRUONG_PHONG_DUYET_THAM_DINH_TT37
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ duyệt")]
            HO_SO_CHO_DUYET = 1,
            [EnumDisplayString("Hồ sơ duyệt lại")]
            HO_SO_DUYET_LAI = 2,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 3,
        }

        public enum FORM_CASE_LANH_DAO_DUYET_THAM_DINH_TT37
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ duyệt")]
            HO_SO_CHO_DUYET = 1,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2,
            [EnumDisplayString("Hồ sơ đã duyệt")]
            HO_SO_DA_DUYET = 3,
        }

        public enum FORM_CASE_VAN_THU_DUYET_THAM_DINH_TT37
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ duyệt")]
            HO_SO_CHO_DUYET = 1,
            [EnumDisplayString("Hồ sơ đã hoàn thành")]
            HO_SO_HOAN_TAT = 2,
        }


        #endregion



        #region FormCase TT59
        public enum FORM_CASE_CHUYEN_VIEN_TONG_HOP_THAM_XET
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0,
            [EnumDisplayString("Hồ sơ chờ tổng hợp")]
            HO_SO_CHO_TONG_HOP = 1,
            [EnumDisplayString("Hồ sơ tổng hợp lại")]
            HO_SO_TONG_HOP_LAI = 2,
            [EnumDisplayString("Hồ sơ tổng hợp bổ sung")]
            HO_SO_TONG_HOP_BO_SUNG = 3,
            [EnumDisplayString("Hồ sơ chờ cập nhật quyết định")]
            HO_SO_CHO_CAP_NHAT_QUYET_DINH = 4,
            [EnumDisplayString("Hồ sơ chờ cập nhật kế hoạch")]
            HO_SO_CHO_CAP_NHAT_KE_HOACH = 5,
            [EnumDisplayString("Hồ sơ đã duyệt")]
            HO_SO_DA_TONG_HOP = 6,
        }

        public enum FORM_CASE_TRUONG_PHONG_KET_LUAN_THAM_XET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ kết luận")]
            HO_SO_CHUA_XU_LY = 1,
            [EnumDisplayString("Hồ sơ trả lại chuyên viên")]
            HO_SO_TRA_LAI = 2,
            [EnumDisplayString("Hồ sơ kết luận lại")]
            HO_SO_XU_LY_LAI = 3,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 4,
        }

        public enum FORM_CASE_LANH_DAO_CUC_DUYET_THAM_XET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ duyệt")]
            HO_SO_CHO_DUYET = 1,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 3,
        }
        public enum FORM_CASE_VAN_THU_DUYET_THAM_XET
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ đóng dấu")]
            HO_SO_CHUA_XU_LY = 1,
            [EnumDisplayString("Hồ sơ đã đóng dấu")]
            HO_SO_DANG_THEO_DOI = 2,
        }

        public enum FORM_CASE_CHUYEN_VIEN_TONG_HOP_THAM_DINH
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ tổng hợp")]
            HO_SO_CHUA_TONG_HOP = 1,
            [EnumDisplayString("Hồ sơ tổng hợp lại")]
            HO_SO_TONG_HOP_LAI = 2,
            [EnumDisplayString("Hồ sơ tổng hợp khắc phục")]
            HO_SO_TONG_HOP_KHAC_PHUC = 3,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 4,
        }

        public enum FORM_CASE_TRUONG_PHONG_KET_LUAN_THAM_DINH
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ kết luận")]
            HO_SO_CHUA_KET_LUAN = 1,
            [EnumDisplayString("Hồ sơ kết luận lại")]
            HO_SO_KET_LUAN_LAI = 2,
            [EnumDisplayString("Hồ sơ trả lại chuyên viên")]
            HO_SO_TRA_LAI = 3,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 4,
        }

        public enum FORM_CASE_LANH_DAO_CUC_DUYET_THAM_DINH
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ duyệt")]
            HO_SO_CHUA_XU_LY = 1,
            [EnumDisplayString("Hồ sơ trả lại")]
            HO_SO_TRA_LAI = 2,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 3,
        }
        public enum FORM_CASE_VAN_THU_DUYET_THAM_DINH
        {
            [EnumDisplayString("Tất cả")]
            GET_ALL = 0,
            [EnumDisplayString("Hồ sơ chờ đóng dấu")]
            HO_SO_CHUA_XU_LY = 1,
            [EnumDisplayString("Hồ sơ đang theo dõi")]
            HO_SO_DANG_THEO_DOI = 2,
        }
        public enum FORM_CASE_THU_HOI_GIAY_CHUNG_NHAN
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0,
            [EnumDisplayString("Hồ sơ chưa thu hồi")]
            HO_SO_CHUA_THU_HOI = 1,
            [EnumDisplayString("Hồ sơ đã thu hồi")]
            HO_SO_DA_THU_HOI = 2,
        }
        #endregion
    }
}