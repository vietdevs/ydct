using Abp.AutoMapper;
using newPSG.PMS.Common.Dto;
using System;
using System.Collections.Generic;

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
#endregion

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(XHoSo))]
    public class HoSo37Dto : XHoSo
    {
        public int? FormId { get; set; }
        public int? FormCase { get; set; }
        public int? FormCase2 { get; set; }
        public List<int> ArrChucNang { get; set; }

        #region HoSoXuLy
        public long HoSoId { get; set; }
        public long? HoSoXuLyId { get; set; }
        public DateTime? NgayHenTra { get; set; }
        //Đơn vị xử lý
        public int? DonViGui { get; set; }
        public long? NguoiXuLyId { get; set; }
        public DateTime? NgayGui { get; set; }
        public string YKienGui { get; set; }
        public int? DonViXuLy { get; set; }
        public long? NguoiGuiId { get; set; }

        public string LyDoTraLai { get; set; }
        //Thành phần xử lý
        public long? LanhDaoBoId { get; set; }
        public bool? LanhDaoBoDaDuyet { get; set; }
        public bool? LanhDaoBoIsCA { get; set; }
        public DateTime? LanhDaoBoNgayKy { get; set; }
        //==
        public long? LanhDaoCucId { get; set; }
        public bool? LanhDaoCucDaDuyet { get; set; }
        public bool? LanhDaoCucIsCA { get; set; }
        public DateTime? LanhDaoCucNgayKy { get; set; }
        //==
        public long? TruongPhongId { get; set; }
        public bool? TruongPhongDaDuyet { get; set; }
        public bool? TruongPhongIsCA { get; set; }
        public DateTime? TruongPhongNgayKy { get; set; }
        //==
        public long? PhoPhongId { get; set; }
        public bool? PhoPhongDaDuyet { get; set; }
        //==
        public long? ChuyenVienThuLyId { get; set; }
        public bool? ChuyenVienThuLyDaDuyet { get; set; }
        public long? ChuyenVienPhoiHopId { get; set; }
        public bool? ChuyenVienPhoiHopDaDuyet { get; set; }
        public string ChuyenVienThuLyName { get; set; }
        public string ChuyenVienPhoiHopName { get; set; }
        //==
        public long? ChuyenGiaId { get; set; }
        public bool? ChuyenGiaDaDuyet { get; set; }
        public long? ToTruongChuyenGiaId { get; set; }
        public bool? ToTruongChuyenGiaDaDuyet { get; set; }
        //==
        public long? VanThuId { get; set; }
        public bool? VanThuDaDuyet { get; set; }
        public bool? VanThuIsCA { get; set; }
        public DateTime? VanThuNgayDongDau { get; set; }
        //==
        public bool? HoSoIsDat { get; set; }
        public string TieuDeCV { get; set; }
        public string NoiDungCV { get; set; }
        public int? TrangThaiCV { get; set; }
        public int? DonViKeTiep { get; set; }

        public long? HoSoXuLyHistoryId_Active { get; set; }

        //Thông tin thẩm xét
        public long? BienBanThamDinhId_ChuyenVienThuLy { get; set; }
        public long? BienBanThamDinhId_ChuyenVienPhoiHop { get; set; }
        public string YKienChung { get; set; }
        public string GiayTiepNhanCA { get; set; }
        public int? TrangThaiXuLy { get; set; }

        // một cửa xử lý
        public DateTime? NgayHenCap { get; set; }
        public string HinhThucCapCTJson { get; set; }
        public string TaiLieuDaNhanJson { get; set; }
        #endregion

        public int? SoNgayXuLy { get; set; }
        public string HsxlDuongDanTepCA { get; set; }
        public string TenHCC { get; set; }
        public string StrLoaiHoSo { get; set; }
        public string StrDonViXuLy { get; set; }
        public string StrDonViGui { get; set; }
        public string StrTrangThai { get; set; }
        public string TenNguoiGui { get; set; }
        public string TenNguoiXuLy { get; set; }
        public int? SoNgayQuaHan { get; set; }
        public string StrQuaHan { get; set; }

        public int? SoGioQuaHanChiTiet { get; set; }
        public int? SoNgayQuaHanChiTiet { get; set; }
        public string StrQuaHanChiTiet { get; set; }

        public List<ItemDto<int>> ArrPhongBanXuLy { get; set; }
        public string TenTruongPhong { get; set; }
        public string TenLanhDaoCuc { get; set; }
        public string TenVanThuMotCua { get; set; }
        public string TenLanhDaoBo { get; set; }

        #region Thanh toán
        public int? TrangThaiNganHang { get; set; }
        public string MaDonHang { get; set; }
        public string MaGiaoDich { get; set; }
        #endregion

        public string SoCongVan { get; set; }
        public DateTime? NgayYeuCauBoSung { get; set; }
        public string NoiDungYeuCauGiaiQuyet { get; set; }
        public string LyDoYeuCauBoSung { get; set; }
        public string TenCanBoHoTro { get; set; }
        public string DienThoaiCanBo { get; set; }
        public string BienBanTongHopUrl { get; set; }

        public DateTime? NgayLapDoanThamDinh { get; set; }
        public long? NguoiLapDoanThamDinhId { get; set; }

    }

    [AutoMap(typeof(XHoSoTepDinhKem))]
    public class HoSoTepDinhKem37Dto : XHoSoTepDinhKem
    {
        public bool? IsSoCongBo { get; set; }
    }

    [AutoMap(typeof(XHoSoXuLy))]
    public class HoSoXuLy37Dto : XHoSoXuLy
    {
        #region Dùng khi có nhiều 2 người thẩm sét
        public BienBanThamDinhThuLy37Dto BienBanThamDinh_ChuyenVienThuLy { set; get; }
        public BienBanThamDinhPhoiHop37Dto BienBanThamDinh_ChuyenVienPhoiHop { set; get; }
        #endregion
    }

    [AutoMap(typeof(XHoSoXuLyHistory))]
    public class HoSoXuLyHistory37Dto : XHoSoXuLyHistory
    {
        public string NguoiXuLy_Pre { set; get; }
        public string NguoiXuLy { set; get; }
        public string HanhDongXuLy { set; get; }
        public string NoteColor { get; set; }
        public string NoteIconClass { get; set; }
        public string StrNgayXuLy { get; set; }
        public int STT { get; set; }
        public string StrKetQuaXuLy { get; set; }
    }

    #region Dùng khi có nhiều người thẩm sét
    public class BienBanThamDinhChiTiet37Dto
    {
        public BienBanThamDinhThuLy37Dto BienBanThamDinhThuLy { get; set; }
        public BienBanThamDinhPhoiHop37Dto BienBanThamDinhPhoiHop { get; set; }
    }

    public class BienBanThamDinhThuLy37Dto
    {
        public string  YKienChuyenVienThuLy{ get; set; }
    }

    public class BienBanThamDinhPhoiHop37Dto
    {
        public bool? IsCopyThamXet { get; set; }
        public string YKienChuyenVienPhoiHop { get; set; }
    }
    #endregion
}