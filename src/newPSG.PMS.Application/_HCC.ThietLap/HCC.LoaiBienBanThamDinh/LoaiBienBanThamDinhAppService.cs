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
    public interface ILoaiBienBanThamDinhAppService : IApplicationService
    {
        Task<PagedResultDto<LoaiBienBanThamDinhDto>> GetAllServerPaging(LoaiBienBanThamDinhInputDto input);
        Task CreateOrUpdate(LoaiBienBanThamDinhDto input);
        Task Delete(int id);
        Task<dynamic> GetAllToDDL();
    }

    [AbpAuthorize]
    public class LoaiBienBanThamDinhAppService : PMSAppServiceBase, ILoaiBienBanThamDinhAppService
    {
        private readonly IRepository<LoaiBienBanThamDinh> _loaiBienBanRepos;
        public LoaiBienBanThamDinhAppService(IRepository<LoaiBienBanThamDinh> loaiBienBanRepos)
        {
            _loaiBienBanRepos = loaiBienBanRepos;
        }

        public async Task<PagedResultDto<LoaiBienBanThamDinhDto>> GetAllServerPaging(LoaiBienBanThamDinhInputDto input)
        {
            var query = (from loaiBienBan in _loaiBienBanRepos.GetAll()
                         select new LoaiBienBanThamDinhDto
                         {
                             Id = loaiBienBan.Id,
                             Name = loaiBienBan.Name,
                             RoleLevel = loaiBienBan.RoleLevel,
                             MoTa = loaiBienBan.MoTa
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Name.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var count = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();
            return new PagedResultDto<LoaiBienBanThamDinhDto>(count, dataGrids);
        }

        public async Task CreateOrUpdate(LoaiBienBanThamDinhDto input)
        {
            try
            {
                if (input.Id > 0)
                {
                    var updateData = await _loaiBienBanRepos.GetAsync(input.Id);
                    input.MapTo(updateData);
                    await _loaiBienBanRepos.UpdateAsync(updateData);
                }
                else
                {
                    var insertInput = input.MapTo<LoaiBienBanThamDinh>();
                    await _loaiBienBanRepos.InsertAsync(insertInput);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            await _loaiBienBanRepos.DeleteAsync(id);
        }

        public async Task<dynamic> GetAllToDDL()
        {
            var query = from dv in _loaiBienBanRepos.GetAll()
                        select new
                        {
                            dv.Id,
                            dv.Name,
                            dv.RoleLevel
                        };

            return await query.ToListAsync();
        }
    }
}
