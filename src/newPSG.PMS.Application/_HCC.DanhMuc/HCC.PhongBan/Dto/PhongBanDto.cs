using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(PhongBan))]
    public class PhongBanDto : PhongBan
    {
        public List<ItemObj<int>> ArrLoaiHoSo { get; set; }
    }
    public class PhongBanInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public bool? IsActive { get; set; }
        public int? LoaiHoSoId { get; set; }
    }
}
