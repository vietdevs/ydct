using Newtonsoft.Json;
using System;

namespace newPSG.PMS.Dto
{
    public class DoanhNghiepModel
    {
        //public long? Id { get; set; }
        [JsonProperty(PropertyName = "ma_so_thue")]
        public string MaSoThue { get; set; }
        [JsonProperty(PropertyName = "ten_doanh_nghiep")]
        public string TenDoanhNghiep { get; set; }
        [JsonProperty(PropertyName = "dia_chi")]
        public string DiaChi { get; set; }
        [JsonProperty(PropertyName = "so_dien_thoai")]
        public string SoDienThoai { get; set; }
        [JsonProperty(PropertyName = "fax")]
        public string Fax { get; set; }
        [JsonProperty(PropertyName = "website")]
        public string Website { get; set; }
        [JsonProperty(PropertyName = "tinh_id")]
        public int? TinhId { get; set; }
        [JsonProperty(PropertyName = "huyen_id")]
        public int? HuyenId { get; set; }
        [JsonProperty(PropertyName = "xa_id")]
        public int? XaId { get; set; }
    }
}
