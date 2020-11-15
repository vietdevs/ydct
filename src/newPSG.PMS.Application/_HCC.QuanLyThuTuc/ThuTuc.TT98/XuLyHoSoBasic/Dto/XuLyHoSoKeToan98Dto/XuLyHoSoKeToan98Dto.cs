namespace newPSG.PMS.Dto
{
    public class ThanhToanChuyenKhoanKeToan98InputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? LoaiHoSoId { get; set; }
        public int? TinhId { get; set; }
        public int? FormCase { get; set; }
    }
    public class LoadXacNhanKeToan98InputDto
    {
        public long ThanhToanId { get; set; }
    }
    public class LuuXacNhanKeToan98InputDto
    {
        public long HoSoId { get; set; }
        public long ThanhToanId { get; set; }
        public int? XacNhanThanhToan { get; set; }
        public string YKienXacNhan { get; set; }
        public int? PhiXacNhan { get; set; }
    }
}