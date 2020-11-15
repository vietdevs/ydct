using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("PhongBanNhomSanPham")]
    public class PhongBanNhomSanPham : CreationAuditedEntity, IMustHaveTenant
    {
        public int PhongBanId { get; set; }
        public int NhomSanPhamId { get; set; }
        public int QuiTrinh { get; set; }
        public int TenantId { get; set; }
    }
}