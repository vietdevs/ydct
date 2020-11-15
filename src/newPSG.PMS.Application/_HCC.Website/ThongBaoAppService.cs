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

    public interface IThongBaoAppService : IApplicationService
    {
        Task<PagedResultDto<ThongBaoDto>> GetAllServerPaging(ThongBaoInputDto input);

        Task<int> CreateOrUpdate(ThongBaoDto input);

        Task Delete(int id);

        Task<ThongBao> GetThongBaoById(int id);
    }

    #endregion INTERFACE

    #region MAIN

    [AbpAuthorize]
    public class ThongBaoAppService : PMSAppServiceBase, IThongBaoAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<ThongBao> _blogRepos;
        private readonly ICacheManager _cacheManager;

        public ThongBaoAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<ThongBao> blogRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _blogRepos = blogRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<ThongBaoDto>> GetAllServerPaging(ThongBaoInputDto input)
        {
            var query = (from ct in _blogRepos.GetAll()
                         select new ThongBaoDto
                         {
                             Id = ct.Id,
                             TenThongBao = ct.TenThongBao,
                             DuongDan = ct.DuongDan,
                             Anh = ct.Anh,
                             MoTa = ct.MoTa,
                             ChiTiet = ct.ChiTiet,
                             TaiLieu = ct.TaiLieu,
                             JsonThongTinKhac = ct.JsonThongTinKhac,
                             IsHome = ct.IsHome,
                             IsActive = ct.IsActive,
                             SortOrder = ct.SortOrder
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.TenThongBao.Contains(input.Filter.Trim()));

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<ThongBaoDto>(rowCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(ThongBaoDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _blogRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _blogRepos.UpdateAsync(updateData);
                return 1;
            }
            else
            {
                var insertData = input.MapTo<ThongBao>();
                int id = await _blogRepos.InsertAndGetIdAsync(insertData);
                return id;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _blogRepos.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        public async Task<ThongBao> GetThongBaoById(int id)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    var blogObj = await _blogRepos.GetAsync(id);
                    return blogObj;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    #endregion MAIN
}