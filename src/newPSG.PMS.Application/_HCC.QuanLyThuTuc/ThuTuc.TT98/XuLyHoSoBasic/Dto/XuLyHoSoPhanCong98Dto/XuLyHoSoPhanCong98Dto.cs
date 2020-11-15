using newPSG.PMS.Common.Dto;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    public class InitPhanCong98InputDto
    {
        public long? PhongBanId { get; set; }
        public long? UserId { get; set; }

        public int? RoleLevel { get; set; }
    }

    public class LoadPhanCongPhongBan98Dto
    {
        public int? LoaiHoSoId { set; get; }
        public string TenLoaiHoSo { set; get; }
        public List<ItemDto<int>> ArrPhongBanXuLy { get; set; }
        public LoadPhanCongPhongBan98Dto()
        {
            ArrPhongBanXuLy = new List<ItemDto<int>>();
        }
    }

    public class PhanCongPhongBan98InputDto
    {
        public List<long> ArrHoSoId { set; get; }
        public int? PhongBanId { set; get; }
        public string TenPhongBan { get; set; }
        public string LyDoTuChoi { get; set; }
    }
    public class MotCuaTraGiayTiepNhan98InputDto
    {
        public long HoSoXuLyId { get; set; }
        public long HoSoId { get; set; }
        public bool? HoSoIsDat { get; set; }
        public int? TrangThaiCV { get; set; }
        public string NoiDungYKien { get; set; }
        public string SoTiepNhan { get; set; }
        public int? DonViKeTiep { get; set; }

        //File PDF Ký
        public string TenTepCA { get; set; }
        public string DuongDanTepCA { get; set; }
        public string GiayTiepNhanCA { get; set; }
    }

    public class PhanCongThamDinh98InputDto
    {
        public List<long> ArrHoSoId { set; get; }
        public long? TruongPhongId { set; get; }
        public long? PhoPhongId { set; get; }
     
        public long? ChuyenVienThuLyId { set; get; }
        public long? ChuyenVienPhoiHopId { set; get; }
        public int? LoaiHoSo { set; get; }
        public bool? IsHoSoUuTien { set; get; }
        public int? QuiTrinh { get; set; }
        public bool? IsChuyenNhanh { get; set; }
        public int? PhoPhongThamXet { get; set; }
    }
}