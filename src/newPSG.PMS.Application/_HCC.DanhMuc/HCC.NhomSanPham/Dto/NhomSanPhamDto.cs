using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(NhomSanPham))]
    public class NhomSanPhamDto: NhomSanPham
    {     
    }
    public class NhomSanPhamInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}
