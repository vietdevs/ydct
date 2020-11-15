using Abp.AutoMapper;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(BoThuTuc))]
    public class BoThuTucDto : BoThuTuc
    {
    }

    public class BoThuTucInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}