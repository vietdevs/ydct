using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("PhongBan")]
    public class PhongBan : FullAuditedEntity, IPassivable, IMustHaveTenant
    {
        public int? PId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string TenPhongBan { get; set; }
        [StringLength(150)]
        [Column(TypeName = "nvarchar")]
        public string MaPhongBan { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoai { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Fax { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Email { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string DiaChi { get; set; }
        public int? QuiTrinh { get; set; }
        public bool IsActive { get; set; }
        public int TenantId { get; set; }
    }
}
