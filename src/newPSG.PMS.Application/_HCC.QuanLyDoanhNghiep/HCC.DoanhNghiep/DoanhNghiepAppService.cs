using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.EntityDB;
using Abp.Runtime.Session;
using Microsoft.AspNet.Identity;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.DoanhNghiepInput;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System;
using Abp.Authorization;
using newPSG.PMS.Authorization.Users.Dto;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization;
using Abp.Domain.Uow;
using System.Configuration;
using newPSG.PMS.MultiTenancy;
using Abp.Extensions;
using System.Text.RegularExpressions;
using Abp.Application.Services;
using newPSG.PMS.Dto;
using newPSG.PMS.Web;
using Newtonsoft.Json;

namespace newPSG.PMS.Services
{
    public interface IDoanhNghiepAppService : IApplicationService
    {
        Task<List<DropDownListOutput>> GetAllDoanhNghiepToDDLAsync();
        DoanhNghiepDto GetDoanhNghiepById(long input);
        object GetDoanhNghiepByCurrentUser();
        string UpdateDoanhNghiepInfo(DoanhNghiepInfoInput input);
        Task<PagedResultDto<DoanhNghiepDto>> GetAllDoanhNghiepServerPagingAsync(GetDoanhNghiepInput input);
        Task<long> CreateOrUpdateDoanhNghiepAsync(CreateOrUpdateDoanhNghiepInfoInput input);
        Task<string> TaoTaiKhoanDoanhNghiep(CreateOrUpdateDoanhNghiepInfoInput input);
        bool checkExistEmail(string email, long id = 0);
        bool checkExistTenDangNhap(string tenDangNhap);
        Task<string> ChangePasswordDoanhNghiep(ChangeDoanhNghiepPasswordInput input);
        Task ChangeActiveDoanhNghiep(long input);
        List<ItemObj<int>> GetListFormCaseDoanhNghiep();
        Task MoTaiKhoanDoanhNghiep(long doanhNghiepId);
        Task KhongChapNhanDangKyDoanhNghiep(DuyetDoanhNghiepInput input);
        Task<List<DoanhNghiepDto>> GetAllDoanhNghiepSearch(GetDoanhNghiepInput input);
        Task<PagedResultDto<DoanhNghiepDto>> DataConDauDoanhNghiep(GetDoanhNghiepInput input);
    }

    [AbpAuthorize]
    public class DoanhNghiepAppService : PMSAppServiceBase, IDoanhNghiepAppService
    {
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<LoaiHinhDoanhNghiep> _loaiHinhRepos;
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Xa, long> _xaRepos;
        private readonly UserAppService _userService;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<ThongTinPhapLy, long> _giayPhepPhapLyRepos;
        private readonly IRepository<ChucVu> _chucVuRepos;
        private readonly IRepository<ChuKy, long> _chuKyRepos;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserEmailer _userEmailer;
        private readonly CustomSessionAppSession _mySession;

        public DoanhNghiepAppService(IRepository<ChuKy, long> chuKyRepos, IRepository<ThongTinPhapLy, long> giayPhepPhapLyRepos,
                                     IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                     IRepository<LoaiHinhDoanhNghiep> loaiHinhRepos,
                                     IAbpSession session,
                                     UserManager userManager,
                                     RoleManager roleManager,
                                     IRepository<PhongBan> phongBanRepos,
                                     IRepository<User, long> userRepos,
                                     IRepository<Tinh> tinhRepos,
                                     IRepository<Huyen, long> huyenRepos,
                                     IRepository<Xa, long> xaRepos,
                                     UserAppService userService,
                                     TenantManager tenantManager,
                                     IUnitOfWorkManager unitOfWorkManager,
                                     IUserEmailer userEmailer,
                                     IRepository<ChucVu> chucVuRepos,
                                     CustomSessionAppSession mySession
        )
        {
            _doanhNghiepRepos = doanhNghiepRepos;
            _loaiHinhRepos = loaiHinhRepos;
            _session = session;
            _userManager = userManager;
            _roleManager = roleManager;
            _phongBanRepos = phongBanRepos;
            _userRepos = userRepos;
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _xaRepos = xaRepos;
            _userService = userService;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _giayPhepPhapLyRepos = giayPhepPhapLyRepos;
            _userEmailer = userEmailer;
            _chucVuRepos = chucVuRepos;
            _chuKyRepos = chuKyRepos;
            _mySession = mySession;
        }

