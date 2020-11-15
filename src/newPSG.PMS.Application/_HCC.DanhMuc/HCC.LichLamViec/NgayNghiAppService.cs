using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.EntityDB;
using System.Data.Entity;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Authorization;
using Abp.Application.Services;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface INgayNghiAppService : IApplicationService
    {
        Task<int> CreateOrUpdate(NgayNghiDto input);
        Task Delete(int id);
        Task<PagedResultDto<NgayNghiDto>> GetAllServerPaging(NgayNghiInputDto input, int lichLamViecId);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class NgayNghiAppService : PMSAppServiceBase, INgayNghiAppService
    {
        private readonly IRepository<NgayNghi> _ngayNghiRepos;
        private readonly ICacheManager _cacheManager;
        public NgayNghiAppService(IRepository<NgayNghi> ngayNghiRepos,
                                  ICacheManager cacheManager)
        {
            _ngayNghiRepos = ngayNghiRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<NgayNghiDto>> GetAllServerPaging(NgayNghiInputDto input, int lichLamViecId)
        {
            var query = (from ngayNghi in _ngayNghiRepos.GetAll()
                         where ngayNghi.LichLamViecId == lichLamViecId
                         select new NgayNghiDto
                         {
                             Id = ngayNghi.Id,
                             IsActive = ngayNghi.IsActive,
                             LyDo = ngayNghi.LyDo,
                             LichLamViecId = ngayNghi.LichLamViecId,
                             NgayBatDau = ngayNghi.NgayBatDau,
                             NgayKetThuc = ngayNghi.NgayKetThuc,
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.LyDo.Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var ngayNghiCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<NgayNghiDto>(ngayNghiCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(NgayNghiDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _ngayNghiRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _ngayNghiRepos.UpdateAsync(updateData);
                _cacheManager.GetCache("tblLichLamViec").Remove("GetAllLichLamViec");
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<NgayNghi>();
                    int id = await _ngayNghiRepos.InsertAndGetIdAsync(insertInput);
                    _cacheManager.GetCache("tblLichLamViec").Remove("GetAllLichLamViec");
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
            await _ngayNghiRepos.DeleteAsync(id);
            _cacheManager.GetCache("tblLichLamViec").Remove("GetAllLichLamViec");
        }
    }
    #endregion
}
