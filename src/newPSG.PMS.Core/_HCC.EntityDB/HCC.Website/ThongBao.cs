using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("Web_ThongBao")]
    public class ThongBao : CreationAuditedEntity<int>
    {
        [Required]
        [StringLength(200)]
        public string TenThongBao { get; set; }

        [StringLength(500)]
        public string DuongDan { get; set; }

        [StringLength(250)]
        public string Anh { get; set; }

        public string MoTa { get; set; }
        public string ChiTiet { get; set; }

        [StringLength(400)]
        public string TaiLieu { get; set; }

        public string JsonThongTinKhac { get; set; }
        public int SortOrder { get; set; }
        public bool IsHome { get; set; }
        public bool IsActive { get; set; }
    }
}