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

    public interface IBoThuTucAppService : IApplicationService
    {
        Task<PagedResultDto<BoThuTucDto>> GetAllServerPaging(BoThuTucInputDto input);

        Task<int> CreateOrUpdate(BoThuTucDto input);

        Task Delete(int id);

        Task<BoThuTuc> GetBoThuTucById(int id);
    }

    #endregion INTERFACE

    #region MAIN

    [AbpAuthorize]
    public class BoThuTucAppService : PMSAppServiceBase, IBoThuTucAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<BoThuTuc> _boThuTucRepos;
        private readonly ICacheManager _cacheManager;

        public BoThuTucAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<BoThuTuc> boThuTucRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _boThuTucRepos = boThuTucRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<BoThuTucDto>> GetAllServerPaging(BoThuTucInputDto input)
        {
            var query = (from ct in _boThuTucRepos.GetAll()
                         select new BoThuTucDto
                         {
                             Id = ct.Id,
                             SoHoSo = ct.SoHoSo,
                             TenThuTuc = ct.TenThuTuc,
                             DuongDan = ct.DuongDan,
                             TenVBQPPL = ct.TenVBQPPL,
                             CoQuanThucHien = ct.CoQuanThucHien,
                             ThongTinChung = ct.ThongTinChung,
                             TrinhTuThucHien = ct.TrinhTuThucHien,
                             ThanhPhanHoSo = ct.ThanhPhanHoSo,
                             YeuCauDieuKien = ct.YeuCauDieuKien,
                             Anh = ct.Anh,
                             JsonThongTinKhac = ct.JsonThongTinKhac,
                             IsHome = ct.IsHome,
                             HanhChinhCap = ct.HanhChinhCap,
                             ThuTucId = ct.ThuTucId,
                             IsActive = ct.IsActive,
                             SortOrder = ct.SortOrder,
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), p => p.TenThuTuc.Contains(input.Filter.Trim()));

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<BoThuTucDto>(rowCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(BoThuTucDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _boThuTucRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _boThuTucRepos.UpdateAsync(updateData);
                return 1;
            }
            else
            {
                var insertData = input.MapTo<BoThuTuc>();
                int id = await _boThuTucRepos.InsertAndGetIdAsync(insertData);
                return id;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _boThuTucRepos.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
        }

        public async Task<BoThuTuc> GetBoThuTucById(int id)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    var boThuTucObj = await _boThuTucRepos.GetAsync(id);
                    return boThuTucObj;
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