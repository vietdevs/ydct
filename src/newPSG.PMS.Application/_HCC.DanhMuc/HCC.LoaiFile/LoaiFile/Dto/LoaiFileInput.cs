using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;


namespace newPSG
{
    [AutoMap(typeof(LoaiFile))]
    public  class LoaiFileInput
    {
        public int? Id { get; set; }
        public string Ten { get; set; }

        public string MoTa { get; set; }

        public bool IsActive { get; set; }
        public bool? IsKhac { get; set; }
        public int? NiisId { get; set; }
    }
    public class CreateOrUpdateLoaiFileInput
    {
        public LoaiFileInput LoaiFile;
    }
    public class GetLoaiFileInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }


}
