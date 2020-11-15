using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("DonViChuyenGia")]
    public class DonViChuyenGia : AuditedEntity
    {
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; }
        public int? ThuTucId { get; set; }
        public long TruongDonViId { get; set; }
        public bool IsTrongCuc { get; set; }
        public int? TinhId { get; set; }
        public long? HuyenId { get; set; }
        public long? XaId { get; set; }
    }
}