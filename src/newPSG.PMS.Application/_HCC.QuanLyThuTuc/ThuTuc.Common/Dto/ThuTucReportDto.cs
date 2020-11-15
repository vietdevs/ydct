using Abp.Application.Services.Dto;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;

namespace newPSG.PMS.ThuTucCommon.Dto
{
    public class HoSoReportDto : HoSoBase
    {
        // total
        public int? TotalRows { get; set; }

        public int? FormId { get; set; }

        public int? FormCase { get; set; }
        public int? FormCase2 { get; set; }
        public List<int> ArrChucNang { get; set; }

        // chua json cac don hang
        public string CommonJson { get; set; }
        public string JsonDonHang { get; set; }

        #region HoSoXuLy
        public long HoSoId { get; set; }
        //public int? LoaiHoSoId { get; set; }
        //public int? QuiTrinh { get; set; }
        //public bool? IsHoSoBS { get; set; }
        //public DateTime? NgayTiepNhan { get; set; }
        public DateTime? NgayHenTra { get; set; }
        //Đơn vị xử lý
        public int? DonViGui { get; set; }
        public long? NguoiXuLyId { get; set; }
        public DateTime? NgayGui { get; set; }
        public string YKienGui { get; set; }
        public int? DonViXuLy { get; set; }
        public long? NguoiGuiId { get; set; }
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
        public DateTime? VanThuNgayDuyet { get; set; }
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
        //public string DuongDanTepCA { get; set; }
        //public DateTime? NgayTraKetQua { get; set; }
        public string GiayTiepNhanCA { get; set; }
        #endregion

        public int? SoNgayXuLy { get; set; }
        public string HsxlDuongDanTepCA { get; set; }
        public string TenChiCuc { get; set; }
        public string StrDonViXuLy { get; set; }
        public string StrDonViGui { get; set; }
        public string StrTrangThai { get; set; }
        public string TenNguoiGui { get; set; }
        public string TenNguoiXuLy { get; set; }
        public int? SoNgayQuaHan { get; set; }
        public string StrQuaHan { get; set; }
        public List<ItemDto<int>> ArrPhongBanXuLy { get; set; }
        public string TenTruongPhong { get; set; }
        public string TenLanhDaoCuc { get; set; }
        public string TenLanhDaoBo { get; set; }

        #region Thanh toán
        public int? TrangThaiNganHang { get; set; }
        public string MaDonHang { get; set; }
        public string MaGiaoDich { get; set; }
        public int? LoaiDonHang { get; set; }
        #endregion
    }
    public class HoSoReportInputDto : PagedAndSortedInputDto
    {
        public int? FormId { get; set; }
        public int? FormCase { get; set; } //Điều kiện lọc 1
        public int? FormCase2 { get; set; } //Điều kiện lọc 2

        public string Keyword { get; set; }
        public string MaHoSo { get; set; }
        public DateTime? NgayThanhToanTu { get; set; }
        public DateTime? NgayThanhToanToi { get; set; }
        public DateTime? NgayNopTu { get; set; }
        public DateTime? NgayNopToi { get; set; }

        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public long? LoaiHoSoId { get; set; }
        public string LoaiDonHangIds { get; set; }
        public int? TinhId { get; set; }
        public int? DoanhNghiepId { get; set; }
        public int? PhongBanId { get; set; }
        public int? NhomThuTucId { get; set; }
        //Host filter
        public int? OnIsCA { get; set; }
        // flag only total
        public bool? IsOnlyToTal { get; set; }
    }

    public class HoSoReportAndTotalFormCase
    {
        public PagedResultDto<HoSoReportDto> DataGrid { get; set; }
        public List<int> TotalCountFormCase { get; set; }
    }
    public class TotalLabelFormCase
    {
        public int? Case0 { get; set; }
        public int? Case1 { get; set; }
        public int? Case2 { get; set; }
        public int? Case3 { get; set; }
        public int? Case4 { get; set; }
        public int? Case5 { get; set; }
        public int? Case6 { get; set; }
        public int? Case7 { get; set; }
        public int? Case8 { get; set; }
        public int? Case9 { get; set; }
    }
    public class ThongKeTheoNgayDashBoardDto
    {
        public DateTime Ngay { get; set; }
        public string NgayThongKe { get; set; }
        public int? SoHoSo { get; set; }
    }
    public class ThongKeTheoNgayDashBoardInput
    {
        public DateTime NgayThongKe { get; set; }
    }
}
