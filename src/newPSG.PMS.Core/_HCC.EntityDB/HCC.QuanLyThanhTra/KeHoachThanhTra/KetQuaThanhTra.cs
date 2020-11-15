using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("KetQuaThanhTra")]
    public class KetQuaThanhTra : FullAuditedEntity<long>
    {
        public long KeHoachChiTietId { get; set; }
        public DateTime? NgayThanhTra { get; set; }
        [Column(TypeName = "ntext")]
        public string KetQua { get; set; }
        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        public string MucDoViPham { get; set; }
        [Column(TypeName = "ntext")]
        public string CachKhacPhuc { get; set; }
        public decimal? TienPhat { get; set; }
        public bool IsViPham { get; set; }
        public bool IsCongBo { get; set; }
    }
}
