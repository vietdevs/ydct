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

using XHoSo = newPSG.PMS.EntityDB.HoSo99;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem99;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy99;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory99;
using XHoSoDto = newPSG.PMS.Dto.HoSo99Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput99Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem99Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy99Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory99Dto;
#endregion

namespace newPSG.PMS.Services
{
    public interface IXuLyHoSoView99AppService : IApplicationService
    {
        Task<dynamic> GetViewHoSo(long hoSoId);
        Task<dynamic> GetHistory(long hoSoId);
    }
    [AbpAuthorize]
    public class XuLyHoSoView99AppService : PMSAppServiceBase, IXuLyHoSoView99AppService
    {
        private readonly IRepository<XHoSo, long> _hoSoRepos;
        private readonly IRepository<XHoSoTepDinhKem, long> _hoSoTepDinhKemRepos;
        private readonly IRepository<XHoSoXuLy, long> _hoSoXuLyRepos;
        private readonly IRepository<XHoSoXuLyHistory, long> _hoSoXuLyHistoryRepos;
        private readonly IRepository<User, long> _userRepos;

        public XuLyHoSoView99AppService(IRepository<XHoSo, long> hoSoRepos,
                                        IRepository<XHoSoTepDinhKem, long> hoSoTepDinhKemRepos,
                                        IRepository<XHoSoXuLy, long> hoSoXuLyRepos,
                                        IRepository<XHoSoXuLyHistory, long> hoSoXuLyHistoryRepos,
                                        IRepository<User, long> userRepos)
        {
            _hoSoRepos = hoSoRepos;
            _hoSoTepDinhKemRepos = hoSoTepDinhKemRepos;
            _hoSoXuLyRepos = hoSoXuLyRepos;
            _hoSoXuLyHistoryRepos = hoSoXuLyHistoryRepos;
            _userRepos = userRepos;
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

                    var listCongVan = await (from hsxl in _hoSoXuLyRepos.GetAll()
                                             where hsxl.HoSoId == hoSoId && hsxl.TrangThaiCV.HasValue
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

                    var giayTiepNhanCA = await (from hsxl in _hoSoXuLyRepos.GetAll()
                                                where hsxl.HoSoId == hoSoId && !string.IsNullOrEmpty(hsxl.GiayTiepNhanCA)
                                                orderby hsxl.Id descending
                                                select new
                                                {
                                                    hsxl.Id,
                                                    hsxl.HoSoId,
                                                    hsxl.DuongDanTepCA,
                                                    hsxl.GiayTiepNhanCA,
                                                    NgayTra = hsxl.LastModificationTime,
                                                    Active = false
                                                }).FirstOrDefaultAsync();

                    var _urlBanDangKy = "/Report99/TemplateHoSo?hoSoId=" + hoSo.Id;
                    if (hoSo.IsCA == true && !String.IsNullOrEmpty(hoSo.DuongDanTepCA))
                    {
                        _urlBanDangKy = "/File/GoToViewTaiLieu?url=" + hoSo.DuongDanTepCA;
                    }

                    return new
                    {
                        hoSo,
                        trangThaiHoSo = hoSo.TrangThaiHoSo,
                        urlBanDangKy = _urlBanDangKy,
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

                            item.HanhDongXuLy = "";
                            switch (item.DonViXuLy)
                            {

                                case (int)CommonENum.DON_VI_XU_LY.DOANH_NGHIEP:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_NOP_HO_SO)
                                    {
                                        item.HanhDongXuLy = "Nộp hồ sơ để rà soát";
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.DOANH_NGHIEP_THANH_TOAN)
                                    {
                                        item.HanhDongXuLy = "Thanh toán hồ sơ";
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.NOP_HO_SO_BO_SUNG)
                                    {
                                        item.HanhDongXuLy = "Nộp hồ sơ bổ sung";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.KE_TOAN:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.KE_TOAN_XAC_NHAN_THANH_TOAN)
                                    {
                                        item.HanhDongXuLy = "Kế toán xác nhận thanh toán";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.MOT_CUA_TIEP_NHAN:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_RA_SOAT_HO_SO)
                                    {
                                        item.HanhDongXuLy = "Rà soát hồ sơ";
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_HO_SO)
                                    {
                                        item.HanhDongXuLy = "Phân hồ sơ";
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.MOT_CUA_PHAN_CONG_LAI_HO_SO)
                                    {
                                        item.HanhDongXuLy = "Phân hồ sơ lại";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.PHONG_BAN_PHAN_CONG:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_CHUA_XU_LY)
                                    {
                                        item.HanhDongXuLy = "Phân công lại";
                                    }
                                    else if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHAN_CONG_LAI_HO_SO_DA_XU_LY)
                                    {
                                        item.HanhDongXuLy = "Phân công lại hồ sơ đã xử lý";
                                    }
                                    else
                                    {
                                        item.HanhDongXuLy = "Phân công";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_DUYET_THAM_XET)
                                    {
                                        item.HanhDongXuLy = "Thẩm định hồ sơ";
                                    }
                                    else if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_LAI)
                                    {
                                        item.HanhDongXuLy = "Thẩm định lại";
                                    }
                                    else if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_BO_SUNG)
                                    {
                                        item.HanhDongXuLy = "Thẩm định hồ sơ bổ sung";
                                    }
                                    else
                                    {
                                        item.HanhDongXuLy = "Thẩm định hồ sơ";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_PHOI_HOP_THAM_XET: //Tổng hơp, thẩm định lại, thẩm định bổ sung
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.CHUYEN_VIEN_TONG_HOP_THAM_DINH)
                                    {
                                        item.HanhDongXuLy = "Cho ý kiến thẩm định";
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_LAI)
                                    {
                                        item.HanhDongXuLy = "Thẩm định lại";
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.THAM_XET_HO_SO_BO_SUNG)
                                    {
                                        item.HanhDongXuLy = "Thẩm định hồ sơ bổ sung";
                                    }
                                    else
                                    {
                                        item.HanhDongXuLy = "Cho ý kiến thẩm định";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.PHO_PHONG:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.PHO_PHONG_DUYET)
                                    {
                                        if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)
                                        {
                                            item.HanhDongXuLy = "Chuyển chuyên viên xem lại";
                                        }
                                        else
                                        {
                                            item.HanhDongXuLy = "Duyệt hồ sơ";
                                        }
                                    }
                                    else
                                    {
                                        item.HanhDongXuLy = "Duyệt hồ sơ";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.TRUONG_PHONG:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.TRUONG_PHONG_DUYET)
                                    {
                                        if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)
                                        {
                                            item.HanhDongXuLy = "Chuyển chuyên viên xem lại";
                                        }
                                        else if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.VAN_THU && item.TrangThaiCV == (int)CommonENum.TRANG_THAI_CONG_VAN.CAN_BO_SUNG)
                                        {
                                            item.HanhDongXuLy = "Duyệt và ký công văn bổ sung";
                                        }
                                        else
                                        {
                                            item.HanhDongXuLy = "Duyệt hồ sơ";
                                        }
                                    }
                                    else
                                    {
                                        item.HanhDongXuLy = "Duyệt hồ sơ";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.LANH_DAO_CUC:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.LANH_DAO_CUC_DUYET)
                                    {
                                        if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET)
                                        {
                                            item.HanhDongXuLy = "Chuyển chuyên viên xem lại";
                                        }
                                        else if (item.DonViKeTiep == (int)CommonENum.DON_VI_XU_LY.VAN_THU)
                                        {
                                            if (item.TrangThaiCV == (int)CommonENum.TRANG_THAI_CONG_VAN.KHONG_DAT)
                                            {
                                                item.HanhDongXuLy = "Duyệt và ký công văn từ chối";
                                            }
                                            else
                                            {
                                                item.HanhDongXuLy = "Duyệt và ký giấy tiếp xác nhận";
                                            }
                                        }
                                        else
                                        {
                                            item.HanhDongXuLy = "Duyệt hồ sơ";
                                        }
                                    }
                                    else
                                    {
                                        item.HanhDongXuLy = "Duyệt hồ sơ";
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.LANH_DAO_BO:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.LANH_DAO_BO_DUYET)
                                    {
                                        if (item.HoSoIsDat == true)
                                        {
                                            item.HanhDongXuLy = "Duyệt và ký giấy tiếp nhận";
                                        }
                                        else
                                        {
                                            item.HanhDongXuLy = "Cho ý kiến công văn";
                                        }
                                    }
                                    break;
                                case (int)CommonENum.DON_VI_XU_LY.VAN_THU:
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.VAN_THU_DONG_DAU)
                                    {
                                        if (item.TrangThaiCV == (int)CommonENum.TRANG_THAI_CONG_VAN.KHONG_DAT)
                                        {
                                            item.HanhDongXuLy = "Đóng dấu công văn từ chối";
                                        }
                                        else if (item.TrangThaiCV == (int)CommonENum.TRANG_THAI_CONG_VAN.DAT)
                                        {
                                            item.HanhDongXuLy = "Đóng dấu giấy xác nhận";
                                        }
                                        else
                                        {
                                            item.HanhDongXuLy = "Đóng dấu công văn bổ sung";
                                        }
                                    }
                                    if (item.ActionEnum == (int)CommonENum.FORM_FUNCTION.VAN_THU_BAO_CAO_CONG_VAN_CO_SAI_SOT && item.HoSoIsDat != true)
                                    {
                                        item.HanhDongXuLy = "Báo cáo công văn sai sót";
                                    }
                                    break;
                            }

                            item.NoteColor = NoteColorGenerate(item);
                            item.NoteIconClass = NoteIconClassGenerate(item);
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

        private string NoteColorGenerate(XHoSoXuLyHistoryDto item)
        {
            string defaultColor = "#bfbfbf";
            string res = "";
            try
            {
                res = defaultColor;
                if (item != null)
                {
                    if (item.HoSoIsDat == true)
                    {
                        res = "#26c281";
                    }
                    else if (item.HoSoIsDat == false)
                    {
                        res = "#f2784b";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return res;
        }

        private string NoteIconClassGenerate(XHoSoXuLyHistoryDto item)
        {
            string defaultClass = "glyphicon glyphicon-arrow-left";
            string res = "";
            try
            {
                res = defaultClass;
                if (item != null)
                {
                    if (item.HoSoIsDat == true)
                    {
                        res = "glyphicon glyphicon-check";
                    }
                    else if (item.HoSoIsDat == false)
                    {
                        res = "glyphicon glyphicon-file";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            return res;
        }

    }
}