#region Class Riêng Cho Từng Thủ tục

using XHoSo = newPSG.PMS.EntityDB.HoSo99;
using XHoSoTepDinhKem = newPSG.PMS.EntityDB.HoSoTepDinhKem99;
using XHoSoXuLy = newPSG.PMS.EntityDB.HoSoXuLy99;
using XHoSoXuLyHistory = newPSG.PMS.EntityDB.HoSoXuLyHistory99;
using XBienBanThamDinh = newPSG.PMS.EntityDB.BienBanThamDinh99;
using XHoSoDto = newPSG.PMS.Dto.HoSo99Dto;
using XHoSoInputDto = newPSG.PMS.Dto.HoSoInput99Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem99Dto;
using XHoSoXuLyDto = newPSG.PMS.Dto.HoSoXuLy99Dto;
using XHoSoXuLyHistoryDto = newPSG.PMS.Dto.HoSoXuLyHistory99Dto;
using XBienBanThamDinhDto = newPSG.PMS.Dto.BienBanThamDinh99Dto;
using newPSG.PMS.Common.Dto;
#endregion

namespace newPSG.PMS.Dto
{
    public class InitThamnDinh99InputDto
    {
        public long? PhongBanId { get; set; }
        public long? UserId { get; set; }
        public int? RoleLevel { get; set; }
    }

    public class LoadThamDinh99InputDto
    {
        public long HoSoXuLyId { get; set; }
        public long? HoSoId { get; set; }
    }

    public class LuuThamDinh99InputDto : XHoSoXuLy
    {
        public long HoSoXuLyId { get; set; }

        #region Object chuyển thẩm định

        public bool? HoSoIsDat_Input { get; set; }
        public int? TrangThaiCV_Input { get; set; }
        public int? DonViKeTiep_Input { get; set; }
        public string NoiDungCV_Input { get; set; }
        public string TieuDeCV_Input { get; set; }
        public bool? IsChuyenNhanh { get; set; }
        public string LyDoChuyenNhanh { get; set; }
        public int? KieuChuyenNhanh { get; set; }
        public int? ActionEnum_Input { get; set; }
        #endregion

        #region Biên bản thẩm định
        public XBienBanThamDinhDto BienBanThamDinh { get; set; }
        #endregion
    }

    #region Object GetThamDinh Common
    public class GetBienBanThamDinh99Dto
    {
        public XHoSoXuLyDto HoSoXuLy { get; set; }
        public XBienBanThamDinhDto BienBanThamDinh { get; set; }
        public XBienBanThamDinhDto BienBanThamDinhCV1 { get; set; }
        public XBienBanThamDinhDto BienBanThamDinhCV2 { get; set; }
        public NguoiDuyet99Dto NguoiDuyet { get; set; }
        public XHoSoXuLyHistoryDto DuyetHoSo { get; set; }
        public bool? IsPhoiHopDuyet { get; set; }
    }
    public class NguoiDuyet99Dto
    {
        public long? ChuyenVienThuLyId { get; set; }
        public string TenChuyenVienThuLy { get; set; }
        public long? ChuyenVienPhoiHopId { get; set; }
        public string TenChuyenVienPhoiHop { get; set; }
        public long? PhoPhongId { get; set; }
        public string TenPhoPhong { get; set; }

        public long? ChuyenGiaId { get; set; }
        public string TenChuyenGia { get; set; }
        public long? ChuyenGiaPhoiHopId { get; set; }
        public string TenChuyenGiaPhoiHop { get; set; }

        public long? ToTruongChuyenGiaId { get; set; }
        public string TenToTruongChuyenGia { get; set; }

        public long? NguoiGuiId { get; set; }
        public string TenNguoiGui { get; set; }
    }    
    #endregion
}