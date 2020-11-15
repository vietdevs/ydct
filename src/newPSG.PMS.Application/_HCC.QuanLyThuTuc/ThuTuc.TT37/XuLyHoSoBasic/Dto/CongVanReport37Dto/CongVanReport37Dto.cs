using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    public class CongVanReport37Dto
    {
        public int? ThuTucId { get; set; }
        public long? HoSoXuLyId { get; set; }
        public int Nam { get; set; }
        public DateTime? NgayNopHoSo { get; set; }
        public string CoQuanTiepNhan { get; set; }
        public string TenDoanhNghiep { get; set; }
        public string TenCoSo { get; set; }
        public string HoTenChuCoSo { get; set; }
        public string DiaChiCoSo { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public DateTime? TruongPhongNgayKy { get; set; }
        public bool? LanhDaoCucIsCA { get; set; }
        public DateTime? LanhDaoCucNgayKy { get; set; }
        public long? NguoiDuyetId { get; set; }
        public string TenNguoiDuyet { get; set; }
        public string HeaderCV { get; set; }
        public string FooterCV { get; set; }
        public string NoiDungCongVan { get; set; }
        public int? TrangThaiXuLy { get; set; }
        public string MaSoThue { get; set; }
        public bool? HoSoIsDat { get; set; }
        public long? DoanhNghiepId { get; set; }
        public long? ChuyenVienId { get; set; }
        public string TenChuyenVien { get; set; }
        public long? TruongPhongId { get; set; }
        public string TenTruongPhong { get; set; }
        public long? LanhDaoCucId { get; set; }
        public long? LanhDaoDuyetThamDinhId { get; set; }
        public string TenLanhDaoCuc { get; set; }
        public string SoDangKy { get; set; }
        public string MaHoSo { get; set; }
        public int? PhongBanId { get; set; }
        public string LoaiSanPhamDangKy { get; set; }
        public string SoGiayTiepNhan { get; set; }
        public int? TinhUyQuyenId { get; set; }
        public int? LoaiHinhCoSoId { get; set; }
        public string LoaihinhCoSoKhac { get; set; }

        public string TenCoSo_DaSua { get; set; }
        public string DiaChiCoSo_DaSua { get; set; }
        public string LoaiHinhCoSo_CVSua { get; set; }
        public string LoaiHinhSanXuat_CVSua { get; set; }
        public string JsonThamDinhCoSo { get; set; }

        public string LoaiHinhDangKyPhuHop { get; set; }
        public int? ChiCucId { get; set; }
        public int? LoaiHoSoId { get; set; }
        public DateTime? NgayTraKetQua { get; set; }
        public string SoCongVan { get; set; }
        public DateTime? NgayCongVan { get; set; }
        public bool? TruongPhongDaDuyet { get; set; }
        public bool? LanhDaoCucDaDuyet { get; set; }
        public bool? LanhDaoCucDaDuyetThamDinh { get; set; }
        public int? LuongXuLy { get; set; }
        public string NoiDungYeuCauGiaiQuyet { get; set; }
        public string LyDo { get; set; }
        public string TenCanBoHoTro { get; set; }
        public string DienThoaiCanBo { get; set; }
    }
}
