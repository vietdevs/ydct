using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("PhanLoaiHoSo_PhanCong")]
    public class PhanLoaiHoSo_PhanCong : CreationAuditedEntity
    {
        public int? PhanLoaiHoSoId { get; set; }
        public int? TieuBanEnum { get; set; }
        public int? SoLuong { get; set; }
        
        public int? LoaiBienBanThamDinhId { get; set; }
    }
}