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

    //Thong tin danh muc tinh
    [Table("DanhMucTinh")]
    public class Tinh : CreationAuditedEntity, IPassivable
    {
        [Required]
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Ten { get; set; }
        public int? VungMienId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string CapHanhChinh { get; set; }
        public int? NiisId { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; }
        public bool IsActive { get; set; }
    }
}
