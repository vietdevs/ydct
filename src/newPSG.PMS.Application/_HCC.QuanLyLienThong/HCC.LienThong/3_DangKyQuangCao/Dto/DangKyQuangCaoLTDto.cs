using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    public class DangKyQuangCaoLTDto : VFA_DangKyQuangCao
    {
        public string StrTinh { get; set; }
    }

    public class PagingDangKyQuangCaoLTInput : PagedAndSortedInputDto
    {
        public int? FormCase { get; set; }

        public string Keyword { get; set; }
        public DateTime? NgayTraKetQuaTu { get; set; }
        public DateTime? NgayTraKetQuaToi { get; set; }

        public long? LoaiHoSoId { get; set; }
        public int? TinhId { get; set; }
        public int? DoanhNghiepId { get; set; }
        public bool? IsChiCuc { get; set; }
        public int? ChiCucId { get; set; }
        public int? ThuTucId { get; set; }
    }

    [AutoMap(typeof(DangKyQuangCaoLTDto))]
    public class CallApiDangKyQuangCaoInput : DangKyQuangCaoLTDto
    {

    }

    public class DangKyQuangCaoRequest
    {
        [JsonProperty(PropertyName = "thong_tin_doanh_nghiep")]
        public DoanhNghiepModel DoanhNghiep { get; set; }
        [JsonProperty(PropertyName = "dang_ky_quang_cao")]
        public DangKyQuangCaoModel DangKyQuangCao { get; set; }
        public DangKyQuangCaoRequest()
        {
            DoanhNghiep = new DoanhNghiepModel();
            DangKyQuangCao = new DangKyQuangCaoModel();
        }
    }

    [AutoMap(typeof(CallApiDangKyQuangCaoInput))]
    public class DangKyQuangCaoModel
    {
        [JsonProperty(PropertyName = "thong_tin_san_pham")]
        public List<SanPhamQuangCao> SanPhams { get; set; }

        [JsonProperty(PropertyName = "thong_tin_loai_hinh_quang_cao")]
        public List<int> LoaiHinhQuangCaos { get; set; }

        [JsonProperty(PropertyName = "thong_tin_loai_hinh_quang_cao_khac")]
        public string LoaiHinhQuangCaoKhacs { get; set; }
        
        [JsonProperty(PropertyName = "giay_xac_nhan_to_base64_string")]
        public string GiayXacNhanToBase64String { get; set; }

        [JsonProperty(PropertyName = "ngay_cap_chung_nhan")]
        public string NgayCapChungNhan { get; set; }
        [JsonProperty(PropertyName = "guid_id")]
        public string Guid { get; set; }
        public DangKyQuangCaoModel()
        {
            SanPhams = new List<SanPhamQuangCao>();
            LoaiHinhQuangCaos = new List<int>();
        }
    }

    public class SanPhamQuangCao
    {
        [JsonProperty(PropertyName = "stt")]
        public string STT { get; set; }
        [JsonProperty(PropertyName = "ten_thuc_pham")]
        public string TenThucPham { get; set; }
        [JsonProperty(PropertyName = "so_giay_cong_bo")]
        public string SoGiayCongBo { get; set; }
    }
}
