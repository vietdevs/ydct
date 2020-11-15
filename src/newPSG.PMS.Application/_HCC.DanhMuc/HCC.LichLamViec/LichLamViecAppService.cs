using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.EntityDB;
using System.Data.Entity;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Authorization;
using Abp.Application.Services;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Services
{
    #region INTERFACE
    public interface ILichLamViecAppService : IApplicationService
    {
        Task<PagedResultDto<LichLamViecDto>> GetAllServerPaging(LichLamViecInputDto input);
        Task<int> CreateOrUpdate(LichLamViecDto input);
        Task Delete(int id);
        DateTime GetNgayHenTra(DateTime ngayNopHoSo, int soNgayXuLy);
        int? GetSoNgayLamViec(DateTime? ngayBatDau, DateTime? ngayKetThuc);
    }
    #endregion

    #region MAIN
    [AbpAuthorize]
    public class LichLamViecAppService : PMSAppServiceBase, ILichLamViecAppService
    {
        private readonly IRepository<LichLamViec> _lichLamViecRepos;
        private readonly IRepository<NgayNghi> _ngayNghiRepos;
        private readonly ICacheManager _cacheManager;
        public LichLamViecAppService(IRepository<LichLamViec> lichLamViecRepos,
                                     IRepository<NgayNghi> ngayNghiRepos,
                                     ICacheManager cacheManager)
        {
            _lichLamViecRepos = lichLamViecRepos;
            _ngayNghiRepos = ngayNghiRepos;
            _cacheManager = cacheManager;
        }

        public async Task<PagedResultDto<LichLamViecDto>> GetAllServerPaging(LichLamViecInputDto input)
        {
            var query = (from lichLamViec in _lichLamViecRepos.GetAll()
                         select new LichLamViecDto
                         {
                             Id = lichLamViec.Id,
                             IsActive = lichLamViec.IsActive,
                             NgayBatDau = lichLamViec.NgayBatDau,
                             NgayKetThuc = lichLamViec.NgayKetThuc,
                             CN = lichLamViec.CN,
                             T2 = lichLamViec.T2,
                             T3 = lichLamViec.T3,
                             T4 = lichLamViec.T4,
                             T5 = lichLamViec.T5,
                             T6 = lichLamViec.T6,
                             T7 = lichLamViec.T7,
                             TenLich = lichLamViec.TenLich,
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Filter), u => u.TenLich.LocDauLowerCaseDB().Contains(input.Filter.Trim().LocDauLowerCaseDB()));

            var lichLamViecCount = await query.CountAsync();
            var dataGrids = await query
                .OrderBy(p => p.Id)
                .PageBy(input)
               .ToListAsync();
            
            return new PagedResultDto<LichLamViecDto>(lichLamViecCount, dataGrids);
        }

        public async Task<int> CreateOrUpdate(LichLamViecDto input)
        {
            if (input.Id > 0)
            {
                // update
                var updateData = await _lichLamViecRepos.GetAsync(input.Id);
                input.MapTo(updateData);
                await _lichLamViecRepos.UpdateAsync(updateData);
                _cacheManager.GetCache("tblLichLamViec").Remove("GetAllLichLamViec");
                return updateData.Id;
            }
            else
            {
                try
                {
                    var insertInput = input.MapTo<LichLamViec>();
                    int id = await _lichLamViecRepos.InsertAndGetIdAsync(insertInput);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    _cacheManager.GetCache("tblLichLamViec").Remove("GetAllLichLamViec");
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
            var ngayNghi = (from nn in _ngayNghiRepos.GetAll()
                            where nn.LichLamViecId == id
                            select nn).ToList();
            foreach (var item in ngayNghi)
            {
                await _ngayNghiRepos.DeleteAsync(item);
            }
            await _lichLamViecRepos.DeleteAsync(id);
            _cacheManager.GetCache("tblLichLamViec").Remove("GetAllLichLamViec");
        }
        
        #region Tính ngày hẹn trả
        private List<LichLamViecDto> GetAllLichLamViec()
        {
            List<LichLamViecDto> listLichLamViec = (from llv in _lichLamViecRepos.GetAll()
                                                    select new LichLamViecDto
                                                    {
                                                        Id = llv.Id,
                                                        CN = llv.CN,
                                                        T2 = llv.T2,
                                                        T3 = llv.T3,
                                                        T4 = llv.T4,
                                                        T5 = llv.T5,
                                                        T6 = llv.T6,
                                                        T7 = llv.T7,
                                                        NgayBatDau = llv.NgayBatDau,
                                                        NgayKetThuc = llv.NgayKetThuc,
                                                        TenLich = llv.TenLich,
                                                    }).ToList();

            foreach (var lichLamViec in listLichLamViec)
            {
                List<NgayNghi> listNgayNghi = (from nn in _ngayNghiRepos.GetAll()
                                               where nn.LichLamViecId == lichLamViec.Id
                                               select nn).ToList();

                lichLamViec.DanhSachNgayNghi = listNgayNghi;

            }
            return listLichLamViec;
        }

        private List<LichLamViecDto> getCauHinhLichLamViec()
        {
            var res = _cacheManager.GetCache("tblLichLamViec")
                .Get("GetAllLichLamViec", () => GetAllLichLamViec()) as List<LichLamViecDto>;

            return res;
        }

        private bool CheckIfNgayLamViec(DateTime dateTime, List<LichLamViecDto> listLichLamViec)
        {
            if (listLichLamViec == null || listLichLamViec.Count == 0)
            {
                return true;
            }
            DateTime dateTimeLocal = dateTime.ToLocalTime();
            DayOfWeek dayOfWeek = dateTimeLocal.DayOfWeek;

            bool isNgayLamViec = true;

            foreach (var item in listLichLamViec)
            {
                if (dateTimeLocal < item.NgayBatDau.Value.ToLocalTime() && dateTimeLocal > item.NgayKetThuc.Value.ToLocalTime())
                {
                    continue;
                }
                if (
                    (dayOfWeek == DayOfWeek.Monday && item.T2 == false)
                    || (dayOfWeek == DayOfWeek.Tuesday && item.T3 == false)
                    || (dayOfWeek == DayOfWeek.Wednesday && item.T4 == false)
                    || (dayOfWeek == DayOfWeek.Thursday && item.T5 == false)
                    || (dayOfWeek == DayOfWeek.Friday && item.T6 == false)
                    || (dayOfWeek == DayOfWeek.Saturday && item.T7 == false)
                    || (dayOfWeek == DayOfWeek.Sunday && item.CN == false)
                )
                {
                    isNgayLamViec = false;
                    break;
                }
                foreach (var nn in item.DanhSachNgayNghi)
                {
                    if (dateTimeLocal >= nn.NgayBatDau.Value.ToLocalTime() && dateTimeLocal <= nn.NgayKetThuc.Value.ToLocalTime())
                    {
                        isNgayLamViec = false;
                        break;
                    }
                }
            }
            return isNgayLamViec;
        }

        public DateTime GetNgayHenTra(DateTime ngayNopHoSo, int soNgayXuLy)
        {
            var cauHinh = getCauHinhLichLamViec();
            int dayCounts = 0;
            DateTime ngayHentra = new DateTime(ngayNopHoSo.Year, ngayNopHoSo.Month, ngayNopHoSo.Day, ngayNopHoSo.Hour, ngayNopHoSo.Minute, ngayNopHoSo.Second, ngayNopHoSo.Millisecond, ngayNopHoSo.Kind);
            while (dayCounts < soNgayXuLy)
            {
                ngayHentra = ngayHentra.AddDays(1);
                if (CheckIfNgayLamViec(ngayHentra, cauHinh))
                {
                    dayCounts++;
                }
            }
            return ngayHentra;
        }

        public int? GetSoNgayLamViec(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            var cauHinh = getCauHinhLichLamViec();
            if (ngayBatDau == null || ngayKetThuc == null)
            {
                return null;
            }
            int dayCounts = 0;
            DateTime ngay_begin = new DateTime(ngayBatDau.Value.Year, ngayBatDau.Value.Month, ngayBatDau.Value.Day);
            DateTime ngay_end = new DateTime(ngayKetThuc.Value.Year, ngayKetThuc.Value.Month, ngayKetThuc.Value.Day);
            ngay_begin = ngay_begin.AddDays(1);
            while (ngay_begin <= ngay_end)
            {
                if (CheckIfNgayLamViec(ngay_begin, cauHinh))
                {
                    dayCounts++;
                }
                ngay_begin = ngay_begin.AddDays(1);
            }
            return dayCounts;
        }
        #endregion
    }
    #endregion
}
