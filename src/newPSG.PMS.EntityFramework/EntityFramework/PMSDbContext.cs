using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Chat;
using newPSG.PMS.Friendships;
using newPSG.PMS.MultiTenancy;
using newPSG.PMS.Storage;
using newPSG.PMS.EntityDB;
using Oracle.ManagedDataAccess.EntityFramework;
using Abp.Notifications;
using Abp.Authorization.Users;
using Abp.Localization;
using Abp.BackgroundJobs;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using EntityFramework.Functions;
using System.Collections.Generic;

namespace newPSG.PMS.EntityFramework
{
    /* Constructors of this DbContext is important and each one has it's own use case.
     * - Default constructor is used by EF tooling on design time.
     * - constructor(nameOrConnectionString) is used by ABP on runtime.
     * - constructor(existingConnection) is used by unit tests.
     * - constructor(existingConnection,contextOwnsConnection) can be used by ABP if DbContextEfTransactionStrategy is used.
     * See http://www.aspnetboilerplate.com/Pages/Documents/EntityFramework-Integration for more.
     */

    public class PMSDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        /* Define an IDbSet for each entity of the application */

        public virtual IDbSet<BinaryObject> BinaryObjects { get; set; }
        public virtual IDbSet<Friendship> Friendships { get; set; }
        public virtual IDbSet<ChatMessage> ChatMessages { get; set; }

        /**Application IDBset**/

        #region Danh mục

        public virtual IDbSet<ChucVu> ChucVu { get; set; }
        public virtual IDbSet<ChuKy> ChuKy { get; set; }
        public virtual IDbSet<QuocGia> QuocGia { get; set; }
        public virtual IDbSet<Tinh> Tinh { get; set; }
        public virtual IDbSet<Huyen> Huyen { get; set; }
        public virtual IDbSet<Xa> Xa { get; set; }
        public virtual IDbSet<LichLamViec> LichLamViec { get; set; }
        public virtual IDbSet<NgayNghi> NgayNghi { get; set; }
        public virtual IDbSet<LoaiHinhDoanhNghiep> LoaiHinhDoanhNghiep { get; set; }
        public virtual IDbSet<HCCSetting> HCCSetting { get; set; }
        public virtual IDbSet<CuaKhau> CuaKhau { get; set; }
        public virtual IDbSet<LoaiFile> LoaiFile { get; set; }

        #endregion Danh mục

        #region Kho ATTP-VFA

        //public virtual IDbSet<VFADangKyCongBo> VFADangKyCongBo { get; set; }
        //public virtual IDbSet<VFA_CoSoDuDieuKien> VFA_CoSoDuDieuKien { get; set; }
        //public virtual IDbSet<VFA_DangKyQuangCao> VFA_DangKyQuangCao { get; set; }
        //public virtual IDbSet<VFA_TuCongBo> VFA_TuCongBo { get; set; }

        #endregion Kho ATTP-VFA

        #region Thiết lập chung

        public virtual IDbSet<DonViChuyenGia> DonViChuyenGia { get; set; }
        public virtual IDbSet<LoaiBienBanThamDinh> LoaiBienBanThamDinh { get; set; }
        public virtual IDbSet<PhanLoaiHoSo> PhanLoaiHoSo { get; set; }
        public virtual IDbSet<PhanLoaiHoSo_Filter> PhanLoaiHoSo_Filter { get; set; }
        public virtual IDbSet<PhanLoaiHoSo_PhanCong> PhanLoaiHoSo_PhanCong { get; set; }
        public virtual IDbSet<TieuChiThamDinh> TieuChiThamDinh { get; set; }
        public virtual IDbSet<TieuChiThamDinh_LyDo> TieuChiThamDinh_LyDo { get; set; }
        public virtual IDbSet<HoSoXuLy_PhanCongSoLuong> HoSoXuLy_PhanCongSoLuong { get; set; }

        #endregion Thiết lập chung

        #region Quản lý doanh nghiệp

        public virtual IDbSet<DoanhNghiep> DoanhNghiep { get; set; }
        public virtual IDbSet<ThongTinPhapLy> ThongTinPhapLy { get; set; }

        #endregion Quản lý doanh nghiệp

        #region Quản lý thanh toán

        public virtual IDbSet<ThanhToan> ThanhToan { get; set; }

        #endregion Quản lý thanh toán

        #region Cấu hình thủ tục

