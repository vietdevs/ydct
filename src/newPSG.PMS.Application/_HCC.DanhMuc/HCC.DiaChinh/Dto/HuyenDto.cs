using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(Huyen))]
    public class HuyenDto: Huyen
    {
        public string StrTinh { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
    }
    public class HuyenInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? TinhId { get; set; }
    }
}
