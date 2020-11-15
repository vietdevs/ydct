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
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface IChucVuAppService : IApplicationService
    {
        Task<PagedResultDto<ChucVuDto>> GetAllServerPaging(ChucVuInputDto input);
        Task<int> CreateOrUpdate(ChucVuDto input);
        Task Delete(int Id);
        Task<List<ItemDto<int>>> GetAllToDDL();
    }
    #endregion

    #region MAIN
    public class ChucVuAppService : PMSAppServiceBase, IChucVuAppService
    {
        private readonly IRepository<ChucVu> _chucVuRepos;
        public ChucVuAppService(IRepository<ChucVu> chucVuRepos)
        {
            _chucVuRepos = chucVuRepos;
        }

        public async Task<PagedResultDto<ChucVuDto>> GetAllServerPaging(ChucVuInputDto input)
        {
            var query = (from chucvu in _chucVuRepos.GetAll()
                         select new ChucVuDto
                         {
                             Id = chucvu.Id,
                             IsActive = chucvu.IsActive,
                             MoTa = chucvu.MoTa,
                             TenChucVu = chucvu.TenChucVu
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenChucVu.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()) || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.LocDauLowerCaseDB()));

            var rowCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            return new PagedResultDto<ChucVuDto>(rowCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(ChucVuDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _chucVuRepos.GetAsync(input.Id);

                input.MapTo(updateData);

                await _chucVuRepos.UpdateAsync(updateData);
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<ChucVu>();
                    int newId = await _chucVuRepos.InsertAndGetIdAsync(insertInput);
                    return newId;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task Delete(int Id)
        {
            await _chucVuRepos.DeleteAsync(Id);
        }
        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from cv in _chucVuRepos.GetAll()
                        where ((cv.IsActive == true))
                        orderby cv.TenChucVu
                        select new ItemDto<int>
                        {
                            Id = cv.Id,
                            Name = cv.TenChucVu
                        };

            return await query.ToListAsync();
        }
    }
    #endregion
}
