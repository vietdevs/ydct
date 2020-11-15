using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(TieuChiThamDinh))]
    public class TieuChiThamDinhDto : TieuChiThamDinh
    {
        public string StrTieuBan
        {
            get
            {
                if (this.TieuBanEnum != null)
                {
                    return CommonENum.GetEnumDescription((CommonENum.TIEU_BAN_CHUYEN_GIA)this.TieuBanEnum);
                }
                return null;
            }
        }
    }

    public class TieuChiThamDinhInputDto : PagedAndSortedInputDto
    {
        public int? ThuTucId { get; set; }
        public int? RoleLevel { get; set; }
        public int? TieuBanEnum { get; set; }
        public int? LoaiBienBanThamDinhId { get; set; }
    }

    public class GetTieuChiThamDinhInputDto
    {
        public int? ThuTucId { get; set; }
        public int? RoleLevel { get; set; }
        public int? TieuBanEnum { get; set; }
        public int? LoaiBienBanThamDinhId { get; set; }
    }
    public class GetTieuChiThamDinhFilterInputDto
    {
        public int? ThuTucId { get; set; }
        public int? RoleLevel { get; set; }
        public int? TieuBanEnum { get; set; }
        public Filter_PhanLoaiHoSoDto Filter { get; set; }
    }
}