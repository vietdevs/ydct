using System;

namespace newPSG.PMS.Dto
{
    public class TraCuuHoSo99Input : PagedAndSortedInputDto
    {
        public string Keyword { get; set; }
        public DateTime? NgayCapTu { get; set; }
        public DateTime? NgayCapToi { get; set; }
        public int? TinhId { get; set; }
        public int? LoaiQuangCaoId { get; set; }
        public int? DoanhNghiepId { get; set; }
        public string DiaChi { get; set; }
        public long? HoSoId { get; set; }
        public int? ChiCucId { get; set; }
        public string MaSoThue { get; set; }
        public string SoDangKy { get; set; }
        public string MaHoSo { get; set; }
        public string TenDoanhNghiep { get; set; }
        public string TenLoaiQuangCao { get; set; }
        public string TenSanPham { get; set; }
        public string TenantName { get; set; }
    }
}
