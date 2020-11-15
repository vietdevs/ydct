
#region Class Riêng Cho Từng Thủ tục
using System;
using XHoSo = newPSG.PMS.EntityDB.HoSo37;
#endregion

namespace newPSG.PMS.Dto
{
    public class TraCuuHoSo37Dto : XHoSo
    {
        public string LoaiQuangCao { get; set; }
        public string StrTinh { get; set; }
        public long? TruongPhongId { get; set; }
        public long? VanThuId { get; set; }
        public long? LanhDaoCucId { get; set; }
        public long? ChuyenVienThuLyId { get; set; }
        public string StrDonViXuLy { get; set; }
        public string StrDonViGui { get; set; }
        public string StrTrangThai { get; set; }
        public int? DonViGui { get; set; }
        public long? NguoiXuLyId { get; set; }
        public DateTime? NgayGui { get; set; }
        public string YKienGui { get; set; }
        public int? DonViXuLy { get; set; }
        public long? NguoiGuiId { get; set; }
        public string GiayTiepNhanCA { get; set; }

        public string ChuyenVienThuLyName { get; set; }
        public string TenNguoiGui { get; set; }
        public string TenNguoiXuLy { get; set; }
    }
    
}
