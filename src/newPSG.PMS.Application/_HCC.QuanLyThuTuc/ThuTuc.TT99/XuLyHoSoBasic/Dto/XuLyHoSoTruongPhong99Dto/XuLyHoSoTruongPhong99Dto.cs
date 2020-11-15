namespace newPSG.PMS.Dto
{    
    public class InitTruongPhongDuyet99InputDto
    {
        public long? PhongBanId { get; set; }
        public long? UserId { get; set; }
        public int? RoleLevel { get; set; }
    }
    public class LoadTruongPhongDuyet99InputDto
    {
        public long HoSoXuLyId { get; set; }
        public long? HoSoId { get; set; }
    }
    public class LuuTruongPhongDuyet99InputDto
    {
        public long HoSoXuLyId { get; set; }
        public long? HoSoId { get; set; }

        public bool? HoSoIsDatChuyenVien { get; set; }

        public bool? HoSoIsDat { get; set; }
        public string NoiDungCV { get; set; }
        public string TieuDeCV { get; set; }
        public int? TrangThaiCV { get; set; }
        public int? DonViKeTiep { get; set; }
        public bool? IsSuaCV { get; set; }
        public long? LanhDaoCucId { get; set; }
        public int? KieuYKien { get; set; }
        public string NoiDungYKien { get; set; }

        //Lãnh đạo phòng cho ý kiến trong Biên Bản Thẩm Định
        public string YKienChung { get; set; }

        //File PDF Ký
        public string TenTepCA { get; set; }
        public string DuongDanTepCA { get; set; }
    }
}