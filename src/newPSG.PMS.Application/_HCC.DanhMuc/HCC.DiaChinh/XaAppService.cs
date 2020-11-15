using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Services
{

    #region INTERFACE
    public interface IXaAppService : IApplicationService
    {
        Task<PagedResultDto<XaDto>> GetAllServerPaging(XaInputDto input);
        Task<long> CreateOrUpdate(XaDto input);
        Task Delete(long input);
        Task<List<XaDto>> GetAllToDDL();
        object GetAll();
    }
    #endregion
    
    #region MAIN
    public class XaAppService : PMSAppServiceBase, IXaAppService
    {
        private readonly IRepository<Xa,long> _xaRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly ICacheManager _cacheManager;
        public XaAppService(IRepository<Xa,long> xaRepos,
                            IRepository<Huyen, long> huyenRepos,
                            IRepository<Tinh> tinhRepos,
                            IAbpSession abpSession, 
                            ICacheManager cacheManager)
        {
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _xaRepos = xaRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<XaDto>> GetAllServerPaging(XaInputDto input)
        {
            var query = (from xa in _xaRepos.GetAll()
                         join huyen in _huyenRepos.GetAll()
                         on xa.HuyenId equals huyen.Id
                         join tinh in _tinhRepos.GetAll()
                         on huyen.TinhId equals tinh.Id
                         select new XaDto
                         {
                             Id = xa.Id,
                             IsActive = xa.IsActive,
                             Ten = xa.Ten,
                             CapHanhChinh = xa.CapHanhChinh,
                             HuyenId = xa.HuyenId,
                             NiisId = xa.NiisId,
                             TinhId = tinh.Id,
                             StrTinh = tinh.Ten,
                             StrHuyen = huyen.Ten,
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Ten.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()) || u.StrHuyen.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.StrTinh.LocDauLowerCaseDB().Contains(input.Filter.Trim()))
                         .WhereIf(input.TinhId.HasValue, u => u.TinhId == input.TinhId.Value)
                         .WhereIf(input.HuyenId.HasValue, u => u.HuyenId == input.HuyenId.Value);

            var xaCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<XaDto>(xaCount, dataGrids);
        }

        public async Task<long> CreateOrUpdate(XaDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _xaRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _xaRepos.UpdateAsync(updateData);
                _cacheManager.GetCache("tblXa").Remove("GetAllXa");
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<Xa>();
                    await _xaRepos.InsertAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    _cacheManager.GetCache("tblXa").Remove("GetAllXa");
                    return insertInput.Id;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task Delete(long id)
        {
            await _xaRepos.DeleteAsync(id);
            _cacheManager.GetCache("tblXa").Remove("GetAllXa");
        }

        public async Task<List<XaDto>> GetAllToDDL()
        {
            var query = from xa in _xaRepos.GetAll()
                        //where ((xa.IsActive == true) && (xa.HuyenId == huyenId))
                        orderby xa.Ten
                        select new XaDto
                        {
                            Id = xa.Id,
                            Name = xa.Ten,
                            ParentId = xa.HuyenId,
                            Ten = xa.Ten,
                            HuyenId = xa.HuyenId
                        };

            return await query.ToListAsync();
        }

        #region Cache
        public object GetAll()
        {
            return _cacheManager.GetCache("tblXa")
                .Get("GetAllXa", () => GetAllFromDB()) as object;
        }

        object GetAllFromDB()
        {
            var query = from xa in _xaRepos.GetAll()
                        where ((xa.IsActive == true))
                        orderby xa.Id
                        select new
                        {
                            Ten = xa.Ten,
                            Id = xa.Id,
                            ParentId = xa.HuyenId,
                        };
            return query.ToList();
        }
        #endregion
    }
    #endregion
}
