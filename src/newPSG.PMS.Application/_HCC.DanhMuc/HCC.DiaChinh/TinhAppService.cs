using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.EntityDB;
using Abp.Application.Services;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Linq.Extensions;
using System.Data.Entity;
using Abp.Authorization;
using newPSG.PMS.Dto;
using newPSG.PMS.Common.Dto;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface ITinhAppService : IApplicationService
    {
        Task<PagedResultDto<TinhDto>> GetAllServerPaging(TinhInputDto input);
        Task<int> CreateOrUpdate(TinhDto input);
        Task Delete(int id);
        Task<List<TinhDto>> GetAllToDDL();
        object getAll();
    }
    #endregion

    #region MAIN
    public class TinhAppService : PMSAppServiceBase, ITinhAppService
    {
        private readonly IRepository<Tinh, int> _tinhRepos;
        private readonly ICacheManager _cacheManager;
        public TinhAppService(IRepository<Tinh, int> tinhRepos, 
                              ICacheManager cacheManager)
        {
            _tinhRepos = tinhRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<TinhDto>> GetAllServerPaging(TinhInputDto input)
        {
            var query = (from tinh in _tinhRepos.GetAll()
                         select new TinhDto
                         {
                             Id = tinh.Id,
                             IsActive = tinh.IsActive,
                             MoTa = tinh.MoTa,
                             Ten = tinh.Ten,
                             NiisId = tinh.NiisId
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Ten.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var tinhCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();
            return new PagedResultDto<TinhDto>(tinhCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(TinhDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _tinhRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _tinhRepos.UpdateAsync(updateData);
                _cacheManager.GetCache("tblTinh").Remove("GetAllTinh");
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<Tinh>();
                    await _tinhRepos.InsertAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    _cacheManager.GetCache("tblTinh").Remove("GetAllTinh");
                    return insertInput.Id;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task Delete(int id)
        {
            await _tinhRepos.DeleteAsync(id);
            _cacheManager.GetCache("tblTinh").Remove("GetAllTinh");
        }

        public async Task<List<TinhDto>> GetAllToDDL()
        {
            var query = from tinh in _tinhRepos.GetAll()
                        where ((tinh.IsActive == true))
                        orderby tinh.Ten
                        select new TinhDto
                        {
                            Ten = tinh.Ten,
                            Id = tinh.Id
                        };

            return await query.ToListAsync();
        }

        #region Cache
        public object getAll()
        {
            return _cacheManager.GetCache("tblTinh")
                .Get("GetAllTinh", () => getAllFromDB()) as object;
        }

        object getAllFromDB()
        {
            var query = from tinh in _tinhRepos.GetAll()
                        where ((tinh.IsActive == true))
                        orderby tinh.Ten
                        select new TinhDto
                        {
                            Ten = tinh.Ten,
                            Id = tinh.Id,
                        };

            var items = query.ToList();
            foreach(var item in items)
            {
                item.TenKhongDau = Utility.StringExtensions.ConvertKhongDau(item.Ten);
            }
            return items;
        }
        #endregion

    }
    #endregion
}
