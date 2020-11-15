using System;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{

    public class HoSoNopRaSoat37InputDto
    {
        public long HoSoId { get; set; }
        public string DuongDanTep { get; set; }
    }
    public class KyVaThanhToan37InputDto 
    {
        public long HoSoId { get; set; }
        public int? ThuTucId { get; set; }
    }

    public class ThanhToanChuyenKhoan37InputDto
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

    public class UpdateKySo37InputDto
    {
        public int? ThuTucId { get; set; }
        public long HoSoId { get; set; }
        public long? HoSoXuLyId { get; set; }
        public string DuongDanTep { get; set; }        
    }

    public class CopyFileTaiLieuDaTaiVaoHoSo37Input
    {
        public long? HoSoId { get; set; }
        public List<HoSoTepDinhKem37Dto> DanhSachFileDaTai { get; set; }
        public string FolderTemp { get; set; }
    }
}