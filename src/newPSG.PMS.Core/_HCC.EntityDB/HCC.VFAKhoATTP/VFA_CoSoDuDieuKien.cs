using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("VFA_CoSoDuDieuKien")]
    public class VFA_CoSoDuDieuKien : HoSoBase, IVFA_Base
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

        #region HoSo04
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenCoSo { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DiaChiCoSo { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string LoaiSanPhamDangKy { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string HoTenChuCoSo { get; set; }

        [Column(TypeName = "ntext")]
        public string JsonThamDinhCoSo { get; set; }

        public bool? IsUyQuyen { get; set; }
        public int? LoaiHinhCoSoId { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string TenLoaiHinhCoSoKhac { get; set; }

        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string BaoCaoKhacPhucUrl { get; set; }
        public DateTime? NgayKetThucKhacPhuc { get; set; }
        public DateTime? NgayKetThucBoSung { get; set; }
        public long? HoSoTrungId { get; set; }
        public int? LoaiThanhToan { get; set; } //enum LOAI_THANH_TOAN
        public decimal? PhiChuyenVienNhap { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string SanPhamDuocCapPhep { get; set; }
        #endregion
    }
}
