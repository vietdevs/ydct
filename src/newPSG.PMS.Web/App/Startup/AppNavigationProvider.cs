using Abp.Application.Navigation;
using Abp.Localization;
using newPSG.PMS.Authorization;
using newPSG.PMS.Web.Navigation;

namespace newPSG.PMS.Web.App.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See .cshtml and .js files under App/Main/views/layout/header to know how to render menu.
    /// </summary>
    public class AppNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {

            #region HUONG DAN SU DUNG

            var huongDanSuDungView = new MenuItemDefinition(
                     "-",
                     L("Pages_HuongDanSuDung_View"),
                     icon: "icms icms-huongdansudung",
                     url: "huongdansudung",
                     requiredPermissionName: AppPermissions.Pages_HuongDanSuDung_View
                 );

            var huongDanSuDung = new MenuItemDefinition(
                     "-",
                     L("Pages_HuongDanSuDung"),
                     icon: "icms icms-cauhinh",
                     requiredPermissionName: AppPermissions.Pages_HuongDanSuDung
                 ).AddItem(new MenuItemDefinition(
                     "-",
                     L("Pages_HuongDanSuDung_DanhMuc"),
                     url: "huongdansudung_chuyenmuc",
                     icon: "icon-info",
                      requiredPermissionName: AppPermissions.Pages_HuongDanSuDung_DanhMuc
                 )).AddItem(new MenuItemDefinition(
                     "-",
                     L("Pages_HuongDanSuDung_BaiViet"),
                     url: "huongdansudung_baiviet",
                     icon: "icon-info",
                      requiredPermissionName: AppPermissions.Pages_HuongDanSuDung_BaiViet
                 ));

            #endregion

            #region QUAN TRI DU AN

            var quanTriDuAn = new MenuItemDefinition(
                     "-",
                     L("Pages_QuanTriDuAn"),
                     icon: "icms icms-cauhinh",
                     requiredPermissionName: AppPermissions.Pages_QuanTriDuAn
                 ).AddItem(new MenuItemDefinition(
                     "-",
                     L("Pages_QuanTriDuAn_View"),
                     url: "quantriduan",
                     icon: "icon-info",
                      requiredPermissionName: AppPermissions.Pages_QuanTriDuAn_View
                 )).AddItem(new MenuItemDefinition(
                     "-",
                     L("Pages_QuanTriDuAn_DanhMuc"),
                     url: "quantriduan_chuyenmuc",
                     icon: "icon-info",
                      requiredPermissionName: AppPermissions.Pages_QuanTriDuAn_DanhMuc
                 )).AddItem(new MenuItemDefinition(
                     "-",
                     L("Pages_QuanTriDuAn_BaiViet"),
                     url: "quantriduan_baiviet",
                     icon: "icon-info",
                      requiredPermissionName: AppPermissions.Pages_QuanTriDuAn_BaiViet
                 ));

            #endregion

            #region Rezo Host

            context.Manager.MainMenu
                .AddItem(new MenuItemDefinition(
                    PageNames.App.Host.Tenants,
                    L("Tenants"),
                    url: "host.tenants",
                    icon: "icon-globe",
                    requiredPermissionName: AppPermissions.Pages_Tenants
                    )
                ).AddItem(new MenuItemDefinition(
                    PageNames.App.Host.Editions,
                    L("Editions"),
                    url: "host.editions",
                    icon: "icon-grid",
                    requiredPermissionName: AppPermissions.Pages_Editions
                    )
                )
                .AddItem(new MenuItemDefinition(
                    PageNames.App.Tenant.Dashboard,
                    L("Dashboard"),
                    url: "tenant.dashboard",
                    icon: "icms icms-home",
                    requiredPermissionName: AppPermissions.Pages_Tenant_Dashboard
                    )
                )

            #endregion Rezo Host

            #region Quản lý doanh nghiệp

                .AddItem(new MenuItemDefinition(
                        "-",
                        L("SuaThongTinDoanhNghiep"),
                        url: "suathongtindoanhnghiep",
                        icon: "icms icms-thongtin",
                        requiredPermissionName: AppPermissions.Pages_QuanLyDoanhNghiep_SuaThongTinDoanhNghiep
                        )
                )
                .AddItem(new MenuItemDefinition(
                        "-",
                        L("QuanLyDoanhNghiep"),
                        icon: "icms icms-doanhnghiep",
                        requiredPermissionName: AppPermissions.Pages_QuanLyDoanhNghiep
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("LoaiHinhDoanhNghiep"),
                        url: "loaihinhdoanhnghiep",
                        icon: "fa fa-indent",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_LoaiHinhDoanhNghiep
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("DanhSachDoanhNghiep"),
                        url: "danhsachdoanhnghiep",
                        icon: "icon-folder-alt",
                        requiredPermissionName: AppPermissions.Pages_QuanLyDoanhNghiep_DanhSachDoanhNghiep
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("DanhSachConDau"),
                        url: "danhsachcondau",
                        icon: "icon-folder-alt",
                        requiredPermissionName: AppPermissions.Pages_QuanLyDoanhNghiep_DanhSachConDau
                        )
                    ))

            #endregion Quản lý doanh nghiệp

            #region Quản lý thanh toán

                    .AddItem(new MenuItemDefinition(
                        "-",
                        L("QuanLyThanhToan"),
                        icon: "icms icms-thanhtoan",
                        requiredPermissionName: AppPermissions.Pages_ThanhToan
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("XacNhanThanhToan"),
                        url: "xacnhanthanhtoanchung",
                        icon: "fa fa-pencil",
                        requiredPermissionName: AppPermissions.Pages_ThanhToan_XacNhanThanhToan
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("HoaDonDienTu"),
                        url: "hoadondientu",
                        icon: "fa fa-pencil",
                        requiredPermissionName: AppPermissions.Pages_ThanhToan_HoaDonDienTu
                        )
                    ))

            #endregion Quản lý thanh toán

            #region Thủ tục hành chính

                .AddItem(new MenuItemDefinition(
                        "-",
                        L("ThuTucHanhChinh"),
                        icon: "icms icms-winner",
                         requiredPermissionName: AppPermissions.Pages_DMThuTuc
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("DanhMucThuTuc"),
                        url: "danhsachthutuc",
                        icon: "icon-list",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_ThuTuc
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("LoaiHoSo"),
                        url: "loaihoso",
                        icon: "fa fa-newspaper-o",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_LoaiHoSo
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("DanhSachThuTuc"),
                        url: "luachonthutuc",
                        icon: "fa fa-newspaper-o",
                         requiredPermissionName: AppPermissions.Pages_DMThuTuc_LuaChonThuTuc
                        )
                    )

            #region ThuTuc99

                    .AddItem(new MenuItemDefinition(
                            "-",
                            L("ThuTuc99"),
                            icon: "icon-notebook"
                            , requiredPermissionName: AppPermissions.Pages_ThuTuc99
                        )

            #region Role Common

                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_DangKyHoSo"),
                                url: "tt99/dangkyhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_DangKyHoSo
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_XacNhanThanhToan"),
                                url: "tt99/xacnhanthanhtoan",
                                icon: "icon-wallet",
                                requiredPermissionName: AppPermissions.Pages_ThanhToan_XacNhanThanhToan
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_MotCuaRaSoat"),
                                url: "tt99/rasoathoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_MotCuaRaSoat
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_MotCuaPhanCong"),
                                url: "tt99/motcuaphancong",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_MotCuaPhanCong
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_PhongBanPhanCong"),
                                url: "tt99/phongbanphancong",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_PhongBanPhanCong
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_TruongPhongDuyet"),
                                url: "tt99/truongphongduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_TruongPhongDuyet
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_LanhDaoCucDuyet"),
                                url: "tt99/lanhdaocucduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_LanhDaoCucDuyet
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_LanhDaoBoDuyet"),
                                url: "tt99/lanhdaoboduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_LanhDaoBoDuyet
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_VanThuDuyet"),
                                url: "tt99/vanthuduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_VanThuDuyet
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_TraCuuHoSo"),
                                url: "tt99/tracuuhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_TraCuuHoSo
                                )
                            )

            #endregion Role Common

                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_ThamDinhHoSo"),
                                url: "tt99/thamdinhhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_ThamDinhHoSo
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc99_PhoPhongDuyet"),
                                url: "tt99/phophongduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc99_PhoPhongDuyet
                                )
                            )

                            )

            #endregion ThuTuc99

            #region ThuTuc98
                    .AddItem(new MenuItemDefinition(
                            "-",
                            L("ThuTuc98"),
                            icon: "icon-notebook"
                            , requiredPermissionName: AppPermissions.Pages_ThuTuc98
                        ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc98_DangKyHoSo"),
                                url: "tt98/dangkyhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc98_DangKyHoSo
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc98_MotCuaRaSoat"),
                                url: "tt98/rasoathoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc98_MotCuaRaSoat
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc98_TruongPhongDuyet"),
                                url: "tt98/truongphongduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc98_TruongPhongDuyet
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc98_TraCuuHoSo"),
                                url: "tt98/tracuuhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc98_TraCuuHoSo
                                )
                            ))
            #endregion


            #region ThuTuc37

                    .AddItem(new MenuItemDefinition(
                            "-",
                            L("ThuTuc37"),
                            icon: "icon-notebook"
                            , requiredPermissionName: AppPermissions.Pages_ThuTuc37
                        )

            #region Role Common

                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_DangKyHoSo"),
                                url: "tt37/dangkyhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_DangKyHoSo
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_XacNhanThanhToan"),
                                url: "tt37/xacnhanthanhtoan",
                                icon: "icon-wallet",
                                requiredPermissionName: AppPermissions.Pages_ThanhToan_XacNhanThanhToan
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_MotCuaRaSoat"),
                                url: "tt37/rasoathoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_MotCuaRaSoat
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_MotCuaPhanCong"),
                                url: "tt37/motcuaphancong",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_MotCuaPhanCong
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_PhongBanPhanCong"),
                                url: "tt37/phongbanphancong",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_PhongBanPhanCong
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_ThamXetHoSo"),
                                url: "tt37/thamxethoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_ThamXetHoSo
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_ThamDinhHoSo"),
                                url: "tt37/thamdinhhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_ThamDinhHoSo
                                )
                            )
                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_TongHopThamDinh"),
                                url: "tt37/tonghopthamdinh",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_TongHopThamDinh
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_TruongPhongDuyet"),
                                url: "tt37/truongphongduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_TruongPhongDuyet
                                )
                            )
                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_TruongPhongDuyetThamDinh"),
                                url: "tt37/thamdinhtruongphongduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_TruongPhongDuyetThamDinh
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_LanhDaoCucDuyet"),
                                url: "tt37/lanhdaocucduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_LanhDaoCucDuyet
                                )
                            )
                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_LanhDaoCucDuyetThamDinh"),
                                url: "tt37/thamdinhlanhdaocucduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_LanhDaoCucDuyetThamDinh
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_LanhDaoBoDuyet"),
                                url: "tt37/lanhdaoboduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_LanhDaoBoDuyet
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_VanThuDuyet"),
                                url: "tt37/vanthuduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_VanThuDuyet
                                )
                            )
                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_VanThuDuyetThamDinh"),
                                url: "tt37/thamdinhvanthuduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_VanThuDuyetThamDinh
                                )
                            ).AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_TraCuuHoSo"),
                                url: "tt37/tracuuhoso",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_TraCuuHoSo
                                )
                            )

            #endregion Role Common
                            .AddItem(new MenuItemDefinition(
                                "-",
                                L("ThuTuc37_PhoPhongDuyet"),
                                url: "tt37/phophongduyet",
                                icon: "icon-note",
                                requiredPermissionName: AppPermissions.Pages_ThuTuc37_PhoPhongDuyet
                                )
                            )

                            )

            #endregion ThuTuc37


            )

            #endregion Thủ tục hành chính

            #region Thống kê báo cáo

                .AddItem(new MenuItemDefinition(
                    "-",
                    L("ThongKeBaoCao"),
                    icon: "icms icms-thongkebaocao"
                ).AddItem(new MenuItemDefinition(
                        "-",
                        L("ThongKeBaoCao_LanhDaoCuc"),
                        url: "thongkebaocao",
                        icon: "fa fa-file-archive-o",
                        requiredPermissionName: AppPermissions.Pages_ThongKeBaoCao_LanhDaoCuc
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("ThongKeBaoCao_VanThu"),
                        url: "thongkebaocaovanthu",
                        icon: "fa fa-file-archive-o",
                        requiredPermissionName: AppPermissions.Pages_ThongKeBaoCao_VanThu
                        )
                     ).AddItem(new MenuItemDefinition(
                        "-",
                        L("ThongKeBaoCao_KeToan"),
                        url: "thongkebaocao",
                        icon: "fa fa-file-archive-o",
                        requiredPermissionName: AppPermissions.Pages_ThongKeBaoCao_KeToan
                        )
                     ).AddItem(new MenuItemDefinition(
                        "-",
                        L("ThongKeBaoCao_MotCua"),
                        url: "thongkebaocaomotcua",
                        icon: "fa fa-file-archive-o",
                        requiredPermissionName: AppPermissions.Pages_ThongKeBaoCao_MotCua
                        )
                     )
                 )

            #endregion Thống kê báo cáo

            #region Thiết lập chung

                    .AddItem(new MenuItemDefinition(
                            "-",
                            L("ThietLap"),
                            icon: "icms icms-thietlap",
                            requiredPermissionName: AppPermissions.Pages_ThietLap
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("QuanLyLichNghiLe"),
                            url: "lichlamviec",
                            icon: "icon-calendar",
                            requiredPermissionName: AppPermissions.Pages_ThietLap_LichLamViec
                            )
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("ThietLap_LoaiBienBanThamDinh"),
                            url: "loaibienbanthamdinh",
                            icon: "icon-calendar",
                            requiredPermissionName: AppPermissions.Pages_ThietLap_LoaiBienBanThamDinh
                            )
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("ThietLap_DonViChuyenGia"),
                            url: "donvichuyengia",
                            icon: "icon-calendar",
                            requiredPermissionName: AppPermissions.Pages_ThietLap_DonViChuyenGia
                            )
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("ThietLap_PhanLoaiHoSo"),
                            url: "phanloaihoso",
                            icon: "icon-calendar",
                            requiredPermissionName: AppPermissions.Pages_ThietLap_PhanLoaiHoSo
                            )
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("ThietLap_TieuChiThamDinh"),
                            url: "tieuchithamdinh",
                            icon: "icon-calendar",
                            requiredPermissionName: AppPermissions.Pages_ThietLap_TieuChiThamDinh
                            )
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("ThietLap_TieuChiThamDinh_LyDo"),
                            url: "tieuchithamdinhlydo",
                            icon: "icon-calendar",
                            requiredPermissionName: AppPermissions.Pages_ThietLap_TieuChiThamDinh_LyDo
                            )
                        )
                    )

            #endregion Thiết lập chung

            #region Quản lý tài khoản

                .AddItem(new MenuItemDefinition(
                    "-",
                    L("QuanLyTaiKhoan"),
                    icon: "icms icms-lock",
                    requiredPermissionName: AppPermissions.Pages_QuanLyTaiKhoan
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("QuanLyPhongBan"),
                        url: "phongban",
                        icon: "fa fa-object-group",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_PhongBan
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("ChuKy"),
                        url: "chuky",
                        icon: "fa fa-pencil",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_ChuKy
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("TaiKhoanCanBoCuc"),
                        icon: "icms users",
                        url: "canbocuc",
                        requiredPermissionName: AppPermissions.Pages_TaiKhoanCanBoCuc
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("TaiKhoanSoYTe"),
                        icon: "icms users",
                        url: "tk-soyte-vienkn",
                        requiredPermissionName: AppPermissions.Pages_TaiKhoanSoYTe
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("TaiKhoanChuyenGia"),
                        icon: "icms users",
                        url: "tk-chuyengia",
                        requiredPermissionName: AppPermissions.Pages_TaiKhoanSoYTe
                        )
                    )
               )

            #endregion Quản lý tài khoản

            #region Danh mục

                .AddItem(new MenuItemDefinition(
                        "-",
                        L("DanhMuc"),
                        icon: "icms icms-danhmuc",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("QuocGia"),
                        url: "quocgia",
                        icon: "icon-globe-alt",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_QuocGia
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("Tinh/TP"),
                        url: "tinh",
                        icon: "fa fa-institution",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_Tinh
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("Huyen"),
                        url: "huyen",
                        icon: "fa fa-map-o",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_Huyen
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("Xa"),
                        url: "xa",
                        icon: "fa fa-map-signs",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_Xa
                        )
                    ).AddItem(new MenuItemDefinition(
                        "-",
                        L("ChucDanh"),
                        url: "chucdanh",
                        icon: "icon-users",
                        requiredPermissionName: AppPermissions.Pages_DanhMuc_ChucDanh
                        )
                    ))

            #endregion Danh mục

            #region Rezo Tenant

                .AddItem(new MenuItemDefinition(
                    PageNames.App.Common.Administration,
                    L("Administration"),
                    icon: "icms icms-admin"
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Common.OrganizationUnits,
                        L("OrganizationUnits"),
                        url: "organizationUnits",
                        icon: "icon-layers",
                        requiredPermissionName: AppPermissions.Pages_Administration_OrganizationUnits
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Common.Roles,
                        L("Roles"),
                        url: "roles",
                        icon: "icon-briefcase",
                        requiredPermissionName: AppPermissions.Pages_Administration_Roles
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Common.Users,
                        L("Users"),
                        url: "users",
                        icon: "icon-users",
                        requiredPermissionName: AppPermissions.Pages_Administration_Users
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Common.Languages,
                        L("Languages"),
                        url: "languages",
                        icon: "fa fa-language",
                        requiredPermissionName: AppPermissions.Pages_Administration_Languages
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Common.AuditLogs,
                        L("AuditLogs"),
                        url: "auditLogs",
                        icon: "fa fa-history",
                        requiredPermissionName: AppPermissions.Pages_Administration_AuditLogs
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Host.Maintenance,
                        L("Maintenance"),
                        url: "host.maintenance",
                        icon: "icon-wrench",
                        requiredPermissionName: AppPermissions.Pages_Administration_Host_Maintenance
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Host.Settings,
                        L("Settings"),
                        url: "host.settings",
                        icon: "icon-settings",
                        requiredPermissionName: AppPermissions.Pages_Administration_Host_Settings
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        PageNames.App.Tenant.Settings,
                        L("Settings"),
                        url: "tenant.settings",
                        icon: "icon-settings",
                        requiredPermissionName: AppPermissions.Pages_Administration_Tenant_Settings
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                        "-",
                        L("QuanLyAdmin"),
                        icon: "fa fa-group",
                        url: "quanlyadmin",
                        requiredPermissionName: AppPermissions.Pages_Tenant_QuanLyAdmin
                        )
                    ))

            #endregion Rezo Tenant

            .AddItem(huongDanSuDung)
            .AddItem(huongDanSuDungView)
            .AddItem(quanTriDuAn)

            #region "Website"

                 .AddItem(new MenuItemDefinition(
                            "-",
                           L("Pages_Website"),
                                icon: "icms icms-danhmuc",
                            requiredPermissionName: AppPermissions.Pages_Website
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("Pages_Website_BoThuTuc"),
                            url: "bothutuc",
                            icon: "icon-direction",
                             requiredPermissionName: AppPermissions.Pages_Website_BoThuTuc
                            )
                        ).AddItem(new MenuItemDefinition(
                            "-",
                            L("Pages_Website_ThongBao"),
                            url: "thongbao",
                            icon: "icon-list",
                             requiredPermissionName: AppPermissions.Pages_Website_ThongBao
                            )
                        ).AddItem(new MenuItemDefinition(
                             "-",
                            L("Pages_Website_LienHe"),
                            url: "lienhe",
                            icon: "icon-calculator",
                             requiredPermissionName: AppPermissions.Pages_Website_LienHe
                            )
                        ).AddItem(new MenuItemDefinition(
                             "-",
                            L("Pages_Website_CauHinhChung"),
                            url: "cauhinhchung",
                            icon: "icon-calculator",
                             requiredPermissionName: AppPermissions.Pages_Website_CauHinhChung
                            )
                        )
                    )

            #endregion "Website"

            ;
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PMSConsts.LocalizationSourceName);
        }
    }
}