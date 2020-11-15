using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(Tinh))]
    public class TinhDto: Tinh
    {
        public string TenKhongDau { get; set; }
    }
    public class TinhInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}