using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.Net.MimeTypes;
using newPSG.PMS.Sessions.Dto;
using newPSG.PMS.ThuTucCommon.Dto;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using static newPSG.PMS.AppQuerySql.WhereQueryFormCaseThuTuc;

namespace newPSG.PMS.Services
{
    public interface IThuTucReportAppService : IApplicationService
    {
        PagedResultDto<HoSoReportDto> GetTraCuuHoSo(HoSoReportInputDto input);
        PagedResultDto<ThanhToanDto> GetListThanhToanChuyenKhoan(HoSoReportInputDto input);
        PagedResultDto<ThanhToanDto> GetListThanhToanChuyenKhoan_TT05(HoSoReportInputDto input, string _maThuTuc);
        TotalLabelFormCase GetTotalFormCase(HoSoReportInputDto input);
        Dto.FileDto ExportExcelSoTheoDoi(HoSoReportInputDto input);
    }

    public class ThuTucReportAppService : PMSAppServiceBase, IThuTucReportAppService
    {
        private readonly IIocResolver _iocResolver;
        private readonly UserLoginInfoDto _userCurrent;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<ThuTuc> _thuTucRepos;
        private readonly ILichLamViecAppService _lichLamViecAppService;
        private readonly IAppFolders _appFolders;

        public ThuTucReportAppService(IAppFolders appFolders,
            IIocResolver iocResolver,
            IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
            IRepository<LoaiHoSo> loaiHoSoRepos,
            IRepository<DoanhNghiep, long> doanhNghiepRepos,
            IRepository<PhongBan> phongBanRepos,
            IRepository<User, long> userRepos,
            ILichLamViecAppService lichLamViecAppService,
            IRepository<ThuTuc> thuTucRepos
        )
        {
            _userCurrent = SessionCustom.UserCurrent;
            _iocResolver = iocResolver;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
            _phongBanRepos = phongBanRepos;
            _userRepos = userRepos;
            _lichLamViecAppService = lichLamViecAppService;
            _appFolders = appFolders;
            _thuTucRepos = thuTucRepos;

        }

        public PagedResultDto<HoSoReportDto> GetTraCuuHoSo(HoSoReportInputDto input)
        {
            int total = 0;
            var dataGrids = new List<HoSoReportDto>();
            try
            {
                var sqlUnionThuTuc = "";
                foreach (int tt in _userCurrent.ThuTucEnum)
                {
                    string maTT = tt < 10 ? $"0{tt}" : tt.ToString();
                    string thuTucQuery = $@" select {SelectStringSql(tt, input)} from dbo.TT{maTT}_HoSo hs
                    left join dbo.TT{maTT}_HoSoXuLy hsxl on hs.HoSoXuLyId_Active = hsxl.Id
                    where hs.IsDeleted = 0 
                    and hs.PId is null 
                    and  {WhereStringSql(tt, input)} ";
                    sqlUnionThuTuc = string.IsNullOrEmpty(sqlUnionThuTuc) ? thuTucQuery : $"{sqlUnionThuTuc} UNION ALL {thuTucQuery}";

                }
                string sqlQuery = $@"select TotalRows = Count(1) Over(),
                                        A.* from ( {sqlUnionThuTuc} ) as A
                                        order by A.CreationTime
                                        OFFSET (@skip) ROWS
	                                    FETCH NEXT @rows ROWS ONLY; ";

                using (var ctx = new AppDbContext())
                {
                    SqlParameter[] prm = ParmeterThuTucReport(input);
                    dataGrids = ctx.Database.SqlQuery<HoSoReportDto>(sqlQuery, prm).ToList();
                    total = dataGrids.Count > 0 ? dataGrids[0].TotalRows.Value : 0;
                }
                for (int idx = 0; idx < dataGrids.Count; idx++)
                {
                    var item = dataGrids[idx];
                    item.FormId = input.FormId;
                    item.FormCase = input.FormCase;
                    item = GetTrangThaiXuLyHoSo(item, input);
                    item = GetPhongBanPhanCong(item, input);

                    item.StrDonViXuLy = item.DonViXuLy != null ? CommonENum.GetEnumDescription((CommonENum.DON_VI_XU_LY)(int)item.DonViXuLy) : "";
                    item.StrDonViGui = item.DonViGui != null ? CommonENum.GetEnumDescription((CommonENum.DON_VI_XU_LY)(int)item.DonViGui) : "";

                    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        if (item.ChuyenVienThuLyId.HasValue)
                        {
                            var chuyenVienThuLyObj = _userRepos.FirstOrDefault(item.ChuyenVienThuLyId.Value);
                            item.ChuyenVienThuLyName = chuyenVienThuLyObj.Surname + " " + chuyenVienThuLyObj.Name;
                        }
                        if (item.ChuyenVienPhoiHopId.HasValue)
                        {
                            var chuyenVienPhoiHopObj = _userRepos.FirstOrDefault(item.ChuyenVienPhoiHopId.Value);
                            item.ChuyenVienPhoiHopName = chuyenVienPhoiHopObj.Surname + " " + chuyenVienPhoiHopObj.Name;
                        }
                        if (item.NguoiGuiId.HasValue)
                        {
                            var objUser = _userRepos.FirstOrDefault(item.NguoiGuiId.Value);
                            item.TenNguoiGui = objUser.Surname + " " + objUser.Name;
                        }
                        if (item.NguoiXuLyId.HasValue)
                        {
                            var objUser = _userRepos.FirstOrDefault(item.NguoiXuLyId.Value);
                            item.TenNguoiXuLy = objUser.Surname + " " + objUser.Name;
                        }
                    }
                    if (item.IsChiCuc != true)
                    {
                        item.TenChiCuc = "Cục Quản Lý Dược - Bộ Y Tế";
                    }

                    #region Số ngày quá hạn
                    item.SoNgayQuaHan = 0;
                    if (item.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT && item.TrangThaiHoSo != (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG && item.NgayHenTra != null)
                    {
                        DateTime ngay_begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        DateTime ngay_end = new DateTime(item.NgayHenTra.Value.Year, item.NgayHenTra.Value.Month, item.NgayHenTra.Value.Day);
                        if (ngay_begin > ngay_end)
                        {
                            TimeSpan Time = ngay_begin - ngay_end;
                            item.SoNgayQuaHan = Time.Days;
                            item.StrQuaHan = "Quá hạn " + item.SoNgayQuaHan + " ngày";
                        }
                        else
                        {
                            item.SoNgayQuaHan = _lichLamViecAppService.GetSoNgayLamViec(ngay_begin, ngay_end);
                            item.StrQuaHan = "Còn " + item.SoNgayQuaHan + " ngày làm việc";
                            item.SoNgayQuaHan = item.SoNgayQuaHan * (-1);
                        }
                    }
                    else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
                    {
                        item.StrQuaHan = "Đã trả công văn (TB)";
                    }
                    else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT)
                    {
                        item.StrQuaHan = "Xử lý xong";
                    }
                    #endregion

                    #region Các phòng ban có thể xử lý (một cửa phân công)
                    if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
                    {

                        var _listPhongBan = (from ploai in _phongBanLoaiHoSoRepos.GetAll()
                                             join pb in _phongBanRepos.GetAll() on ploai.PhongBanId equals pb.Id
                                             where ploai.LoaiHoSoId == item.LoaiHoSoId
                                             select new ItemDto<int>
                                             {
                                                 Id = pb.Id,
                                                 Name = pb.TenPhongBan,
                                                 Checked = false
                                             }).ToList();

                        if (item.PhongBanId.HasValue && item.PhongBanId > 0)
                        {
                            foreach (var phongban in _listPhongBan)
                            {
                                if (phongban.Id == item.PhongBanId)
                                {
                                    phongban.Checked = true;
                                }
                            }
                        }

                        item.ArrPhongBanXuLy = _listPhongBan;
                    }
                    #endregion

                    #region Chức năng trên grid

                    List<int> _arrChucNang = new List<int>();

                    //FORM 2
                    if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);
                        if (!item.TruongPhongId.HasValue)
                        {
                            _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.MOT_CUA_RA_SOAT_HO_SO);
                        }
                    }

