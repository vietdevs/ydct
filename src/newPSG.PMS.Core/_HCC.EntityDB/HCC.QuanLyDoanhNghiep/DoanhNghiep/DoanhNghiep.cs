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
    //Thong tin doanh nghiep
    [Table("DoanhNghiep")]
    public class DoanhNghiep : FullAuditedEntity<long>
    {
        [Required]
        [StringLength(50)]
        public string MaSoThue { get; set; }

        [StringLength(2000)]
        public string TenDoanhNghiep { get; set; }
        [StringLength(300)]
        [Column(TypeName = "nvarchar")]
        public string TenTiengAnh { get; set; }

        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string TenVietTat { get; set; }
        [StringLength(2000)]
        [Column(TypeName = "nvarchar")]
        public string DiaChi { get; set; }
        public int? LoaiHinhDoanhNghiepId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string TenLoaiHinh { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoai { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Fax { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Website { get; set; }
        public int? TinhId { get; set; }
        public int? HuyenId { get; set; }
        public int? XaId { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string EmailDoanhNghiep { get; set; }
        public int? ChucVuNguoiDaiDienID { get; set; }
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenDangNhap { get; set; }
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string EmailXacNhan { get; set; }
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenNguoiDaiDien { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string DienThoaiNguoiDaiDien { get; set; }

        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Tinh { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Huyen { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string Xa { get; set; }

        public bool? IsDaXuLy { get; set; }
        public string LyDoKhongDuyet { get; set; }

        //Có chũ ký số
        public bool? HasCA { get; set; }

        //Sinh số CA
        public int? NamCA { get; set; }
        public int? SoThuTuc0CA { get; set; }

        #region Danh sách thủ tục
        public int? SoThuTuc1CA { get; set; }
        public int? SoThuTuc2CA { get; set; }
        public int? SoThuTuc3CA { get; set; }
        public int? SoThuTuc4CA { get; set; }
        public int? SoThuTuc5CA { get; set; }
        public int? SoThuTuc6CA { get; set; }
        public int? SoThuTuc7CA { get; set; }
        public int? SoThuTuc8CA { get; set; }
        public int? SoThuTuc9CA { get; set; }
        public int? SoThuTuc10CA { get; set; }
        #endregion
        
    }
}
