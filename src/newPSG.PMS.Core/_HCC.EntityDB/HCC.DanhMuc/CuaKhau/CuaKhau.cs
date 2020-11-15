using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{

    //Thong tin danh muc quoc gia
    [Table("DanhMucCuaKhau")]
    public class CuaKhau : CreationAuditedEntity, IPassivable
    {
        [StringLength(10)]
        public string CuaKhauCode { get; set; }
        [Required]
        [StringLength(3000)]
        [Column(TypeName = "nvarchar")]
        public string TenCuaKhauVN { get; set; }

        [StringLength(3000)]
        [Column(TypeName = "nvarchar")]
        public string TenCuaKhauNuocNgoai { get; set; }

        public int? TinhId { get; set; }

        public bool IsActive { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string QuocGia { get; set; }
        
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string KhuKinhTe { get; set; }
    }
}
