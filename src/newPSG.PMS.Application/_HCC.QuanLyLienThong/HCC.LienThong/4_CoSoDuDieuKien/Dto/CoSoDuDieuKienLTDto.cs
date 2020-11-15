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
    public class CoSoDuDieuKienLTDto : VFA_CoSoDuDieuKien
    {
        public int? HoSoTinhId { get; set; }
        public long? HoSoHuyenId { get; set; }
        public long? HoSoXaId { get; set; }

        public string StrTinh { get; set; }
    }

    public class PagingCoSoDuDieuKienLTInput : PagedAndSortedInputDto
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

    [AutoMap(typeof(CoSoDuDieuKienLTDto))]
    public class CallApiCoSoDuDieuKienInput : CoSoDuDieuKienLTDto
    {

    }


    public class CoSoDuDieuKienRequest
    {
        [JsonProperty(PropertyName = "thong_tin_doanh_nghiep")]
        public DoanhNghiepModel DoanhNghiep { get; set; }
        [JsonProperty(PropertyName = "co_so_du_dieu_kien")]
        public CoSoDuDieuKienModel CoSoDuDieuKien { get; set; }

        public CoSoDuDieuKienRequest()
        {
            DoanhNghiep = new DoanhNghiepModel();
            CoSoDuDieuKien = new CoSoDuDieuKienModel();
        }
    }

    [AutoMap(typeof(CallApiCoSoDuDieuKienInput))]
    public class CoSoDuDieuKienModel
    {
        [JsonProperty(PropertyName = "guid_id")]
        public string Guid { get; set; }
        [JsonProperty(PropertyName = "ten_co_so")]
        public string TenCoSo { get; set; }
        [JsonProperty(PropertyName = "dia_chi_co_so")]
        public string DiaChiCoSo { get; set; }
        [JsonProperty(PropertyName = "ho_ten_chu_co_so")]
        public string HoTenChuCoSo { get; set; }
        [JsonProperty(PropertyName = "san_pham_duoc_cap_phep")]
        public string SanPhamDuocCapPhep { get; set; } //Chua biet
        [JsonProperty(PropertyName = "loai_hinh_co_so_id")]
        public int? LoaiHinhCoSoId { get; set; }

        [JsonProperty(PropertyName = "tinh_id")]
        public int? TinhId { get; set; }
        [JsonProperty(PropertyName = "huyen_id")]
        public int? HuyenId { get; set; }
        [JsonProperty(PropertyName = "xa_id")]
        public int? XaId { get; set; }

        [JsonProperty(PropertyName = "giay_xac_nhan_to_base64_string")]
        public string GiayXacNhanToBase64String { get; set; }

        [JsonProperty(PropertyName = "ngay_cap_chung_nhan")]
        public string NgayCapChungNhan { get; set; }
    }
}
