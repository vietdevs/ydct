using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("HD_Articles")]
    public class Article : FullAuditedEntity<int>, IPassivable
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string File { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
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