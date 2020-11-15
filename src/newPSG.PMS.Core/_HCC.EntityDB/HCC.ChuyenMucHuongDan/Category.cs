using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newPSG.PMS.EntityDB
{
    [Table("HD_Categories")]
    public class Category : FullAuditedEntity<int>, IPassivable
    {
        [Required]
        public string Title { get; set; }

        public int? GroupEnum { get; set; }//nhóm hướng dẫn, tin tức

        public string Description { get; set; }
        public string Details { get; set; }
        public string File { get; set; }
        public int? ParentId { get; set; }
        public int? Level { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public int? RoleLevel { get; set; }

        #region HCC
        public int? TrangThaiXuLy { get; set; }

        public int? ThuTucEnum { get; set; }
        public string FileDinhKemJson { get; set; }
        public int? NhomYeuCauId { get; set; } //CategoryId
        public string BienBanHopJson { get; set; }

        //Mức độ phức tạp của thủ tục
        public int? SoThanhPhanThamGia { get; set; }
        public string ThanhPhanThamGiaJson { get; set; }
        public int? SoLuongQuyTrinh { get; set; }
        public string LuongQuyTrinhJson { get; set; }
        public int? SoBuocThucHien { get; set; }
        public string BuocThucHienJson { get; set; }
        public int? SoBuocPhatSinh { get; set; }
        public string BuocPhatSinhJson { get; set; }
        public int? SoBuocHuyBo { get; set; }
        public string BuocHuyBoJson { get; set; }
        #endregion
    }
}