        public async Task<List<DropDownListOutput>> GetAllDoanhNghiepToDDLAsync()
        {
            var query = from dv in _doanhNghiepRepos.GetAll()
                        where ((dv.IsDaXuLy == true))
                        orderby dv.Id
                        select new DropDownListOutput
                        {
                            Name = dv.TenDoanhNghiep,
                            Id = dv.Id,
                        };

            return await query.ToListAsync();
        }
        public async Task<List<DoanhNghiepDto>> GetAllDoanhNghiepSearch(GetDoanhNghiepInput input)
        {
            var data = new List<DoanhNghiepDto>();
            var query = (from dn in _doanhNghiepRepos.GetAll()
                         where (string.IsNullOrEmpty(input.MaSoThue) || dn.MaSoThue.ToLower().Contains(input.MaSoThue))
                         select new DoanhNghiepDto
                         {
                             Id = dn.Id,
                             TenDoanhNghiep = dn.TenDoanhNghiep,
                             TinhId = dn.TinhId,
                             LoaiHinhDoanhNghiepId = dn.LoaiHinhDoanhNghiepId,
                             DiaChi = dn.DiaChi,
                             MaSoThue = dn.MaSoThue,
                             SoDienThoai = dn.SoDienThoai
                         })
                         .WhereIf(input.TinhId != null, u => u.TinhId == input.TinhId)
                         .WhereIf(input.LoaiHinhDoanhNghiepId != null, u => u.LoaiHinhDoanhNghiepId == input.LoaiHinhDoanhNghiepId);

            data = await query.ToListAsync();

            //if (!string.IsNullOrEmpty(input.MaSoThue))
            //{
            //    var lowerInput = RemoveUnicode(input.MaSoThue.Trim().ToLower());
            //    data.Where(p => RemoveUnicode(p.MaSoThue.Trim().ToLower()).Contains(lowerInput) || RemoveUnicode(p.TenDoanhNghiep.Trim().ToLower()).Contains(lowerInput));
            //}

            return data;

        }

        public DoanhNghiepDto GetDoanhNghiepById(long input)
        {
            var query = _doanhNghiepRepos.Get(input);
            var result = query.MapTo<DoanhNghiepDto>();
            return result;
        }

        public object GetDoanhNghiepByCurrentUser()
        {
            var userId = _session.UserId;
            var userInfo = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var dn = _doanhNghiepRepos.FirstOrDefault(x => x.Id == userInfo.DoanhNghiepId);
            if (dn != null)
            {
                if (!dn.HasCA.HasValue || dn.HasCA == true)
                {
                    dn.HasCA = true;
                }
                var loaihinh = _loaiHinhRepos.FirstOrDefault(x => x.Id == dn.LoaiHinhDoanhNghiepId);
                return new
                {
                    DoanhNghiep = dn,
                    LoaiHinh = loaihinh
                };
            }
            else
            {
                return null;
            }
        }
        public string UpdateDoanhNghiepInfo(DoanhNghiepInfoInput input)
        {
            try
            {
                var name = input.TenNguoiDaiDien.Split(new char[] { ' ' }, 2);
                if (name.Length > 1)
                {
                    var firstname = name[0];
                    var lastname = name[1];
                    var update = _doanhNghiepRepos.Get(input.Id);
                    if (update != null)
                    {
                        update.TenDoanhNghiep = input.TenDoanhNghiep;
                        update.TenTiengAnh = input.TenTiengAnh;
                        update.TenVietTat = input.TenVietTat;
                        update.LoaiHinhDoanhNghiepId = input.LoaiHinhDoanhNghiepId;
                        var loaihinh = _loaiHinhRepos.FirstOrDefault(x => x.Id == input.LoaiHinhDoanhNghiepId);
                        update.TenLoaiHinh = loaihinh != null ? loaihinh.TenLoaiHinh : "";

                        update.TinhId = input.TinhId;
                        var tinh = _tinhRepos.FirstOrDefault(x => x.Id == input.TinhId);
                        update.Tinh = tinh != null ? tinh.Ten : "";

                        update.HuyenId = input.HuyenId;
                        var huyen = _huyenRepos.FirstOrDefault(x => x.Id == input.HuyenId);
                        update.Huyen = huyen != null ? huyen.Ten : "";

                        update.XaId = input.XaId;
                        var xa = _xaRepos.FirstOrDefault(x => x.Id == input.XaId);
                        update.Xa = xa != null ? xa.Ten : "";

                        update.DiaChi = input.DiaChi;
                        update.SoDienThoai = input.SoDienThoai;
                        update.Fax = input.Fax;
                        update.EmailDoanhNghiep = input.EmailDoanhNghiep;
                        update.Website = input.Website;
                        update.TenNguoiDaiDien = input.TenNguoiDaiDien;
                        update.EmailXacNhan = input.EmailXacNhan;
                        update.ChucVuNguoiDaiDienID = input.ChucVuNguoiDaiDienID;
                        update.DienThoaiNguoiDaiDien = input.DienThoaiNguoiDaiDien;
                        update.HasCA = input.HasCA;
                        _doanhNghiepRepos.Update(update);



                        User vUser = null;
                        vUser = _userRepos.FirstOrDefault(x => x.Id == _session.UserId);
                        if (vUser != null)
                        {
                            vUser.Name = lastname;
                            vUser.Surname = firstname;
                            vUser.PhoneNumber = input.SoDienThoai;
                            vUser.EmailAddress = input.EmailDoanhNghiep;
                            _userRepos.Update(vUser);
                        }
                        CurrentUnitOfWork.SaveChanges();
                    }
                    return "ok";
                }
                else
                {
                    return "thieu_ten";
                }
            }
            catch (Exception ex)
            {
                return ex + "loi_khong_biet";
            }
        }

