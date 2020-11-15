namespace newPSG.PMS.Dto
{
    public class LanhDaoXuLy99Input 
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
}