using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        #region KET_QUA_XU_LY
        public enum KET_QUA_XU_LY
        {
            [EnumDisplayString("Đạt")]
            DAT = 1,
            [EnumDisplayString("Cần bổ sung")]
            BO_SUNG = 2,
            [EnumDisplayString("Không đạt")]
            KHONG_DAT = 3
        }
        
        #endregion

        #region TRANG_THAI_HO_SO

        public enum TRANG_THAI_HO_SO
        {
            [EnumDisplayString("Đã lưu")]
            DA_LUU = 0,
            [EnumDisplayString("Chờ một cửa tiếp nhận")]
            DA_NOP_HO_SO_MOI = 1,
            [EnumDisplayString("Một cửa trả lại")]
            MOT_CUA_TRA_LAI = 11,
            [EnumDisplayString("Chờ doanh nghiệp thanh toán")]
            CHO_DOANH_NGHIEP_THANH_TOAN = 12,
            [EnumDisplayString("Chờ kế toán xác nhận")]
            CHO_KE_TOAN_XAC_NHAN = 13,
            [EnumDisplayString("Thanh toán thất bại")]
            THANH_TOAN_THAT_BAI = 14,
            [EnumDisplayString("Chờ trả giấy tiếp nhận")]
            CHO_TRA_GIAY_TIEP_NHAN = 19,
            [EnumDisplayString("Một cửa đã tiếp nhận")]
            MOT_CUA_DA_TIEP_NHAN = 2,
            [EnumDisplayString("Sửa đổi bổ sung")]
            SUA_DOI_BO_SUNG = 4,
            [EnumDisplayString("Sửa đổi khắc phục cơ sở")]
            SUA_DOI_KHAC_PHUC = 41,
            [EnumDisplayString("Đã nộp bổ sung")]
            DA_NOP_BO_SUNG = 5,
            [EnumDisplayString("Đã hoàn tất")]
            DA_HOAN_TAT = 6,
            [EnumDisplayString("Từ chối cấp phép")]
            TU_CHOI_CAP_PHEP = 7,
            [EnumDisplayString("Hồ sơ thẩm định lại")]
            HO_SO_THAM_DINH_LAI = 8,
            [EnumDisplayString("Hồ sơ cho phép khảo nghiệm")]
            CHO_PHEP_KHAO_NGHIEM = 99,
            [EnumDisplayString("Lãnh đạo đã phân công hồ sơ")]
            LANH_DAO_DA_PHAN_CONG_HO_SO = 111,
            [EnumDisplayString("Phòng ban đã phân công tới chuyên viên")]
            PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN = 112,
            [EnumDisplayString("Hồ sơ thẩm xét lại")]
            HO_SO_THAM_XET_LAI = 113,
            [EnumDisplayString("Lãnh đạo đã duyệt thẩm xét")]
            LANH_DAO_DA_DUYET_THAM_XET = 115,
            [EnumDisplayString("Phòng ban từ chối tiếp nhận hồ sơ")]
            PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO = 116,
            [EnumDisplayString("Chuyên viên từ chối tiếp nhận hồ sơ")]
            CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO = 117,
        }

        public enum TRANG_THAI_CONG_VAN
        {
            [EnumDisplayString("Công văn không đạt")]
            KHONG_DAT = 0,
            [EnumDisplayString("Công văn đạt")]
            DAT = 1,
            [EnumDisplayString("Công văn đạt xin ý kiến lãnh đạo cục")]
            DAT_XIN_Y_KIEN = 2,
            [EnumDisplayString("Cần bổ sung")]
            CAN_BO_SUNG = 3,
            [EnumDisplayString("Hồ sơ đạt, trình lãnh đạo ký")]
            DAT_TRINH_LANH_DAO_KY = -1
        }
        #endregion

        #region TRANG_THAI_KE_TOAN
        public enum TRANG_THAI_KE_TOAN
        {
            [EnumDisplayString("Kế toán chưa xác nhận")]
            KE_TOAN_CHUA_XAC_NHAN = 0,
            [EnumDisplayString("Kế toán xác nhận thành công")]
            KE_TOAN_XAC_NHAN_THANH_CONG = 1,
            [EnumDisplayString("Kế toán xác nhận không thành công")]
            KE_TOAN_XAC_NHAN_KHONG_THANH_CONG = 2,
        }
        public enum TRANG_THAI_THANH_TOAN_HO_SO
        {
            [EnumDisplayString("Tất cả")]
            TAT_CA = 0,
            [EnumDisplayString("Hồ sơ chưa có thanh toán")]
            HO_SO_CHUA_CO_THANH_TOAN = 1,
            [EnumDisplayString("Hồ sơ có phát sinh thanh toán")]
            HO_SO_CO_PHAT_SINH_THANH_TOAN = 2,
            [EnumDisplayString("Hồ sơ thanh toán thành công")]
            HO_SO_THANH_TOAN_THANH_CONG = 3,
            [EnumDisplayString("Hồ sơ thanh toán chưa thành công")]
            HO_SO_THANH_TOAN_CHUA_THANH_CONG = 4,
            [EnumDisplayString("Hồ sơ đang chờ xác nhận thanh toán")]
            HO_SO_CHO_XAC_NHAN_THANH_TOAN = 5
        }
        #endregion

        #region TT03 
        public enum TRANG_THAI_CONG_VAN_TT03
        {

            [EnumDisplayString("Duyệt ký cho phép khảo nghiệm")]
            DUYET_KY_CHO_PHEP_KHAO_NGHIEM = 1
          ,
            [EnumDisplayString("Duyệt ký yêu cầu sửa đổi bổ sung")]
            DUYET_KY_YEU_CAU_SUA_DOI_BO_SUNG = 2
          ,
            [EnumDisplayString("Duyệt ký không cho phép khảo nghiệm")]
            DUYET_KY_KHONG_CHO_PHEP_KHAO_NGHIEM = 3
          ,

            [EnumDisplayString("Duyệt cấp số đăng ký")]
            DUYET_CAP_SO_DANG_KY = 4
          ,
            [EnumDisplayString("Duyệt yêu cầu sửa đổi bổ sung cấp số đăng ký")]
            DUYET_YEU_CAU_SUA_DOI_BO_SUNG_CAP_SO_DANG_KY = 5
          ,
            [EnumDisplayString("Duyệt không cấp số đăng ký")]
            DUYET_KHONG_CAP_SO_DANG_KY = 6
          ,
           
        }

        public enum LOAI_HO_SO_TT03
        {
            [EnumDisplayString("Hồ sơ đăng ký trong nước")]
            TRONG_NUOC = 1,
            [EnumDisplayString("Hồ sơ đăng ký nước ngoài")]
            NUOC_NGOAI = 2,
        }

        public enum TRANG_THAI_BIEN_BAN_CHUYEN_GIA
        {
            [EnumDisplayString("Chưa thẩm định")]
            CHUA_THAM_DINH = 0,
            [EnumDisplayString("Lưu nháp")]
            LUU_NHAP = 1,
            [EnumDisplayString("Thẩm định lại")]
            THAM_DINH_LAI = 3,
            [EnumDisplayString("Hoàn thành")]
            HOAN_THANH = 100,
        }
        public enum TRANG_THAI_BIEN_BAN_HOI_DONG_THAM_DINH
        {
            [EnumDisplayString("Chưa thẩm định")]
            CHUA_THAM_DINH = 0,
            [EnumDisplayString("Lưu nháp")]
            LUU_NHAP = 1,
            [EnumDisplayString("Thẩm định lại")]
            THAM_DINH_LAI = 3,
            [EnumDisplayString("Hoàn thành")]
            HOAN_THANH = 100,
        }
        #endregion

        #region TT04 - Cơ sở đủ điều kiện Y Tế
        public enum TT04_TRANG_THAI_DANH_CHO_TRA_CUU
        {

            [EnumDisplayString("Mới nộp")]
            MOI_NOP = 1
          ,
            [EnumDisplayString("Đã phân công chờ thẩm xét")]
            DA_PHAN_CONG_CHO_THAM_XET = 2
          ,
            [EnumDisplayString("Đã thẩm xét, chờ lãnh đạo phòng kết luận")]
            DA_THAM_XET_CHO_LANH_DAO_PHONG_KET_LUAN = 3
          ,
            [EnumDisplayString("Chuyển chuyên viên thẩm xét lại")]
            CHUYEN_CHUYEN_VIEN_THAM_XET_LAI = 4
          ,
            [EnumDisplayString("Trưởng phòng đã kết luận thẩm xét, chờ LĐC duyệt")]
            DA_KET_LUAN_THAM_XET_CHO_LDC_DUYET = 5
          ,
            [EnumDisplayString("LĐC yêu cầu thẩm xét lại")]
            LDC_YEU_CAU_THAM_XET_LAI = 6
          ,
            [EnumDisplayString("Đã duyệt thẩm xét, chờ văn thư đóng dấu")]
            DA_DUYET_THAM_XET_CHO_VAN_THU_DONG_DAU = 7
          ,
            [EnumDisplayString("Đã trả kết quả thẩm xét hồ sơ")]
            DA_TRA_KET_QUA_THAM_XET_HO_SO = 8
          ,
            [EnumDisplayString("Chờ kế toán xác nhận thanh toán")]
            CHO_KE_TOAN_XAC_NHAN = 9
          ,
            [EnumDisplayString("Chờ chuyên viên duyệt thẩm định")]
            CHO_CHUYEN_VIEN_DUYET_THAM_DINH = 10
          ,
            [EnumDisplayString("Chờ doanh nghiệp nộp khắc phục cơ sở")]
            CHO_DOANH_NGHIEP_NOP_KHAC_PHUC = 11
          ,
            [EnumDisplayString("Chuyển chuyên viên duyệt thẩm định lại")]
            CHUYEN_CHUYEN_VIEN_DUYET_THAM_DINH_LAI = 12
          ,
            [EnumDisplayString("Chuyên viên đã duyệt thẩm định, chờ trưởng phòng kết luận")]
            CHO_TRUONG_PHONG_KET_LUAN_THAM_DINH = 13
          ,
            [EnumDisplayString("Trưởng phòng đã kết luận thẩm định, chờ LĐC duyệt")]
            CHO_LDC_DUYET_THAM_DINH = 14
          ,
            [EnumDisplayString("LĐC yêu cầu duyệt thẩm định lại")]
            LDC_YEU_CAU_DUYET_THAM_DINH_LAI = 15
          ,
            [EnumDisplayString("LĐC đã duyệt thẩm định, chờ văn thư đóng dấu")]
            LDC_DA_DUYET_THAM_DINH_CHO_VAN_THU_DONG_DAU = 16
          ,
            [EnumDisplayString("Đã trả kết quả thẩm định cơ sở")]
            DA_TRA_KET_QUA_THAM_DINH_CO_SO = 17
        }
        #endregion

        #region TT05 - Cơ sở đủ điều kiện Công thương
        public enum TT05_TRANG_THAI_DANH_CHO_TRA_CUU
        {

            [EnumDisplayString("Mới nộp")]
            MOI_NOP = 1
          ,
            [EnumDisplayString("Đã phân công chờ thẩm xét")]
            DA_PHAN_CONG_CHO_THAM_XET = 2
          ,
            [EnumDisplayString("Đã thẩm xét, chờ lãnh đạo phòng kết luận")]
            DA_THAM_XET_CHO_LANH_DAO_PHONG_KET_LUAN = 3
          ,
            [EnumDisplayString("Chuyển chuyên viên thẩm xét lại")]
            CHUYEN_CHUYEN_VIEN_THAM_XET_LAI = 4
          ,
            [EnumDisplayString("Trưởng phòng đã kết luận thẩm xét, chờ LĐC duyệt")]
            DA_KET_LUAN_THAM_XET_CHO_LDC_DUYET = 5
          ,
            [EnumDisplayString("LĐC yêu cầu thẩm xét lại")]
            LDC_YEU_CAU_THAM_XET_LAI = 6
          ,
            [EnumDisplayString("Đã duyệt thẩm xét, chờ văn thư đóng dấu")]
            DA_DUYET_THAM_XET_CHO_VAN_THU_DONG_DAU = 7
          ,
            [EnumDisplayString("Đã trả kết quả thẩm xét hồ sơ")]
            DA_TRA_KET_QUA_THAM_XET_HO_SO = 8
          ,
            [EnumDisplayString("Chờ kế toán xác nhận thanh toán")]
            CHO_KE_TOAN_XAC_NHAN = 9
          ,
            [EnumDisplayString("Chờ chuyên viên duyệt thẩm định")]
            CHO_CHUYEN_VIEN_DUYET_THAM_DINH = 10
          ,
            [EnumDisplayString("Chờ doanh nghiệp nộp khắc phục cơ sở")]
            CHO_DOANH_NGHIEP_NOP_KHAC_PHUC = 11
          ,
            [EnumDisplayString("Chuyển chuyên viên duyệt thẩm định lại")]
            CHUYEN_CHUYEN_VIEN_DUYET_THAM_DINH_LAI = 12
          ,
            [EnumDisplayString("Chuyên viên đã duyệt thẩm định, chờ trưởng phòng kết luận")]
            CHO_TRUONG_PHONG_KET_LUAN_THAM_DINH = 13
          ,
            [EnumDisplayString("Trưởng phòng đã kết luận thẩm định, chờ LĐC duyệt")]
            CHO_LDC_DUYET_THAM_DINH = 14
          ,
            [EnumDisplayString("LĐC yêu cầu duyệt thẩm định lại")]
            LDC_YEU_CAU_DUYET_THAM_DINH_LAI = 15
          ,
            [EnumDisplayString("LĐC đã duyệt thẩm định, chờ văn thư đóng dấu")]
            LDC_DA_DUYET_THAM_DINH_CHO_VAN_THU_DONG_DAU = 16
          ,
            [EnumDisplayString("Đã trả kết quả thẩm định cơ sở")]
            DA_TRA_KET_QUA_THAM_DINH_CO_SO = 17
        }
        #endregion

        #region TT06 - Cơ sở đủ điều kiện Nông nghiệp
        public enum TT06_TRANG_THAI_DANH_CHO_TRA_CUU
        {

            [EnumDisplayString("Mới nộp")]
            MOI_NOP = 1
          ,
            [EnumDisplayString("Đã phân công chờ thẩm xét")]
            DA_PHAN_CONG_CHO_THAM_XET = 2
          ,
            [EnumDisplayString("Đã thẩm xét, chờ lãnh đạo phòng kết luận")]
            DA_THAM_XET_CHO_LANH_DAO_PHONG_KET_LUAN = 3
          ,
            [EnumDisplayString("Chuyển chuyên viên thẩm xét lại")]
            CHUYEN_CHUYEN_VIEN_THAM_XET_LAI = 4
          ,
            [EnumDisplayString("Trưởng phòng đã kết luận thẩm xét, chờ LĐC duyệt")]
            DA_KET_LUAN_THAM_XET_CHO_LDC_DUYET = 5
          ,
            [EnumDisplayString("LĐC yêu cầu thẩm xét lại")]
            LDC_YEU_CAU_THAM_XET_LAI = 6
          ,
            [EnumDisplayString("Đã duyệt thẩm xét, chờ văn thư đóng dấu")]
            DA_DUYET_THAM_XET_CHO_VAN_THU_DONG_DAU = 7
          ,
            [EnumDisplayString("Đã trả kết quả thẩm xét hồ sơ")]
            DA_TRA_KET_QUA_THAM_XET_HO_SO = 8
          ,
            [EnumDisplayString("Chờ kế toán xác nhận thanh toán")]
            CHO_KE_TOAN_XAC_NHAN = 9
          ,
            [EnumDisplayString("Chờ chuyên viên duyệt thẩm định")]
            CHO_CHUYEN_VIEN_DUYET_THAM_DINH = 10
          ,
            [EnumDisplayString("Chờ doanh nghiệp nộp khắc phục cơ sở")]
            CHO_DOANH_NGHIEP_NOP_KHAC_PHUC = 11
          ,
            [EnumDisplayString("Chuyển chuyên viên duyệt thẩm định lại")]
            CHUYEN_CHUYEN_VIEN_DUYET_THAM_DINH_LAI = 12
          ,
            [EnumDisplayString("Chuyên viên đã duyệt thẩm định, chờ trưởng phòng kết luận")]
            CHO_TRUONG_PHONG_KET_LUAN_THAM_DINH = 13
          ,
            [EnumDisplayString("Trưởng phòng đã kết luận thẩm định, chờ LĐC duyệt")]
            CHO_LDC_DUYET_THAM_DINH = 14
          ,
            [EnumDisplayString("LĐC yêu cầu duyệt thẩm định lại")]
            LDC_YEU_CAU_DUYET_THAM_DINH_LAI = 15
          ,
            [EnumDisplayString("LĐC đã duyệt thẩm định, chờ văn thư đóng dấu")]
            LDC_DA_DUYET_THAM_DINH_CHO_VAN_THU_DONG_DAU = 16
          ,
            [EnumDisplayString("Đã trả kết quả thẩm định cơ sở")]
            DA_TRA_KET_QUA_THAM_DINH_CO_SO = 17
        }
        #endregion

        public enum TRANG_THAI_THAM_DINH_HO_SO_37
        {
            [EnumDisplayString("Hồ sơ chờ thẩm định")]
            HO_SO_CHO_THAM_DINH = 371,

            [EnumDisplayString("Hồ sơ chờ tổng hợp thẩm định")]
            HO_SO_CHO_TONG_HOP_THAM_DINH = 372,

            [EnumDisplayString("Hồ tổng hợp thẩm định đã lưu")]
            HO_SO_TONG_HOP_THAM_DINH_DA_LUU = 373,

            [EnumDisplayString("Tổng hợp thẩm định lại")]
            TONG_HOP_THAM_DINH_LAI = 374,

            [EnumDisplayString("Lãnh đạo cục duyệt thẩm định")]
            LANH_DAO_CUC_DUYET_THAM_DINH = 375,

            [EnumDisplayString("Chờ văn thư trả kết quả cho người đăng ký")]
            HO_SO_CHO_VAN_THU_TRA_KET_QUA = 376,

        }

        public enum TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37
        {
            [EnumDisplayString("Chờ một cửa tiếp nhận")]
            DA_NOP_HO_SO_MOI = 1,

            [EnumDisplayString("Một cửa đã tiếp nhận")]
            MOT_CUA_DA_TIEP_NHAN = 2,

            [EnumDisplayString("Một cửa trả lại")]
            MOT_CUA_TRA_LAI = 3,

            [EnumDisplayString("Chờ trả giấy tiếp nhận")]
            CHO_TRA_GIAY_TIEP_NHAN = 4,

            [EnumDisplayString("Lãnh đạo đã phân công hồ sơ")]
            LANH_DAO_DA_PHAN_CONG_HO_SO = 5,

            [EnumDisplayString("Phòng ban từ chối tiếp nhận hồ sơ")]
            PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO = 6,

            [EnumDisplayString("Chuyên viên từ chối tiếp nhận hồ sơ")]
            CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO = 7,

            [EnumDisplayString("Sửa đổi bổ sung")]
            SUA_DOI_BO_SUNG = 9,

            [EnumDisplayString("Hồ sơ thẩm xét lại")]
            HO_SO_THAM_XET_LAI = 10,

            [EnumDisplayString("Hồ sơ chờ thẩm định")]
            HO_SO_CHO_THAM_DINH = 11,

            [EnumDisplayString("Hồ sơ chờ tổng hợp thẩm định")]
            HO_SO_CHO_TONG_HOP_THAM_DINH = 12,

            [EnumDisplayString("Hồ tổng hợp thẩm định đã lưu")]
            HO_SO_TONG_HOP_THAM_DINH_DA_LUU = 13,

            [EnumDisplayString("Tổng hợp thẩm định lại")]
            TONG_HOP_THAM_DINH_LAI = 14,

            [EnumDisplayString("Lãnh đạo cục duyệt thẩm định")]
            LANH_DAO_CUC_DUYET_THAM_DINH = 15,

            [EnumDisplayString("Chờ văn thư trả kết quả cho người đăng ký")]
            HO_SO_CHO_VAN_THU_TRA_KET_QUA = 16,

            [EnumDisplayString("Đã hoàn tất")]
            DA_HOAN_TAT = 17,
            
            
            
            
        }

    }
}
