using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace newPSG.PMS
{
    public static class AppQuerySql
    {
        public static class WhereQueryFormCaseThuTuc
        {
            /// <summary>
            /// Hồ sơ doanh nghiệp đã nộp
            /// </summary>
            public static string COMMON_HO_SO_XU_LY = $@" (hs.TrangThaiHoSo is not null 
                and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU} 
                and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT}
                and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI}) ";

            public static string COMMON_HO_SO_TENANT = $@" (hs.ChiCucId is null)  ";
            public static class FORM_DANG_KY_HO_SO
            {
                public static string TAT_CA = " hs.DoanhNghiepId = @doanhnghiepid ";
                public static string HO_SO_MOI = $@" Isnull(hs.TrangThaiHoSo,0) = {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU} ";
                public static string HO_SO_CHO_TIEP_NHAN = $@" (hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI}) and not (hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP}  and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN})";
                public static string HO_SO_CHO_THANH_TOAN = $@" (hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_DOANH_NGHIEP_THANH_TOAN} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI})";
                public static string HO_SO_CHO_XAC_NHAN_THANH_TOAN = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN} ";
                public static string HO_SO_BI_TRA_LAI = $@" (hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI}) and (hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN}  and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP})";
                public static string HO_SO_CAN_BO_SUNG = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_KHAC_PHUC} ";
                public static string HO_SO_HOAN_TAT = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT} ";
                public static string HO_SO_CHO_NOP_GIAY_KIEM_NGHIEM = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_PHEP_KHAO_NGHIEM} ";
            }
            public static class FORM_MOT_CUA_TIEP_NHAN_03
            {
                public static string TAT_CA = $@" ({COMMON_HO_SO_TENANT}) ";
                public static string HO_SO_NOP_MOI = $@" {TAT_CA}  and hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI} ";
                public static string HO_SO_NOP_BO_SUNG = $@"hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG}";
                public static string HO_SO_CHO_TRA_GIAY_TIEP_NHAN = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN} ";
            }
            public static class FORM_MOT_CUA_TIEP_NHAN
            {
                public static string TAT_CA = $@" ({COMMON_HO_SO_TENANT} and hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI}) ";
                public static string HO_SO_NOP_MOI = $@" {TAT_CA}  and hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI} ";

            }
            public static class FORM_MOT_CUA_PHAN_CONG
            {
                public static string TAT_CA = $@"( {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hs.TrangThaiHoSo in ({(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN})) ";
                public static string HO_SO_CHO_PHAN_CONG = $@" {TAT_CA}  and hs.PhongBanId is null ";

            }
            public static class FORM_PHONG_BAN_PHAN_CONG
            {
                public static string TAT_CA = $@" (hs.PhongBanId = @phongbanid) and (hs.IsChuyenAuto = 1 or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} or hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG}) ";
                public static string CHUA_PHAN_CONG = $@" hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG} ";
                public static string DA_PHAN_CONG = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG}  and IsNull(hsxl.ChuyenVienThuLyDaDuyet,0) = 0 ";
                public static string DA_XU_LY = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG}  and ( hsxl.ChuyenVienThuLyDaDuyet = 1 or hsxl.ChuyenVienPhoiHopDaDuyet = 1 ) ";
            }
            public static class FORM_THAM_XET_HO_SO
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and ( hsxl.ChuyenVienThuLyId = @chuyenvienid or hsxl.ChuyenVienPhoiHopId = @chuyenvienid ) ";
                public static string HO_SO_THAM_XET_MOI = $@" IsNull(hs.IsHoSoBS,0)= 0
             and ( 
             ((hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET} or hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET}) and (hsxl.ChuyenVienPhoiHopId = @chuyenvienid and ISNULL(hsxl.ChuyenVienPhoiHopDaDuyet,0) =0 ))
            or
            ((hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET} or hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET}) and (hsxl.ChuyenVienThuLyId = @chuyenvienid and ISNULL(hsxl.ChuyenVienThuLyDaDuyet,0)= 0))
            ) ";
                public static string HO_SO_DA_THAM_XET = $@" 
                            ((hsxl.ChuyenVienPhoiHopId = @chuyenvienid and hsxl.ChuyenVienPhoiHopDaDuyet = 1) and ( hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and ( hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET} or hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET} )))
                            or
                            ((hsxl.ChuyenVienThuLyId = @chuyenvienid and hsxl.ChuyenVienThuLyDaDuyet = 1) and (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and (hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET} or hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET} )))
                            ";
                public static string HO_SO_THAM_XET_BO_SUNG = $@" hsxl.ChuyenVienThuLyId = @chuyenvienid and hs.IsHoSoBS = 1
                           and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP} 
                           and (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET} or hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP})";
                public static string HO_SO_THAM_XET_LAI = $@" hsxl.ChuyenVienThuLyId = @chuyenvienid and hsxl.ChuyenVienThuLyDaDuyet = 1
                           and ((hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} or hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.PHO_PHONG}) and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET
})";
                public static string HO_SO_DANG_THEO_DOI = $@" (hsxl.ChuyenVienPhoiHopId = @chuyenvienid and hsxl.ChuyenVienPhoiHopDaDuyet = 1)
                                                        or ((hsxl.ChuyenVienThuLyId = @chuyenvienid and hsxl.ChuyenVienThuLyDaDuyet = 1)
                                                        and  not(hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and (hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} or hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.PHO_PHONG}))
                                                        and not(hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and (hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET} or hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET})))
                                                        ";

                // formcase 2
                public static string VI_TRI_CHUYEN_VIEN_THU_LY = $@" hsxl.ChuyenVienThuLyId = @chuyenvienid ";
                public static string VI_TRI_CHUYEN_VIEN_PHOI_HOP = $@" hsxl.ChuyenVienPhoiHopId = @chuyenvienid ";
                //public static string VI_TRI_CHUYEN_VIEN_THU_LY_SOAN_CONG_VAN = $@" (hsxl.IsAllChuyenGiaDaDuyet = 1 OR hsxl.IsAllHoiDongThamDinhDaDuyet = 1) and(hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET})";
            }
            public static class FORM_PHO_PHONG_DUYET
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hsxl.PhoPhongId = @phophongid ";
                public static string HO_SO_CHUA_DUYET = $@" hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.PHO_PHONG} ";
                public static string HO_SO_DA_DUỴET = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.PHO_PHONG} and hsxl.PhoPhongDaDuyet = 1 ";
            }
            public static class FORM_TRUONG_PHONG_DUYET
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hsxl.TruongPhongId = @truongphongid  and (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} or hsxl.TruongPhongDaDuyet = 1) ";
                public static string HO_SO_CHUA_DUYET = $@" hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} ";
                public static string HO_SO_DA_DUỴET = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} and hsxl.TruongPhongDaDuyet = 1 ";
            }

            public static class FORM_LANH_DAO_CUC_DUYET
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hsxl.LanhDaoCucId = @lanhdaocuc ";
                public static string HO_SO_DAT_CHUA_DUYET = $@" hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC} and hsxl.HoSoIsDat = 1 ";
                public static string HO_SO_DA_DUỴET = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC} and hsxl.LanhDaoCucDaDuyet = 1 ";
            }
            public static class FORM_LANH_DAO_CUC_DUYET_CONG_VAN
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hsxl.LanhDaoCucId = @lanhdaocuc ";
                public static string CONG_VAN_CHUA_DUYET = $@" hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC} and hsxl.HoSoIsDat = 0 ";
                public static string CONG_VAN_DA_DUỴET = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC} and hsxl.LanhDaoCucDaDuyet = 1 and hsxl.HoSoIsDat = 0";
            }
            public static class FORM_LANH_DAO_BO_DUYET
            {
                public static string TAT_CA = $@" hsxl.LanhDaoBoId = @lanhdaobo and  hs.TrangThaiHoSo  in ({(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN}
                           ,{(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG}
                            ,{(int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT}) ";
                public static string HO_SO_CHUA_DUYET = $@" hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_BO}";
                public static string HO_SO_DA_DUỴET = $@" hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_BO} and hsxl.LanhDaoCucDaDuyet = 1 ";
            }
            public static class FORM_VAN_THU_DUYET
            {
                public static string TAT_CA_BASIC = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI} and hsxl.LanhDaoCucDaDuyet is not null ";
                public static string TAT_CA = TAT_CA_BASIC + $@" and ((hsxl.DonViXuLy={(int)CommonENum.DON_VI_XU_LY.VAN_THU} and hsxl.VanThuId is null) or hsxl.VanThuId = @vanthuid)";
                public static string HO_SO_CHUA_DUYET = TAT_CA_BASIC + " and" + $@" (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.VAN_THU} and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC})";
                public static string HO_SO_DA_DUỴET = TAT_CA_BASIC + " and" + $@" hsxl.VanThuId = @vanthuid and hsxl.VanThuIsCA = 1";
            }
            public static class FORM_VAN_THU_DUYET_CONG_VAN
            {
                public static string TAT_CA_BASIC = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI}";
                public static string TAT_CA = TAT_CA_BASIC + $@" and ((hsxl.VanThuId is null) or hsxl.VanThuId = @vanthuid)";
                public static string CONG_VAN_CHUA_DUYET = TAT_CA_BASIC + " and" + $@" (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.VAN_THU} and hsxl.HoSoIsDat = 0)";
                public static string CONG_VAN_TRA_LAI = TAT_CA_BASIC + " and" + $@" hsxl.VanThuId = @vanthuid and hsxl.VanThuDaDuyet = 1 and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG}";
                public static string CONG_VAN_DA_DUỴET = TAT_CA_BASIC + " and" + $@" hsxl.VanThuId = @vanthuid and hsxl.VanThuIsCA = 1 and hsxl.HoSoIsDat = 0 and hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} and hsxl.DonViXuLy <> {(int)CommonENum.DON_VI_XU_LY.VAN_THU} ";
            }
            public static class FORM_TRA_CUU_HO_SO
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and {COMMON_HO_SO_TENANT} and hs.TrangThaiHoSo is not null ";
                public static string HO_SO_MOI = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU} ";
                public static string HO_SO_DANG_XU_LY = $@" hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU}
                                  and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG}
                                  and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT} ";
                public static string HO_SO_YEU_CAU_BO_SUNG = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG} ";
                public static string HO_SO_HOAN_THANH = $@" hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT} ";

            }

            public static class FORM_KE_TOAN_XAC_NHAN_THANH_TOAN
            {
                public static string TAT_CA = $@"hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN} or hs.TrangThaiHoSo= {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} or hs.TrangThaiHoSo={(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI}";
                public static string HO_SO_CHO_XAC_NHAN = $@"hs.TrangThaiHoSo ={(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN} and tt.TrangThaiKeToan ={(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN}";
                public static string HO_SO_THANH_TOAN_THANH_CONG = $@"hs.TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} and tt.TrangThaiKeToan ={(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_THANH_CONG}";
                public static string HO_SO_THANH_TOAN_THAT_BAI = $@"hs.TrangThaiHoSo ={(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI} and tt.TrangThaiKeToan={(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_KHONG_THANH_CONG}";
            }
            public static class FORM_TONG_HOP_THAM_XET_HO_SO
            {
                public static string TAT_CA = $@"( (hs.TrangThaiHoSo={(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} or hs.TrangThaiHoSo={(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG})"
                                            + $@" and hsxl.LuongXuLy= {(int)CommonENum.LUONG_XU_LY.LUONG_HO_SO}"
                                            + $@" and hsxl.ChuyenVienThuLyId= @chuyenvienid"
                                            + $@" and hsxl.ChuyenVienThuLyDaDuyet= 1 )";
                public static string HO_SO_CHO_TONG_HOP = TAT_CA + $@" and (hsxl.DonViXuLy= {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and hsxl.DonViGui <>{(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG} and  hsxl.DonViGui<>{(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP})";
                public static string HO_SO_TONG_HOP_LAI = TAT_CA + $@" and (hsxl.DonViXuLy= {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG})";
                public static string HO_SO_TONG_HOP_BO_SUNG = TAT_CA + $@" and (hsxl.DonViXuLy= {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP} and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP})";
                public static string HO_SO_XU_LY = "("+ HO_SO_CHO_TONG_HOP + ")or(" + HO_SO_TONG_HOP_LAI + ")or(" + HO_SO_TONG_HOP_BO_SUNG + ")";
            }
            public static class FORM_THAM_XET_HO_SO_TT04
            {
                public static string TAT_CA = $@"hsxl.LuongXuLy = {(int)CommonENum.LUONG_XU_LY.LUONG_HO_SO} and (hs.TrangThaiHoSo= {(int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN} or hs.TrangThaiHoSo={(int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG})"
                                            + $@"and (hsxl.ChuyenVienThuLyId=@chuyenvienid or hsxl.ChuyenVienPhoiHopId=@chuyenvienid)";
                public static string HO_SO_THAM_XET_MOI = TAT_CA + $@"and (hsxl.IsHoSoBS is null or hsxl.IsHoSoBS=0)"
                                                        + $@"and( 
                                                                ((hsxl.ChuyenVienPhoiHopId = @chuyenvienid and (hsxl.ChuyenVienPhoiHopDaDuyet is null or hsxl.ChuyenVienPhoiHopDaDuyet=0) ) and (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET} or hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET}))
                                                                or ((hsxl.ChuyenVienThuLyId = @chuyenvienid and (hsxl.ChuyenVienThuLyDaDuyet  is null or hsxl.ChuyenVienThuLyDaDuyet=0) ) and (hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET} or hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET}))
                                                                )";
            }
            public static class FORM_THONG_KE_HO_SO_CHUNG
            {
                public static string TAT_CA = $@" hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.DA_LUU} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN} and hs.TrangThaiHoSo <> {(int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI} ";

                public static string HO_SO_DA_TIEP_NHAN = TAT_CA + "and hs.MotCuaChuyenId is not null" + " and hs.PhongBanId is not null";

                public static string HO_SO_DANG_XU_LY = HO_SO_DA_TIEP_NHAN + " and hsxl.IsHoSoBS <> 1 " + $@" and hsxl.LanhDaoCucId is null";
                public static string HO_SO_CHO_DUYET = HO_SO_DA_TIEP_NHAN + " and hsxl.IsHoSoBS <> 1 " + " and hsxl.LanhDaoCucId is not null " + $@" and hsxl.LanhDaoCucDaDuyet <> 1";
                public static string HO_SO_DA_DUYET = HO_SO_DA_TIEP_NHAN + " and hsxl.IsHoSoBS <> 1 " + " and hsxl.LanhDaoCucId is not null " + $@" and hsxl.LanhDaoCucDaDuyet = 1";

                public static string HO_SO_CAN_SUA_DOI_BO_SUNG = HO_SO_DA_TIEP_NHAN + " and hsxl.IsHoSoBS = 1";

                public static string HO_SO_CHUA_TIEP_NHAN = TAT_CA + "and hs.MotCuaChuyenId is null";
                public static string HO_SO_VAN_THU_TRA_LAI = TAT_CA + "and hs.MotCuaChuyenId is not null " + " and hs.PhongBanId is null";

            }
            public static class FORM_THONG_KE_MOT_CUA_PHAN_CONG
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and (hs.MotCuaChuyenId = @motcuachuyenid) ";
                public static string HO_SO_DA_TIEP_NHAN = $@" {COMMON_HO_SO_XU_LY} and  hs.MotCuaChuyenId = @motcuachuyenid and hs.PhongBanId is not null ";
                public static string HO_SO_BI_TU_CHOI = $@" {COMMON_HO_SO_XU_LY} and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN} and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP} and hsxl.NguoiXuLyId=@motcuachuyenid ";
            }
            public static class FORM_THONG_KE_VAN_THU
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} and (hsxl.VanThuId = @vanthuid) ";
                public static string HO_SO_DA_DONG_DAU = TAT_CA + " and" + $@" hsxl.VanThuId = @vanthuid and hsxl.VanThuIsCA = 1 ";
                public static string HO_SO_BI_TU_CHOI = TAT_CA + " and" + $@" hsxl.VanThuId = @vanthuid and hsxl.VanThuDaDuyet = 0 ";
            }
            public static class FORM_THONG_KE_ADMIN
            {
                public static string TAT_CA = $@" {COMMON_HO_SO_XU_LY} ";
                public static string HO_SO_DA_TIEP_NHAN = $@" {COMMON_HO_SO_XU_LY} and hs.PhongBanId is not null ";
                public static string HO_SO_BI_TU_CHOI = $@" {COMMON_HO_SO_XU_LY} and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN} and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP} ";
                public static string HO_SO_DA_GIAI_QUYET = $@" {COMMON_HO_SO_XU_LY} and hsxl.DonViGui = {(int)CommonENum.DON_VI_XU_LY.VAN_THU} and hsxl.DonViXuLy = {(int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP} and hsxl.VanThuIsCA = 1";
                public static string HO_SO_CHO_BO_SUNG = $@" {COMMON_HO_SO_XU_LY} and hsxl.IsHoSoBS = 1 ";
            }
        }
        public static List<SqlParameter> ParameterThuTucQuery(int? doanhNghiepId = null, int? phongBanId = null, int? nhomThuTucId = null)
        {
            List<SqlParameter> prm = new List<SqlParameter>()
                         {
                                new SqlParameter("@doanhnghiepid",SqlDbType.BigInt),
                                new SqlParameter("@phongbanid",SqlDbType.BigInt),
                                new SqlParameter("@truongphongid",SqlDbType.BigInt),
                                new SqlParameter("@chuyenvienid",SqlDbType.BigInt),
                                new SqlParameter("@phophongid",SqlDbType.BigInt),
                                new SqlParameter("@lanhdaocuc",SqlDbType.BigInt),
                                new SqlParameter("@lanhdaobo",SqlDbType.BigInt),
                                new SqlParameter("@motcuachuyenid",SqlDbType.BigInt),
                                new SqlParameter("@vanthuid",SqlDbType.BigInt),
                                new SqlParameter("@nhomthutucid",SqlDbType.BigInt),
                                new SqlParameter("@tenantid", SqlDbType.Int)
                         };
            prm[0].Value = (object)SessionCustom.UserCurrent.DoanhNghiepId ?? (object)doanhNghiepId ?? DBNull.Value;
            prm[1].Value = (object)phongBanId ?? (object)SessionCustom.UserCurrent.PhongBanId ?? DBNull.Value;
            prm[2].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[3].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[4].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[5].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[6].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[7].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[8].Value = (object)SessionCustom.UserCurrent.Id ?? DBNull.Value;
            prm[9].Value = (object)nhomThuTucId ?? DBNull.Value;
            prm[10].Value = (object)SessionCustom.UserCurrent.TenantId ?? DBNull.Value;
            return prm;
        }
    }
}
