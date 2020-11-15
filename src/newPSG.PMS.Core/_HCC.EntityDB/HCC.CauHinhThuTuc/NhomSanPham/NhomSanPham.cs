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

    //Thong tin danh muc nhom san pham cho thu tuc tu cong bo
    [Table("NhomSanPham")]
    public class NhomSanPham : CreationAuditedEntity, IPassivable
    {
        [StringLength(255)]
        public string TenNhomSanPham { get; set; }
        [StringLength(1000)]
        public string MoTa { get; set; }
        public bool IsActive { get; set; }
        public int? NiisId { get; set; }
    }
}
