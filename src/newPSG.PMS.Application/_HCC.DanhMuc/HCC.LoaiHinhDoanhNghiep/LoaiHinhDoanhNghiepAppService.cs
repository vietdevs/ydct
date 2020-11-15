using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface ILoaiHinhDoanhNghiepAppService : IApplicationService
    {
        Task<PagedResultDto<LoaiHinhDoanhNghiepDto>> GetAllServerPaging(GetLoaiHinhDoanhNghiepInputDto input);
        Task<int> CreateOrUpdate(LoaiHinhDoanhNghiepDto input);
        Task Delete(int id);
        Task<List<ItemDto<int>>> GetAllToDDL();
        object GetAll();
    }
    #endregion

    #region MAIN
    public class LoaiHinhDoanhNghiepAppService : PMSAppServiceBase, ILoaiHinhDoanhNghiepAppService
    {
        private readonly IRepository<LoaiHinhDoanhNghiep> _loaiHinhDoanhNghiepRepos;
        private readonly ICacheManager _cacheManager;

        public LoaiHinhDoanhNghiepAppService(IRepository<LoaiHinhDoanhNghiep> loaiHinhDoanhNghiepRepos,
                                             ICacheManager cacheManager)
        {
            _loaiHinhDoanhNghiepRepos = loaiHinhDoanhNghiepRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<LoaiHinhDoanhNghiepDto>> GetAllServerPaging(GetLoaiHinhDoanhNghiepInputDto input)
        {
            var query = (from chucvu in _loaiHinhDoanhNghiepRepos.GetAll()
                         select new LoaiHinhDoanhNghiepDto
                         {
                             Id = chucvu.Id,
                             IsActive = chucvu.IsActive,
                             MoTa = chucvu.MoTa,
                             TenLoaiHinh = chucvu.TenLoaiHinh,
                         });
            //.WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenLoaiHinh.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var ChucVuCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<LoaiHinhDoanhNghiepDto>(ChucVuCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(LoaiHinhDoanhNghiepDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _loaiHinhDoanhNghiepRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _loaiHinhDoanhNghiepRepos.UpdateAsync(updateData);
                _cacheManager.GetCache("tblLoaiHinhDoanhNghiep").Remove("GetAllLoaiHinhDoanhNghiep");
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<LoaiHinhDoanhNghiep>();
                    int id = await _loaiHinhDoanhNghiepRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    _cacheManager.GetCache("tblLoaiHinhDoanhNghiep").Remove("GetAllLoaiHinhDoanhNghiep");
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
            await _loaiHinhDoanhNghiepRepos.DeleteAsync(id);
            _cacheManager.GetCache("tblLoaiHinhDoanhNghiep").Remove("GetAllLoaiHinhDoanhNghiep");
        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from lh in _loaiHinhDoanhNghiepRepos.GetAll()
                        where ((lh.IsActive == true))
                        orderby lh.TenLoaiHinh
                        select new ItemDto<int>
                        {
                            Id = lh.Id,
                            Name = lh.TenLoaiHinh
                        };

            return await query.ToListAsync();
        }

        #region Cache
        public object GetAll()
        {
            return _cacheManager.GetCache("tblLoaiHinhDoanhNghiep")
                .Get("GetAllLoaiHinhDoanhNghiep", () => getAllFromDB()) as object;
        }

        object getAllFromDB()
        {
            var query = from loaiHinhDoanhNghiep in _loaiHinhDoanhNghiepRepos.GetAll()
                        where loaiHinhDoanhNghiep.IsActive == true
                        orderby loaiHinhDoanhNghiep.TenLoaiHinh
                        select new
                        {
                            TenLoaiHinh = loaiHinhDoanhNghiep.TenLoaiHinh,
                            Id = loaiHinhDoanhNghiep.Id
                        };
            return query.ToList();
        }
        #endregion

    }
    #endregion
}
