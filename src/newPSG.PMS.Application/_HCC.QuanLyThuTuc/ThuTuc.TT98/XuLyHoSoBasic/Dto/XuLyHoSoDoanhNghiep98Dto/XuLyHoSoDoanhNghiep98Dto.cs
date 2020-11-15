using System;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    public class KyVaThanhToan98InputDto 
    {
        public long HoSoId { get; set; }
        public int? ThuTucId { get; set; }
    }

    public class ThanhToanChuyenKhoan98InputDto
    {
        public int? ThuTucId { get; set; }
        public long HoSoId { get; set; }
        public int? PhiDaNop { get; set; }
        public string SoTaiKhoanNop { get; set; }
        public string SoTaiKhoanHuong { get; set; }
        public string MaGiaoDich { get; set; }
        public string MaDonHang { get; set; }
        public DateTime? NgayGiaoDich { get; set; }
        public string DuongDanHoaDonThanhToan { get; set; }
        public string GhiChu { get; set; }
    }

    public class UpdateKySo98InputDto
    {
        public int? ThuTucId { get; set; }
        public long HoSoId { get; set; }
        public long? HoSoXuLyId { get; set; }
        public string DuongDanTep { get; set; }        
    }

    public class CopyFileTaiLieuDaTaiVaoHoSo98Input
    {
        public long? HoSoId { get; set; }
        public List<HoSoTepDinhKem98Dto> DanhSachFileDaTai { get; set; }
        public string FolderTemp { get; set; }
    }
}