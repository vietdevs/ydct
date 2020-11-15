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
    [Table("NgayNghi")]
    public class NgayNghi : CreationAuditedEntity, IPassivable
    {
        public int LichLamViecId { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string LyDo { get; set; }
        public bool IsActive { get; set; }
    }
}
