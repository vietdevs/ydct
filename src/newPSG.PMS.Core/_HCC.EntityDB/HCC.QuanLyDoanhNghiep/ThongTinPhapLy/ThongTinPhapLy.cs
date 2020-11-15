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
    //Thong tin phap ly cua doanh nghiep
    [Table("ThongTinPhapLy")]
    public class ThongTinPhapLy : FullAuditedEntity<long>
    {
        public long DoanhNghiepId { get; set; }
        public int? LoaiTepDinhKem { get; set; }
        [StringLength(300)]
        [Column(TypeName = "nvarchar")]
        public string TenTep { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DuongDanTep { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string MoTaTep { get; set; }
        public bool? DaTaiLen { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public bool? IsNew { get; set; }
    }
}
