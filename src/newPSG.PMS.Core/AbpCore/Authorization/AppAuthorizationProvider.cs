using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace newPSG.PMS.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            #region Hệ Thống Zero

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));

            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));

            //TENANT-SPECIFIC PERMISSIONS

            pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);

            #endregion Hệ Thống Zero

            //CUSTOM PERMISSIONS

            #region Quản lý Admin

            //Tenant
            administration.CreateChildPermission(AppPermissions.Pages_Tenant_QuanLyAdmin, L("QuanLyAdmin"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Quản lý Admin

            #region Danh mục

            var danhmuc = pages.CreateChildPermission(AppPermissions.Pages_DanhMuc, L("DanhMuc"), multiTenancySides: MultiTenancySides.Tenant); //NO
            danhmuc.CreateChildPermission(AppPermissions.Pages_DanhMuc_QuocGia, L("QuocGia"), multiTenancySides: MultiTenancySides.Tenant);
            danhmuc.CreateChildPermission(AppPermissions.Pages_DanhMuc_Tinh, L("Tinh"), multiTenancySides: MultiTenancySides.Tenant);
            danhmuc.CreateChildPermission(AppPermissions.Pages_DanhMuc_Huyen, L("Huyen"), multiTenancySides: MultiTenancySides.Tenant);
            danhmuc.CreateChildPermission(AppPermissions.Pages_DanhMuc_Xa, L("Xa"), multiTenancySides: MultiTenancySides.Tenant);
            danhmuc.CreateChildPermission(AppPermissions.Pages_DanhMuc_ChucDanh, L("ChucDanh"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Danh mục

            #region Doanh nghiệp

            var quanlydoanhnghiep = pages.CreateChildPermission(AppPermissions.Pages_QuanLyDoanhNghiep, L("QuanLyDoanhNghiep"), multiTenancySides: MultiTenancySides.Tenant); //NO
            quanlydoanhnghiep.CreateChildPermission(AppPermissions.Pages_DanhMuc_LoaiHinhDoanhNghiep, L("LoaiHinhDoanhNghiep"), multiTenancySides: MultiTenancySides.Tenant);
            quanlydoanhnghiep.CreateChildPermission(AppPermissions.Pages_QuanLyDoanhNghiep_DanhSachDoanhNghiep, L("DanhSachDoanhNghiep"), multiTenancySides: MultiTenancySides.Tenant);
            quanlydoanhnghiep.CreateChildPermission(AppPermissions.Pages_QuanLyDoanhNghiep_DanhSachConDau, L("DanhSachConDau"), multiTenancySides: MultiTenancySides.Tenant);
            quanlydoanhnghiep.CreateChildPermission(AppPermissions.Pages_QuanLyDoanhNghiep_SuaThongTinDoanhNghiep, L("SuaThongTinDoanhNghiep"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Doanh nghiệp

            #region Quản lý Thanh toán

            var quanlythanhtoan = pages.CreateChildPermission(AppPermissions.Pages_ThanhToan, L("QuanLyThanhToan"), multiTenancySides: MultiTenancySides.Tenant);
            quanlythanhtoan.CreateChildPermission(AppPermissions.Pages_ThanhToan_XacNhanThanhToan, L("XacNhanThanhToan"), multiTenancySides: MultiTenancySides.Tenant);
            quanlythanhtoan.CreateChildPermission(AppPermissions.Pages_ThanhToan_HoaDonDienTu, L("HoaDonDienTu"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Quản lý Thanh toán

            #region Lựa chọn thủ tục

            var tt = pages.CreateChildPermission(AppPermissions.Pages_DMThuTuc, L("ThuTucHanhChinh"), multiTenancySides: MultiTenancySides.Tenant); //NO
            tt.CreateChildPermission(AppPermissions.Pages_DanhMuc_ThuTuc, L("DanhMucThuTuc"), multiTenancySides: MultiTenancySides.Tenant);
            tt.CreateChildPermission(AppPermissions.Pages_DanhMuc_LoaiHoSo, L("LoaiHoSo"), multiTenancySides: MultiTenancySides.Tenant);
            tt.CreateChildPermission(AppPermissions.Pages_DMThuTuc_LuaChonThuTuc, L("LuaChonThuTuc"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Lựa chọn thủ tục

            #region Thủ tục hành chính

            #region Thủ tục 99

            #region Role Common

            var thutuc_99 = pages.CreateChildPermission(AppPermissions.Pages_ThuTuc99, L("ThuTuc99"));//NO

            //doanh nghiep
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_DangKyHoSo, L("ThuTuc99_DangKyHoSo"));

            //mot cua ra soat
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_MotCuaRaSoat, L("ThuTuc99_MotCuaRaSoat"));

            //mot cua phan cong
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_MotCuaPhanCong, L("ThuTuc99_MotCuaPhanCong"));

            //phong ban phan cong
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_PhongBanPhanCong, L("ThuTuc99_PhongBanPhanCong"));

            //truongphong
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_TruongPhongDuyet, L("ThuTuc99_TruongPhongDuyet"));

            //lanhdaocuc
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_LanhDaoCucDuyet, L("ThuTuc99_LanhDaoCucDuyet"));

            //lanhdaobo
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_LanhDaoBoDuyet, L("ThuTuc99_LanhDaoBoDuyet"));

            //vanthu
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_VanThuDuyet, L("ThuTuc99_VanThuDuyet"));

            //tracuuhoso
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_TraCuuHoSo, L("ThuTuc99_TraCuuHoSo"));

            #endregion Role Common

            //chuyen vien
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_ThamDinhHoSo, L("ThuTuc99_ThamDinhHoSo"));

            //phophong
            thutuc_99.CreateChildPermission(AppPermissions.Pages_ThuTuc99_PhoPhongDuyet, L("ThuTuc99_PhoPhongDuyet"));

            #endregion Thủ tục 99

            #region  Thủ tục 98
            var thutuc_98 = pages.CreateChildPermission(AppPermissions.Pages_ThuTuc98, L("ThuTuc98"));//NO

            //doanh nghiep
            thutuc_98.CreateChildPermission(AppPermissions.Pages_ThuTuc98_DangKyHoSo, L("ThuTuc98_DangKyHoSo"));

            //mot cua ra soat
            thutuc_98.CreateChildPermission(AppPermissions.Pages_ThuTuc98_MotCuaRaSoat, L("ThuTuc98_MotCuaRaSoat"));

            //truongphong
            thutuc_98.CreateChildPermission(AppPermissions.Pages_ThuTuc98_TruongPhongDuyet, L("ThuTuc98_TruongPhongDuyet"));

            //tracuuhoso
            thutuc_98.CreateChildPermission(AppPermissions.Pages_ThuTuc98_TraCuuHoSo, L("ThuTuc98_TraCuuHoSo"));

            #endregion

            #region Thủ tục 37

            #region Role Common

            var thutuc_37 = pages.CreateChildPermission(AppPermissions.Pages_ThuTuc37, L("ThuTuc37"));//NO

            //doanh nghiep
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_DangKyHoSo, L("ThuTuc37_DangKyHoSo"));

            //mot cua ra soat
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_MotCuaRaSoat, L("ThuTuc37_MotCuaRaSoat"));

            // kế toán xác nhận
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_KeToanXacNhan, L("ThuTuc37_XacNhanThanhToan"));

            //mot cua phan cong
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_MotCuaPhanCong, L("ThuTuc37_MotCuaPhanCong"));

            //phong ban phan cong
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_PhongBanPhanCong, L("ThuTuc37_PhongBanPhanCong"));

            //truongphong
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_TruongPhongDuyet, L("ThuTuc37_TruongPhongDuyet"));

            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_TruongPhongDuyetThamDinh, L("ThuTuc37_TruongPhongDuyetThamDinh"));

            //lanhdaocuc
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_LanhDaoCucDuyet, L("ThuTuc37_LanhDaoCucDuyet"));
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_LanhDaoCucDuyetThamDinh, L("ThuTuc37_LanhDaoCucDuyetThamDinh"));

            //lanhdaobo
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_LanhDaoBoDuyet, L("ThuTuc37_LanhDaoBoDuyet"));

            //vanthu
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_VanThuDuyet, L("ThuTuc37_VanThuDuyet"));
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_VanThuDuyetThamDinh, L("ThuTuc37_VanThuDuyetThamDinh"));

            //tracuuhoso
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_TraCuuHoSo, L("ThuTuc37_TraCuuHoSo"));

            #endregion Role Common

            //chuyen vien
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_ThamXetHoSo, L("ThuTuc37_ThamXetHoSo"));
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_ThamDinhHoSo, L("ThuTuc37_ThamDinhHoSo"));
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_TongHopThamDinh, L("ThuTuc37_TongHopThamDinh"));

            //phophong
            thutuc_37.CreateChildPermission(AppPermissions.Pages_ThuTuc37_PhoPhongDuyet, L("ThuTuc37_PhoPhongDuyet"));

            #endregion Thủ tục 37

            #endregion Thủ tục hành chính

            #region Thống kê & báo cáo

            var thongke = pages.CreateChildPermission(AppPermissions.Pages_ThongKeBaoCao, L("ThongKeBaoCao"), multiTenancySides: MultiTenancySides.Tenant); //NO
            thongke.CreateChildPermission(AppPermissions.Pages_ThongKeBaoCao_LanhDaoCuc, L("ThongKeBaoCao_LanhDaoCuc"), multiTenancySides: MultiTenancySides.Tenant);
            thongke.CreateChildPermission(AppPermissions.Pages_ThongKeBaoCao_VanThu, L("ThongKeBaoCao_VanThu"), multiTenancySides: MultiTenancySides.Tenant);
            thongke.CreateChildPermission(AppPermissions.Pages_ThongKeBaoCao_KeToan, L("ThongKeBaoCao_KeToan"), multiTenancySides: MultiTenancySides.Tenant);
            thongke.CreateChildPermission(AppPermissions.Pages_ThongKeBaoCao_MotCua, L("ThongKeBaoCao_MotCua"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Thống kê & báo cáo

            #region Hướng dẫn sử dụng

            var huongDanView = pages.CreateChildPermission(AppPermissions.Pages_HuongDanSuDung_View, L("Pages_HuongDanSuDung_View"));
            var huongDan = pages.CreateChildPermission(AppPermissions.Pages_HuongDanSuDung, L("Pages_HuongDanSuDung"));
            huongDan.CreateChildPermission(AppPermissions.Pages_HuongDanSuDung_DanhMuc, L("Pages_HuongDanSuDung_DanhMuc"));
            huongDan.CreateChildPermission(AppPermissions.Pages_HuongDanSuDung_BaiViet, L("Pages_HuongDanSuDung_BaiViet"));
            #endregion Hướng dẫn sử dụng

            #region Quản lý tài khoản

            var quanlytk = pages.CreateChildPermission(AppPermissions.Pages_QuanLyTaiKhoan, L("QuanLyTaiKhoan"), multiTenancySides: MultiTenancySides.Tenant);
            quanlytk.CreateChildPermission(AppPermissions.Pages_DanhMuc_PhongBan, L("QuanLyPhongBan"), multiTenancySides: MultiTenancySides.Tenant);
            quanlytk.CreateChildPermission(AppPermissions.Pages_DanhMuc_ChuKy, L("ChuKy"), multiTenancySides: MultiTenancySides.Tenant);
            quanlytk.CreateChildPermission(AppPermissions.Pages_TaiKhoanCanBoCuc, L("TaiKhoanCanBoCuc"), multiTenancySides: MultiTenancySides.Tenant);
            quanlytk.CreateChildPermission(AppPermissions.Pages_TaiKhoanSoYTe, L("TaiKhoanSoYTe"), multiTenancySides: MultiTenancySides.Tenant);
            quanlytk.CreateChildPermission(AppPermissions.Pages_TaiKhoanChuyenGia, L("TaiKhoanChuyenGia"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Quản lý tài khoản

            #region Thiết lập chung

            var thietlap = pages.CreateChildPermission(AppPermissions.Pages_ThietLap, L("ThietLap"), multiTenancySides: MultiTenancySides.Tenant); //NO
            thietlap.CreateChildPermission(AppPermissions.Pages_ThietLap_LichLamViec, L("QuanLyLichNghiLe"), multiTenancySides: MultiTenancySides.Tenant);
            thietlap.CreateChildPermission(AppPermissions.Pages_ThietLap_LoaiBienBanThamDinh, L("ThietLap_LoaiBienBanThamDinh"), multiTenancySides: MultiTenancySides.Tenant);
            thietlap.CreateChildPermission(AppPermissions.Pages_ThietLap_DonViChuyenGia, L("ThietLap_DonViChuyenGia"), multiTenancySides: MultiTenancySides.Tenant);
            thietlap.CreateChildPermission(AppPermissions.Pages_ThietLap_PhanLoaiHoSo, L("ThietLap_PhanLoaiHoSo"), multiTenancySides: MultiTenancySides.Tenant);
            thietlap.CreateChildPermission(AppPermissions.Pages_ThietLap_TieuChiThamDinh, L("ThietLap_TieuChiThamDinh"), multiTenancySides: MultiTenancySides.Tenant);
            thietlap.CreateChildPermission(AppPermissions.Pages_ThietLap_TieuChiThamDinh_LyDo, L("ThietLap_TieuChiThamDinh_LyDo"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion Thiết lập chung

            #region "Website"

            var website = pages.CreateChildPermission(AppPermissions.Pages_Website, L("Pages_Website"));
            website.CreateChildPermission(AppPermissions.Pages_Website_BoThuTuc, L("Pages_Website_BoThuTuc"));
            website.CreateChildPermission(AppPermissions.Pages_Website_ThongBao, L("Pages_Website_ThongBao"));
            website.CreateChildPermission(AppPermissions.Pages_Website_LienHe, L("Pages_Website_LienHe"));
            website.CreateChildPermission(AppPermissions.Pages_Website_CauHinhChung, L("Pages_Website_CauHinhChung"));

            #endregion "Website"

            #region Quản trị DA

            var quanTriDA = pages.CreateChildPermission(AppPermissions.Pages_QuanTriDuAn, L("Pages_QuanTriDuAn"));
            quanTriDA.CreateChildPermission(AppPermissions.Pages_QuanTriDuAn_DanhMuc, L("Pages_QuanTriDuAn_DanhMuc"));
            quanTriDA.CreateChildPermission(AppPermissions.Pages_QuanTriDuAn_BaiViet, L("Pages_QuanTriDuAn_BaiViet")); 
            quanTriDA.CreateChildPermission(AppPermissions.Pages_QuanTriDuAn_View, L("Pages_QuanTriDuAn_View"));
            #endregion
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PMSConsts.LocalizationSourceName);
        }
    }
}