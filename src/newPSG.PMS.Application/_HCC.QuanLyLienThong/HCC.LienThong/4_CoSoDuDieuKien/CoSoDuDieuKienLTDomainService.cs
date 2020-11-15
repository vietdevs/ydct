using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.EntityDB;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System;
using Abp.Authorization;
using Abp.Application.Services;
using Newtonsoft.Json;

#region Class Riêng Cho Từng Thủ tục
using XHoSoVFA = newPSG.PMS.EntityDB.VFA_CoSoDuDieuKien;
using XHoSoLienThong = newPSG.PMS.Dto.CoSoDuDieuKienLTDto;
using XCallApiInput = newPSG.PMS.Dto.CallApiCoSoDuDieuKienInput;
using XPagingInput = newPSG.PMS.Dto.PagingCoSoDuDieuKienLTInput;
using newPSG.PMS.Common;
using System.Collections.Generic;
using Castle.Core.Logging;
using newPSG.PMS.Dto;
using newPSG.PMS.API;
using Abp.Domain.Services;
using Abp.Domain.Uow;
#endregion

namespace newPSG.PMS.Services
{
    public interface ICoSoDuDieuKienLTDomainService : IDomainService
    {
        Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input, int? chiCucId);

        Task<KetQuaLienThong> LienThongHoSo(List<XCallApiInput> input, int chiCucId, string strTinh = "");

