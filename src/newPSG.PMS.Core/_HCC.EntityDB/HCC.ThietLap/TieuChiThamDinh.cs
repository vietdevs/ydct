using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{

    [Table("TieuChiThamDinh")]
    public class TieuChiThamDinh : CreationAuditedEntity
    {
        public int? ThuTucId { get; set; }
        public int? RoleLevel { get; set; }
        public int? TieuBanEnum { get; set; }
        public int? LoaiBienBanThamDinhId { get; set; }
        public int? PId { get; set; }
        public int? Level { get; set; }
        public bool? IsTieuDe { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string MaNoiDung { get; set; }
        [StringLength(20)]
        [Column(TypeName = "nvarchar")]
        public string STT { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TieuDeThamDinh { get; set; }
        public bool? IsValidate { get; set; }
    }
}