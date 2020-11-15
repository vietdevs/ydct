using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("Web_CauHinhChung")]
    public class CauHinhChung : CreationAuditedEntity<int>
    {
        [Required]
        [StringLength(150)]
        public string TuKhoa { get; set; }

        public int KieuCauHinh { get; set; }
        public string GiaTri { get; set; }
        public string MoTa { get; set; }
        public int NhomCauHinh { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }
}