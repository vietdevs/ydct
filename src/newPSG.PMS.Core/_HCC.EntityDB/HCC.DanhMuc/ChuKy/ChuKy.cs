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

    //Thong tin ChuKy Ca
    [Table("ChuKy")]
    public class ChuKy : AuditedEntity<long>, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime, IPassivable
    {
        public long? PId { get; set; }
        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenChuKy { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string MaChuKy { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; }
        public int? LoaiChuKy { get; set; }
        public string ChanChuKy { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string UrlImage { get; set; }
        public byte[] DataImage { get; set; }
        public int? ChieuRong { get; set; }
        public int? ChieuCao { get; set; }

        public bool? IsDaXuLy { get; set; }
    }
}
