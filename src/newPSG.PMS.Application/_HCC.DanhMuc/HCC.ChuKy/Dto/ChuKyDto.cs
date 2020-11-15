using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(ChuKy))]
    public class ChuKyDto: ChuKy
    {
    }

    public class ChuKyInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? LoaiChuKy { get; set; }
        public long UserId { get; set; }
    }
}
