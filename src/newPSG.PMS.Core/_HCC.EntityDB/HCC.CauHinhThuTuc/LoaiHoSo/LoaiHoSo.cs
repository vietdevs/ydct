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
    [Table("LoaiHoSo")]
    public class LoaiHoSo : CreationAuditedEntity, IPassivable
    {
        public int? ThuTucId { get; set; }
        //Thông tin
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenLoaiHoSo { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; }
        public bool IsActive { get; set; }

        //Cấu hình xử lý
        public bool? IsXuLy { get; set; }
        public int? QuiTrinhXuLy { get; set; }
        public int? SoNgayXuLy { get; set; }
        public decimal? PhiXuLy { get; set; }

        //[Column(TypeName = "image")]
        public byte[] DataImage { get; set; }
        public bool? IsCoChiTieu { get; set; }
        public int? NiisId { get; set; }
        public int? SoNgaySuaDoiBoSung { get; set; }
    }
}
