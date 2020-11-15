using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("KeHoachThanhTraChiTiet")]
    public class KeHoachThanhTraChiTiet : FullAuditedEntity<long>
    {
        public long KeHoachThanhTraId { get; set; }
        public int? EnumChiTietId { get; set; }
        public long? ChiTietId { get; set; }
        public long? DoanhNghiepId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string DiaChi { get; set; }
        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        public string TenDoanhNghiep { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string MaSoThue { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string TenSanPhamDangKy { get; set; }
        public int? NhomSanPhamDangKyId { get; set; }
    }
}
