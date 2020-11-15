using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IPhongBanAppService : IApplicationService
    {
        Task<PagedResultDto<PhongBanDto>> GetAllServerPaging(PhongBanInputDto input);
        Task<int> CreateOrUpdate(PhongBanDto input);
        Task Delete(int id);
        Task<List<ItemDto<int>>> GetAllToDDL();
        PhongBan GetPhongBanByCurrentUser();
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class PhongBanAppService : PMSAppServiceBase, IPhongBanAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<PhongBanLoaiHoSo> _phongBanLoaiHoSoRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        private readonly ICacheManager _cacheManager;
        public PhongBanAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<PhongBan> phongBanRepos,
                                  IRepository<PhongBanLoaiHoSo> phongBanLoaiHoSoRepos,
                                  IRepository<LoaiHoSo> loaiHoSoRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _phongBanRepos = phongBanRepos;
            _phongBanLoaiHoSoRepos = phongBanLoaiHoSoRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<PhongBanDto>> GetAllServerPaging(PhongBanInputDto input)
        {
            var listPB = (from pb in _phongBanLoaiHoSoRepos.GetAll() where pb.LoaiHoSoId == input.LoaiHoSoId select pb.PhongBanId).ToList();

            var query = (from pb in _phongBanRepos.GetAll()
                         where (input.LoaiHoSoId == null) || (input.LoaiHoSoId != null && listPB.Contains(pb.Id))
                         select new PhongBanDto
                         {
                             Id = pb.Id,
                             PId = pb.PId,
                             TenPhongBan = pb.TenPhongBan,
                             MaPhongBan = pb.MaPhongBan,
                             SoDienThoai = pb.SoDienThoai,
                             Fax = pb.Fax,
                             Email = pb.Email,
                             DiaChi = pb.DiaChi,
                             IsActive = pb.IsActive,
                             TenantId = pb.TenantId,
                             QuiTrinh = pb.QuiTrinh
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.TenPhongBan.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || p.MaPhongBan.Contains(input.Filter.Trim().LocDauLowerCaseDB()))
                         .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive);

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            foreach (var item in dataGrids)
            {
                item.ArrLoaiHoSo = (from hs in _loaiHoSoRepos.GetAll()
                                    join phs in _phongBanLoaiHoSoRepos.GetAll() on hs.Id equals phs.LoaiHoSoId
                                    where phs.PhongBanId == item.Id
                                    select new ItemObj<int>
                                    {
                                        Id = hs.Id,
                                        Name = hs.TenLoaiHoSo
                                    }).ToList();
            }

            return new PagedResultDto<PhongBanDto>(rowCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(PhongBanDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _phongBanRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _phongBanRepos.UpdateAsync(updateData);

                _phongBanLoaiHoSoRepos.Delete(p => p.PhongBanId == updateData.Id);

                foreach (var loaiHoSo in input.ArrLoaiHoSo)
                {
                    await _phongBanLoaiHoSoRepos.InsertAsync(new PhongBanLoaiHoSo
                    {
                        PhongBanId = updateData.Id,
                        LoaiHoSoId = loaiHoSo.Id,
                        QuiTrinh = updateData.QuiTrinh.HasValue ? updateData.QuiTrinh.Value : 0
                    });
                }
                return updateData.Id;
            }
            else
            {
                var insertData = input.MapTo<PhongBan>();
                int id = await _phongBanRepos.InsertAndGetIdAsync(insertData);

                foreach (var loaiHoSo in input.ArrLoaiHoSo)
                {
                    await _phongBanLoaiHoSoRepos.InsertAsync(new PhongBanLoaiHoSo
                    {
                        PhongBanId = id,
                        LoaiHoSoId = loaiHoSo.Id,
                        QuiTrinh = insertData.QuiTrinh.HasValue ? insertData.QuiTrinh.Value : 0
                    });
                }
                return id;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var phongBanLoaiHS = (from pl in _phongBanLoaiHoSoRepos.GetAll()
                                      where pl.PhongBanId == id
                                      select pl).ToList();
                foreach (var item in phongBanLoaiHS)
                {
                    await _phongBanLoaiHoSoRepos.DeleteAsync(item);
                }
                await _phongBanRepos.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from pb in _phongBanRepos.GetAll()
                        where ((pb.IsActive == true))
                        orderby pb.TenPhongBan
                        select new ItemDto<int>
                        {
                            Id = pb.Id,
                            Name = pb.TenPhongBan
                        };

            return await query.ToListAsync();
        }

        public PhongBan GetPhongBanByCurrentUser()
        {
            var userId = _session.UserId;
            var userInfo = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (userInfo == null) return null;

            var pb = userInfo.PhongBanId.HasValue ? _phongBanRepos.FirstOrDefault(a => a.Id == userInfo.PhongBanId) : null;
            return pb;
        }
    }
    #endregion

}
