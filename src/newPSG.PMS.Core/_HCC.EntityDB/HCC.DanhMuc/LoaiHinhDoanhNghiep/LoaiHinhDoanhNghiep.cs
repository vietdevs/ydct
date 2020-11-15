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

    //Thong tin danh muc loai hinh doanh nghiep
    [Table("DanhMucLoaiHinhDoanhNghiep")]
    public class LoaiHinhDoanhNghiep : CreationAuditedEntity, IPassivable
    {
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenLoaiHinh { get; set; }

        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; }

        public bool IsActive { get; set; }
    }
}
