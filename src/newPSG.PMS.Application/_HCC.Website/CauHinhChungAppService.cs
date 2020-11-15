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
using Abp.Domain.Uow;
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

    public interface ICauHinhChungAppService : IApplicationService
    {
        Task<PagedResultDto<CauHinhChungDto>> GetAllServerPaging(CauHinhChungInputDto input);

        Task<List<CauHinhChungDto>> GetAll();

        Task<int> CreateOrUpdate(CauHinhChungDto input);

        Task<int> UpdateAll(List<CauHinhChungDto> input);

        Task Delete(int id);

        Task<CauHinhChung> GetCauHinhChungById(int id);
    }

    #endregion INTERFACE

    #region MAIN

    [AbpAuthorize]
    public class CauHinhChungAppService : PMSAppServiceBase, ICauHinhChungAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<CauHinhChung> _systemConfigRepos;
        private readonly ICacheManager _cacheManager;

        public CauHinhChungAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<CauHinhChung> systemConfigRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _systemConfigRepos = systemConfigRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<CauHinhChungDto>> GetAllServerPaging(CauHinhChungInputDto input)
        {
            var query = (from st in _systemConfigRepos.GetAll()
                         orderby st.SortOrder
                         select new CauHinhChungDto
                         {
                             Id = st.Id,
                             TuKhoa = st.TuKhoa,
                             GiaTri = st.GiaTri,
                             KieuCauHinh = st.KieuCauHinh,
                             IsActive = st.IsActive,
                             MoTa = st.MoTa,
                             NhomCauHinh = st.NhomCauHinh
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.TuKhoa.Contains(input.Filter.Trim()));

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<CauHinhChungDto>(rowCount, dataGrids);
        }

        public async Task<int> UpdateAll(List<CauHinhChungDto> input)
        {
            foreach (var item in input)
            {
                var updateData = await _systemConfigRepos.GetAsync(item.Id);

                item.MapTo(updateData);

                await _systemConfigRepos.UpdateAsync(updateData);
            }
            return 1;
        }

        public async Task<int> CreateOrUpdate(CauHinhChungDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _systemConfigRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _systemConfigRepos.UpdateAsync(updateData);
                return 1;
            }
            else
            {
                var insertData = input.MapTo<CauHinhChung>();
                int id = await _systemConfigRepos.InsertAndGetIdAsync(insertData);
                return id;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _systemConfigRepos.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        public async Task<CauHinhChung> GetCauHinhChungById(int id)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    var systemConfigObj = await _systemConfigRepos.GetAsync(id);
                    return systemConfigObj;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<List<CauHinhChungDto>> GetAll()
        {
            var query = (from st in _systemConfigRepos.GetAll()
                         orderby st.SortOrder
                         select new CauHinhChungDto
                         {
                             Id = st.Id,
                             TuKhoa = st.TuKhoa,
                             GiaTri = st.GiaTri,
                             KieuCauHinh = st.KieuCauHinh,
                             IsActive = st.IsActive,
                             MoTa = st.MoTa,
                             NhomCauHinh = st.NhomCauHinh
                         }).ToListAsync();
            return query;
        }
    }

    #endregion MAIN
}