        private string SetPath(DoanhNghiep dn)
        {
            var path = "";
            path = !string.IsNullOrEmpty(dn.Tinh) ? RemoveUnicode(dn.Tinh).ToLower().Trim().Replace(" ", "-") : "unknown";
            path = path + @"\" + dn.MaSoThue + @"\hosophaply\";
            return path;
        }

        public async Task<PagedResultDto<DoanhNghiepDto>> GetAllDoanhNghiepServerPagingAsync(GetDoanhNghiepInput input)
        {
            var doanhNghiepCount = 0;
            var dataGrids = new List<DoanhNghiepDto>();

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                input.MaSoThue = Utility.StringExtensions.FomatAndKhongDau(input.MaSoThue);
                var query = (from doanhnghiep in _doanhNghiepRepos.GetAll()
                             join user_r in _userRepos.GetAll() on doanhnghiep.Id equals user_r.DoanhNghiepId into userJoined
                             from user in userJoined.DefaultIfEmpty()
                             join chucvu_r in _chucVuRepos.GetAll() on doanhnghiep.ChucVuNguoiDaiDienID equals chucvu_r.Id into chucvuJoined
                             from chucvu in chucvuJoined.DefaultIfEmpty()
                             where (string.IsNullOrEmpty(input.MaSoThue) || doanhnghiep.MaSoThue.ToLower().Contains(input.MaSoThue))
                             select new DoanhNghiepDto
                             {
                                 Id = doanhnghiep.Id,
                                 MaSoThue = doanhnghiep.MaSoThue,
                                 TenDoanhNghiep = doanhnghiep.TenDoanhNghiep,
                                 DiaChi = doanhnghiep.DiaChi,
                                 LoaiHinhDoanhNghiepId = doanhnghiep.LoaiHinhDoanhNghiepId,
                                 TenLoaiHinh = doanhnghiep.TenLoaiHinh,
                                 TenTiengAnh = doanhnghiep.TenTiengAnh,
                                 TenVietTat = doanhnghiep.TenVietTat,
                                 SoDienThoai = doanhnghiep.SoDienThoai,
                                 Fax = doanhnghiep.Fax,
                                 Website = doanhnghiep.Website,
                                 TinhId = doanhnghiep.TinhId,
                                 Tinh = doanhnghiep.Tinh,
                                 EmailDoanhNghiep = doanhnghiep.EmailDoanhNghiep,
                                 EmailXacNhan = doanhnghiep.EmailXacNhan,
                                 HuyenId = doanhnghiep.HuyenId,
                                 Huyen = doanhnghiep.Huyen,
                                 XaId = doanhnghiep.XaId,
                                 Xa = doanhnghiep.Xa,
                                 TenNguoiDaiDien = doanhnghiep.TenNguoiDaiDien,
                                 ChucVuNguoiDaiDienID = doanhnghiep.ChucVuNguoiDaiDienID,
                                 DienThoaiNguoiDaiDien = doanhnghiep.DienThoaiNguoiDaiDien,
                                 IsDaXuLy = doanhnghiep.IsDaXuLy,
                                 CreationTime = doanhnghiep.CreationTime,
                                 UserActive = user.IsActive,
                                 UserId = user.Id,
                                 TenChucVuNguoiDaiDien = chucvu.TenChucVu,
                                                              
                             })
                             .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.MaSoThue.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MaSoThue.LocDauLowerCaseDB().Contains(input.Filter.Replace("_", "-").LocDauLowerCaseDB()) || u.TenDoanhNghiep.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.TenLoaiHinh.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.TenNguoiDaiDien.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()))
                             .WhereIf(input.TinhId != null, u => u.TinhId == input.TinhId)
                             .WhereIf(input.LoaiHinhDoanhNghiepId != null, u => u.LoaiHinhDoanhNghiepId == input.LoaiHinhDoanhNghiepId);

