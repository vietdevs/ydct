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
    [Table("DanhMucXa")]
    public class Xa : FullAuditedEntity<long>, IPassivable
    {
        [Required]
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Ten { get; set; }
        public long? HuyenId { get; set; }

        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string CapHanhChinh { get; set; }
        public long? NiisId { get; set; }
        public bool IsActive { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string MaLGSP { get; set; }
    }
}
