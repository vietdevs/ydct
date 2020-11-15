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
using Newtonsoft.Json;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface ILoaiHoSoAppService : IApplicationService
    {
        Task<PagedResultDto<LoaiHoSoDto>> GetAllServerPaging(LoaiHoSoInputDto input);
        Task<int> CreateOrUpdate(LoaiHoSoDto input);
        Task<int> CreateOrUpdateHanXuLy(LoaiHoSoDto input);
        Task Delete(int id);
        Task<LoaiHoSoDto> GetById(int id);
        Task<dynamic> GetByThuTucId(int ThuTucId);
        Task<List<ItemDto<int>>> GetAllToDDL();
        Task<List<LoaiHoSoDto>> GetLoaiHoSoTT02ToDDL();
        Task<List<LoaiHoSoDto>> GetLoaiHoSoByThuTucToDDL(int thuTucEnum);
        Task<List<LoaiHoSoDto>> GetListByThuTucId(int thuTucId = 0);
        LoaiHoSoDto GetLoaiHoSoById(int loaiHoSoId);
    }
    #endregion

    #region MAIN
    public class LoaiHoSoAppService : PMSAppServiceBase, ILoaiHoSoAppService
    {
        private readonly IRepository<LoaiHoSo> _LoaiHoSoRepos;
        private readonly IRepository<LoaiHoSo_HanXuLy> _LoaiHoSoHanXuLyRepos;
        private readonly IRepository<PhongBan> _phongBanRepos;
        private readonly IRepository<ThuTuc> _thuTucRepos;
        public LoaiHoSoAppService(
            IRepository<LoaiHoSo> LoaiHoSoRepos,
            IRepository<LoaiHoSo_HanXuLy> LoaiHoSoHanXuLyRepos,
            IRepository<PhongBan> phongBanRepos,
            IRepository<ThuTuc> thuTucRepos)
        {
            _LoaiHoSoRepos = LoaiHoSoRepos;
            _LoaiHoSoHanXuLyRepos = LoaiHoSoHanXuLyRepos;
            _phongBanRepos = phongBanRepos;
            _thuTucRepos = thuTucRepos;
        }

        public async Task<PagedResultDto<LoaiHoSoDto>> GetAllServerPaging(LoaiHoSoInputDto input)
        {
            var query = (from lhs in _LoaiHoSoRepos.GetAll()
                         select new LoaiHoSoDto
                         {
                             Id = lhs.Id,
                             TenLoaiHoSo = lhs.TenLoaiHoSo,
                             IsActive = lhs.IsActive,
                             MoTa = lhs.MoTa,
                             IsXuLy = lhs.IsXuLy,
                             QuiTrinhXuLy = lhs.QuiTrinhXuLy,
                             PhiXuLy = lhs.PhiXuLy,
                             SoNgayXuLy = lhs.SoNgayXuLy,
                             DataImage = lhs.DataImage,
                             ThuTucId = lhs.ThuTucId,
                             IsCoChiTieu = lhs.IsCoChiTieu,
                             NiisId = lhs.NiisId,
                             SoNgaySuaDoiBoSung = lhs.SoNgaySuaDoiBoSung
                         })
            .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenLoaiHoSo.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()) || u.MoTa.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));
            var LoaiHoSoCount = await query.CountAsync();
            var LoaiHoSoGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();

            foreach (var item in LoaiHoSoGrids)
            {
                item.TenThuTuc = item.ThuTucId != null ? CommonENum.GetEnumDescription((CommonENum.THU_TUC_ID)(int)item.ThuTucId) : "";
            }

            return new PagedResultDto<LoaiHoSoDto>(LoaiHoSoCount, LoaiHoSoGrids);
        }

        public async Task<int> CreateOrUpdate(LoaiHoSoDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _LoaiHoSoRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _LoaiHoSoRepos.UpdateAsync(updateData);
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<LoaiHoSo>();
                    int id = await _LoaiHoSoRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return id;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<int> CreateOrUpdateHanXuLy(LoaiHoSoDto input)
        {
            if (input.ThuTucId > 0)
            {
                // update
                var updateData = await _LoaiHoSoHanXuLyRepos.FirstOrDefaultAsync(x => x.ThuTucId == input.ThuTucId);
                if (updateData!=null)
                {
                    updateData.JsonHanXuLy = input.JsonHanXuLy;
                    await _LoaiHoSoHanXuLyRepos.UpdateAsync(updateData);
                    return updateData.Id;
                }
                else
                {
                    try
                    {
                        var insertInput = new LoaiHoSo_HanXuLy();
                        insertInput.ThuTucId = input.ThuTucId;
                        insertInput.JsonHanXuLy = input.JsonHanXuLy;
                        int id = await _LoaiHoSoHanXuLyRepos.InsertOrUpdateAndGetIdAsync(insertInput);
                        await CurrentUnitOfWork.SaveChangesAsync();
                        return id;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return 1;
            
        }
        public async Task Delete(int id)
        {
            await _LoaiHoSoRepos.DeleteAsync(id);
        }

        public async Task<LoaiHoSoDto> GetById(int id)
        {
            if (id > 0)
            {
                // update
                var loaiHoSo = await _LoaiHoSoRepos.GetAsync(id);
                var res = loaiHoSo.MapTo<LoaiHoSoDto>();
                return res;
            }
            return null;
        }
        public async Task<dynamic> GetByThuTucId(int ThuTucId)
        {
            if (ThuTucId > 0)
            {
                var query = await _LoaiHoSoHanXuLyRepos.GetAllListAsync(x => x.ThuTucId == ThuTucId);
                var res = new
                {
                    listLoaiHoSoHanXuLy = query.ToList(),
                };

                return res;

            }
            return null;
        }

        public async Task<List<ItemDto<int>>> GetAllToDDL()
        {
            var query = from lhs in _LoaiHoSoRepos.GetAll()
                        where ((lhs.IsActive == true))
                        orderby lhs.TenLoaiHoSo
                        select new ItemDto<int>
                        {
                            Id = lhs.Id,
                            Name = lhs.TenLoaiHoSo,
                            ThuTucId = lhs.ThuTucId
                        };

            return await query.ToListAsync();
        }

        public async Task<List<LoaiHoSoDto>> GetLoaiHoSoTT02ToDDL()
        {
            var query = from lhs in _LoaiHoSoRepos.GetAll()
                        where ((lhs.IsActive == true) && lhs.ThuTucId == (int)CommonENum.THU_TUC_ID.THU_TUC_02)
                        orderby lhs.TenLoaiHoSo
                        select new LoaiHoSoDto
                        {
                            Id = lhs.Id,
                            TenLoaiHoSo = lhs.TenLoaiHoSo,
                            MoTa = lhs.MoTa,
                            PhiXuLy = lhs.PhiXuLy,
                            QuiTrinhXuLy = lhs.QuiTrinhXuLy,
                            SoNgayXuLy = lhs.SoNgayXuLy,
                            DataImage = lhs.DataImage,
                            IsCoChiTieu = lhs.IsCoChiTieu
                        };
            return await query.ToListAsync();
        }

        public async Task<List<LoaiHoSoDto>> GetLoaiHoSoByThuTucToDDL(int thuTucEnum)
        {
            var query = from lhs in _LoaiHoSoRepos.GetAll()
                        where ((lhs.IsActive == true) && lhs.ThuTucId == thuTucEnum)
                        orderby lhs.TenLoaiHoSo
                        select new LoaiHoSoDto
                        {
                            Id = lhs.Id,
                            TenLoaiHoSo = lhs.TenLoaiHoSo,
                            MoTa = lhs.MoTa,
                            PhiXuLy = lhs.PhiXuLy,
                            QuiTrinhXuLy = lhs.QuiTrinhXuLy,
                            SoNgayXuLy = lhs.SoNgayXuLy,
                            DataImage = lhs.DataImage,
                            IsCoChiTieu = lhs.IsCoChiTieu
                        };
            return await query.ToListAsync();
        }


        public async Task<List<LoaiHoSoDto>> GetListByThuTucId(int thuTucId = 0)
        {
            var query = from lhs in _LoaiHoSoRepos.GetAll()
                        where ((lhs.IsActive == true) && lhs.ThuTucId == thuTucId)
                        orderby lhs.TenLoaiHoSo
                        select new LoaiHoSoDto
                        {
                            Id = lhs.Id,
                            TenLoaiHoSo = lhs.TenLoaiHoSo,
                            MoTa = lhs.MoTa,
                            PhiXuLy = lhs.PhiXuLy,
                            QuiTrinhXuLy = lhs.QuiTrinhXuLy,
                            SoNgayXuLy = lhs.SoNgayXuLy,
                            DataImage = lhs.DataImage
                        };

            return await query.ToListAsync();
        }

        public LoaiHoSoDto GetLoaiHoSoById(int loaiHoSoId)
        {
            var query = (from lhs in _LoaiHoSoRepos.GetAll()
                         where ((lhs.IsActive == true) && lhs.Id == loaiHoSoId)
                         select new LoaiHoSoDto
                         {
                             Id = lhs.Id,
                             TenLoaiHoSo = lhs.TenLoaiHoSo,
                             MoTa = lhs.MoTa,
                             PhiXuLy = lhs.PhiXuLy,
                             QuiTrinhXuLy = lhs.QuiTrinhXuLy,
                             IsActive = lhs.IsActive
                         }).FirstOrDefault();
            return query;
        }
    }
    #endregion
}