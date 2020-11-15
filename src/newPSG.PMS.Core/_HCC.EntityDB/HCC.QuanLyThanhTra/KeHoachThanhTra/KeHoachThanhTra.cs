using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("KeHoachThanhTra")]
    public class KeHoachThanhTra : FullAuditedEntity<long>
    {
        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        public string TenKeHoach { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        public string TenTruongDoan  { get; set; }
        [Column(TypeName = "ntext")]
        public string NoiDungThanhTra { get; set; }

        public bool? IsChiCuc { get; set; }
        public int? ChiCucId { get; set; }
    }
}
