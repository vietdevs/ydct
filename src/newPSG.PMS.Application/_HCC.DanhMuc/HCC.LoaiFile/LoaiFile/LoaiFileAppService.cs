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

namespace newPSG.PMS.Common
{
    #region INTERFACE
    public interface ILoaiFileAppService : IApplicationService
    {
        List<LoaiFileDto> GetAllLoaiFile();
        Task<int> CreateOrUpdateLoaiFile(CreateOrUpdateLoaiFileInput input);
        Task DeleteLoaiFile(EntityDto<int> input);
        Task<PagedResultDto<LoaiFileDto>> GetAllLoaiFileServerPaging(GetLoaiFileInput input);
        Task<List<ItemDto<int>>> GetAllToDDL();
    }
    #endregion

    #region MAIN
    //[AbpAuthorize]
    public class LoaiFileAppService : PMSAppServiceBase, ILoaiFileAppService
    {

        private readonly IRepository<LoaiFile> _loaiFileRepos;
        private readonly ICacheManager _cacheManager;

        public LoaiFileAppService(
            IRepository<LoaiFile> loaiHinhCoSoRepos,
            ICacheManager cacheManager)
        {

            _loaiFileRepos = loaiHinhCoSoRepos;
            _cacheManager = cacheManager;
        }




        public List<LoaiFileDto> GetAllLoaiFile()
        {
            return _cacheManager.GetCache("tblLoaiFile")
                .Get("GetAllLoaiFile", () => getAllLoaiFileFromDB()) as List<LoaiFileDto>;
        }

        List<LoaiFileDto> getAllLoaiFileFromDB()
        {
            var query = from loaiHinhCoSo in _loaiFileRepos.GetAll()
                        where loaiHinhCoSo.IsActive == true
                        orderby loaiHinhCoSo.Id
                        select new LoaiFileDto
                        {
                            Id = loaiHinhCoSo.Id,
                            Ten = loaiHinhCoSo.Ten,
                            IsActive = loaiHinhCoSo.IsActive,
                            IsKhac = loaiHinhCoSo.IsKhac
                        };
            List<LoaiFileDto> ret = query.ToList();
            return ret;
        }

        public async Task<int> CreateOrUpdateLoaiFile(CreateOrUpdateLoaiFileInput input)
        {
            try
            {
                if (input.LoaiFile.Id.HasValue)
                {
                    // update
                    var updateData = await _loaiFileRepos.GetAsync(input.LoaiFile.Id.Value);
                    input.LoaiFile.MapTo(updateData);
                    await _loaiFileRepos.UpdateAsync(updateData);
                    _cacheManager.GetCache("tblLoaiFile").Remove("GetAllLoaiFile");
                    return updateData.Id;
                }

                var insertInput = input.LoaiFile.MapTo<LoaiFile>();
                int id = await _loaiFileRepos.InsertAndGetIdAsync(insertInput);
                await CurrentUnitOfWork.SaveChangesAsync();
                _cacheManager.GetCache("tblLoaiFile").Remove("GetAllLoaiFile");
                return id;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return 0;
            }
        }

        public async Task DeleteLoaiFile(EntityDto<int> input)
        {
            var ChucVu = await _loaiFileRepos.GetAsync(input.Id);
            await _loaiFileRepos.DeleteAsync(ChucVu);
            _cacheManager.GetCache("tblLoaiFile").Remove("GetAllLoaiFile");
        }

        public async Task<PagedResultDto<LoaiFileDto>> GetAllLoaiFileServerPaging(GetLoaiFileInput input)
        {
            var query = (from loaihinh in _loaiFileRepos.GetAll()
                         select new LoaiFileDto
                         {
                             Id = loaihinh.Id,
                             IsActive = loaihinh.IsActive,
                             MoTa = loaihinh.MoTa,
                             Ten = loaihinh.Ten,
                             IsKhac = loaihinh.IsKhac,
                             NiisId = loaihinh.NiisId
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Ten.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB())
                         || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));


            var ChucVuCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            var ChucVuListDtos = dataGrids.MapTo<List<LoaiFileDto>>();
            return new PagedResultDto<LoaiFileDto>(ChucVuCount, ChucVuListDtos);
        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from dv in _loaiFileRepos.GetAll()
                        where dv.IsActive == true
                        orderby dv.Id
                        select new ItemDto<int>
                        {
                            Name = dv.Ten,
                            Id = dv.Id,
                        };

            return await query.ToListAsync();
        }
    }
    #endregion
}
