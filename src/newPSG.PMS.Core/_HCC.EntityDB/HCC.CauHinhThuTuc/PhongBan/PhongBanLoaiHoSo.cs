using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("PhongBanLoaiHoSo")]
    public class PhongBanLoaiHoSo : CreationAuditedEntity, IMustHaveTenant
    {
        public int PhongBanId { get; set; }
        public int LoaiHoSoId { get; set; }
        public int? QuiTrinh { get; set; }
        public int TenantId { get; set; }
    }
}
