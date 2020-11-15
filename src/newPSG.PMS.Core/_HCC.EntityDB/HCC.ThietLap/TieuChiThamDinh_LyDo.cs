using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{

    [Table("TieuChiThamDinh_LyDo")]
    public class TieuChiThamDinh_LyDo : CreationAuditedEntity
    {
        public int? TieuChiThamDinhId { get; set; }
        [StringLength(100)]
        [Column(TypeName = "nvarchar")]
        public string MaLyDo { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string LyDo { get; set; }
    }
}