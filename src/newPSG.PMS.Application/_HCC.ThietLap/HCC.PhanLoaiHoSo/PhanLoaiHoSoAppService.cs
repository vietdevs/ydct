using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Services
{
    public interface IPhanLoaiHoSoAppService : IApplicationService
    {
        Task<PagedResultDto<PhanLoaiHoSoDto>> GetAllServerPaging(PhanLoaiHoSoDtoInput input);
        Task<int> CreateOrUpdate(PhanLoaiHoSoDto input);
        Task Delete(int setting_LoaiHoSoId);
    }

    public class PhanLoaiHoSoAppService : PMSAppServiceBase, IPhanLoaiHoSoAppService
    {
        private readonly IRepository<PhanLoaiHoSo> _setting_LoaiHoSoRepos;
        private readonly IRepository<PhanLoaiHoSo_PhanCong> _setting_LoaiHoSo_BienBanRepos;
        private readonly IRepository<PhanLoaiHoSo_Filter> _setting_LoaiHoSo_FilterRepos;
        private readonly IRepository<LoaiBienBanThamDinh> _loaiBienBanRepos;

        public PhanLoaiHoSoAppService(
            IRepository<PhanLoaiHoSo> setting_LoaiHoSoRepos,
            IRepository<PhanLoaiHoSo_PhanCong> setting_LoaiHoSo_BienBanRepos,
            IRepository<PhanLoaiHoSo_Filter> setting_LoaiHoSo_FilterRepos,
            IRepository<LoaiBienBanThamDinh> loaiBienBanRepos
        )
        {
            _setting_LoaiHoSoRepos = setting_LoaiHoSoRepos;
            _setting_LoaiHoSo_BienBanRepos = setting_LoaiHoSo_BienBanRepos;
            _setting_LoaiHoSo_FilterRepos = setting_LoaiHoSo_FilterRepos;
            _loaiBienBanRepos = loaiBienBanRepos;
        }

        public async Task<PagedResultDto<PhanLoaiHoSoDto>> GetAllServerPaging(PhanLoaiHoSoDtoInput input)
        {
            var query = (from data in _setting_LoaiHoSoRepos.GetAll()
                         select new PhanLoaiHoSoDto
                         {
                             Id = data.Id,
                             Name = data.Name,
                             RoleLevel = data.RoleLevel,
                             ThuTucId = data.ThuTucId
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.Name.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var count = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.ThuTucId).ThenBy(x => x.RoleLevel)
                .PageBy(input)
               .ToListAsync();

            if (dataGrids != null)
            {
                foreach (var item in dataGrids)
                {
                    var _loaiHoSo_BienBan = (from loaiHS_BB in _setting_LoaiHoSo_BienBanRepos.GetAll()
                                             join t_loaiBienBan in _loaiBienBanRepos.GetAll() on loaiHS_BB.LoaiBienBanThamDinhId equals t_loaiBienBan.Id into tb_loaiBienBan
                                             from loaiBienBan in tb_loaiBienBan.DefaultIfEmpty()
                                             where loaiHS_BB.PhanLoaiHoSoId == item.Id
                                             select new PhanLoaiHoSo_PhanCongDto
                                             {
                                                 Id = loaiHS_BB.Id,
                                                 PhanLoaiHoSoId = loaiHS_BB.PhanLoaiHoSoId,
                                                 LoaiBienBanThamDinhId = loaiHS_BB.LoaiBienBanThamDinhId,
                                                 TieuBanEnum = loaiHS_BB.TieuBanEnum,
                                                 SoLuong = loaiHS_BB.SoLuong,
                                                 StrLoaiBienBanThamDinh = loaiBienBan.Name,
                                             }).ToList();

                    if (_loaiHoSo_BienBan != null && _loaiHoSo_BienBan.Count > 0)
                    {
                        item.ListLoaiHoSo_BienBan = _loaiHoSo_BienBan;
                    }

                    var _loaiHoSo_Filter = _setting_LoaiHoSo_FilterRepos.GetAll().Where(x => x.PhanLoaiHoSoId == item.Id).ToList();
                    if (_loaiHoSo_Filter != null && _loaiHoSo_Filter.Count > 0)
                    {
                        item.ListLoaiHoSo_Filter = new List<PhanLoaiHoSo_FilterDto>();
                        foreach (var filter in _loaiHoSo_Filter)
                        {
                            var _item = filter.MapTo<PhanLoaiHoSo_FilterDto>();
                            item.ListLoaiHoSo_Filter.Add(_item);
                        }
                    }
                }
            }

            return new PagedResultDto<PhanLoaiHoSoDto>(count, dataGrids);
        }

        public async Task<int> CreateOrUpdate(PhanLoaiHoSoDto input)
        {
            try
            {
                if (input.Id > 0)
                {
                    // update
                    var updateData = await _setting_LoaiHoSoRepos.GetAsync(input.Id);
                    input.MapTo(updateData);
                    await _setting_LoaiHoSoRepos.UpdateAsync(updateData);
                    return updateData.Id;
                }
                else
                {
                    var insertInput = input.MapTo<PhanLoaiHoSo>();
                    int id = await _setting_LoaiHoSoRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return id;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Xảy ra lỗi trong quá trình xử lý", ex.Message);
            }
        }

        public async Task Delete(int setting_LoaiHoSoId)
        {
            await _setting_LoaiHoSoRepos.DeleteAsync(setting_LoaiHoSoId);
            await _setting_LoaiHoSo_BienBanRepos.DeleteAsync(x => x.PhanLoaiHoSoId == setting_LoaiHoSoId);
            await _setting_LoaiHoSo_FilterRepos.DeleteAsync(x => x.PhanLoaiHoSoId == setting_LoaiHoSoId);
        }
    }
}