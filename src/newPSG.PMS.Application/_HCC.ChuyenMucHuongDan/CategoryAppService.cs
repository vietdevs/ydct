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
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.Services
{
    #region INTERFACE

    public interface ICategoryAppService : IApplicationService
    {
        Task<PagedResultDto<CategoryDto>> GetAllServerPaging(CategoryInputDto input);
        PagedResultDto<CategoryDto> GetAllServerPagingLevel(CategoryInputDto input);

        Task<int> CreateOrUpdate(CategoryDto input);
        Task Delete(int id);

        List<ItemWebDto<int>> AllCategoryToDDL(CategoryInputDto input);
        List<CategoryDto> AllCategory(CategoryInputDto input);
        List<CategoryDto> AllCategoryItems(CategoryInputDto input);

        Task TaoLinkHDSD();
    }

    #endregion INTERFACE

    #region MAIN

    public class CategoryAppService : PMSAppServiceBase, ICategoryAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<Category> _categoryRepos;
        private readonly IRepository<Article> _articleRepos;
        private readonly ICacheManager _cacheManager;

        public CategoryAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<Category> categoryRepos,
                                  IRepository<Article> articleRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _categoryRepos = categoryRepos;
            _articleRepos = articleRepos;
            _cacheManager = cacheManager;
        }

        #region Danh Sách & View

        [AbpAuthorize]
        public async Task<PagedResultDto<CategoryDto>> GetAllServerPaging(CategoryInputDto input)
        {
            var query = (from ct in _categoryRepos.GetAll()
                         select new CategoryDto
                         {
                             Id = ct.Id,
                             Title = ct.Title,
                             Description = ct.Description,
                             Details = ct.Details,
                             File = ct.File,
                             ParentId = ct.ParentId,
                             Level = ct.Level,
                             IsActive = ct.IsActive,
                             SortOrder = ct.SortOrder,
                             RoleLevel = ct.RoleLevel
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.Title.Contains(input.Filter.Trim()))
                         .WhereIf(input.RoleLevel.HasValue, p => p.RoleLevel == input.RoleLevel);

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.SortOrder)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<CategoryDto>(rowCount, dataGrids);
        }

        [AbpAuthorize]
        public PagedResultDto<CategoryDto> GetAllServerPagingLevel(CategoryInputDto input)
        {
            var arrList = AllCategory(input);

            var rowCount = arrList.Count();
            var dataGrids = arrList
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<CategoryDto>(rowCount, dataGrids);
        }

        #region Cây danh mục - Phân trang

        [AbpAuthorize]
        public List<CategoryDto> AllCategory(CategoryInputDto input)
        {
            var _queryList = (from c in _categoryRepos.GetAll()
                              select new CategoryDto
                              {
                                  Id = c.Id,
                                  Title = c.Title,
                                  Description = c.Description,
                                  GroupEnum = c.GroupEnum,
                                  Details = c.Details,
                                  File = c.File,
                                  ParentId = c.ParentId,
                                  Level = c.Level,
                                  IsActive = c.IsActive,
                                  SortOrder = c.SortOrder,
                                  RoleLevel = c.RoleLevel
                              })
                            .WhereIf(input.GroupEnum.HasValue, p => p.GroupEnum == input.GroupEnum)
                            .WhereIf(input.RoleLevel.HasValue, x => x.RoleLevel == input.RoleLevel);

            List<CategoryDto> _arList = new List<CategoryDto>();

            var arrCate = _queryList.Where(x => x.Level == 0 || x.Level == null).OrderBy(x => x.SortOrder).ToList();
            foreach (var cate in arrCate)
            {
                _arList.Add(cate);
                SubCategory(cate.Id, _queryList, _arList);
            }
            return _arList;
        }

        private void SubCategory(int? parentId, IQueryable<CategoryDto> queryList, List<CategoryDto> arrList)
        {
            var subcate = queryList.Where(x => x.ParentId == parentId).OrderBy(x => x.SortOrder).ToList();
            if (subcate.Count() > 0)
            {
                foreach (var cate in subcate)
                {
                    arrList.Add(cate);
                    SubCategory(cate.Id, queryList, arrList);
                }
            }
        }
        #endregion

        #endregion

        #region Thêm, Sửa, Xóa
        [AbpAuthorize]
        public async Task<int> CreateOrUpdate(CategoryDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _categoryRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _categoryRepos.UpdateAsync(updateData);
                return 1;
            }
            else
            {
                var insertData = input.MapTo<Category>();
                int id = await _categoryRepos.InsertAndGetIdAsync(insertData);
                return id;
            }
        }

        [AbpAuthorize]
        public async Task Delete(int id)
        {
            try
            {
                await _categoryRepos.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }
        #endregion

        #region DropDownList - Đệ quy

        [AbpAuthorize]
        public List<ItemWebDto<int>> AllCategoryToDDL(CategoryInputDto input)
        {
            var queryList = (from c in _categoryRepos.GetAll()
                             select new ItemWebDto<int>
                             {
                                 Id = c.Id,
                                 Title = c.Title,
                                 GroupEnum = c.GroupEnum,
                                 ParentId = c.ParentId,
                                 Level = c.Level,
                                 RoleLevel = c.RoleLevel,
                                 SortOrder = c.SortOrder
                             })
                            .WhereIf(input.GroupEnum.HasValue, p => p.GroupEnum == input.GroupEnum)
                            .WhereIf(input.CurrentId.HasValue, x => x.Id != input.CurrentId);

            List<ItemWebDto<int>> arrList = new List<ItemWebDto<int>>();

            var arrCate = queryList.Where(x => x.Level == 0 || x.Level == null).OrderBy(x => x.SortOrder).ToList();
            foreach (var cate in arrCate)
            {
                arrList.Add(cate);
                SubCategoryToDDL(cate.Id, "--- ", queryList, arrList);
            }
            return arrList;
        }

        private void SubCategoryToDDL(int? parentId, string space, IQueryable<ItemWebDto<int>> queryList, List<ItemWebDto<int>> arrList)
        {
            var subcate = queryList.Where(x => x.ParentId == parentId).OrderBy(x => x.SortOrder).ToList();
            if (subcate.Count() > 0)
            {
                foreach (var cate in subcate)
                {
                    cate.Title = space + cate.Title;
                    arrList.Add(cate);
                    SubCategoryToDDL(cate.Id, space + "--- ", queryList, arrList);
                }
            }
        }
        #endregion

        #region Cây danh mục - Items
        public List<CategoryDto> AllCategoryItems(CategoryInputDto input)
        {
            var _queryList = (from c in _categoryRepos.GetAll()
                              select new CategoryDto
                              {
                                  Id = c.Id,
                                  Title = c.Title,
                                  Description = c.Description,
                                  GroupEnum = c.GroupEnum,
                                  Details = c.Details,
                                  File = c.File,
                                  ParentId = c.ParentId,
                                  Level = c.Level,
                                  IsActive = c.IsActive,
                                  SortOrder = c.SortOrder,
                                  RoleLevel = c.RoleLevel
                              })
                             .WhereIf(input.GroupEnum.HasValue, p => p.GroupEnum == input.GroupEnum)
                             .WhereIf(input.RoleLevel.HasValue, x => x.RoleLevel == input.RoleLevel);

            List<CategoryDto> _arList = new List<CategoryDto>();

            var arrCate = _queryList.Where(x => x.Level == 0 || x.Level == null).OrderBy(x => x.SortOrder).ToList();
            foreach (var cate in arrCate)
            {
                cate.Items = _queryList.Where(x => x.ParentId == cate.Id).ToList();
                var arrBaiViet = _articleRepos.GetAll().Where(x => x.CategoryId == cate.Id).ToList();
                cate.ArrArticles = arrBaiViet.MapTo<List<ArticleDto>>();

                _arList.Add(cate);
                SubCategory(cate.Id, _queryList, _arList);
            }
            return _arList;
        }
        private void SubCategoryItems(int? parentId, IQueryable<CategoryDto> queryList, List<CategoryDto> arrList)
        {
            var subcate = queryList.Where(x => x.ParentId == parentId).OrderBy(x => x.SortOrder).ToList();
            if (subcate.Count() > 0)
            {
                foreach (var cate in subcate)
                {
                    cate.Items = queryList.Where(x => x.ParentId == cate.Id).ToList();
                    var arrBaiViet = _articleRepos.GetAll().Where(x => x.CategoryId == cate.Id).ToList();
                    cate.ArrArticles = arrBaiViet.MapTo<List<ArticleDto>>();
                    arrList.Add(cate);
                    SubCategoryItems(cate.Id, queryList, arrList);
                }
            }
        }
        #endregion

        #region Tạo Link HDSD
        public async Task TaoLinkHDSD()
        {
            var listcate = await _categoryRepos.GetAll().Where(x => x.IsActive == true).ToListAsync();
            if (listcate != null)
            {
                foreach (var item in listcate)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        var _url = item.Title.Replace(" ", "-").Replace(",", "-").Replace(".", "-");
                        item.Description = RemoveUnicode(_url).ToLower();
                        await _categoryRepos.UpdateAsync(item);
                    }
                }
            }
            var listarticle = await _articleRepos.GetAll().Where(x => x.IsActive == true).ToListAsync();
            if (listarticle != null)
            {
                foreach (var item in listarticle)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        var _url = item.Title.Replace(" ", "-").Replace(",", "-").Replace(".", "-");
                        item.Description = RemoveUnicode(_url).ToLower();
                        await _articleRepos.UpdateAsync(item);
                    }
                }
            }
        }
        #endregion
    }

    #endregion MAIN
}