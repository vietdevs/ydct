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
    public interface IHuyenAppService : IApplicationService
    {
        Task<PagedResultDto<HuyenDto>> GetAllServerPaging(HuyenInputDto input);
        Task<long> CreateOrUpdate(HuyenDto input);
        Task Delete(long id);
        Task<List<HuyenDto>> GetAllToDDL();
        Task<List<HuyenDto>> GetHuyenByIdTinhToDDL(long TinhId);
        object GetAll();
    }
    #endregion

    #region MAIN
    public class HuyenAppService : PMSAppServiceBase, IHuyenAppService
    {
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IAbpSession _abpSession;
        private readonly ICacheManager _cacheManager;
        public HuyenAppService(IRepository<Huyen, long> huyenRepos,
                               IRepository<Tinh> tinhRepos,
                               IAbpSession abpSession,
                               ICacheManager cacheManager)
        {
            _huyenRepos = huyenRepos;
            _tinhRepos = tinhRepos;
            _abpSession = abpSession;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<HuyenDto>> GetAllServerPaging(HuyenInputDto input)
        {
            var query = (from huyen in _huyenRepos.GetAll()
                         join tinh in _tinhRepos.GetAll()
                         on huyen.TinhId equals tinh.Id
                         select new HuyenDto
                         {
                             Id = huyen.Id,
                             IsActive = huyen.IsActive,
                             Ten = huyen.Ten,
                             TinhId = huyen.TinhId,
                             NiisId = huyen.NiisId,
                             CapHanhChinh = huyen.CapHanhChinh,
                             StrTinh = tinh.Ten
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Ten.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()) || u.StrTinh.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()))
            .WhereIf(input.TinhId.HasValue, u => u.TinhId == input.TinhId.Value);


            var huyenCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();
            return new PagedResultDto<HuyenDto>(huyenCount, dataGrids);
        }

        public async Task<long> CreateOrUpdate(HuyenDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _huyenRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _huyenRepos.UpdateAsync(updateData);
                _cacheManager.GetCache("tblHuyen").Remove("getAllHuyen");
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<Huyen>();
                    await _huyenRepos.InsertAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    _cacheManager.GetCache("tblHuyen").Remove("getAllHuyen");
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
            var huyen = await _huyenRepos.GetAsync(id);
            await _huyenRepos.DeleteAsync(huyen);
            _cacheManager.GetCache("tblHuyen").Remove("getAllHuyen");
        }

        public async Task<List<HuyenDto>> GetAllToDDL()
        {
            var query = from huyen in _huyenRepos.GetAll()
                            //where ((huyen.IsActive == true) && (huyen.TinhId == tinhId))
                        orderby huyen.Ten
                        select new HuyenDto
                        {
                            Id = huyen.Id,
                            Name = huyen.Ten,
                            ParentId = huyen.TinhId,
                            Ten= huyen.Ten,
                            TinhId = huyen.TinhId
                        };

            return await query.ToListAsync();
        }

        public async Task<List<HuyenDto>> GetHuyenByIdTinhToDDL(long TinhId = -1)
        {
            var query = from huyen in _huyenRepos.GetAll()
                        orderby huyen.Ten
                        select new HuyenDto
                        {
                            Id = huyen.Id,
                            Name = huyen.Ten,
                            ParentId = huyen.TinhId,
                            Ten = huyen.Ten,
                            TinhId = huyen.TinhId
                        };
            if (TinhId >= 0)
            {
                query = query.Where(p => p.TinhId == TinhId);
            }
            return await query.ToListAsync();
        }

        #region Cache
        public object GetAll()
        {
            return _cacheManager.GetCache("tblHuyen")
                .Get("getAllHuyen", () => getAllFromDB()) as object;
        }

        object getAllFromDB()
        {
            var query = from huyen in _huyenRepos.GetAll()
                        where ((huyen.IsActive == true))
                        orderby huyen.Id
                        select new
                        {
                            Ten = huyen.Ten,
                            Id = huyen.Id,
                            ParentId = huyen.TinhId,
                        };
            return query.ToList();
        }


        #endregion

    }
    #endregion
}
