
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{

    [Table("PhanLoaiHoSo")]
    public class PhanLoaiHoSo : CreationAuditedEntity
    {
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; }
        public int? RoleLevel { get; set; }
        public int? ThuTucId { get; set; }
    }
}