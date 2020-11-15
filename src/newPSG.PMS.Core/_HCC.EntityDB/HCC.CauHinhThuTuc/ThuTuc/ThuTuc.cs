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
    [Table("ThuTuc")]
    public class ThuTuc : AuditedEntity, IAudited, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime, IPassivable
    {
        public int? NhomThuTucId { get; set; }
        public int? ThuTucIdEnum { get; set; }
        //Thông tin
        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaThuTuc { get; set; }
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenThuTuc { get; set; }
        [Column(TypeName = "ntext")]
        public string MoTa { get; set; }
        public bool IsActive { get; set; }

        //Cấu hình xử lý
        public int? QuiTrinhXuLy { get; set; }
        public int? SoNgayXuLy { get; set; }
        public bool? IsPhiXuLy { get; set; }
        public decimal? PhiXuLy { get; set; }
        
        public byte[] DataImage { get; set; }
        public string PathImage { get; set; }

        public string LinhVuc { get; set; }
        public string CoQuanThucHien { get; set; }
    }

    [Table("HCCSetting")]
    public class HCCSetting : AuditedEntity, IAudited, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime
    {
        public int? TenantId { get; set; }
        public long? UserId { get; set; }
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string Value { get; set; }
    }
}
