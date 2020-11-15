using Abp.AutoMapper;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(LienHe))]
    public class LienHeDto : LienHe
    {
    }

    public class LienHeInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}