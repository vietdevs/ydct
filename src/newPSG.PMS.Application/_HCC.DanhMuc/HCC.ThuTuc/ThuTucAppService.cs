using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using Newtonsoft.Json;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IThuTucAppService : IApplicationService
    {
        Task<PagedResultDto<ThuTucDto>> GetAllServerPaging(ThuTucInputDto input);
        Task<int> CreateOrUpdate(ThuTucDto input);
        Task Delete(int id);
        Task<ThuTucDto> GetById(int id);
        Task<List<ItemDto<int>>> GetAllToDDL();

        #region Lựa chọn thủ tục
        Task<List<ThuTucDto>> GetLuaChonThuTuc(ThuTucInputDto input);
        Task<ExcuteZeroResult> Favorites(long Id);
        Task<List<ThuTucDto>> DataThuTucYeuThich();
        #endregion
    }
    #endregion

    #region MAIN
    public class ThuTucAppService : PMSAppServiceBase, IThuTucAppService
    {
        private readonly IRepository<ThuTuc> _ThuTucRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<HCCSetting> _hCCSettingRepos;
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoHoSoRepos;

        public ThuTucAppService(IRepository<LoaiHoSo> loaiHoSoHoSoRepos, IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos, UserManager userManager, IAbpSession session, IRepository<HCCSetting> hCCSettingRepos, IRepository<ThuTuc> ThuTucRepos,
                                  IRepository<PhongBan> phongBanRepos)
        {
            _ThuTucRepos = ThuTucRepos;
            _phongBanRepos = phongBanRepos;
            _hCCSettingRepos = hCCSettingRepos;
            _session = session;
            _userManager = userManager;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _loaiHoSoHoSoRepos = loaiHoSoHoSoRepos;
        }

        public async Task<PagedResultDto<ThuTucDto>> GetAllServerPaging(ThuTucInputDto input)
        {
            try
            {
                var query = (from a in _ThuTucRepos.GetAll()
                             select new ThuTucDto
                             {
                                 Id = a.Id,
                                 TenThuTuc = a.TenThuTuc,
                                 IsActive = a.IsActive,
                                 MoTa = a.MoTa,
                                 QuiTrinhXuLy = a.QuiTrinhXuLy,
                                 PhiXuLy = a.PhiXuLy,
                                 SoNgayXuLy = a.SoNgayXuLy,
                                 DataImage = a.DataImage,
                                 CoQuanThucHien = a.CoQuanThucHien,
                                 IsPhiXuLy = a.IsPhiXuLy,
                                 LinhVuc = a.LinhVuc,
                                 MaThuTuc = a.MaThuTuc,
                                 PathImage = a.PathImage,
                                 ThuTucIdEnum = a.ThuTucIdEnum,
                             })
                .WhereIf(!string.IsNullOrEmpty(input.Filter),
                                u => u.TenThuTuc.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB())
                                || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

                var ThuTucCount = await query.CountAsync();
                var ThuTucGrids = await query
                    .OrderBy(p => p.Id)
                    .PageBy(input)
                   .ToListAsync();

                #region ThuThuc YeuThich
                var userId = _session.UserId;
                string name = "Setting.Favorites";
                var setting = _hCCSettingRepos.GetAll().FirstOrDefault(a => a.Name == name && a.UserId == userId);
                var ids = new List<long>();
                if (setting != null && !string.IsNullOrEmpty(setting.Value))
                {
                    ids = JsonConvert.DeserializeObject<List<long>>(setting.Value);
                }
                #endregion

                var user = await UserManager.GetUserByIdAsync(_session.UserId.Value);
                var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

                grantedPermissions = grantedPermissions.Where(a => a.Name.Contains("Pages.ThuTuc")).ToList();

                foreach (var item in ThuTucGrids)
                {
                    if (ids.Any(a => a == item.Id)) item.Css = "font-yellow-crusta";
                    else item.Css = "font-default";

                    var str = item.ThuTucIdEnum.Value.ToString().Length == 1 ?
                        ($"Pages.ThuTuc0{item.ThuTucIdEnum.Value.ToString()}") : ($"Pages.ThuTuc{item.ThuTucIdEnum.Value.ToString()}");

                    item.IsRole = grantedPermissions.Any(a => a.Name == str);
                    item.TenKhongDau = Utility.StringExtensions.ConvertKhongDau(item.TenThuTuc);
                }

                //ThuTucGrids = ThuTucGrids.Where(a => a.IsRole == true).ToList();
                return new PagedResultDto<ThuTucDto>(ThuTucCount, ThuTucGrids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateOrUpdate(ThuTucDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _ThuTucRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _ThuTucRepos.UpdateAsync(updateData);
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<ThuTuc>();
                    int id = await _ThuTucRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return id;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task Delete(int id)
        {
            await _ThuTucRepos.DeleteAsync(id);
        }

        public async Task<ThuTucDto> GetById(int id)
        {
            if (id > 0)
            {
                // update
                var ThuTuc = await _ThuTucRepos.GetAsync(id);
                var res = ThuTuc.MapTo<ThuTucDto>();
                return res;
            }
            return null;
        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from lhs in _ThuTucRepos.GetAll()
                        where ((lhs.IsActive == true))
                        orderby lhs.TenThuTuc
                        select new ItemDto<int>
                        {
                            Id = lhs.ThuTucIdEnum.HasValue ? lhs.ThuTucIdEnum.Value : 0,
                            Name = "(" + lhs.MaThuTuc + ") " + lhs.TenThuTuc
                        };

            return await query.ToListAsync();
        }

        #region Lựa chọn thủ tục

        public async Task<List<ThuTucDto>> GetLuaChonThuTuc(ThuTucInputDto input)
        {
            try
            {
                #region ThuThuc YeuThich
                var userId = _session.UserId;
                string name = "Setting.Favorites";
                var setting = _hCCSettingRepos.GetAll().FirstOrDefault(a => a.Name == name && a.UserId == userId);
                var ids = new List<long>();
                if (setting != null && !string.IsNullOrEmpty(setting.Value))
                {
                    ids = JsonConvert.DeserializeObject<List<long>>(setting.Value);
                }
                #endregion

                var query = (from a in _ThuTucRepos.GetAll()
                             select new ThuTucDto
                             {
                                 Id = a.Id,
                                 TenThuTuc = a.TenThuTuc,
                                 IsActive = a.IsActive,
                                 MoTa = a.MoTa,
                                 QuiTrinhXuLy = a.QuiTrinhXuLy,
                                 PhiXuLy = a.PhiXuLy,
                                 SoNgayXuLy = a.SoNgayXuLy,
                                 DataImage = a.DataImage,
                                 CoQuanThucHien = a.CoQuanThucHien,
                                 IsPhiXuLy = a.IsPhiXuLy,
                                 LinhVuc = a.LinhVuc,
                                 MaThuTuc = a.MaThuTuc,
                                 PathImage = a.PathImage,
                                 ThuTucIdEnum = a.ThuTucIdEnum
                             })
                            .WhereIf(!string.IsNullOrEmpty(input.Filter),
                                u => u.TenThuTuc.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB())
                                || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

                var ThuTucGrids = await query
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                #region ThuTuc DuocPhanQuyen
                var user = await UserManager.GetUserByIdAsync(_session.UserId.Value);
                var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

                grantedPermissions = grantedPermissions.Where(a => a.Name.Contains("Pages.ThuTuc")).ToList();

                foreach (var item in ThuTucGrids)
                {
                    if (ids.Any(a => a == item.Id)) item.Css = "font-yellow-crusta";
                    else item.Css = "font-default";

                    var str = item.ThuTucIdEnum.Value.ToString().Length == 1 ?
                        ($"Pages.ThuTuc0{item.ThuTucIdEnum.Value.ToString()}") : ($"Pages.ThuTuc{item.ThuTucIdEnum.Value.ToString()}");

                    item.IsRole = grantedPermissions.Any(a => a.Name == str);
                    item.TenKhongDau = Utility.StringExtensions.ConvertKhongDau(item.TenThuTuc);
                }

                ThuTucGrids = ThuTucGrids.Where(a => a.IsRole == true).ToList();
                #endregion

                return ThuTucGrids;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ExcuteZeroResult> Favorites(long Id)
        {
            var result = new ExcuteZeroResult();
            try
            {
                var userId = _session.UserId;
                string name = "Setting.Favorites";
                var query = _hCCSettingRepos.GetAll().FirstOrDefault(a => a.Name == name && a.UserId == userId);
                if (query != null)
                {
                    var ids = new List<long>();
                    if (!string.IsNullOrEmpty(query.Value))
                    {
                        ids = JsonConvert.DeserializeObject<List<long>>(query.Value);
                    }

                    if (ids.Any(a => a == Id)) ids = ids.Where(a => a != Id).ToList();
                    else ids.Add(Id);

                    query.Value = JsonConvert.SerializeObject(ids);
                    await _hCCSettingRepos.UpdateAsync(query);
                }
                else
                {
                    var ids = new List<long>();
                    ids.Add(Id);

                    var obj = new HCCSetting();
                    obj.UserId = userId;
                    obj.Name = name;
                    obj.TenantId = _session.TenantId;
                    obj.Value = JsonConvert.SerializeObject(ids);
                    await _hCCSettingRepos.InsertAsync(obj);
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<List<ThuTucDto>> DataThuTucYeuThich()
        {

            #region ThuThuc YeuThich
            var userId = _session.UserId;
            string name = "Setting.Favorites";
            var setting = _hCCSettingRepos.GetAll().FirstOrDefault(a => a.Name == name && a.UserId == userId);
            var ids = new List<long>();
            if (setting != null && !string.IsNullOrEmpty(setting.Value))
            {
                ids = JsonConvert.DeserializeObject<List<long>>(setting.Value);
            }
            #endregion

            var ThuTucGrids = await _ThuTucRepos.GetAll().Where(a => ids.Contains(a.Id))
                .Select(a => new ThuTucDto()
                {
                    Id = a.Id,
                    CoQuanThucHien = a.CoQuanThucHien,
                    TenThuTuc = a.TenThuTuc,
                    MaThuTuc = a.MaThuTuc,
                    ThuTucIdEnum = a.ThuTucIdEnum,
                    MoTa = a.MoTa
                }).ToListAsync();

            #region ThuTuc DuocPhanQuyen
            var user = await UserManager.GetUserByIdAsync(_session.UserId.Value);
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            grantedPermissions = grantedPermissions.Where(a => a.Name.Contains("Pages.ThuTuc")).ToList();

            foreach (var item in ThuTucGrids)
            {
                var str = item.ThuTucIdEnum.Value.ToString().Length == 1 ?
                    ($"Pages.ThuTuc0{item.ThuTucIdEnum.Value.ToString()}") : ($"Pages.ThuTuc{item.ThuTucIdEnum.Value.ToString()}");

                item.IsRole = grantedPermissions.Any(a => a.Name == str);
                item.TenKhongDau = Utility.StringExtensions.ConvertKhongDau(item.TenThuTuc);
            }

            ThuTucGrids = ThuTucGrids.Where(a => a.IsRole == true).ToList();
            #endregion

            return ThuTucGrids;
        }
        #endregion
    }
    #endregion
}