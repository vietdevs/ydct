using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    //Thong tin ho so
    [Table("TT98_HoSo")]
    public class HoSo98 : HoSoBase
    {
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenCoSo { get; set; } //Thông tin demo, có thể xóa nếu nghiệp vụ không cần

        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string DiaChiCoSo { get; set; } //Thông tin demo, có thể xóa nếu nghiệp vụ không cần

        [Column(TypeName = "ntext")]
        public string JsonDonHang { get; set; } //Thông tin demo, có thể xóa nếu nghiệp vụ không cần
    }
}