        Task<KetQuaLienThong> AotuLienThongHoSo(int? chiCucId);
    }

    public class CoSoDuDieuKienLTDomainService : PMSDomainServiceBase, ICoSoDuDieuKienLTDomainService
    {
        private readonly IRepository<XHoSoVFA, long> _vfaHoSoRepos;
        private readonly ICallApiDomainService _callApiAppService;
        private readonly IRepository<DoanhNghiep, long> _doanhNghiepRepos;
        private readonly IRepository<QuocGia, int> _quocGiaRepos;
        private readonly IRepository<Tinh, int> _tinhRepos;
        private readonly IRepository<Huyen, long> _huyenRepos;
        private readonly IRepository<Xa, long> _xaRepos;
        private readonly ILogger _logger;

        public CoSoDuDieuKienLTDomainService(IRepository<XHoSoVFA, long> vfaHoSoRepos,
                                        ICallApiDomainService callApiAppService,
                                        IRepository<DoanhNghiep, long> doanhNghiepRepos,
                                        IRepository<QuocGia, int> quocGiaRepos,
                                        IRepository<Tinh, int> tinhRepos,
                                        IRepository<Huyen, long> huyenRepos,
                                        IRepository<Xa, long> xaRepos,
                                        ILogger logger)
        {
            _vfaHoSoRepos = vfaHoSoRepos;
            _callApiAppService = callApiAppService;
            _doanhNghiepRepos = doanhNghiepRepos;
            _quocGiaRepos = quocGiaRepos;
            _tinhRepos = tinhRepos;
            _huyenRepos = huyenRepos;
            _xaRepos = xaRepos;
            _logger = logger;
        }

        [UnitOfWork]
        public async Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input, int? chiCucId)
        {
            try
            {
                var query = QueryAll(input, chiCucId.Value);

                #region Query fromCase
                if (input.FormCase != 0)
                {
                    if (input.FormCase == 1) //Hồ sơ chưa liên thông
                    {
                        query = query.Where(p => p.TrangThaiLienThong == null);
                    }
                    else if (input.FormCase == 2) //Hồ sơ đã liên thông nhưng không thành công
                    {
                        query = query.Where(p => p.TrangThaiLienThong == (int)CommonENum.TRANG_THAI_LIEN_THONG.DA_LIEN_THONG_KHONG_THANH_CONG);
                    }
                    else if (input.FormCase == 3) //Hồ sơ đã liên thông thành công
                    {
                        query = query.Where(p => p.TrangThaiLienThong == (int)CommonENum.TRANG_THAI_LIEN_THONG.DA_LIEN_THONG_THANH_CONG/* && p.LienThongId != null*/);
                    }
                }
                #endregion

                var _total = await query.CountAsync();
                var dataGrids = await query
                       .OrderBy(p => p.HoSoId)
                       .PageBy(input)
                       .ToListAsync();

                if (dataGrids != null && dataGrids.Count() > 0)
                {
                    foreach (var item in dataGrids)
                    {
                        if (item.TinhId.HasValue)
                        {
                            var tinh = await _tinhRepos.GetAsync(item.TinhId.Value);
                            item.StrTinh = tinh != null ? tinh.Ten : "";
                        }
                    }
                }

                return new PagedResultDto<XHoSoLienThong>(_total, dataGrids);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }

        #region Call API Cục-ATTP
        [UnitOfWork]
        public async Task<KetQuaLienThong> LienThongHoSo(List<XCallApiInput> input, int chiCucId, string strTinh = "")
        {
            KetQuaLienThong KetQua = new KetQuaLienThong();
            try
            {
                var objToken = await _callApiAppService.AutoGetToKen(chiCucId);
                if (objToken == null)
                {
                    KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.CHUA_CO_TAI_KHOAN;
                    return KetQua;
                }
                if (input != null && input.Count > 0)
                {
                    var _list = new List<CoSoDuDieuKienRequest>();
                    int countTongHoSo = 0;
                    foreach (var item in input)
                    {
                        var _vfaHoSo = await _vfaHoSoRepos.GetAsync(item.Id);
                        if (_vfaHoSo.TrangThaiLienThong == (int)CommonENum.TRANG_THAI_LIEN_THONG.DA_LIEN_THONG_THANH_CONG)
                        {
                            KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.KHONG_CO_HO_SO_HOAC_DA_LIEN_THONG;
                            continue;
                        }

                        var _request = new CoSoDuDieuKienRequest();

                        #region DoanhNgiep
                        _request.DoanhNghiep = item.MapTo<DoanhNghiepModel>();
                        var tinh = await _tinhRepos.FirstOrDefaultAsync(item.TinhId.Value);
                        if (tinh != null)
                        {
                            _request.DoanhNghiep.TinhId = tinh.NiisId;
                        }
                        var huyen = await _huyenRepos.FirstOrDefaultAsync(item.HuyenId.Value);
                        if (huyen != null)
                        {
                            _request.DoanhNghiep.HuyenId = (int)huyen.NiisId;
                        }
                        var xa = await _xaRepos.FirstOrDefaultAsync(item.XaId.Value);
                        if (xa != null)
                        {
                            _request.DoanhNghiep.XaId = (int)xa.NiisId;
                        }
                        #endregion

                        #region CoSoDuDieuKien
                        _request.CoSoDuDieuKien = item.MapTo<CoSoDuDieuKienModel>();
                       _request.CoSoDuDieuKien.GiayXacNhanToBase64String = FileToBase64String(item.GiayTiepNhan);
                        _request.CoSoDuDieuKien.NgayCapChungNhan = item.NgayTraKetQua.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        //Tinh, Huyen, Xa
                        var tinhHoSo = await _tinhRepos.FirstOrDefaultAsync(item.HoSoTinhId.Value);
                        if (tinhHoSo != null)
                        {
                            _request.CoSoDuDieuKien.TinhId = tinhHoSo.NiisId;
                        }
                        var huyenHoSo = await _huyenRepos.FirstOrDefaultAsync(item.HoSoHuyenId.Value);
                        if (huyenHoSo != null)
                        {
                            _request.CoSoDuDieuKien.HuyenId = (int)huyenHoSo.NiisId;
                        }
                        var xaHoSo = await _xaRepos.FirstOrDefaultAsync(item.HoSoXaId.Value);
                        if (xaHoSo != null)
                        {
                            _request.CoSoDuDieuKien.XaId = (int)xaHoSo.NiisId;
                        }
                        #endregion

                        #region Update TrangThaiLienThong
                        _vfaHoSo.TrangThaiLienThong = (int)CommonENum.TRANG_THAI_LIEN_THONG.DA_LIEN_THONG_KHONG_THANH_CONG;
                        _vfaHoSo.NgayLienThong = DateTime.Now;

                        //Guid
                        if (string.IsNullOrEmpty(_vfaHoSo.Guid))
                        {
                            string guid = "";
                            if (string.IsNullOrEmpty(strTinh))
                            {
                                guid = CreateGuid(_vfaHoSo.Id, RemoveUnicodeTinh(chiCucId));
                            }
                            else
                            {
                                guid = CreateGuid(_vfaHoSo.Id, strTinh);
                            }
                            _request.CoSoDuDieuKien.Guid = guid;
                            _vfaHoSo.Guid = guid;
                        }
                        else
                        {
                            _request.CoSoDuDieuKien.Guid = _vfaHoSo.Guid;
                        }

                        await _vfaHoSoRepos.UpdateAsync(_vfaHoSo);
                        CurrentUnitOfWork.SaveChanges();
                        #endregion

                        _list.Add(_request);
                        countTongHoSo++;
                    }
                    KetQua.TongSoHoSo = countTongHoSo;

                    string dataJson = JsonConvert.SerializeObject(_list);
                    string api = "api/services/app/coSoDuDieuKienApi/Insert";
                    var objResult = await _callApiAppService.CallApi(chiCucId, dataJson, api, objToken.Result.ToString());
                    if (objResult.Result != null && objResult.Result.ListResult.Count > 0 && objResult.Success == true)
                    {
                        int countHoSoThatBai = 0;
                        int countHoSoThanhCong = 0;
                        foreach (var result in objResult.Result.ListResult)
                        {
                            var _vfaHoSoUpdate = await _vfaHoSoRepos.FirstOrDefaultAsync(p => p.Guid == result.Guid);
                            if (_vfaHoSoUpdate != null && (result.Code == MessageErorr.Er100.Code || result.Code == MessageErorr.Er03.Code))
                            {
                                _vfaHoSoUpdate.TrangThaiLienThong = (int)CommonENum.TRANG_THAI_LIEN_THONG.DA_LIEN_THONG_THANH_CONG;
                                _vfaHoSoUpdate.NgayLienThongThanhCong = DateTime.Now;
                                _vfaHoSoUpdate.LienThongId = result.LienThongId;
                                await _vfaHoSoRepos.UpdateAsync(_vfaHoSoUpdate);

                                countHoSoThanhCong++;
                            }
                            else
                            {
                                countHoSoThatBai++;
                            }
                        }
                        KetQua.HoSoThanhCong = countHoSoThanhCong;
                        KetQua.HoSoThatBai = countHoSoThatBai;
                        KetQua.ListResult = objResult.Result.ListResult;
                        KetQua.TrangThaiLienThongRequest = KetQua.HoSoThanhCong == 0 ? KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.KHONG_THANH_CONG : (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.THANH_CONG;
                    }
                    else
                    {
                        KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.CHUA_CO_TAI_KHOAN;
                    }
                }
                return KetQua;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.KHONG_THANH_CONG;
                return KetQua;
            }
        }

        [UnitOfWork]
        public async Task<KetQuaLienThong> AotuLienThongHoSo(int? chiCucId)
        {
            try
            {
                var KetQua = new KetQuaLienThong();
                if (_callApiAppService.IsAccountLienThong(chiCucId.Value))
                {
                    var input = new XPagingInput();

                    string strTinh = RemoveUnicodeTinh(chiCucId.Value);

                    int _chiCucId = chiCucId == null ? 0 : chiCucId.Value;
                    var query = QueryAll(input, _chiCucId);
                    var list = await query.Where(p => p.TrangThaiLienThong != (int)CommonENum.TRANG_THAI_LIEN_THONG.DA_LIEN_THONG_THANH_CONG)
                                          .OrderBy(p => p.HoSoId)
                                          .ToListAsync();

                    if (list.Count > 0)
                    {
                        List<XCallApiInput> inputCallApi = list.MapTo<List<XCallApiInput>>();

                        KetQua = await LienThongHoSo(inputCallApi, _chiCucId, strTinh);
                    }
                    else
                    {
                        KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.KHONG_CO_HO_SO_HOAC_DA_LIEN_THONG;
                    }
                }
                else
                {
                    KetQua.TrangThaiLienThongRequest = (int)CommonENum.TRANG_THAI_LIEN_THONG_REQUEST.CHUA_CO_TAI_KHOAN;
                }
                return KetQua;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }
        #endregion
        private IQueryable<XHoSoLienThong> QueryAll(XPagingInput input, int chiCucId)
        {
            long.TryParse(input.Keyword, out long hoSoId);
            DateTime NgayTraKetQuaToi = new DateTime(), NgayTraKetQuaTu = new DateTime();
            if (input.NgayTraKetQuaToi.HasValue && input.NgayTraKetQuaTu.HasValue)
            {
                NgayTraKetQuaToi = new DateTime(input.NgayTraKetQuaToi.Value.Year, input.NgayTraKetQuaToi.Value.Month, input.NgayTraKetQuaToi.Value.Day, 23, 59, 59);
                NgayTraKetQuaTu = new DateTime(input.NgayTraKetQuaTu.Value.Year, input.NgayTraKetQuaTu.Value.Month, input.NgayTraKetQuaTu.Value.Day, 0, 0, 0);
            }

            var query = (from hoso in _vfaHoSoRepos.GetAll()
                         where hoso.TrangThaiHoSo == (int)CommonENum.TRANG_THAI_HO_SO.DA_HOAN_TAT && hoso.ChiCucId == chiCucId
                         join r_dn in _doanhNghiepRepos.GetAll() on hoso.DoanhNghiepId equals r_dn.Id into tb_dn //Left Join
                         from dn in tb_dn.DefaultIfEmpty()
                         select new XHoSoLienThong
                         {
                             Id = hoso.Id,
                             HoSoId = hoso.HoSoId,
                             ChiCucId = hoso.ChiCucId,
                             ThuTucId = hoso.ThuTucId,
                             MaSoThue = hoso.MaSoThue,
                             MaHoSo = hoso.MaHoSo,
                             SoDangKy = hoso.SoDangKy,
                             IsCA = hoso.IsCA,
                             OnIsCA = hoso.OnIsCA,
                             IsHoSoBS = hoso.IsHoSoBS,
                             LoaiHoSoId = hoso.LoaiHoSoId,
                             DoanhNghiepId = hoso.DoanhNghiepId,
                             TenDoanhNghiep = hoso.TenDoanhNghiep,
                             TenCoSo = hoso.TenCoSo,
                             DiaChiCoSo = hoso.DiaChiCoSo,
                             NgayTraKetQua = hoso.NgayTraKetQua,
                             NgayTiepNhan = hoso.NgayTiepNhan,
                             HoSoXuLyId_Active = hoso.HoSoXuLyId_Active,
                             TrangThaiHoSo = hoso.TrangThaiHoSo,
                             PhongBanId = hoso.PhongBanId,
                             TrangThaiLienThong = hoso.TrangThaiLienThong,
                             GiayTiepNhan = hoso.GiayTiepNhan,
                             GiayTiepNhanFull = hoso.GiayTiepNhanFull,
                             HoTenChuCoSo = hoso.HoTenChuCoSo,
                             SanPhamDuocCapPhep = hoso.SanPhamDuocCapPhep,
                             LoaiHinhCoSoId = hoso.LoaiHinhCoSoId,
                             HoSoTinhId = (hoso != null) ? hoso.TinhId : 0,
                             HoSoHuyenId = (hoso != null) ? hoso.HuyenId : 0,
                             HoSoXaId = (hoso != null) ? hoso.XaId : 0,
                             Guid = hoso.Guid,
                             //DoanhNghiep
                             DiaChi = dn.DiaChi,
                             SoDienThoai = dn.SoDienThoai,
                             Fax = dn.Fax,
                             TinhId = (dn != null) ? dn.TinhId : 0,
                             HuyenId = (dn != null) ? dn.HuyenId : 0,
                             XaId = (dn != null) ? dn.XaId : 0,
                             Website = dn.Website,
                             TenNguoiDaiDien = dn.TenNguoiDaiDien,
                         })
                         .WhereIf(!string.IsNullOrEmpty(input.Keyword),
                                 u => (u.SoDangKy != null && u.SoDangKy.LocDauLowerCaseDB().Contains(input.Keyword.LocDauLowerCaseDB()))
                                 || (u.MaHoSo != null && u.MaHoSo.Contains(input.Keyword.LocDauLowerCaseDB()))
                                 || (u.TenDoanhNghiep != null && u.TenDoanhNghiep.LocDauLowerCaseDB().Contains(input.Keyword.LocDauLowerCaseDB()))
                                 || (u.Id == hoSoId))
                        .WhereIf(input.NgayTraKetQuaTu.HasValue && input.NgayTraKetQuaToi.HasValue, u => !u.NgayTraKetQua.HasValue || (u.NgayTraKetQua >= NgayTraKetQuaTu && u.NgayTraKetQua <= NgayTraKetQuaToi))
                        .WhereIf(input.TinhId.HasValue, u => u.TinhId == input.TinhId.Value)
                        .WhereIf(input.DoanhNghiepId.HasValue, u => u.DoanhNghiepId == input.DoanhNghiepId.Value);

            return query;
        }
    }
}