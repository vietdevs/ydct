using System;

namespace newPSG.PMS.Dto
{
    public class TraCuuHoSo37Input : PagedAndSortedInputDto
    {
        public int? TinhId { get; set; }
        public long? HuyenId { get; set; }
        public long? XaId { get; set; }
        public int? DoanhNghiepId { get; set; }
        public string DiaChi { get; set; }
        public string MaSoThue { get; set; }
        public string MaHoSo { get; set; }
        public int? TrangThaiHoSo { get; set; }
        public long? TruongPhongId { get; set; }
        public long? VanThuId { get; set; }
        public long? LanhDaoCucId { get; set; }
        public long? ChuyenVienThuLyId { get; set; }
        public string HoTenNguoiDeNghi { get; set; }
    }
}
