using Abp.AutoMapper;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(ThongBao))]
    public class ThongBaoDto : ThongBao
    {
    }

    public class ThongBaoInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}