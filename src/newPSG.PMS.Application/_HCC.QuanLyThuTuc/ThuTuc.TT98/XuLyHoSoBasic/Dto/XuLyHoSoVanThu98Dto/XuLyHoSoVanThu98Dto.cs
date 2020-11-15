namespace newPSG.PMS.Dto
{
    public class VanThuXuLy98InputDto
    {
        public long HoSoXuLyId { get; set; }
        public long HoSoId { get; set; }
        public string NoiDungYKien { get; set; }
        public int? TrangThaiXuLy { get; set; }

        //File PDF Ký
        public string TenTepCA { get; set; }
        public string DuongDanTepCA { get; set; }
        public string GiayTiepNhanCA { get; set; } //Giấy tiếp nhận full
    }
}