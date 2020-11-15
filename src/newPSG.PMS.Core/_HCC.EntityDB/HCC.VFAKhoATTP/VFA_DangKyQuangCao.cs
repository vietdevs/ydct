using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("VFA_DangKyQuangCao")]
    public class VFA_DangKyQuangCao : HoSoBase, IVFA_Base
    {
        public long? HoSoId { get; set; }
        public long? LienThongId { get; set; }
        [Column(TypeName = "ntext")]
        public string TepDinhKemKQJson { get; set; }
        [Column(TypeName = "ntext")]
        public string KetQuaXuLyJson { get; set; } //Object
        public int? TrangThaiLienThong { get; set; }
        public DateTime? NgayLienThong { get; set; }
        public DateTime? NgayLienThongThanhCong { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Guid { get; set; }

        #region HoSo03
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenSanPham { get; set; }

        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenCoSo { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DiaChiCoSo { get; set; }
        [Column(TypeName = "ntext")]
        public string JsonSanPham { get; set; }
        [Column(TypeName = "ntext")]
        public string LoaiQuangCaoJson { get; set; }
        [Column(TypeName = "ntext")]
        public string LoaiQuangCaoKhacJson { get; set; }
        public bool? IsDangKyCongBo { get; set; }
        #endregion
    }
}
