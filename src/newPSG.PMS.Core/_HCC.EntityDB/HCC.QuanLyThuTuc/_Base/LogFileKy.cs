using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    public class LogFileKy : AuditedEntity<long>, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime
    {
        public int? ThuTucId { get; set; }
        public long? HoSoId { get; set; }
        public int? LoaiTepDinhKem { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenTep { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DuongDanTep { get; set; }
        public bool? DaSuDung { get; set; }
    }
}
