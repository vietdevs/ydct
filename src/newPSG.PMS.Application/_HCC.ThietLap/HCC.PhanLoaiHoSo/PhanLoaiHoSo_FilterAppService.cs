using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Services
{
    public interface IPhanLoaiHoSo_FilterAppService : IApplicationService
    {
        Task<List<PhanLoaiHoSo_FilterDto>> GetLoaiHoSo_Filter(PhanLoaiHoSo_Filter_DtoInput input);
        Task<int> CreateOrUpdate(PhanLoaiHoSo_FilterDto input);
        Task Delete(int setting_LoaiHoSo_FilterId);
    }

    public class PhanLoaiHoSo_FilterAppService : PMSAppServiceBase, IPhanLoaiHoSo_FilterAppService
    {
        private readonly IRepository<PhanLoaiHoSo> _setting_LoaiHoSoRepos;
        private readonly IRepository<PhanLoaiHoSo_PhanCong> _setting_LoaiHoSo_BienBanRepos;
        private readonly IRepository<PhanLoaiHoSo_Filter> _setting_LoaiHoSo_FilterRepos;

        public PhanLoaiHoSo_FilterAppService(
            IRepository<PhanLoaiHoSo> setting_LoaiHoSoRepos,
            IRepository<PhanLoaiHoSo_PhanCong> setting_LoaiHoSo_BienBanRepos,
            IRepository<PhanLoaiHoSo_Filter> setting_LoaiHoSo_FilterRepos
        )
        {
            _setting_LoaiHoSoRepos = setting_LoaiHoSoRepos;
            _setting_LoaiHoSo_BienBanRepos = setting_LoaiHoSo_BienBanRepos;
            _setting_LoaiHoSo_FilterRepos = setting_LoaiHoSo_FilterRepos;
        }

        public async Task<List<PhanLoaiHoSo_FilterDto>> GetLoaiHoSo_Filter(PhanLoaiHoSo_Filter_DtoInput input)
        {
            var query = (from data in _setting_LoaiHoSo_FilterRepos.GetAll().Where(x => x.PhanLoaiHoSoId == input.PhanLoaiHoSoId)
                         orderby data.Id
                         select new PhanLoaiHoSo_FilterDto
                         {
                             Id = data.Id,
                             Name = data.Name,
                             PhanLoaiHoSoId = data.PhanLoaiHoSoId,
                             JsonFilter = data.JsonFilter,
                         });

            return await query.ToListAsync();
        }

        public async Task<int> CreateOrUpdate(PhanLoaiHoSo_FilterDto input)
        {
            try
            {
                if (input.Id > 0)
                {
                    // update
                    var updateData = await _setting_LoaiHoSo_FilterRepos.GetAsync(input.Id);
                    input.MapTo(updateData);
                    updateData.JsonFilter = LowercaseJsonSerializer.SerializeObject(input.Filter);
                    await _setting_LoaiHoSo_FilterRepos.UpdateAsync(updateData);
                    return updateData.Id;
                }
                else
                {
                    var insertInput = input.MapTo<PhanLoaiHoSo_Filter>();
                    insertInput.JsonFilter = LowercaseJsonSerializer.SerializeObject(input.Filter);
                    int id = await _setting_LoaiHoSo_FilterRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return id;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Xảy ra lỗi trong quá trình xử lý", ex.Message);
            }
        }

        public async Task Delete(int setting_LoaiHoSo_FilterId)
        {
            await _setting_LoaiHoSo_FilterRepos.DeleteAsync(setting_LoaiHoSo_FilterId);
        }
    }
}