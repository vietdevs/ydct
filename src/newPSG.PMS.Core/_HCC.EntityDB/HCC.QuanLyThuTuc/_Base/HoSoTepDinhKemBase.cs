using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static newPSG.PMS.CommonENum;

namespace newPSG.PMS.EntityDB
{
    public class HoSoTepDinhKemBase : AuditedEntity<long>, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime, IPassivable
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
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string MoTaTep { get; set; }
        public bool? IsCA { get; set; }
        public bool? DaTaiLen { get; set; }
        public bool IsActive { get; set; }
        public long? UploadFileId { get; set; }
    }
}