        public virtual IDbSet<PhongBan> PhongBan { get; set; }
        public virtual IDbSet<LoaiHoSo> LoaiHoSo { get; set; }
        public virtual IDbSet<LoaiHoSo_HanXuLy> LoaiHoSo_HanXuLy { get; set; }

        public virtual IDbSet<PhongBanLoaiHoSo> PhongBanLoaiHoSo { get; set; }
        public virtual IDbSet<PhongBanNhomSanPham> PhongBanNhomSanPham { get; set; }
        public virtual IDbSet<ThuTuc> ThuTuc { get; set; }
        public virtual IDbSet<NhomSanPham> NhomSanPham { get; set; }

        #endregion Cấu hình thủ tục

        #region LogUploadFile

        public virtual IDbSet<LogUploadFile> LogUploadFile { get; set; }
        public virtual IDbSet<LogFileKy> LogFileKy { get; set; }

        #endregion LogUploadFile

        #region -- Cấp số

        public virtual IDbSet<CapSoThuTuc> CapSoThuTuc { get; set; }
        public virtual IDbSet<CapSoCongVan> CapSoCongVan { get; set; }
        public virtual IDbSet<CapSoQuyetDinh> CapSoQuyetDinh { get; set; }

       
        #region Thủ tục TT37

        public virtual IDbSet<HoSo37> HoSo37 { get; set; }
        public virtual IDbSet<HoSoTepDinhKem37> HoSoTepDinhKem37 { get; set; }
        public virtual IDbSet<HoSoXuLy37> HoSoXuLy37 { get; set; }
        public virtual IDbSet<HoSoXuLyHistory37> HoSoXuLyHistory37 { get; set; }
        public virtual IDbSet<TT37_HoSoDoanThamDinh> TT37_HoSoDoanThamDinh { get; set; }
        public virtual IDbSet<BienBanThamDinh37> BienBanThamDinh37 { get; set; }
        public virtual IDbSet<TT37_HoSoPhamViHD> TT37_HoSoPhamViHD { get; set; }
        public virtual IDbSet<TT37_PhamViHoatDong> TT37_PhamViHoatDong { get; set; }

        #endregion Thủ tục TT37

        #region Thủ tục TT98

        public virtual IDbSet<HoSo98> HoSo98 { get; set; }
        public virtual IDbSet<HoSoTepDinhKem98> HoSoTepDinhKem98 { get; set; }
        public virtual IDbSet<HoSoXuLy98> HoSoXuLy98 { get; set; }
        public virtual IDbSet<HoSoXuLyHistory98> HoSoXuLyHistory98 { get; set; }

        #endregion Thủ tục TT99

        #region Thủ tục TT99

        public virtual IDbSet<HoSo99> HoSo99 { get; set; }
        public virtual IDbSet<HoSoTepDinhKem99> HoSoTepDinhKem99 { get; set; }
        public virtual IDbSet<HoSoXuLy99> HoSoXuLy99 { get; set; }
        public virtual IDbSet<HoSoXuLyHistory99> HoSoXuLyHistory99 { get; set; }
        public virtual IDbSet<BienBanThamDinh99> BienBanThamDinh99 { get; set; }

        #endregion Thủ tục TT99

        #endregion -- Cấp số

        #region Quản lý thanh tra

        public virtual IDbSet<KeHoachThanhTra> KeHoachThanhTra { get; set; }
        public virtual IDbSet<KeHoachThanhTraChiTiet> KeHoachThanhTraChiTiet { get; set; }
        public virtual IDbSet<KetQuaThanhTra> KetQuaThanhTra { get; set; }

        #endregion Quản lý thanh tra

        #region "Website"

        public virtual IDbSet<BoThuTuc> BoThuTuc { get; set; }
        public virtual IDbSet<ThongBao> ThongBao { get; set; }
        public virtual IDbSet<LienHe> LienHe { get; set; }
        public virtual IDbSet<CauHinhChung> CauHinhChung { get; set; }

        #endregion "Website"

        #region HuongDanSuDung

        public virtual IDbSet<Category> Category { get; set; }
        public virtual IDbSet<Article> Article { get; set; }
        #endregion

        public PMSDbContext()
            : base("Default")
        {
        }

        public PMSDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public PMSDbContext(DbConnection existingConnection)
           : base(existingConnection, false)
        {
        }

        public PMSDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Fix Error - Update DB
            //Database.SetInitializer<PMSDbContext>(null);

            base.OnModelCreating(modelBuilder);

            #region Function DB

            //Function
            modelBuilder.Conventions.Add(new FunctionConvention(typeof(DBFunctions)));

            #endregion Function DB
        }
    }
}