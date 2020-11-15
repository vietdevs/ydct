
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{

    [Table("PhanLoaiHoSo_Filter")]
    public class PhanLoaiHoSo_Filter : CreationAuditedEntity
    {
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; } 
        public int? PhanLoaiHoSoId { get; set; }
        public string JsonFilter { get; set; }
    }
}