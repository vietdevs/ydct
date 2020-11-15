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

    //Thong tin ngay nghi le
    [Table("LichLamViec")]
    public class LichLamViec : CreationAuditedEntity, IPassivable
    {
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string TenLich { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool T2 { get; set; }
        public bool T3 { get; set; }
        public bool T4 { get; set; }
        public bool T5 { get; set; }
        public bool T6 { get; set; }
        public bool T7 { get; set; }
        public bool CN { get; set; }
        public bool IsActive { get; set; }
    }
}
