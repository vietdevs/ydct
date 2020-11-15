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
    [AutoMap(typeof(Xa))]
    public class XaDto: Xa
    {
        public int TinhId { get; set; }
        public string StrTinh { get; set; }
        public string StrHuyen { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
    }
    public class XaInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? TinhId { get; set; }
        public long? HuyenId { get; set; }
    }
}
