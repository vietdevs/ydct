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

    public interface IArticleAppService : IApplicationService
    {
        Task<PagedResultDto<ArticleDto>> GetAllServerPaging(ArticleInputDto input);

        Task<List<ArticleDto>> GetAll(int? roleLevel);

        Task<int> CreateOrUpdate(ArticleDto input);

        Task Delete(int id);
    }

    #endregion INTERFACE

    #region MAIN

    public class ArticleAppService : PMSAppServiceBase, IArticleAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<Article> _ArticleRepos;
        private readonly IRepository<Category> _categoryRepos;
        private readonly ICategoryAppService _categoryService;
        private readonly ICacheManager _cacheManager;

        public ArticleAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<Article> ArticleRepos,
                                  IRepository<Category> categoryRepos,
                                  ICategoryAppService categoryService,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _ArticleRepos = ArticleRepos;
            _categoryRepos = categoryRepos;
            _categoryService = categoryService;
            _cacheManager = cacheManager;
        }

        [AbpAuthorize]
        public async Task<PagedResultDto<ArticleDto>> GetAllServerPaging(ArticleInputDto input)
        {
            var query = (from ct in _ArticleRepos.GetAll()
                         join c in _categoryRepos.GetAll() on ct.CategoryId equals c.Id
                         select new ArticleDto
                         {
                             Id = ct.Id,
                             CategoryId = ct.CategoryId,
                             Description = ct.Description,
                             Details = ct.Details,
                             Title = ct.Title,
                             File = ct.File,
                             IsActive = ct.IsActive,
                             SortOrder = ct.SortOrder,
                             RoleLevel = ct.RoleLevel,
                             CategoryName = c.Title
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.Title.Contains(input.Filter.Trim()))
                         .WhereIf(input.CategoryId > 0, p => p.CategoryId == input.CategoryId);

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<ArticleDto>(rowCount, dataGrids);
        }

        [AbpAuthorize]
        public async Task<int> CreateOrUpdate(ArticleDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _ArticleRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _ArticleRepos.UpdateAsync(updateData);
                return 1;
            }
            else
            {
                var insertData = input.MapTo<Article>();
                int id = await _ArticleRepos.InsertAndGetIdAsync(insertData);
                return id;
            }
        }

        [AbpAuthorize]
        public async Task Delete(int id)
        {
            try
            {
                await _ArticleRepos.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        public async Task<List<ArticleDto>> GetAll(int? roleLevel)
        {
            var query = (from ct in _ArticleRepos.GetAll()
                         select new ArticleDto
                         {
                             Id = ct.Id,
                             CategoryId = ct.CategoryId,
                             Description = ct.Description,
                             Details = ct.Details,
                             Title = ct.Title,
                             File = ct.File,
                             SortOrder = ct.SortOrder,
                             RoleLevel = ct.RoleLevel
                         })
                         .WhereIf(roleLevel > 0, p => p.RoleLevel == roleLevel);
            return await query.ToListAsync();
        }
    }

    #endregion MAIN
}