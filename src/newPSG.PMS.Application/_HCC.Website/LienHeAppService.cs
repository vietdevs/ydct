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

    public interface ILienHeAppService : IApplicationService
    {
        Task<PagedResultDto<LienHeDto>> GetAllServerPaging(LienHeInputDto input);

        Task<int> CreateOrUpdate(LienHeDto input);

        Task Delete(int id);

        Task<LienHe> GetLienHeById(int id);
    }

    #endregion INTERFACE

    #region MAIN

    [AbpAuthorize]
    public class LienHeAppService : PMSAppServiceBase, ILienHeAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<LienHe> _blogRepos;
        private readonly ICacheManager _cacheManager;

        public LienHeAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<LienHe> blogRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _blogRepos = blogRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<LienHeDto>> GetAllServerPaging(LienHeInputDto input)
        {
            var query = (from ct in _blogRepos.GetAll()
                         select new LienHeDto
                         {
                             Id = ct.Id,
                             HoTen = ct.HoTen,
                             DienThoai = ct.DienThoai,
                             Email = ct.Email
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.HoTen.Contains(input.Filter.Trim()));

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<LienHeDto>(rowCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(LienHeDto input)
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
                var insertData = input.MapTo<LienHe>();
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

        public async Task<LienHe> GetLienHeById(int id)
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