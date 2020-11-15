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
    public class BienBanThamDinhBase : AuditedEntity<long>, ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime
    {
        public int? ThuTucId { get; set; }
        public int? LoaiHoSoId { get; set; }
        public long? HoSoId { get; set; }
        public long? HoSoXuLyId { get; set; }
        public long? NguoiThamXetId { get; set; }
        public int? VaiTroThamXet { get; set; }
        [Column(TypeName = "ntext")]
        public string NoiDungThamXetJson { get; set; }
        public string YKienBoSung { get; set; }
        public bool? IsThamXetDat { get; set; }
        public bool? IsCopyThamXet { get; set; }
        public long? CopyThamXetId { get; set; }
    }
}
