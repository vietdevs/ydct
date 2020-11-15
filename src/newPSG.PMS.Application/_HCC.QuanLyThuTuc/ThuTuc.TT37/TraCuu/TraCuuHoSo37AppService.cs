using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using newPSG.PMS.EntityDB;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

#region Class Riêng Cho Từng Thủ tục
using XHoSo = newPSG.PMS.EntityDB.HoSo37;
using XHoSoDto = newPSG.PMS.Dto.TraCuuHoSo37Dto;
using TraCuuHoSoInput = newPSG.PMS.Dto.TraCuuHoSo37Input;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using System.Collections.Generic;
using Newtonsoft.Json;
using Abp.Runtime.Session;
using Abp.Domain.Uow;
#endregion

namespace newPSG.PMS.Common
{
    public interface ITraCuuHoSo37AppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoDto>> GetListHoSoTraCuuPaging(TraCuuHoSoInput input);
        Task<List<DropDownListOutput>> GetListTruongPhong();
        Task<List<DropDownListOutput>> GetListChuyenVien();
        Task<List<DropDownListOutput>> GetListVanThu();
        Task<List<DropDownListOutput>> GetListLanhDaoCuc();
        List<DropDownListOutput> GetTrangThaiTraCuu();
    }

    public class TraCuuHoSo37AppService : PMSAppServiceBase, ITraCuuHoSo37AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<HoSoXuLy37, long> _hoSoXuLyRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<TT37_HoSoDoanThamDinh, long> _hoSoDoanThamDinh;
        private readonly IAbpSession _session;

        public TraCuuHoSo37AppService(IRepository<XHoSo, long> hoSoRepos,
                                      IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                      IRepository<HoSoXuLy37, long> hoSoXuLyRepos,
                                      IRepository<User, long> userRepos,
                                      IRepository<LoaiHoSo> loaiHoSoRepos,
                                      IRepository<PhongBan> phongBanRepos,
                                      IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
                                      IRepository<TT37_HoSoDoanThamDinh, long> hoSoDoanThamDinh,
                                      IAbpSession session)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
            _userRepos = userRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
            _phongBanRepos = phongBanRepos;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _hoSoDoanThamDinh = hoSoDoanThamDinh;
            _session = session;
        }

        public async Task<PagedResultDto<XHoSoDto>> GetListHoSoTraCuuPaging(TraCuuHoSoInput input)
        {
            try
            {
                var query = (from hoso in _hoSoRepos.GetAll()
                             join hsxl in _hoSoXuLyRepos.GetAll() on hoso.HoSoXuLyId_Active equals hsxl.Id
                             join r_dn in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals r_dn.Id into tb_dn //Left Join
                             from dn in tb_dn.DefaultIfEmpty()
                             where hoso.PId == null
                             select new XHoSoDto
                             {
                                 Id = hoso.Id,
                                 DoanhNghiepId = hoso.DoanhNghiepId,
                                 SoDangKy = hoso.SoDangKy,
                                 TenDoanhNghiep = hoso.TenDoanhNghiep,
                                 TenNguoiDaiDien = hoso.TenNguoiDaiDien,
                                 NgaySinh = hoso.NgaySinh,
                                 TinhId = (dn != null) ? dn.TinhId : 0,
                                 HuyenId = dn.HuyenId,
                                 XaId = dn.XaId,
                                 StrTinh = (dn != null) ? dn.Tinh : string.Empty,
                                 DiaChi = hoso.DiaChi,
                                 NgayTraKetQua = hoso.NgayTraKetQua,
                                 MaHoSo = hoso.MaHoSo,
                                 GiayTiepNhan = hoso.GiayTiepNhan,
                                 GiayTiepNhanCA = hsxl.GiayTiepNhanCA,
                                 SoGiayTiepNhan = hoso.SoGiayTiepNhan,
                                 LoaiHoSoId = hoso.LoaiHoSoId,
                                 TrangThaiHoSo = hoso.TrangThaiHoSo,
                                 VanThuId = hsxl.VanThuId,
                                 ChuyenVienThuLyId = hsxl.ChuyenVienThuLyId,
                                 TruongPhongId = hsxl.TruongPhongId,
                                 LanhDaoCucId = hsxl.LanhDaoCucId,
                                 NgayNopRaSoat = hoso.NgayNopRaSoat,
                                 NgayTiepNhan = hsxl.NgayTiepNhan,
                                 DonViGui = hsxl.DonViGui,
                                 DonViXuLy = hsxl.DonViXuLy,
                                 NguoiXuLyId = hsxl.NguoiXuLyId,
                                 NguoiGuiId = hsxl.NguoiGuiId,
                                 NgayGui = hsxl.NgayGui,
                                 


                                 HoTenNguoiDeNghi = hoso.HoTenNguoiDeNghi,
                                 DiaChiCuTru = hoso.DiaChiCuTru,
                                 EmailNguoiDeNghi = hoso.EmailNguoiDeNghi,
                                 DienThoaiNguoiDeNghi = hoso.DienThoaiNguoiDeNghi,
                                 SoNhanBiet = hoso.SoNhanBiet
                             }).WhereIf(input.TinhId.HasValue, x => x.TinhId == input.TinhId)
                             .WhereIf(input.HuyenId.HasValue, x => x.HuyenId == input.HuyenId)
                             .WhereIf(input.XaId.HasValue, x => x.XaId == input.XaId)
                             .WhereIf(input.VanThuId.HasValue, x => x.VanThuId == input.VanThuId)
                             .WhereIf(input.ChuyenVienThuLyId.HasValue, x => x.ChuyenVienThuLyId == input.ChuyenVienThuLyId)
                             .WhereIf(input.TruongPhongId.HasValue, x => x.TruongPhongId == input.TruongPhongId)
                             .WhereIf(input.LanhDaoCucId.HasValue, x => x.LanhDaoCucId == input.LanhDaoCucId)
                             .WhereIf(!String.IsNullOrEmpty(input.HoTenNguoiDeNghi), x => x.HoTenNguoiDeNghi.Contains(input.HoTenNguoiDeNghi))
                             .WhereIf(!String.IsNullOrEmpty(input.MaHoSo), x => x.MaHoSo.Contains(input.MaHoSo))
                             .WhereIf(!String.IsNullOrEmpty(input.MaSoThue), x => x.MaHoSo.Contains(input.MaSoThue))
                             .WhereIf(!String.IsNullOrEmpty(input.DiaChi), x => x.DiaChi.Contains(input.DiaChi));
                var a = query.ToList();

                if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.DA_NOP_HO_SO_MOI)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.MOT_CUA_DA_TIEP_NHAN)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.MOT_CUA_TRA_LAI)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.CHO_TRA_GIAY_TIEP_NHAN)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN);

                if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.LANH_DAO_DA_PHAN_CONG_HO_SO)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.HO_SO_THAM_XET_LAI)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.HO_SO_THAM_XET_LAI);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.SUA_DOI_BO_SUNG)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.HO_SO_CHO_THAM_DINH)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_THAM_DINH);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.TONG_HOP_THAM_DINH_LAI)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.TONG_HOP_THAM_DINH_LAI);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.HO_SO_TONG_HOP_THAM_DINH_DA_LUU)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_TONG_HOP_THAM_DINH_DA_LUU);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.LANH_DAO_CUC_DUYET_THAM_DINH)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.LANH_DAO_CUC_DUYET_THAM_DINH);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.HO_SO_CHO_VAN_THU_TRA_KET_QUA)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_VAN_THU_TRA_KET_QUA);

                else if (input.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37.DA_HOAN_TAT)
                    query = query.Where(x => x.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT);


                var _total = await query.CountAsync();
                var dataGrids = await query
                    .OrderByDescending(p => p.NgayTraKetQua)
                    .PageBy(input)
                   .ToListAsync();
                foreach (var item in dataGrids)
                {
                    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        if (item.ChuyenVienThuLyId.HasValue)
                        {
                            var chuyenVienThuLyObj = _userRepos.FirstOrDefault(item.ChuyenVienThuLyId.Value);
                            item.ChuyenVienThuLyName = chuyenVienThuLyObj.Surname + " " + chuyenVienThuLyObj.Name;
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

                    #region Trạng thái hồ sơ
                    item.StrTrangThai = GetTrangThaiXuLyHoSo(item);
                    #endregion

                    item.StrDonViXuLy = item.DonViXuLy != null ? CommonENum.GetEnumDescription((CommonENum.DON_VI_XU_LY)(int)item.DonViXuLy) : "";
                    item.StrDonViGui = item.DonViGui != null ? CommonENum.GetEnumDescription((CommonENum.DON_VI_XU_LY)(int)item.DonViGui) : "";
                }

                return new PagedResultDto<XHoSoDto>(_total, dataGrids);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }
        public IQueryable<PhongBan> GetQueryPhongBanXuLyHoSo()
        {
            try
            {
                var loaiHoSoTT37 = _loaiHoSoRepos.GetAll().Where(x => x.ThuTucId == (int)CommonENum.THU_TUC_ID.THU_TUC_37).Select(x => x.Id).ToList();
                var phongBanlhsTT37 = _phongBanLoaiHoSoRepos.GetAll().Where(x => loaiHoSoTT37.Contains(x.LoaiHoSoId));
                var query = (from phongban in _phongBanRepos.GetAll()
                             join pblhs in phongBanlhsTT37 on phongban.Id equals pblhs.PhongBanId
                             orderby phongban.TenPhongBan ascending
                             select phongban).Distinct();
                return query;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetQueryPhongBanXuLyHoSo {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }

        public async Task<List<DropDownListOutput>> GetListTruongPhong()
        {
            try
            {
                var phongXuLy = GetQueryPhongBanXuLyHoSo().Select(x => x.Id).ToList();

                var _queryTruongPhong = from u in _userRepos.GetAll()
                                        where u.RoleLevel == (int)CommonENum.ROLE_LEVEL.TRUONG_PHONG
                                        && (u.PhongBanId.HasValue && phongXuLy.Contains(u.PhongBanId.Value))
                                        select new DropDownListOutput
                                        {
                                            Id = u.Id,
                                            Name = u.Surname + " " + u.Name,
                                            IsActive = u.IsActive
                                        };

                var res = await _queryTruongPhong.ToListAsync();
                foreach (var item in res)
                {
                    if (item.IsActive != true)
                    {
                        item.Name = item.Name + " (Không hoạt động)";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetListTruongPhong {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return new List<DropDownListOutput>();
            }
        }

        public async Task<List<DropDownListOutput>> GetListChuyenVien()
        {
            try
            {
                var phongXuLy = GetQueryPhongBanXuLyHoSo().Select(x => x.Id).ToList();

                var _queryChuyenVien = from u in _userRepos.GetAll()
                                       where (u.RoleLevel == (int)CommonENum.ROLE_LEVEL.CHUYEN_VIEN || u.RoleLevel == (int)CommonENum.ROLE_LEVEL.PHO_PHONG)
                                       && (u.PhongBanId.HasValue && phongXuLy.Contains(u.PhongBanId.Value))
                                       select new DropDownListOutput
                                       {
                                           Id = u.Id,
                                           Name = u.Surname + " " + u.Name,
                                           IsActive = u.IsActive
                                       };

                var res = await _queryChuyenVien.ToListAsync();
                foreach (var item in res)
                {
                    if (item.IsActive != true)
                    {
                        item.Name = item.Name + " (Không hoạt động)";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetListChuyenVien {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return new List<DropDownListOutput>();
            }
        }

        public async Task<List<DropDownListOutput>> GetListLanhDaoCuc()
        {
            try
            {
                var _queryLanhDaoCuc = from u in _userRepos.GetAll()
                                       where u.RoleLevel == (int)CommonENum.ROLE_LEVEL.LANH_DAO_CUC
                                       orderby u.Stt descending
                                       select new DropDownListOutput
                                       {
                                           Id = u.Id,
                                           Name = u.Surname + " " + u.Name,
                                           IsActive = u.IsActive
                                       };

                var res = await _queryLanhDaoCuc.ToListAsync();
                foreach (var item in res)
                {
                    if (item.IsActive != true)
                    {
                        item.Name = item.Name + " (Không hoạt động)";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetListLanhDaoCuc {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }

        public async Task<List<DropDownListOutput>> GetListKeToan()
        {
            try
            {
                var _queryKeToan = from u in _userRepos.GetAll()
                                   where u.RoleLevel == (int)CommonENum.ROLE_LEVEL.KE_TOAN
                                   orderby u.Stt descending
                                   select new DropDownListOutput
                                   {
                                       Id = u.Id,
                                       Name = u.Surname + " " + u.Name,
                                       IsActive = u.IsActive
                                   };

                var res = await _queryKeToan.ToListAsync();
                foreach (var item in res)
                {
                    if (item.IsActive != true)
                    {
                        item.Name = item.Name + " (Không hoạt động)";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetListKeToan {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }

        public async Task<List<DropDownListOutput>> GetListVanThu()
        {
            try
            {
                var _queryVanThu = from u in _userRepos.GetAll()
                                   where u.RoleLevel == (int)CommonENum.ROLE_LEVEL.VAN_THU
                                   orderby u.Stt descending
                                   select new DropDownListOutput
                                   {
                                       Id = u.Id,
                                       Name = u.Surname + " " + u.Name,
                                       IsActive = u.IsActive
                                   };

                var res = await _queryVanThu.ToListAsync();
                foreach (var item in res)
                {
                    if (item.IsActive != true)
                    {
                        item.Name = item.Name + " (Không hoạt động)";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetListVanThu {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return new List<DropDownListOutput>();
            }
        }

        public List<DropDownListOutput> GetTrangThaiTraCuu()
        {
            List<DropDownListOutput> objTemList = new List<DropDownListOutput>();
            try
            {
                foreach (object iEnumItem in Enum.GetValues(typeof(CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37)))
                {
                    DropDownListOutput objTem = new DropDownListOutput();
                    objTem.Name = CommonENum.GetEnumDescription((CommonENum.TRANG_THAI_HO_SO_FOR_TRA_CUU_TT37)(int)iEnumItem);
                    objTem.Id = ((int)iEnumItem);
                    objTemList.Add(objTem);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} GetTrangThaiTraCuu {ex.Message} {JsonConvert.SerializeObject(ex)}");
            }
            return objTemList;
        }
        private string GetTrangThaiXuLyHoSo(XHoSoDto item)
        {
            var nguoiThamDinhHoSo = _hoSoDoanThamDinh.GetAll().Where(x => x.HoSoId == item.Id && x.ThuTucId == (int)CommonENum.THU_TUC_ID.THU_TUC_37).Select(x => x.UserId).ToList();
            string trangThaiXuLy = "";
            if (!item.TrangThaiHoSo.HasValue)
            {
                trangThaiXuLy = "Hồ sơ lưu nháp";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_LUU)
            {
                trangThaiXuLy = "Hồ sơ soạn mới";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_DOANH_NGHIEP_THANH_TOAN)
            {
                trangThaiXuLy = "Chờ thanh toán";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.TU_CHOI_CAP_PHEP)
            {
                trangThaiXuLy = "Hồ sơ đã hoàn tất";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
            {
                trangThaiXuLy = "Hồ sơ cần bổ sung";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_BO_SUNG)
            {
                trangThaiXuLy = "Hồ sơ nộp bổ sung";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_KE_TOAN_XAC_NHAN)
            {
                trangThaiXuLy = "Hồ sơ đã gửi thanh toán, đang chờ xác nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.THANH_TOAN_THAT_BAI)
            {
                trangThaiXuLy = "Hồ sơ thanh toán thất bại";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI && item.GiayTiepNhanCA == null)
            {
                trangThaiXuLy = "Hồ sơ chờ tiếp nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_NOP_HO_SO_MOI && item.GiayTiepNhanCA != null)
            {
                trangThaiXuLy = "Hồ sơ chờ gửi phân công";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_TRA_LAI)
            {
                trangThaiXuLy = "Hồ sơ bị trả lại";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHO_TRA_GIAY_TIEP_NHAN)
            {
                trangThaiXuLy = "Chờ trả giấy tiếp nhận";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_DA_PHAN_CONG_TOI_CHUYEN_VIEN)
            {
                trangThaiXuLy = "Hồ sơ chờ rà soát";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.MOT_CUA_DA_TIEP_NHAN || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.LANH_DAO_DA_PHAN_CONG_HO_SO)
            {
                trangThaiXuLy = "Hồ sơ chờ phân công ";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.PHONG_BAN_TU_CHOI_TIEP_NHAN_HO_SO || item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.CHUYEN_VIEN_TU_CHOI_TIEP_NHAN_HO_SO)
            {
                trangThaiXuLy = "Phân công lại hồ sơ ";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.SUA_DOI_BO_SUNG)
            {
                trangThaiXuLy = "Hồ sơ cần bổ sung";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH && nguoiThamDinhHoSo.Contains(_session.UserId))
            {
                trangThaiXuLy = "Hồ sơ chờ thẩm định";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_TONG_HOP_THAM_DINH && item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
            {
                trangThaiXuLy = "Hồ sơ chờ tổng hợp thẩm định";
            }
            else if (item.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_THAM_DINH_HO_SO_37.HO_SO_CHO_VAN_THU_TRA_KET_QUA)
            {
                trangThaiXuLy = "Hồ sơ chờ văn thư trả kết quả";
            }
            else
            {
                if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP && item.DonViGui == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN)
                {
                    trangThaiXuLy = "Hồ sơ bị trả lại";
                }
                else if (item.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG && item.DonViGui == (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN)
                {
                    trangThaiXuLy = "Hồ sơ được tiếp nhận";
                }
                else
                {
                    trangThaiXuLy = "Hồ sơ đang thẩm định";
                }
            }
            return trangThaiXuLy;
        }
    }
}
