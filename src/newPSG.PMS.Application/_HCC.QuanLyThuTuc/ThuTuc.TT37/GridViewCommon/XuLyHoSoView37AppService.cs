using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Roles;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Editions;
using newPSG.PMS.EntityDB;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo37;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem37;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy37;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory37;
using XHoSoDto = newPSG.PMS.Dto.HoSo37Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput37Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem37Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy37Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory37Dto;
using newPSG.PMS.Dto;
using System.Collections.Generic;
using newPSG.PMS.Exporting;
#endregion

namespace newPSG.PMS.Services
{
    public interface IXuLyHoSoView37AppService : IApplicationService
    {
        Task<dynamic> GetViewHoSo(long hoSoId);
        Task<dynamic> GetHistory(long hoSoId);
        Task<FileDto> ExportToExcel(long hoSoId);
    }
    [AbpAuthorize]
    public class XuLyHoSoView37AppService : PMSAppServiceBase, IXuLyHoSoView37AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;
        private readonly IXuLyHoSoViewExcelExporter _iXuLyHoSoViewExcelExporter;
        public XuLyHoSoView37AppService(IRepository<XHoSo, long> hoSoRepos,
                                        IRepository<XHoSoTepDinhKem, long> hoSoTepDinhKemRepos,
                                        IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                        IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                        IRepository<User, long> userRepos,
                                        IXuLyHoSoViewExcelExporter iXuLyHoSoViewExcelExporter)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoTepDinhKemRepos = hoSoTepDinhKemRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
            _iXuLyHoSoViewExcelExporter = iXuLyHoSoViewExcelExporter;
        }

        public async Task<dynamic> GetViewHoSo(long hoSoId)
        {
            try
            {
                if (hoSoId > 0)
                {
                    var hoSo = await _hoSoRepos.GetAsync(hoSoId);
                    var query_TDK = from tdk in _hoSoTepDinhKemRepos.GetAll()
                                    where tdk.HoSoId == hoSoId
                                    orderby tdk.Id
                                    select new
                                    {
                                        tdk.DuongDanTep,
                                        tdk.LoaiTepDinhKem,
                                        tdk.IsCA,
                                        tdk.TenTep,
                                        tdk.MoTaTep,
                                        Active = false
                                    };
                    var _giayTiepNhanUrl = "";
                    var _bienBanTongHopUrl = "";
                    if (hoSo.HoSoXuLyId_Active.HasValue)
                    {
                        var hosoxl = await _hoSoXuLyRepos.GetAsync(hoSo.HoSoXuLyId_Active.Value);
                         _giayTiepNhanUrl = hosoxl.GiayTiepNhanCA;
                        _bienBanTongHopUrl = hosoxl.BienBanTongHopUrl;
                    }
                    

                    var listCongVan = await (from hsxl in _hoSoXuLyRepos.GetAll()
                                             where hsxl.HoSoId == hoSoId && hsxl.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.BO_SUNG
                                             orderby hsxl.Id descending
                                             select new
                                             {
                                                 hsxl.Id,
                                                 hsxl.HoSoId,
                                                 hsxl.DuongDanTepCA,
                                                 hsxl.LanhDaoCucIsCA,
                                                 hsxl.TruongPhongIsCA,
                                                 hsxl.TrangThaiCV,
                                                 hsxl.GiayTiepNhanCA,
                                                 hsxl.VanThuIsCA,
                                                 NgayTra = hsxl.LastModificationTime,
                                                 Active = false
                                             }).ToListAsync();

                    var _urlBanDangKy = "/Report37/TemplateHoSo?hoSoId=" + hoSo.Id;
                    if (hoSo.IsCA == true &&!String.IsNullOrEmpty(hoSo.DuongDanTepCA))
                    {
                        _urlBanDangKy = string.Format("/File/GoToViewTaiLieu?url={0}", hoSo.DuongDanTepCA);
                    }

                    return new
                    {
                        hoSo,
                        trangThaiHoSo = hoSo.TrangThaiHoSo,
                        urlBanDangKy = _urlBanDangKy,
                        giayTiepNhanUrl = _giayTiepNhanUrl,
                        bienBanTongHopUrl = _bienBanTongHopUrl,
                        danhSachTepDinhKem = await query_TDK.ToListAsync(),
                        danhSachCongVan = listCongVan
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return null;
        }

        //History
        public async Task<dynamic> GetHistory(long hoSoId)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    if (hoSoId > 0)
                    {

                        var _queryYKien = from yk in _hoSoXuLyHistoryRepos.GetAll()
                                          join r_us in _userRepos.GetAll() on yk.NguoiXuLyId equals r_us.Id into tb_us //Left Join
                                          from us in tb_us.DefaultIfEmpty()
                                          where yk.HoSoId == hoSoId
                                          orderby yk.Id descending
                                          select new XHoSoXuLyHistoryDto
                                          {
                                              Id = yk.Id,
                                              HoSoXuLyId = yk.HoSoXuLyId,
                                              DonViXuLy = yk.DonViXuLy,
                                              DonViKeTiep = yk.DonViKeTiep,
                                              NoiDungYKien = yk.NoiDungYKien,
                                              PhoPhongId = yk.PhoPhongId,
                                              ChuyenVienThuLyId = yk.ChuyenVienThuLyId,
                                              ChuyenVienPhoiHopId = yk.ChuyenVienPhoiHopId,
                                              IsChuyenNhanh = yk.IsChuyenNhanh,
                                              LyDoChuyenNhanh = yk.LyDoChuyenNhanh,
                                              HoSoIsDat_Pre = yk.HoSoIsDat_Pre,
                                              HoSoIsDat = yk.HoSoIsDat,
                                              TrangThaiCV = yk.TrangThaiCV,
                                              IsSuaCV = yk.IsSuaCV,
                                              NoiDungCV = yk.NoiDungCV,
                                              CreationTime = yk.CreationTime,
                                              LastModificationTime = yk.LastModificationTime,
                                              NguoiXuLy = us.Surname + " " + us.Name,
                                              ActionEnum = yk.ActionEnum
                                          };

                        var _listYKien = await _queryYKien.ToListAsync();

                        foreach (var item in _listYKien)
                        {
                            //Tên người xử lý
                            if(item.NguoiXuLyId != null && _userRepos.GetAll().Any(x => x.Id == item.NguoiXuLyId && x.IsActive == true))
                            {
                                var objUser = _userRepos.FirstOrDefault(x => x.Id == item.NguoiXuLyId);
                                item.NguoiXuLy = objUser.Surname + " " + objUser.Name;
                            }

                            switch (item.DonViXuLy)
                            {
                                case (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG:
                                    item.NguoiXuLy = "PC - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP:
                                    item.NguoiXuLy = "CV - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET:
                                    item.NguoiXuLy = "CV1 - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET:
                                    item.NguoiXuLy = "CV2 - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.PHO_PHONG:
                                    item.NguoiXuLy = "PP - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG:
                                    item.NguoiXuLy = "TP - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC:
                                    item.NguoiXuLy = "LĐC - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.LANH_DAO_BO:
                                    item.NguoiXuLy = "LĐB - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.VAN_THU:
                                    item.NguoiXuLy = "VT - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP:
                                    item.NguoiXuLy = "DN - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.KE_TOAN:
                                    item.NguoiXuLy = "KT - " + item.NguoiXuLy;
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN:
                                    item.NguoiXuLy = "MC - " + item.NguoiXuLy;
                                    break;
                            }

                            item.HanhDongXuLy = GetHanhDongXuLy(item);
                        }

                        //TimeLine
                        var _timeLine = _listYKien.Where(p => p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET
                                        || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.PHO_PHONG || p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC 
                                        || (p.DonViXuLy == (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP && p.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)).ToList();

                        return new
                        {
                            listYKien = _listYKien,
                            timeLine = _timeLine
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return null;
        }

        public async Task<FileDto> ExportToExcel(long hoSoId)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var data = new List<HoSoXuLyHistory37Dto>();
                    var _listYKien = await GetListHistoryByHoSoId(hoSoId);
                    if (_listYKien != null && _listYKien.Count() > 0)
                    {
                        var i = 1;
                        foreach (var item in _listYKien)
                        {
                            var lstItem = new HoSoXuLyHistory37Dto();
                            lstItem.STT = i;
                            lstItem.NguoiXuLy = item.NguoiXuLy;
                            lstItem.StrNgayXuLy = item.NgayXuLy.HasValue ? item.NgayXuLy.Value.ToString("dd/MM/yyyy HH:mm") : "";

                            #region Kết quả
                            if (item.TrangThaiXuLy.HasValue)
                            {
                                if (item.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.DAT)
                                {
                                    lstItem.StrKetQuaXuLy = "Đạt";
                                }
                                if (item.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.BO_SUNG)
                                {
                                    lstItem.StrKetQuaXuLy = "Bổ sung";
                                }
                                if (item.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.KHONG_DAT)
                                {
                                    lstItem.StrKetQuaXuLy = "Không đạt";
                                }
                            }
                            else
                            {
                                lstItem.StrKetQuaXuLy = "Đã xử lý";
                            }
                            #endregion

                            lstItem.NguoiXuLy = GetFullNguoiXuLy(item);
                            lstItem.HanhDongXuLy = GetHanhDongXuLy(item);
                            lstItem.NoiDungYKien = item.NoiDungYKien;
                            i++;
                            data.Add(lstItem);
                        }
                    }
                    return _iXuLyHoSoViewExcelExporter.ExportToFile(data, hoSoId);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }

        }

        private async Task<List<XHoSoXuLyHistoryDto>> GetListHistoryByHoSoId(long hoSoId)
        {
            try
            {
                if (hoSoId > 0)
                {
                    var _queryYKien = from yk in _hoSoXuLyHistoryRepos.GetAll()
                                      join r_us in _userRepos.GetAll() on yk.NguoiXuLyId equals r_us.Id into tb_us //Left Join
                                      from us in tb_us.DefaultIfEmpty()
                                      where yk.HoSoId == hoSoId
                                      orderby yk.Id descending
                                      select new XHoSoXuLyHistoryDto
                                      {
                                          Id = yk.Id,
                                          HoSoXuLyId = yk.HoSoXuLyId,
                                          DonViXuLy = yk.DonViXuLy,
                                          DonViKeTiep = yk.DonViKeTiep,
                                          NoiDungYKien = yk.NoiDungYKien,
                                          ChuyenVienThuLyId = yk.ChuyenVienThuLyId,
                                          ChuyenVienPhoiHopId = yk.ChuyenVienPhoiHopId,
                                          IsChuyenNhanh = yk.IsChuyenNhanh,
                                          LyDoChuyenNhanh = yk.LyDoChuyenNhanh,
                                          HoSoIsDat_Pre = yk.HoSoIsDat_Pre,
                                          HoSoIsDat = yk.HoSoIsDat,
                                          TrangThaiCV = yk.TrangThaiCV,
                                          IsSuaCV = yk.IsSuaCV,
                                          NoiDungCV = yk.NoiDungCV,
                                          CreationTime = yk.CreationTime,
                                          LastModificationTime = yk.LastModificationTime,
                                          NguoiXuLy = us.Surname + " " + us.Name,
                                          ActionEnum = yk.ActionEnum,
                                          NgayXuLy = yk.NgayXuLy,
                                          FullJson = yk.FullJson,

                                      };

                    var _listYKien = await _queryYKien.ToListAsync();
                    return _listYKien;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                return null;
            }
        }

        private string GetHanhDongXuLy(XHoSoXuLyHistoryDto item)
        {
            string res = "";
            try
            {
                switch (item.DonViXuLy)
                {
                    //case (int)CommonENum.DON_VI_XU_LY.QUAN_TRI:
                    //    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_TU_DONG)
                    //    {
                    //        res = "Phân công tự động";
                    //    }
                    //    break;
                    case (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.NOP_HO_SO_DE_RA_SOAT)
                        {
                            res = "Nộp hồ sơ để rà soát";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_NOP_HO_SO)
                        {
                            res = "Nộp hồ sơ mới";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BO_SUNG)
                        {
                            res = "Nộp hồ sơ bổ sung";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_THANH_TOAN)
                        {
                            res = "Nộp phí";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.NOP_BAO_CAO_KHAC_PHUC)
                        {
                            res = "Nộp báo cáo khắc phục";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_RA_SOAT_HO_SO)
                        {
                            res = "Rà soát hồ sơ";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_GUI_LANH_DAO)
                        {
                            res = "Gửi lãnh đạo phân công hồ sơ";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.KE_TOAN:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.KE_TOAN_XAC_NHAN_THANH_TOAN)
                        {
                            res = "Kế toán xác nhận nộp phí";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.MOT_CUA_PHAN_CONG:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO)
                        {
                            res = "Phân hồ sơ";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_LAI_HO_SO)
                        {
                            res = "Phân hồ sơ lại";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHONG_BAN_PHAN_CONG)
                        {
                            res = "Phân công";
                        }
                        //if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHAN_CONG_TU_DONG)
                        //{
                        //    res = "Phân công tự động";
                        //}
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_CHUA_XU_LY)
                        {
                            res = "Phân công lại";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_DA_XU_LY)
                        {
                            res = "Phân công lại hồ sơ đã xử lý";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_MOI)
                        {
                            res = "Thẩm xét hồ sơ";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.LAP_DOAN_THAM_DINH_HO_SO)
                        {
                            res = "Thành lập đoàn thẩm định";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_GIA_THAM_DINH)
                        {
                            res = "Thẩm định hồ sơ";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_DUYET_THAM_XET)
                        {
                            res = "Tổng hợp thẩm xét";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_LAI)
                        {
                            res = "Thẩm xét lại";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_BO_SUNG)
                        {
                            res = "Thẩm xét hồ sơ bổ sung";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH)
                        {
                            res = "Tổng hợp thẩm định";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH_LAI)
                        {
                            res = "Tổng hợp thẩm định lại";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH_BO_SUNG)
                        {
                            res = "Tổng hợp thẩm định bổ sung";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.PHO_PHONG:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHO_PHONG_DUYET)
                        {
                            if (item.HoSoIsDat == true)
                            {
                                res = "Duyệt hồ sơ";
                            }
                            else
                            {
                                res = "Duyệt công văn";
                            }
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET)
                        {
                            if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
                            {
                                res = "Chuyển chuyên viên tổng hợp lại";
                            }
                            else
                            {
                                if (item.TrangThaiXuLy == (int)CommonENum.KET_QUA_XU_LY.BO_SUNG)
                                {
                                    res = "Duyệt và ký hồ sơ bổ sung";
                                }
                                else
                                {
                                    res = "Duyệt hồ sơ";
                                }
                            }
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET_THAM_DINH)
                        {
                            res = "Kết luận thẩm định";
                            if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
                            {
                                res = "Chuyển chuyên viên tổng hợp lại";
                            }

                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET_THAM_DINH_LAI)
                        {
                            res = "Kết luận thẩm định lại";
                            if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP)
                            {
                                res = "Chuyển chuyên viên tổng hợp lại";
                            }

                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_GIA_THAM_DINH)
                        {
                            res = "Thẩm định hồ sơ";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC:
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET)
                        {
                            if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG)
                            {
                                res = "Chuyển trưởng phòng xem lại";
                            }
                            else
                            {
                                res = "Duyệt và ký giấy";
                            }
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET_THAM_DINH)
                        {
                            res = "Duyệt kết quả thẩm định";
                        }
                        if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.LANH_DAO_PHAN_CONG_HO_SO)
                        {
                            res = "Lãnh đạo phân công hồ sơ";
                        }
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.VAN_THU:
                        res = "Đóng dấu giấy tờ";
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                res = "";
            }
            return res;
        }

        /// <summary>
        /// Lấy họ tên + vai trò của người xử lý trong từng note của lịch sử
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetFullNguoiXuLy(XHoSoXuLyHistoryDto item)
        {
            string res = item.NguoiXuLy;
            try
            {
                switch (item.DonViXuLy)
                {
                    case (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP:
                        res = "DN - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN:
                        res = "MC - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.KE_TOAN:
                        res = "KT - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.MOT_CUA_PHAN_CONG:
                        res = "MC - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG:
                        res = "PC - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET:
                        res = "CV1 - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET:
                        res = "CV2 - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP:
                        res = "CV1 - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG:
                        res = "TP - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC:
                        res = "LĐC - " + item.NguoiXuLy;
                        break;
                    case (int)CommonENum.DON_VI_XU_LY.VAN_THU:
                        res = "VT - " + item.NguoiXuLy;
                        break;
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                res = "";
            }
            return res;
        }

    }
}