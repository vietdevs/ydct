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
    public interface IQuocGiaAppService : IApplicationService
    {
        Task<PagedResultDto<QuocGiaDto>> GetAllServerPaging(QuocGiaInputDto input);
        Task<int> CreateOrUpdate(QuocGiaDto input);
        Task Delete(int id);
        Task<List<ItemDto<int>>> GetAllToDDL();
    }

    [AbpAuthorize]
    public class QuocGiaAppService : PMSAppServiceBase, IQuocGiaAppService
    {
        private readonly IRepository<QuocGia> _quocGiaRepos;
        public QuocGiaAppService(IRepository<QuocGia> quocGiaRepos)
        {
            _quocGiaRepos = quocGiaRepos;
        }

        public async Task<PagedResultDto<QuocGiaDto>> GetAllServerPaging(QuocGiaInputDto input)
        {
            var query = (from quocGia in _quocGiaRepos.GetAll()
                         select new QuocGiaDto
                         {
                             Id = quocGia.Id,
                             IsActive = quocGia.IsActive,
                             MoTa = quocGia.MoTa,
                             TenQuocGia = quocGia.TenQuocGia,
                             MaQuocGia = quocGia.MaQuocGia,
                             COUNTRY_IMG_URL = quocGia.COUNTRY_IMG_URL,
                             NiisId = quocGia.NiisId
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenQuocGia.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MoTa.Trim().LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MaQuocGia.Trim().LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));
            var QuocGiaCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();
            return new PagedResultDto<QuocGiaDto>(QuocGiaCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(QuocGiaDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _quocGiaRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _quocGiaRepos.UpdateAsync(updateData);
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<QuocGia>();
                    int id = await _quocGiaRepos.InsertAndGetIdAsync(insertInput);
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
            await _quocGiaRepos.DeleteAsync(id);
        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from dv in _quocGiaRepos.GetAll()
                        where ((dv.IsActive == true))
                        orderby dv.TenQuocGia
                        select new ItemDto<int>
                        {
                            Name = dv.TenQuocGia,
                            Id = dv.Id,
                        };

            return await query.ToListAsync();
        }
    }
}
