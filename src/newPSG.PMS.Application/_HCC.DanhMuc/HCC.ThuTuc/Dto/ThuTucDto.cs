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

    [AutoMap(typeof(ThuTuc))]
    public class ThuTucDto : ThuTuc
    {
        public string TenKhongDau { get; set; }
        public string Css { get; set; }
        public bool? IsRole { get; set; }
    }

    public class ThuTucInputDto : PagedAndSortedInputDto
    {
        public int? ID { get; set; }
        public string Filter { get; set; }
    }
}
