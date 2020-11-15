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
   
    public class KetQuaLienThong
    {
        public List<ErrorApi> ListResult { get; set; }
        public int? TongSoHoSo { get; set; }
        public int? HoSoThatBai { get; set; }
        public int? HoSoThanhCong { get; set; }
        public int? TrangThaiLienThongRequest { get; set; }
        public string Message { get; set; }
    }

    public class TaiKhoanLienThongInput
    {
        [JsonProperty(PropertyName = "pwd")]
        public string Password { get; set; }
        [JsonProperty(PropertyName = "usr")]
        public string UsernameOrEmailAddress { get; set; }
        [JsonProperty(PropertyName = "tenancy_name")]
        public string TenancyName { get; set; }
        public string DomainLienThong { get; set; }
    }

    public class ResultApi
    {
        public ResultDto Result { get; set; }
        public bool? Success { get; set; }
        public object Error { get; set; }
        public string Message { get; set; }
    }
    
    public class ResultDto
    {
        [JsonProperty(PropertyName = "result")]
        public List<ErrorApi> ListResult { get; set; }
        public bool? Success { get; set; }
        public object Error { get; set; }
        public string Message { get; set; }
    }

    public class ErrorApi
    {
        [JsonProperty(PropertyName = "lien_thong_id")]
        public long? LienThongId { get; set; }
        public string Code { get; set; }
        [JsonProperty(PropertyName = "guid_id")]
        public string Guid { get; set; }
        public string Message { get; set; }
        public ErrorApi(string _code, string _message)
        {
            Code = _code;
            Message = _message;
        }
    }

    public class TokenDto
    {
        public object Result { get; set; }
        public bool? Success { get; set; }
        public object Error { get; set; }
        public string Message { get; set; }
    }
}
