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

    //Thong tin danh muc quoc gia
    [Table("DanhMucQuocGia")]
    public class QuocGia : CreationAuditedEntity, IPassivable
    {
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenQuocGia { get; set; }

        [Required]
        [StringLength(30)]
        [Column(TypeName = "nvarchar")]
        public string MaQuocGia { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; }
        public bool IsActive { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string COUNTRY_IMG_URL { get; set; }
        public int? NiisId { get; set; }
    }
}
