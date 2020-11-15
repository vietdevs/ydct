namespace newPSG.PMS.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static class AppPermissions
    {
        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        #region Hệ Thống Zero

        public const string Pages = "Pages";

        public const string Pages_Administration = "Pages.Administration";

        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";

        public const string Pages_Administration_Languages = "Pages.Administration.Languages";
        public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
        public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
        public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
        public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";

        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";

        public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";

        //TENANT-SPECIFIC PERMISSIONS

        public const string Pages_Tenant_Dashboard = "Pages.Tenant.Dashboard";

        public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";

        //HOST-SPECIFIC PERMISSIONS

        public const string Pages_Editions = "Pages.Editions";
        public const string Pages_Editions_Create = "Pages.Editions.Create";
        public const string Pages_Editions_Edit = "Pages.Editions.Edit";
        public const string Pages_Editions_Delete = "Pages.Editions.Delete";

        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";

        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";

        #endregion Hệ Thống Zero

        #region Quản lý Admin

        //Tenant
        public const string Pages_Tenant_QuanLyAdmin = "Pages.QuanLyAdmin";

        #endregion Quản lý Admin

        #region Danh mục

        public const string Pages_DanhMuc = "Pages.DanhMuc";
        public const string Pages_DanhMuc_QuocGia = "Pages.DanhMuc.QuocGia";
        public const string Pages_DanhMuc_Tinh = "Pages.DanhMuc.Tinh";
        public const string Pages_DanhMuc_Huyen = "Pages.DanhMuc.Huyen";
        public const string Pages_DanhMuc_Xa = "Pages.DanhMuc.Xa";
        public const string Pages_DanhMuc_ChucDanh = "Pages.DanhMuc.ChucDanh";

        #endregion Danh mục

        #region Doanh nghiệp

        //doanhnghiep
        public const string Pages_QuanLyDoanhNghiep_SuaThongTinDoanhNghiep = "Pages.QuanLyDoanhNghiep.SuaThongTinDoanhNghiep";

        //quantri
        public const string Pages_QuanLyDoanhNghiep = "Pages.QuanLyDoanhNghiep";

        public const string Pages_DanhMuc_LoaiHinhDoanhNghiep = "Pages.DanhMuc.LoaiHinhDoanhNghiep";
        public const string Pages_QuanLyDoanhNghiep_DanhSachDoanhNghiep = "Pages.QuanLyDoanhNghiep.DanhSachDoanhNghiep";
        public const string Pages_QuanLyDoanhNghiep_DanhSachConDau = "Pages.QuanLyDoanhNghiep.DanhSachConDau";

        #endregion Doanh nghiệp

        #region Quản lý Thanh toán

        public const string Pages_ThanhToan = "Pages.ThanhToan";
        public const string Pages_ThanhToan_XacNhanThanhToan = "Pages.ThanhToan.XacNhanThanhToan";
        public const string Pages_ThanhToan_HoaDonDienTu = "Pages.ThanhToan.HoaDonDienTu";

        #endregion Quản lý Thanh toán

        #region Lựa chọn thủ tục

        public const string Pages_DMThuTuc = "Pages.DMThuTuc";
        public const string Pages_DanhMuc_ThuTuc = "Pages.DanhMuc.ThuTuc";
        public const string Pages_DanhMuc_LoaiHoSo = "Pages.DanhMuc.LoaiHoSo";
        public const string Pages_DMThuTuc_LuaChonThuTuc = "Pages.DMThuTuc.LuaChonThuTuc";

        #endregion Lựa chọn thủ tục

        #region Quản lý thủ tục

        #region Thủ tục 99

        public const string Pages_ThuTuc99 = "Pages.ThuTuc99";

        #region Role Common

        //doanh nghiep
        public const string Pages_ThuTuc99_DangKyHoSo = "Pages.ThuTuc99.DangKyHoSo";

        //ke toan - dùng chung phần 'Quản Lý Thanh Toán'
        //public const string Pages_ThuTuc99_KeToanXacNhan = "Pages.ThuTuc99.KeToanXacNhan";

        //mot cua ra soat
        public const string Pages_ThuTuc99_MotCuaRaSoat = "Pages.ThuTuc99.MotCuaRaSoat";

        //mot cua phan cong
        public const string Pages_ThuTuc99_MotCuaPhanCong = "Pages.ThuTuc99.MotCuaPhanCong";

        //phong ban phan cong
        public const string Pages_ThuTuc99_PhongBanPhanCong = "Pages.ThuTuc99.PhongBanPhanCong";

        //truongphong
        public const string Pages_ThuTuc99_TruongPhongDuyet = "Pages.ThuTuc99.TruongPhongDuyet";

        //lanhdaocuc
        public const string Pages_ThuTuc99_LanhDaoCucDuyet = "Pages.ThuTuc99.LanhDaoCucDuyet";

        //lanhdaobo
        public const string Pages_ThuTuc99_LanhDaoBoDuyet = "Pages.ThuTuc99.LanhDaoBoDuyet";

        //vanthu
        public const string Pages_ThuTuc99_VanThuDuyet = "Pages.ThuTuc99.VanThuDuyet";

        //tracuuhoso
        public const string Pages_ThuTuc99_TraCuuHoSo = "Pages.ThuTuc99.TraCuuHoSo";

        #endregion Role Common

        //chuyen vien
        public const string Pages_ThuTuc99_ThamDinhHoSo = "Pages.ThuTuc99.ThamDinhHoSo";

        //pho phong
        public const string Pages_ThuTuc99_PhoPhongDuyet = "Pages.ThuTuc99.PhoPhongDuyet";

        #endregion Thủ tục 99

        #region Thủ tục 98

        public const string Pages_ThuTuc98 = "Pages.ThuTuc98";

        //doanh nghiep
        public const string Pages_ThuTuc98_DangKyHoSo = "Pages.ThuTuc98.DangKyHoSo";

        //mot cua ra soat
        public const string Pages_ThuTuc98_MotCuaRaSoat = "Pages.ThuTuc98.MotCuaRaSoat";

        //truongphong
        public const string Pages_ThuTuc98_TruongPhongDuyet = "Pages.ThuTuc98.TruongPhongDuyet";

        //tracuuhoso
        public const string Pages_ThuTuc98_TraCuuHoSo = "Pages.ThuTuc98.TraCuuHoSo";


        #endregion


        #region Thủ tục 37

        public const string Pages_ThuTuc37 = "Pages.ThuTuc37";

        #region Role Common

        //doanh nghiep
        public const string Pages_ThuTuc37_DangKyHoSo = "Pages.ThuTuc37.DangKyHoSo";

        //ke toan - dùng chung phần 'Quản Lý Thanh Toán'
        //public const string Pages_ThuTuc10_KeToanXacNhan = "Pages.ThuTuc10.KeToanXacNhan";

        //mot cua ra soat
        public const string Pages_ThuTuc37_MotCuaRaSoat = "Pages.ThuTuc37.MotCuaRaSoat";

        // kế toán xác nhận
        public const string Pages_ThuTuc37_KeToanXacNhan = "Pages.ThuTuc37.KeToanXacNhan";

        //mot cua phan cong
        public const string Pages_ThuTuc37_MotCuaPhanCong = "Pages.ThuTuc37.MotCuaPhanCong";

        //phong ban phan cong
        public const string Pages_ThuTuc37_PhongBanPhanCong = "Pages.ThuTuc37.PhongBanPhanCong";

        //chuyen vien
        public const string Pages_ThuTuc37_ThamXetHoSo = "Pages.ThuTuc37.ThamXetHoSo";
        public const string Pages_ThuTuc37_ThamDinhHoSo = "Pages.ThuTuc37.ThamDinhHoSo";
        public const string Pages_ThuTuc37_TongHopThamDinh = "Pages.ThuTuc37.TongHopThamDinh";

        //truongphong
        public const string Pages_ThuTuc37_TruongPhongDuyet = "Pages.ThuTuc37.TruongPhongDuyet";
        public const string Pages_ThuTuc37_TruongPhongDuyetThamDinh = "Pages.ThuTuc37.TruongPhongDuyetThamDinh";

        //lanhdaocuc
        public const string Pages_ThuTuc37_LanhDaoCucDuyet = "Pages.ThuTuc37.LanhDaoCucDuyet";
        public const string Pages_ThuTuc37_LanhDaoCucDuyetThamDinh = "Pages.ThuTuc37.LanhDaoCucDuyetThamDinh";

        //lanhdaobo
        public const string Pages_ThuTuc37_LanhDaoBoDuyet = "Pages.ThuTuc37.LanhDaoBoDuyet";

        //vanthu
        public const string Pages_ThuTuc37_VanThuDuyet = "Pages.ThuTuc37.VanThuDuyet";
        public const string Pages_ThuTuc37_VanThuDuyetThamDinh = "Pages.ThuTuc37.VanThuDuyetThamDinh";

        //tracuuhoso
        public const string Pages_ThuTuc37_TraCuuHoSo = "Pages.ThuTuc37.TraCuuHoSo";

        #endregion Role Common



        //pho phong
        public const string Pages_ThuTuc37_PhoPhongDuyet = "Pages.ThuTuc37.PhoPhongDuyet";

        #endregion Thủ tục 37



        #endregion Quản lý thủ tục

        #region Thống kê & báo cáo

        public const string Pages_ThongKeBaoCao = "Pages.ThongKeBaoCao";
        public const string Pages_ThongKeBaoCao_LanhDaoCuc = "Pages.ThongKeBaoCao.LanhDaoCuc";
        public const string Pages_ThongKeBaoCao_VanThu = "Pages.ThongKeBaoCao.VanThu";
        public const string Pages_ThongKeBaoCao_KeToan = "Pages.ThongKeBaoCao.KeToan";
        public const string Pages_ThongKeBaoCao_MotCua = "Pages.ThongKeBaoCao.MotCua";

        #endregion Thống kê & báo cáo

        #region Quản lý tài khoản

        public const string Pages_QuanLyTaiKhoan = "Pages.QuanLyTaiKhoan";
        public const string Pages_DanhMuc_PhongBan = "Pages.DanhMuc.PhongBan";
        public const string Pages_DanhMuc_ChuKy = "Pages.DanhMuc.ChuKy";
        public const string Pages_TaiKhoanCanBoCuc = "Pages.TaiKhoanCanBoCuc";
        public const string Pages_TaiKhoanSoYTe = "Pages.TaiKhoanSoYTe";
        public const string Pages_TaiKhoanChuyenGia = "Pages.TaiKhoanChuyenGia";

        #endregion Quản lý tài khoản

        #region Thiết lập chung

        public const string Pages_ThietLap = "Pages.ThietLap";
        public const string Pages_ThietLap_LichLamViec = "Pages.DanhMuc.LichLamViec";
        public const string Pages_ThietLap_LoaiBienBanThamDinh = "Pages.ThietLap.LoaiBienBanThamDinh";
        public const string Pages_ThietLap_DonViChuyenGia = "Pages.ThietLap.DonViChuyenGia";
        public const string Pages_ThietLap_PhanLoaiHoSo = "Pages.ThietLap.PhanLoaiHoSo";
        public const string Pages_ThietLap_TieuChiThamDinh = "Pages.ThietLap.TieuChiThamDinh";
        public const string Pages_ThietLap_TieuChiThamDinh_LyDo = "Pages.ThietLap.TieuChiThamDinh_LyDo";

        #endregion Thiết lập chung

        #region "Website"

        public const string Pages_Website = "Pages.Website";

        public const string Pages_Website_BoThuTuc = "Pages.Website.BoThuTuc";
        public const string Pages_Website_ThongBao = "Pages.Website.ThongBao";
        public const string Pages_Website_LienHe = "Pages.Website.LienHe";
        public const string Pages_Website_CauHinhChung = "Pages.Website.CauHinhChung";

        #endregion "Website"
        
        #region Hướng Dẫn Sử Dụng

        public const string Pages_HuongDanSuDung_View = "Pages.HuongDanSuDung.View";
        public const string Pages_HuongDanSuDung = "Pages.HuongDanSuDung";
        public const string Pages_HuongDanSuDung_DanhMuc = "Pages.HuongDanSuDung.DanhMuc";
        public const string Pages_HuongDanSuDung_BaiViet = "Pages.HuongDanSuDung.BaiViet";
        #endregion Hướng Dẫn Sử Dụng

        #region Quản trị dự án

        public const string Pages_QuanTriDuAn_View = "Pages.QuanTriDuAn.View";
        public const string Pages_QuanTriDuAn = "Pages.QuanTriDuAn";
        public const string Pages_QuanTriDuAn_DanhMuc = "Pages.QuanTriDuAn.DanhMuc";
        public const string Pages_QuanTriDuAn_BaiViet = "Pages.QuanTriDuAn.BaiViet";
        #endregion

    }
}