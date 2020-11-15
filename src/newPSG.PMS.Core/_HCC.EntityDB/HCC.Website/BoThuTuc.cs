using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("Web_BoThuTuc")]
    public class BoThuTuc : CreationAuditedEntity<int>
    {
        [Required]
        [StringLength(150)]
        public string SoHoSo { get; set; }

        [StringLength(500)]
        public string TenThuTuc { get; set; }

        [StringLength(500)]
        public string DuongDan { get; set; }

        [StringLength(500)]
        public string TenVBQPPL { get; set; }

        [StringLength(500)]
        public string CoQuanThucHien { get; set; }

        public string ThongTinChung { get; set; }
        public string TrinhTuThucHien { get; set; }
        public string ThanhPhanHoSo { get; set; }
        public string YeuCauDieuKien { get; set; }

        [StringLength(250)]
        public string Anh { get; set; }

        public string JsonThongTinKhac { get; set; }
        public bool IsHome { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }

        public int HanhChinhCap { get; set; }
        public int? ThuTucId { get; set; }
    }
}