using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("TT37_HoSoDoanThamDinh")]
    public class TT37_HoSoDoanThamDinh : CreationAuditedEntity<long>
    {
        public long? HoSoId { get; set; }
        public int? ThuTucId { get; set; }
        public long? UserId { get; set; }
        public int? VaiTroEnum { get; set; }
        public string TenVaiTro { get; set; }
        public int? TrangThaiXuLy { get; set; }
        public string NoiDungYkien { get; set; }
    }
}
