using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("TT99_HoSoXuLy")]
    public class HoSoXuLy99 : HoSoXuLyBase
    {
        //Thông tin thẩm xét
        public long? BienBanThamDinhId_ChuyenVienThuLy { get; set; }
        public long? BienBanThamDinhId_ChuyenVienPhoiHop { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string YKienChung { get; set; }
    }
}