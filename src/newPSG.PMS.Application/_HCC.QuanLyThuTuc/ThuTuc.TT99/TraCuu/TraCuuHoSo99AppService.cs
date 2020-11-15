using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using newPSG.PMS.EntityDB;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

#region Class Riêng Cho Từng Thủ tục
using XHoSo = newPSG.PMS.EntityDB.HoSo99;
using XHoSoDto = newPSG.PMS.Dto.TraCuuHoSo99Dto;
using TraCuuHoSoInput = newPSG.PMS.Dto.TraCuuHoSo99Input;
#endregion

namespace newPSG.PMS.Common
{
    public interface ITraCuuHoSo99AppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoDto>> GetListHoSoTraCuuPaging(TraCuuHoSoInput input);
    }

    public class TraCuuHoSo99AppService : PMSAppServiceBase, ITraCuuHoSo99AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;

        public TraCuuHoSo99AppService(IRepository<XHoSo, long> hoSoRepos,
                                      IRepository<DoanhNghiep, long> doanhNghiepRepos)
        {
            _hoSoRepos = hoSoRepos;
            _doanhNghiepRepos = doanhNghiepRepos;
        }

        public async Task<PagedResultDto<XHoSoDto>> GetListHoSoTraCuuPaging(TraCuuHoSoInput input)
        {
            try
            {
                var query = (from hoso in _hoSoRepos.GetAll()
                             join r_dn in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals r_dn.Id into tb_dn //Left Join
                             from dn in tb_dn.DefaultIfEmpty()
                             where hoso.PId == null
                             && (!input.TinhId.HasValue || hoso.TinhId == input.TinhId)
                             && (string.IsNullOrEmpty(input.SoDangKy) || hoso.SoDangKy.Trim().Contains(input.SoDangKy.Trim()))
                             && (string.IsNullOrEmpty(input.MaHoSo) || hoso.MaHoSo.Trim().Contains(input.MaHoSo.Trim()))
                             && (string.IsNullOrEmpty(input.TenDoanhNghiep) || hoso.TenDoanhNghiep.Trim().Contains(input.TenDoanhNghiep.Trim()))
                             && (string.IsNullOrEmpty(input.DiaChi) || hoso.DiaChi.Trim().Contains(input.DiaChi.Trim()))
                             select new XHoSoDto
                             {
                                 Id = hoso.Id,
                                 DoanhNghiepId = hoso.DoanhNghiepId,
                                 SoDangKy = hoso.SoDangKy,
                                 TenDoanhNghiep = hoso.TenDoanhNghiep,
                                 TinhId = (dn != null) ? dn.TinhId : 0,
                                 StrTinh = (dn != null) ? dn.Tinh : string.Empty,
                                 DiaChi = hoso.DiaChi,
                                 DiaChiCoSo = hoso.DiaChiCoSo,
                                 NgayTraKetQua = hoso.CreationTime,
                                 MaHoSo = hoso.MaHoSo,
                                 GiayTiepNhan = hoso.GiayTiepNhan,
                                 SoGiayTiepNhan = hoso.SoGiayTiepNhan,
                                 LoaiHoSoId = hoso.LoaiHoSoId
                             });
                var _total = await query.CountAsync();
                var dataGrids = await query
                    .OrderByDescending(p => p.NgayTraKetQua)
                    .PageBy(input)
                   .ToListAsync();

                return new PagedResultDto<XHoSoDto>(_total, dataGrids);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }
    }
}
