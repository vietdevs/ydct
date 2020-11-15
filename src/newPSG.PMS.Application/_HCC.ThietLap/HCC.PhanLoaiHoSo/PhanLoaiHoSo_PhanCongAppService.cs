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
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Service
{
    public interface IPhanLoaiHoSo_PhanCongAppService : IApplicationService
    {
        Task<List<PhanLoaiHoSo_PhanCongDto>> GetLoaiHoSo_BienBan(int setting_LoaiHoSoId);
        Task<ServiceReturn<string>> CreateOrUpdate(PhanLoaiHoSo_PhanCong_CreateInputDto input);
    }

    public class PhanLoaiHoSo_PhanCongAppService : PMSAppServiceBase, IPhanLoaiHoSo_PhanCongAppService
    {
        private readonly IRepository<PhanLoaiHoSo> _setting_LoaiHoSoRepos;
        private readonly IRepository<PhanLoaiHoSo_PhanCong> _setting_LoaiHoSo_BienBanRepos;
        private readonly IRepository<PhanLoaiHoSo_Filter> _setting_LoaiHoSo_FilterRepos;

        public PhanLoaiHoSo_PhanCongAppService(
            IRepository<PhanLoaiHoSo> setting_LoaiHoSoRepos,
            IRepository<PhanLoaiHoSo_PhanCong> setting_LoaiHoSo_BienBanRepos,
            IRepository<PhanLoaiHoSo_Filter> setting_LoaiHoSo_FilterRepos
        )
        {
            _setting_LoaiHoSoRepos = setting_LoaiHoSoRepos;
            _setting_LoaiHoSo_BienBanRepos = setting_LoaiHoSo_BienBanRepos;
            _setting_LoaiHoSo_FilterRepos = setting_LoaiHoSo_FilterRepos;
        }

        public async Task<List<PhanLoaiHoSo_PhanCongDto>> GetLoaiHoSo_BienBan(int setting_LoaiHoSoId)
        {
            var query = from data in _setting_LoaiHoSo_BienBanRepos.GetAll().Where(x => x.PhanLoaiHoSoId == setting_LoaiHoSoId)
                        orderby data.TieuBanEnum
                        select new PhanLoaiHoSo_PhanCongDto
                        {
                            Id = data.Id,
                            PhanLoaiHoSoId = data.PhanLoaiHoSoId,
                            TieuBanEnum = data.TieuBanEnum,
                            LoaiBienBanThamDinhId = data.LoaiBienBanThamDinhId,
                            SoLuong = data.SoLuong,
                        };

            return await query.ToListAsync();
        }

        public async Task<ServiceReturn<string>> CreateOrUpdate(PhanLoaiHoSo_PhanCong_CreateInputDto input)
        {
            if (input.ListPhanLoaiHoSo_PhanCong.Count < 0)
            {
                return new ServiceReturn<string>("Danh sách truyền vào rỗng!", Abp.Logging.LogSeverity.Warn);
            }

            if (input.ListPhanLoaiHoSo_PhanCong.GroupBy(x => x.TieuBanEnum).Any(g => g.Count() > 1))
            {
                return new ServiceReturn<string>("Không thể thiết lập nhiều hơn 1 tiểu ban cho 1 loại hồ sơ!", Abp.Logging.LogSeverity.Warn);
            }

            var listPhanLoaiHoSo_PhanCong = await _setting_LoaiHoSo_BienBanRepos.GetAll()
                .Where(x => x.PhanLoaiHoSoId == input.PhanLoaiHoSoId)
                .ToListAsync();

            foreach (var item in input.ListPhanLoaiHoSo_PhanCong)
            {
                var loaiHSPC_TieuBan_DB = listPhanLoaiHoSo_PhanCong.FirstOrDefault(x => x.TieuBanEnum == item.TieuBanEnum);
                if (loaiHSPC_TieuBan_DB == null)
                {
                    var insertInput = item.MapTo<PhanLoaiHoSo_PhanCong>();
                    insertInput.PhanLoaiHoSoId = input.PhanLoaiHoSoId;
                    await _setting_LoaiHoSo_BienBanRepos.InsertAsync(insertInput);
                }
                else
                {
                    var updateData = await _setting_LoaiHoSo_BienBanRepos.GetAsync(item.Id);
                    item.MapTo(updateData);
                    await _setting_LoaiHoSo_BienBanRepos.UpdateAsync(updateData);
                    listPhanLoaiHoSo_PhanCong.Remove(loaiHSPC_TieuBan_DB);
                }
            }

            //Xóa những phân từ không nằm trong DS
            foreach (var item in listPhanLoaiHoSo_PhanCong)
            {
                await _setting_LoaiHoSo_BienBanRepos.DeleteAsync(item.Id);
            }

            return new ServiceReturn<string>("Xử lý thành công!");
        }
    }
}