                if (_session.TenantId.HasValue)
                {
                    var tenant = await _tenantManager.GetByIdAsync(_session.TenantId.Value);
                    if (tenant != null && tenant.TenancyName != "Default")
                    {
                        query = query.Where(x => x.TinhId == tenant.TinhId);
                    }
                }

                if (input.FormCase == (int)CommonENum.FORM_CASE_DOANH_NGHIEP.DOANH_NGHIEP_CHUA_DUOC_DUYET)
                {
                    query = query.Where(x => x.IsDaXuLy != true);
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_DOANH_NGHIEP.DUOC_DUYET_CHAP_NHAN)
                {
                    query = query.Where(x => x.IsDaXuLy == true && x.UserId != null);
                }
                else if (input.FormCase == (int)CommonENum.FORM_CASE_DOANH_NGHIEP.DUOC_DUYET_KHONG_CHAP_NHAN)
                {
                    query = query.Where(x => x.IsDaXuLy == true && x.UserId == null);
                }
                doanhNghiepCount = await query.CountAsync();
                dataGrids = await query
                   .OrderByDescending(p => p.CreationTime)
                   .PageBy(input)
                   .ToListAsync();

                foreach (var item in dataGrids)
                {
                    item.ThongTinPhapLy = _giayPhepPhapLyRepos.GetAll().Where(x => x.DoanhNghiepId == item.Id).ToList();
                }
            }
            return new PagedResultDto<DoanhNghiepDto>(doanhNghiepCount, dataGrids);
        }

        public async Task<PagedResultDto<DoanhNghiepDto>> DataConDauDoanhNghiep(GetDoanhNghiepInput input)
        {
            var query = (from a in _doanhNghiepRepos.GetAll()
                         join c in _chuKyRepos.GetAll() on a.Id equals c.PId
                         where c.LoaiChuKy == (int)CommonENum.LOAI_CHU_KY.CHU_KY
                         orderby c.IsDaXuLy, c.LastModificationTime descending
                         select new DoanhNghiepDto()
                         {
                             Id = a.Id,
                             MaSoThue = a.MaSoThue,
                             TenDoanhNghiep = a.TenDoanhNghiep,
                             DiaChi = a.DiaChi,
                             IsDaXuLy = a.IsDaXuLy,
                             chuKy = new ChuKyDto()
                             {
                                 Id = c.Id,
                                 IsActive = c.IsActive,
                                 LoaiChuKy = c.LoaiChuKy,
                                 MoTa = c.MoTa,
                                 MaChuKy = c.MaChuKy,
                                 TenChuKy = c.TenChuKy,
                                 UrlImage = c.UrlImage,
                                 DataImage = c.DataImage,
                                 ChanChuKy = c.ChanChuKy,
                                 ChieuCao = c.ChieuCao,
                                 ChieuRong = c.ChieuRong,
                                 IsDaXuLy = c.IsDaXuLy
                             }
                         }).WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.MaSoThue.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MaSoThue.LocDauLowerCaseDB().Contains(input.Filter.Replace("_", "-").LocDauLowerCaseDB()) || u.TenDoanhNghiep.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.TenLoaiHinh.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.TenNguoiDaiDien.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()))
                         .WhereIf(input.TinhId != null, u => u.TinhId == input.TinhId)
                         .WhereIf(input.LoaiHinhDoanhNghiepId != null, u => u.LoaiHinhDoanhNghiepId == input.LoaiHinhDoanhNghiepId);

            if (input.FormCase == (int)CommonENum.FORM_CASE_DOANH_NGHIEP.DOANH_NGHIEP_CHUA_DUOC_DUYET)
            {
                query = query.Where(x => x.chuKy.IsDaXuLy != true);
            }
            else if (input.FormCase == (int)CommonENum.FORM_CASE_DOANH_NGHIEP.DUOC_DUYET_CHAP_NHAN)
            {
                query = query.Where(x => x.chuKy.IsDaXuLy == true);
            }

            var dataGrids = await query.PageBy(input).ToListAsync();

            var doanhNghiepCount = await query.CountAsync();
            return new PagedResultDto<DoanhNghiepDto>(doanhNghiepCount, dataGrids);
        }

        [AbpAllowAnonymous]
        public async Task<long> CreateOrUpdateDoanhNghiepAsync(CreateOrUpdateDoanhNghiepInfoInput input)
        {
            if (input.DoanhNghiep.Id != 0)
            {
                // update
                var updateData = await _doanhNghiepRepos.GetAsync(input.DoanhNghiep.Id);

                input.DoanhNghiep.MapTo(updateData);

                await _doanhNghiepRepos.UpdateAsync(updateData);
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.DoanhNghiep.MapTo<DoanhNghiep>();
                    long id = await _doanhNghiepRepos.InsertAndGetIdAsync(insertInput);

                    #region HoSoPhapLy
                    var giayPheps = _giayPhepPhapLyRepos.GetAll().Where(a => a.DoanhNghiepId == id).ToList();
                    foreach (var item in giayPheps)
                    {
                        _giayPhepPhapLyRepos.Delete(item);
                    }

                    var path = SetPath(insertInput);
                    var temp = "";

                    foreach (var item in input.DoanhNghiep.GiayPhepPhapLys)
                    {
                        item.DoanhNghiepId = id;
                        item.DaTaiLen = true;
                        item.IsActive = true;

                        if (item.IsNew == true) temp = item.DuongDanTep;
                        item.DuongDanTep = item.IsNew == true ? CopyTo(item.DuongDanTep, path) : item.DuongDanTep;

                        _giayPhepPhapLyRepos.Insert(item);
                    }

                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        var index = temp.LastIndexOf("\\");
                        temp = temp.Substring(0, index);
                        EmptyFolder(temp);
                    }
                    #endregion

                    return id;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task DeleteDoanhNghiepAsync(EntityDto<long> input)
        {
            var DoanhNghiep = await _doanhNghiepRepos.GetAsync(input.Id);
            await _doanhNghiepRepos.DeleteAsync(DoanhNghiep);
        }

        [AbpAllowAnonymous]
        public async Task<string> TaoTaiKhoanDoanhNghiep(CreateOrUpdateDoanhNghiepInfoInput input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input.DoanhNghiep.TenDangNhap))
                {
                    if (checkExistTenDangNhap(input.DoanhNghiep.TenDangNhap))
                    {
                        return "ten_dang_nhap_da_co";
                    }
                    if (checkExistEmail(input.DoanhNghiep.EmailDoanhNghiep))
                    {
                        return "email_da_co";
                    }
                    if (IsValidEmail(input.DoanhNghiep.EmailXacNhan))
                    {
                        if (checkExistEmail(input.DoanhNghiep.EmailXacNhan))
                        {
                            return "email_da_co";
                        }
                        CreateOrUpdateUserInput userDoanhNghiep = new CreateOrUpdateUserInput();
                        UserEditDto userdto = new UserEditDto();
                        userdto.Surname = input.DoanhNghiep.SurName;
                        userdto.Name = input.DoanhNghiep.Name;
                        userdto.EmailAddress = input.DoanhNghiep.EmailXacNhan;
                        userdto.IsActive = false;
                        userdto.IsLockoutEnabled = true;
                        userdto.IsTwoFactorEnabled = false;
                        userdto.UserName = input.DoanhNghiep.TenDangNhap.Trim();
                        userdto.PhoneNumber = input.DoanhNghiep.SoDienThoai;
                        userdto.PhongBanId = null;
                        userdto.RoleLevel = (int)CommonENum.ROLE_LEVEL.DOANH_NGHIEP;
                        userdto.ShouldChangePasswordOnNextLogin = true;
                        userDoanhNghiep.SendActivationEmail = true;
                        userDoanhNghiep.SetRandomPassword = true;
                        userDoanhNghiep.User = userdto;
                        List<string> roleNames = new List<string>();

                        int tenancyDoanhNghiepId = 1;

                        if (!ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"].IsNullOrEmpty())
                        {
                            tenancyDoanhNghiepId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"]);
                        }

                        using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                        {
                            var roleDoanhNghiep = _roleManager.Roles.Where(x => x.IsDefault == true && x.TenantId == tenancyDoanhNghiepId).ToList();
                            if (roleDoanhNghiep != null && roleDoanhNghiep.Count() > 0)
                            {
                                foreach (var role in roleDoanhNghiep)
                                {
                                    roleNames.Add(role.Name);
                                }
                            }
                        }
                        userDoanhNghiep.AssignedRoleNames = roleNames.ToArray();
                        if (userDoanhNghiep.AssignedRoleNames.Length > 0)
                        {
                            using (var unitOfWork = _unitOfWorkManager.Begin())
                            {
                                var userId = await _userService.CreateUserFrontEndAsync(userDoanhNghiep);
                                if (input.DoanhNghiep.TinhId != null)
                                {
                                    var tinh = _tinhRepos.Get(input.DoanhNghiep.TinhId.Value);
                                    input.DoanhNghiep.Tinh = tinh != null ? tinh.Ten : "";
                                }
                                if (input.DoanhNghiep.HuyenId.HasValue)
                                {
                                    var huyen = _huyenRepos.Get(input.DoanhNghiep.HuyenId.Value);
                                    input.DoanhNghiep.Huyen = huyen != null ? huyen.Ten : "";
                                }
                                if (input.DoanhNghiep.XaId.HasValue)
                                {
                                    var xa = _xaRepos.Get(input.DoanhNghiep.XaId.Value);
                                    input.DoanhNghiep.Xa = xa != null ? xa.Ten : "";
                                }

                                if (input.DoanhNghiep.LoaiHinhDoanhNghiepId != null)
                                {
                                    var loaihinh = _loaiHinhRepos.Get(input.DoanhNghiep.LoaiHinhDoanhNghiepId.Value);
                                    input.DoanhNghiep.TenLoaiHinh = loaihinh != null ? loaihinh.TenLoaiHinh : "";
                                }
                                input.DoanhNghiep.IsDaXuLy = false;
                                input.DoanhNghiep.MaSoThue = input.DoanhNghiep.MaSoThue.Trim();

                                long id = await CreateOrUpdateDoanhNghiepAsync(input);

                                UpdateUserDoanhNghiepId(userId, id);

                                unitOfWork.Complete();
                            }
                        }
                        else
                        {
                            return "vai_tro_doanh_nghiep_khong_ton_tai";
                        }

                        return "ok";

                    }
                    else
                    {
                        return "email_khong_ton_tai";
                    }
                }
                else
                {
                    return "ten_dang_nhap_khong_hop_le";
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} TaoTaiKhoanDoanhNghiep {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return ex.Message;
            }
        }

        public void UpdateUserDoanhNghiepId(long _userId, long doanhNghiepId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var user = _userRepos.FirstOrDefault(x => x.Id == _userId);
                if (user != null)
                {
                    user.DoanhNghiepId = doanhNghiepId;
                    _userRepos.UpdateAsync(user);
                }
            }
        }

        [AbpAllowAnonymous]
        public bool checkExistEmail(string email, long id = 0)
        {
            var res = false;
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (!string.IsNullOrEmpty(email) && _userRepos.GetAll().Any(x => x.EmailAddress == email && x.DoanhNghiepId != id))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        [AbpAllowAnonymous]
        public bool checkExistTenDangNhap(string tenDangNhap)
        {
            var res = false;
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (!string.IsNullOrEmpty(tenDangNhap) && _userRepos.GetAll().Any(x => x.UserName == tenDangNhap))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }


        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task<string> ChangePasswordDoanhNghiep(ChangeDoanhNghiepPasswordInput input)
        {
            try
            {
                var doanhnghiep = _doanhNghiepRepos.FirstOrDefault(x => x.MaSoThue == input.MaSoThue);
                if (doanhnghiep != null)
                {
                    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var user = UserManager.Users.FirstOrDefault(x => x.DoanhNghiepId == doanhnghiep.Id);
                        if (user != null)
                        {
                            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
                            return L("OK");
                        }
                        else
                        {
                            return L("KhongTimThayDuLieu");
                        }
                    }
                }
                else
                {
                    return L("KhongTimThayDuLieu");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")} ChangePasswordDoanhNghiep {ex.Message} {JsonConvert.SerializeObject(ex)}");
                return "Lỗi dữ liệu";
            }
        }

        public async Task ChangeActiveDoanhNghiep(long input)
        {
            var doanhnghiep = _doanhNghiepRepos.Get(input);
            if (doanhnghiep != null)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var user = await _userRepos.FirstOrDefaultAsync(x => x.DoanhNghiepId == doanhnghiep.Id);
                    if (user != null)
                    {
                        user.IsActive = !user.IsActive;
                        await _userRepos.UpdateAsync(user);
                        if (user.IsActive == true)
                        {
                            await _userEmailer.SendEmailActiveUserAccountAsync(user);
                        }
                        else
                        {
                            await _userEmailer.SendEmailDeactiveUserAccountAsync(user);
                        }
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public List<ItemObj<int>> GetListFormCaseDoanhNghiep()
        {
            try
            {
                var _list = new List<ItemObj<int>>();
                foreach (object iEnumItem in Enum.GetValues(typeof(CommonENum.FORM_CASE_DOANH_NGHIEP)))
                {
                    _list.Add(new ItemObj<int>
                    {
                        Id = (int)iEnumItem,
                        Name = CommonENum.GetEnumDescription((CommonENum.FORM_CASE_DOANH_NGHIEP)(int)iEnumItem)
                    });
                }
                return _list;
            }
            catch
            {
                return null;
            }
        }

        public async Task MoTaiKhoanDoanhNghiep(long doanhNghiepId)
        {
            try
            {
                var doanhNghiep = await _doanhNghiepRepos.GetAsync(doanhNghiepId);
                if (doanhNghiep != null)
                {
                    doanhNghiep.IsDaXuLy = true;
                    await _doanhNghiepRepos.UpdateAsync(doanhNghiep);
                    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var user = await _userRepos.FirstOrDefaultAsync(x => x.DoanhNghiepId == doanhNghiep.Id);
                        if (user != null)
                        {
                            user.IsActive = true;
                            var userPassword = User.CreateRandomPassword();
                            user.Password = new PasswordHasher().HashPassword(userPassword);
                            await _userRepos.UpdateAsync(user);
                            user.SetNewEmailConfirmationCode();
                            await _userEmailer.SendEmailActivationLinkAsync(user, userPassword);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task KhongChapNhanDangKyDoanhNghiep(DuyetDoanhNghiepInput input)
        {
            try
            {
                var doanhNghiep = await _doanhNghiepRepos.GetAsync(input.Id);
                if (doanhNghiep != null)
                {
                    doanhNghiep.IsDaXuLy = true;
                    doanhNghiep.LyDoKhongDuyet = input.LyDoKhongDuyet;
                    await _doanhNghiepRepos.UpdateAsync(doanhNghiep);
                    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var user = await _userRepos.FirstOrDefaultAsync(x => x.DoanhNghiepId == doanhNghiep.Id);
                        if (user != null)
                        {
                            await _userEmailer.SendEmailDoNotExceptRegisterAsync(user, input.LyDoKhongDuyet);
                            await _userRepos.DeleteAsync(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}