                    //FORM 7
                    if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET)
                    {
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_HO_SO);
                        _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_BAN_DANG_KY);

                        if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.VAN_THU)
                        {
                            if (item.HoSoIsDat == true)
                            {
                                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DUYET);
                                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DONG_DAU);
                            }
                            else
                            {
                                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.XEM_CONG_VAN);
                                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DUYET);
                                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_DONG_DAU);
                                _arrChucNang.Add((int)CommonENum.FORM_FUNCTION.VAN_THU_BAO_CAO_CONG_VAN_CO_SAI_SOT);
                            }
                        }
                    }

                    item.ArrChucNang = _arrChucNang;
                    #endregion

                    dataGrids[idx] = item;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return new PagedResultDto<HoSoReportDto>(total, dataGrids);
        }

        public PagedResultDto<ThanhToanDto> GetListThanhToanChuyenKhoan(HoSoReportInputDto input)
        {
            string keyWord = Utility.StringExtensions.FomatAndKhongDau(input.Keyword);
            keyWord = string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord;

            var sqlUnionThuTuc = "";
            foreach (int tt in _userCurrent.ThuTucEnum)
            {
                string maTT = tt < 10 ? $"0{tt}" : tt.ToString();

                string thuTucQuery = $@"
                        select 
                        '{maTT}' as MaThuTuc,
                        hoso.Id as Id,
                        tt.DoanhNghiepId as DoanhNghiepId,
                        hoso.MaHoSo as MaHoSo,
                        hoso.SoDangKy as SoDangKy,
                        hoso.TenCoSo as TenSanPham,
                        hoso.LoaiHoSoId as LoaiHoSoId,
                        lhs.TenLoaiHoSo as StrLoaiHoSo,
                        tt.HoSoId as HoSoId,
                        tt.GhiChu as GhiChu,
                        tt.KenhThanhToan as KenhThanhToan,
                        tt.PhiDaNop as PhiDaNop,
                        tt.PhiXacNhan as PhiXacNhan,
                        tt.SoTaiKhoanNop as SoTaiKhoanNop,
                        tt.SoTaiKhoanHuong as SoTaiKhoanHuong,
                        tt.MaGiaoDich as MaGiaoDich,
                        tt.MaDonHang as MaDonHang,
                        tt.NgayGiaoDich as NgayGiaoDich,
                        tt.TrangThaiKeToan as TrangThaiKeToan,
                        tt.TrangThaiNganHang as TrangThaiNganHang,
                        dn.TenDoanhNghiep as TenDoanhNghiep,
                        tt.TenantId as TenantId,
                        hoso.IsChiCuc as IsChiCuc,
                        hoso.ChiCucId as ChiCucId,
                        hoso.TrangThaiHoSo as TrangThaiHoSo,
                        ISNULL(dn.TinhId, 0) as TinhId,
                        ISNULL(dn.Tinh,'') as  StrTinh,
                        hoso.ThanhToanId_Active as ThanhToanId_Active

                        from dbo.TT{maTT}_HoSo hoso
                        join dbo.ThanhToan tt on hoso.ThanhToanId_Active = tt.Id
                        left join dbo.LoaiHoSo lhs on hoso.LoaiHoSoId = lhs.Id
                        left join dbo.DoanhNghiep dn on hoso.DoanhNghiepId = dn.Id
                        where
                        (
                        hoso.PId is null and tt.KenhThanhToan = {(int)CommonENum.KENH_THANH_TOAN.HINH_THUC_CHUYEN_KHOAN}
                            and hoso.IsDeleted = 0 and tt.IsDeleted = 0
                        )

                        and 
                        ( ISNULL(@keyword,'') = '' 
                        or  hoso.MaHoSo like'%'+@keyword+'%'
                        or hoso.SoDangKy like'%'+@keyword+'%' 
                        or dbo.f_LocDauLowerCaseDB(dn.TenDoanhNghiep) like'%'+@keyword+'%'
                        or tt.MaGiaoDich like'%'+@keyword+'%'
                        or tt.MaDonHang like '%'+@keyword+'%'
                        )

                        ";

                if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_CHO_XAC_NHAN_THANH_TOAN)
                {
                    thuTucQuery += $" and ( TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN}  ) and TrangThaiKeToan = {(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN} ";
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_DA_XAC_NHAN_THANH_TOAN_THANH_CONG)
                {
                    thuTucQuery += $" and ( TrangThaiKeToan = {(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_THANH_CONG} ) ";
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_DA_XAC_NHAN_THANH_TOAN_THAT_BAI)
                {
                    thuTucQuery += $" and ( TrangThaiKeToan = {(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_KHONG_THANH_CONG} ) ";
                }

                sqlUnionThuTuc = string.IsNullOrEmpty(sqlUnionThuTuc) ? thuTucQuery : $"{sqlUnionThuTuc} UNION ALL {thuTucQuery}";

            }

            string sqlQuery = $@"select TotalRows = Count(1) Over(),
                                        A.* from ( {sqlUnionThuTuc} ) as A
                                        order by A.Id
                                        OFFSET (@skip) ROWS
	                                    FETCH NEXT @rows ROWS ONLY; ";

            int total = 0;
            var dataGrids = new List<ThanhToanDto>();
            SqlParameter[] prm = new SqlParameter[]
                    {
                    new SqlParameter("@skip", input.SkipCount),
                    new SqlParameter("@rows", input.MaxResultCount),
                    new SqlParameter("@keyword", keyWord),
                    };


            using (var ctx = new AppDbContext())
            {
                dataGrids = ctx.Database.SqlQuery<ThanhToanDto>(sqlQuery, prm).ToList();
                total = dataGrids.Count > 0 ? dataGrids[0].TotalRows.Value : 0;
            }
            return new PagedResultDto<ThanhToanDto>(total, dataGrids);

        }


        // Test kế toán TT05 - Xóa sau
        public PagedResultDto<ThanhToanDto> GetListThanhToanChuyenKhoan_TT05(HoSoReportInputDto input, string _maThuTuc)
        {
            string keyWord = Utility.StringExtensions.FomatAndKhongDau(input.Keyword);
            keyWord = string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord;

            var sqlUnionThuTuc = "";
            //foreach (int tt in _userCurrent.ThuTucEnum)
            //{
            //string maTT = tt < 10 ? $"0{tt}" : tt.ToString();

            string thuTucQuery = $@"
                        select 
                        '{_maThuTuc}' as MaThuTuc,
                        hoso.Id as Id,
                        tt.DoanhNghiepId as DoanhNghiepId,
                        hoso.MaHoSo as MaHoSo,
                        hoso.SoDangKy as SoDangKy,
                        hoso.TenCoSo as TenSanPham,
                        hoso.LoaiHoSoId as LoaiHoSoId,
                        lhs.TenLoaiHoSo as StrLoaiHoSo,
                        tt.HoSoId as HoSoId,
                        tt.GhiChu as GhiChu,
                        tt.KenhThanhToan as KenhThanhToan,
                        tt.PhiDaNop as PhiDaNop,
                        tt.PhiXacNhan as PhiXacNhan,
                        tt.SoTaiKhoanNop as SoTaiKhoanNop,
                        tt.SoTaiKhoanHuong as SoTaiKhoanHuong,
                        tt.MaGiaoDich as MaGiaoDich,
                        tt.MaDonHang as MaDonHang,
                        tt.NgayGiaoDich as NgayGiaoDich,
                        tt.TrangThaiKeToan as TrangThaiKeToan,
                        tt.TrangThaiNganHang as TrangThaiNganHang,
                        dn.TenDoanhNghiep as TenDoanhNghiep,
                        tt.TenantId as TenantId,
                        hoso.IsChiCuc as IsChiCuc,
                        hoso.ChiCucId as ChiCucId,
                        hoso.TrangThaiHoSo as TrangThaiHoSo,
                        ISNULL(dn.TinhId, 0) as TinhId,
                        ISNULL(dn.Tinh,'') as  StrTinh,
                        hoso.ThanhToanId_Active as ThanhToanId_Active

                        from dbo.{_maThuTuc}_HoSo hoso
                        join dbo.ThanhToan tt on hoso.ThanhToanId_Active = tt.Id
                        left join dbo.LoaiHoSo lhs on hoso.LoaiHoSoId = lhs.Id
                        left join dbo.DoanhNghiep dn on hoso.DoanhNghiepId = dn.Id
                        where
                        (
                        hoso.PId is null and tt.KenhThanhToan = {(int)CommonENum.KENH_THANH_TOAN.HINH_THUC_CHUYEN_KHOAN}
                            and hoso.IsDeleted = 0 and tt.IsDeleted = 0
                        )

                        and 
                        ( ISNULL(@keyword,'') = '' 
                        or  hoso.MaHoSo like'%'+@keyword+'%'
                        or hoso.SoDangKy like'%'+@keyword+'%' 
                        or dbo.f_LocDauLowerCaseDB(dn.TenDoanhNghiep) like'%'+@keyword+'%'
                        or tt.MaGiaoDich like'%'+@keyword+'%'
                        or tt.MaDonHang like '%'+@keyword+'%'
                        )

                        ";

            if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_CHO_XAC_NHAN_THANH_TOAN)
            {
                thuTucQuery += $" and ( TrangThaiHoSo = {(int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN}  ) and TrangThaiKeToan = {(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_CHUA_XAC_NHAN} ";
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_DA_XAC_NHAN_THANH_TOAN_THANH_CONG)
            {
                thuTucQuery += $" and ( TrangThaiKeToan = {(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_THANH_CONG} ) ";
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_XAC_NHAN_THANH_TOAN.HO_SO_DA_XAC_NHAN_THANH_TOAN_THAT_BAI)
            {
                thuTucQuery += $" and ( TrangThaiKeToan = {(int)CommonENum.TRANG_THAI_KE_TOAN.KE_TOAN_XAC_NHAN_KHONG_THANH_CONG} ) ";
            }

            sqlUnionThuTuc = string.IsNullOrEmpty(sqlUnionThuTuc) ? thuTucQuery : $"{sqlUnionThuTuc} UNION ALL {thuTucQuery}";
            //queryThuTuc = string.IsNullOrEmpty(sqlUnionThuTuc) ? thuTucQuery : $"{sqlUnionThuTuc} UNION ALL {thuTucQuery}";

            //}

            string sqlQuery = $@"select TotalRows = Count(1) Over(),
                                        A.* from ( {thuTucQuery} ) as A
                                        order by A.Id
                                        OFFSET (@skip) ROWS
	                                    FETCH NEXT @rows ROWS ONLY; ";

            int total = 0;
            var dataGrids = new List<ThanhToanDto>();
            SqlParameter[] prm = new SqlParameter[]
                    {
                    new SqlParameter("@skip", input.SkipCount),
                    new SqlParameter("@rows", input.MaxResultCount),
                    new SqlParameter("@keyword", keyWord),
                    };


            using (var ctx = new AppDbContext())
            {
                dataGrids = ctx.Database.SqlQuery<ThanhToanDto>(sqlQuery, prm).ToList();
                total = dataGrids.Count > 0 ? dataGrids[0].TotalRows.Value : 0;
            }
            return new PagedResultDto<ThanhToanDto>(total, dataGrids);

        }

        public TotalLabelFormCase GetTotalFormCase(HoSoReportInputDto input)
        {
            try
            {
                SqlParameter[] prm = ParmeterThuTucReport(input);
                var sqlUnionThuTuc = "";
                if (_userCurrent.ThuTucEnum != null && _userCurrent.ThuTucEnum.Count() > 0)
                {
                    foreach (int tt in _userCurrent.ThuTucEnum)
                    {
                        string maTT = tt < 10 ? $"0{tt}" : tt.ToString();

                        var abc = SelectFormCase(input);

                        string thuTucQuery = $@" select hs.Id
                        {SelectFormCase(input)}
                        from dbo.TT{maTT}_HoSo hs
                        left join dbo.TT{maTT}_HoSoXuLy hsxl on hs.HoSoXuLyId_Active = hsxl.Id
                        where hs.IsDeleted = 0 
                        and hs.PId is null 
                        and  {WhereCommonSql(tt, input)} ";

                        sqlUnionThuTuc = string.IsNullOrEmpty(sqlUnionThuTuc) ? thuTucQuery : $"{sqlUnionThuTuc} UNION ALL {thuTucQuery}";

                    }
                    string sqlQuery = $@"select Sum(A.FormCase0) As Case0
                            ,Sum(A.FormCase1) As Case1
                            ,Sum(A.FormCase2) As Case2
                            ,Sum(A.FormCase3) As Case3
                            ,Sum(A.FormCase4) As Case4
                            ,Sum(A.FormCase5) As Case5
                            ,Sum(A.FormCase6) As Case6
                            ,Sum(A.FormCase7) As Case7
                            ,Sum(A.FormCase8) As Case8
                            ,Sum(A.FormCase9) As Case9
                            from ( {sqlUnionThuTuc} ) as A 
                            Where A.FormCase0 = 1";

                    using (var ctx = new AppDbContext())
                    {
                        var data = ctx.Database.SqlQuery<TotalLabelFormCase>(sqlQuery, prm).FirstOrDefault();
                        return data;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }

        }

        #region get query union all cac thu tuc
        private SqlParameter[] ParmeterThuTucReport(HoSoReportInputDto input)
        {
            long hoSoId = 0;
            long.TryParse(input.Keyword, out hoSoId);

            DateTime NgayNopToi = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue, NgayNopTu = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            if (input.NgayNopToi.HasValue && input.NgayNopTu.HasValue)
            {
                NgayNopToi = new DateTime(input.NgayNopToi.Value.Year, input.NgayNopToi.Value.Month, input.NgayNopToi.Value.Day, 23, 59, 59);
                NgayNopTu = new DateTime(input.NgayNopTu.Value.Year, input.NgayNopTu.Value.Month, input.NgayNopTu.Value.Day, 0, 0, 0);
            }

            DateTime TuNgay = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue, DenNgay = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            if (input.DenNgay.HasValue && input.TuNgay.HasValue)
            {
                DenNgay = new DateTime(input.DenNgay.Value.Year, input.DenNgay.Value.Month, input.DenNgay.Value.Day, 23, 59, 59);
                TuNgay = new DateTime(input.TuNgay.Value.Year, input.TuNgay.Value.Month, input.TuNgay.Value.Day, 0, 0, 0);
            }

            string keyWord = Utility.StringExtensions.FomatAndKhongDau(input.Keyword);
            keyWord = string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord;
            input.DoanhNghiepId = input.DoanhNghiepId.HasValue ? input.DoanhNghiepId.Value : 0;

            List<SqlParameter> lstPrm = AppQuerySql.ParameterThuTucQuery(input.DoanhNghiepId, input.PhongBanId, input.NhomThuTucId);
            lstPrm.Add(new SqlParameter("@skip", input.SkipCount));
            lstPrm.Add(new SqlParameter("@rows", input.MaxResultCount));
            lstPrm.Add(new SqlParameter("@hosoid", hoSoId));

            if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_MOT_CUA || input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_ADMIN || input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_VAN_THU)
            {
                lstPrm.Add(new SqlParameter("@startdate", TuNgay));
                lstPrm.Add(new SqlParameter("@enddate", DenNgay));
            }
            else
            {
                lstPrm.Add(new SqlParameter("@startdate", NgayNopTu));
                lstPrm.Add(new SqlParameter("@enddate", NgayNopToi));
            }
            lstPrm.Add(new SqlParameter("@keyword", keyWord));
            return lstPrm.ToArray();
        }
        public string SelectStringSql(int tt, HoSoReportInputDto input)
        {
            string select = "";
            string colLoaiDonHang = " LoaiDonHang = 0, ";
            select = $@" 
                            hs.ThuTucId,
                            hs.Id,
                            hs.MaSoThue,
                            hs.MaHoSo,
                            hs.SoDangKy,
                            hs.IsCA,
                            hs.OnIsCA,
                            hs.IsHoSoBS,
                            {colLoaiDonHang}
                            hs.IsHoSoUuTien,
                            hs.JsonDonHang,                                      
                            hs.hoSoXuLyId_Active,
                            hsxl.DonViGui,
                            hsxl.DonViXuLy,
                           
                            NgayGui = hsxl.NgayGui,
                            NguoiGuiId = hsxl.NguoiGuiId,
                            NguoiXuLyId = hsxl.NguoiXuLyId,

                            hs.LoaiHoSoId,
                            hs.TrangThaiHoSo,

                            --Doanh nghiệp
                            hs.DoanhNghiepId,
                            hs.TenDoanhNghiep,

                            --Kế toán
                            hs.NgayThanhToan,
                            hs.NgayXacNhanThanhToan,
                            hs.KeToanId,

                            --Sắp xếp
                            hs.CreationTime,
                            hs.LastModifierUserId,

                            --Một cửa phân công
                            hs.IsChuyenAuto,
                            hs.NgayChuyenAuto,
                            hs.MotCuaChuyenId,
                            hs.NgayMotCuaChuyen,
                            hs.PhongBanId,

                            hsxl.ChuyenVienThuLyId,

                            --Nộp hồ sơ
                            hsxl.NgayTiepNhan,
                            hsxl.VanThuNgayDuyet,
                            hsxl.NgayHenTra,
                            hsxl.HoSoIsDat,
                            hsxl.VanThuDaDuyet
                            ";
            select += ",'' as HoSoFullJson";

            return select;
        }
        public string SelectFormCase(HoSoReportInputDto input)
        {
            string select = "";
            if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1   ELSE 0  END AS FormCase0", FORM_MOT_CUA_PHAN_CONG.TAT_CA);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1   ELSE 0  END AS FormCase1", FORM_MOT_CUA_PHAN_CONG.HO_SO_CHO_PHAN_CONG);

                select += @"
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET)
            {
                select += $@",CASE   WHEN ( {FORM_VAN_THU_DUYET.TAT_CA} )   THEN 1    ELSE 0  END AS FormCase0
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET.HO_SO_CHUA_DUYET} )   THEN 1    ELSE 0  END AS FormCase1
                            , CASE   WHEN ( {FORM_VAN_THU_DUYET.HO_SO_DA_DUỴET} )   THEN 1    ELSE 0  END AS FormCase2
                            , 0 AS FormCase3
                            , 0 AS FormCase4
                            , 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_CHUNG)
            {
                select += string.Format(@", CASE   WHEN ( {0} )   THEN 1    ELSE 0  END AS FormCase0", FORM_THONG_KE_HO_SO_CHUNG.TAT_CA);
                select += string.Format(@", CASE   WHEN ( {0} )   THEN 1    ELSE 0  END AS FormCase1", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_DA_TIEP_NHAN);
                select += string.Format(@", CASE   WHEN ( {0} )   THEN 1    ELSE 0  END AS FormCase2", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_DANG_XU_LY);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase3", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_CHO_DUYET);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase4", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_DA_DUYET);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase5", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_CAN_SUA_DOI_BO_SUNG);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase6", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_CHUA_TIEP_NHAN);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase7", FORM_THONG_KE_HO_SO_CHUNG.HO_SO_VAN_THU_TRA_LAI);
                select += @", 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_ADMIN)
            {
                select += string.Format(@", CASE   WHEN ( {0} )   THEN 1    ELSE 0  END AS FormCase0", FORM_THONG_KE_ADMIN.TAT_CA);
                select += string.Format(@", CASE   WHEN ( {0} )   THEN 1    ELSE 0  END AS FormCase1", FORM_THONG_KE_ADMIN.HO_SO_DA_TIEP_NHAN);
                select += string.Format(@", CASE   WHEN ( {0} )   THEN 1    ELSE 0  END AS FormCase2", FORM_THONG_KE_ADMIN.HO_SO_BI_TU_CHOI);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase3", FORM_THONG_KE_ADMIN.HO_SO_DA_GIAI_QUYET);
                select += string.Format(@", CASE   WHEN ( {0} )  THEN 1    ELSE 0  END AS FormCase4", FORM_THONG_KE_ADMIN.HO_SO_CHO_BO_SUNG);
                select += @", 0 AS FormCase5
                            , 0 AS FormCase6
                            , 0 AS FormCase7
                            , 0 AS FormCase8
                            , 0 AS FormCase9";
            }
            return select;



        }
        public string WhereCommonSql(int tt, HoSoReportInputDto input)
        {
            string filterKeyWord = @"'%'+@keyword+'%'";
            string sqlWhere = "";
            string sqlWhereTime = "";

            long keyNumber = 0;
            long.TryParse(input.Keyword, out keyNumber);

            if (!string.IsNullOrEmpty(input.LoaiDonHangIds))
            {
                var loaiDonHangIds = input.LoaiDonHangIds.Split(',');
                var listWhereTT = new List<string>();
                foreach (var item in loaiDonHangIds)
                {
                    var ttnumber = int.Parse(item.Split('_').Last());
                    var ldhenum = item.Split('_').First();
                    listWhereTT.Add($@" hs.ThuTucId = { ttnumber} ");
                    if (ttnumber == tt)
                        sqlWhere += $@" and hs.LoaiDonHang = {ldhenum} ";
                }
                sqlWhere += " and (" + string.Join(" or ", listWhereTT) + ") ";
            }
            if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_VAN_THU)
            {
                sqlWhereTime = $@" and(@startdate is null or (hsxl.VanThuNgayDuyet is not null and hsxl.VanThuNgayDuyet >= @startdate)) 
                                   and(@enddate is null or(hsxl.VanThuNgayDuyet is not null and hsxl.VanThuNgayDuyet <= @enddate)) ";
            }
            else
            {
                sqlWhereTime = $@" and(@startdate is null or (hsxl.NgayTiepNhan is not null and hsxl.NgayTiepNhan >= @startdate)) 
                                   and(@enddate is null or(hsxl.NgayTiepNhan is not null and hsxl.NgayTiepNhan <= @enddate)) ";
            }

            string where = $@" ( 
                                ISNULL(@keyword,'') = '' 
                                or hs.SoDangKy like {filterKeyWord}
                                or hs.MaHoSo like  {filterKeyWord}
                                or dbo.f_LocDauLowerCaseDB(hs.TenDoanhNghiep) like  {filterKeyWord}
                                or hs.TenDoanhNghiep like  {filterKeyWord}
                                or hs.MaSoThue like {filterKeyWord}
                                or hs.ThuTucId = {keyNumber}
                                or hs.Id = {keyNumber}
                                )
                                and(@hosoid = 0 or hs.Id = @hosoid)
                                {sqlWhere}
                                {sqlWhereTime}
                                and(ISNULL(@doanhnghiepid,0) = 0 or hs.DoanhNghiepId = @doanhnghiepid) 
                                and(ISNULL(@phongbanid, 0) = 0 or hs.PhongBanId = @phongbanid) 
                                and(ISNULL(@nhomthutucid, 0) = 0 or hs.ThuTucId in (select Id from dbo.ThuTuc where NhomThuTucId=@nhomthutucid)) ";
            return where;
        }
        public string WhereStringSql(int tt, HoSoReportInputDto input)
        {

            string where = WhereCommonSql(tt, input);
            #region mot cua phan cong
            if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {
                where += $" and {FORM_MOT_CUA_PHAN_CONG.TAT_CA}";
                switch (input.FormCase)
                {
                    case (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_NOP_MOI:
                        // phong dang ky
                        where += $" and {FORM_MOT_CUA_PHAN_CONG.HO_SO_CHO_PHAN_CONG}";
                        break;

                }
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET)
            {
                //where += $" and ({FORM_VAN_THU_DUYET.TAT_CA})";
                switch (input.FormCase)
                {
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET.GET_ALL:
                        where += $" and ({FORM_VAN_THU_DUYET.TAT_CA})";
                        break;
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_CHUA_DUYET:
                        where += $" and ({FORM_VAN_THU_DUYET.HO_SO_CHUA_DUYET})";
                        break;
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_DA_DUYET:
                        where += $" and ({FORM_VAN_THU_DUYET.HO_SO_DA_DUỴET})";
                        break;
                }
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET_CONG_VAN)
            {
                //where += $" and ({FORM_VAN_THU_DUYET.TAT_CA})";
                switch (input.FormCase)
                {
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET_CONG_VAN.GET_ALL:
                        where += $" and ({FORM_VAN_THU_DUYET_CONG_VAN.TAT_CA})";
                        break;
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET_CONG_VAN.CONG_VAN_CHUA_DUYET:
                        where += $" and ({FORM_VAN_THU_DUYET_CONG_VAN.CONG_VAN_CHUA_DUYET})";
                        break;
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET_CONG_VAN.CONG_VAN_DA_DUYET:
                        where += $" and ({FORM_VAN_THU_DUYET_CONG_VAN.CONG_VAN_DA_DUỴET})";
                        break;
                    case (int)CommonENum.FORM_CASE_VAN_THU_DUYET_CONG_VAN.CONG_VAN_TRA_LAI:
                        where += $" and ({FORM_VAN_THU_DUYET_CONG_VAN.CONG_VAN_TRA_LAI})";
                        break;
                }
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_MOT_CUA)
            {
                where += $" and ({FORM_THONG_KE_MOT_CUA_PHAN_CONG.TAT_CA})";
                switch (input.FormCase)
                {
                    case (int)CommonENum.FORM_CASE_THONG_KE_MOT_CUA.HO_SO_DA_TIEP_NHAN:
                        where += $" and ({FORM_THONG_KE_MOT_CUA_PHAN_CONG.HO_SO_DA_TIEP_NHAN})";
                        break;
                    case (int)CommonENum.FORM_CASE_THONG_KE_MOT_CUA.HO_SO_TRA_LAI:
                        where += $" and ({FORM_THONG_KE_MOT_CUA_PHAN_CONG.HO_SO_BI_TU_CHOI})";
                        break;
                }
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_VAN_THU)
            {
                where += $" and ({FORM_THONG_KE_VAN_THU.TAT_CA})";
                switch (input.FormCase)
                {
                    case (int)CommonENum.FORM_CASE_THONG_KE_VAN_THU.HO_SO_DA_DONG_DAU:
                        where += $" and ({FORM_THONG_KE_VAN_THU.HO_SO_DA_DONG_DAU})";
                        break;
                    case (int)CommonENum.FORM_CASE_THONG_KE_VAN_THU.HO_SO_TRA_LAI:
                        where += $" and ({FORM_THONG_KE_VAN_THU.HO_SO_BI_TU_CHOI})";
                        break;
                }
            }
            else if (input.FormId == (int)CommonENum.FORM_ID.FORM_THONG_KE_ADMIN)
            {
                where += $" and ({FORM_THONG_KE_ADMIN.TAT_CA})";
                switch (input.FormCase)
                {
                    case (int)CommonENum.FORM_CASE_THONG_KE_ADMIN.HO_SO_DA_TIEP_NHAN:
                        where += $" and ({FORM_THONG_KE_ADMIN.HO_SO_DA_TIEP_NHAN})";
                        break;
                    case (int)CommonENum.FORM_CASE_THONG_KE_ADMIN.HO_SO_TRA_LAI:
                        where += $" and ({FORM_THONG_KE_ADMIN.HO_SO_BI_TU_CHOI})";
                        break;
                    case (int)CommonENum.FORM_CASE_THONG_KE_ADMIN.HO_SO_DA_GIAI_QUYET:
                        where += $" and ({FORM_THONG_KE_ADMIN.HO_SO_DA_GIAI_QUYET})";
                        break;
                    case (int)CommonENum.FORM_CASE_THONG_KE_ADMIN.HO_SO_CHO_BO_SUNG:
                        where += $" and ({FORM_THONG_KE_ADMIN.HO_SO_CHO_BO_SUNG})";
                        break;
                }
            }
            #endregion
            return where;
        }
        private HoSoReportDto GetTrangThaiXuLyHoSo(HoSoReportDto item, HoSoReportInputDto input)
        {

            if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {

                if ((item.DonViGui == (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP || item.DonViGui == (int)CommonENum.DON_VI_XU_LY.KE_TOAN) && item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_PHAN_CONG)
                {
                    item.FormCase = (int)CommonENum.FORM_CASE_MOT_CUA_PHAN_CONG.HO_SO_NOP_MOI;
                    item.StrTrangThai = "Hồ sơ chờ tiếp nhận";
                }

            }
            if (input.FormId == (int)CommonENum.FORM_ID.FORM_VAN_THU_DUYET)
            {

                if (item.VanThuDaDuyet == true || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT || item.FormCase == (int)CommonENum.FORM_CASE_THONG_KE_VAN_THU.HO_SO_TRA_LAI || item.DonViGui == (int)CommonENum.DON_VI_XU_LY.VAN_THU)
                {
                    item.FormCase = (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_DA_DUYET;
                }
                else
                {
                    item.FormCase = (int)CommonENum.FORM_CASE_VAN_THU_DUYET.HO_SO_CHUA_DUYET;
                }
            }
            return item;
        }
        private HoSoReportDto GetPhongBanPhanCong(HoSoReportDto item, HoSoReportInputDto input)
        {
            if (input.FormId == (int)CommonENum.FORM_ID.FORM_MOT_CUA_PHAN_CONG)
            {

                var _listPhongBan = (from ploai in _phongBanLoaiHoSoRepos.GetAll()
                                     join pb in _phongBanRepos.GetAll() on ploai.PhongBanId equals pb.Id
                                     where ploai.LoaiHoSoId == item.LoaiHoSoId
                                     select new ItemDto<int>
                                     {
                                         Id = pb.Id,
                                         Name = pb.TenPhongBan,
                                         Checked = false
                                     }).ToList();

                if (item.PhongBanId.HasValue && item.PhongBanId > 0)
                {
                    foreach (var phongban in _listPhongBan)
                    {
                        if (phongban.Id == item.PhongBanId)
                        {
                            phongban.Checked = true;
                        }
                    }
                }

                item.ArrPhongBanXuLy = _listPhongBan;
            }
            return item;
        }
        #endregion

        public Dto.FileDto ExportExcelSoTheoDoi(HoSoReportInputDto input)
        {
            try
            {
                input.MaxResultCount = Int32.MaxValue - 1;
                var ret = GetTraCuuHoSo(input);

                var tempExcelPath = Path.Combine(_appFolders.TempFileBaoCaoFolder, "RptSoTheoDoi.xlsx");
                using (var templateXls = new ExcelPackage(new FileInfo(tempExcelPath)))
                {
                    var workSheet = templateXls.Workbook.Worksheets.FirstOrDefault();
                    var rowIndex = 6;

                    workSheet.Cells[3, 1].Value = $"Từ ngày {(input.TuNgay == null ? "" : input.TuNgay.Value.ToString("dd/MM/yyyy"))} tới ngày {(input.DenNgay == null ? "" : input.DenNgay.Value.ToString("dd/MM/yyyy"))}";

                    foreach (var item in ret.Items)
                    {
                        workSheet.Cells[rowIndex, 1].Value = rowIndex - 5;
                        workSheet.Cells[rowIndex, 2].Value = item.NgayTiepNhan;
                        workSheet.Cells[rowIndex, 3].Value = item.MaHoSo;
                        workSheet.Cells[rowIndex, 4].Value = $"{ GetName(item)}";
                        workSheet.Cells[rowIndex, 5].Value = item.TenLoaiHoSo;
                        workSheet.Cells[rowIndex, 6].Value = item.TenDoanhNghiep;
                        rowIndex++;
                    }
                    #region Footer
                    workSheet.Cells[rowIndex, 1].Value = $"Tổng số: {ret.TotalCount} đơn hàng";
                    workSheet.Cells[rowIndex, 1, rowIndex, 6].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

                    if (rowIndex > 10)
                    {
                        workSheet.Cells[6, 1, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[6, 1, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[6, 1, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[6, 1, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    #endregion

                    var file = new Dto.FileDto("RptSoTheoDoi" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx", MimeTypeNames.ApplicationVndMsExcel);
                    var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileToken);
                    templateXls.SaveAs(new FileInfo(filePath));
                    return file;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        private string GetName(HoSoReportDto input)
        {
            string result = string.Empty;
            try
            {
                //switch (input.ThuTucId)
                //{
                //    case 38:
                //        var donHang38s = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.HoSoDonHang38Dto>(input.JsonDonHang);
                //        result = donHang38s.DanhSachThuoc == null || donHang38s.DanhSachThuoc.Count() == 0 ? "" : donHang38s.DanhSachThuoc[0].TenThuoc;
                //        break;
                //    case 39:
                //        var donHang39s = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.HoSoDonHang39Dto>(input.JsonDonHang);
                //        result = donHang39s.DanhSachThuoc == null || donHang39s.DanhSachThuoc.Count() == 0 ? "" : donHang39s.DanhSachThuoc[0].Teninn;
                //        break;
                //}
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return result;
        }
    }
}