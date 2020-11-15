using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(LichLamViec))]
    public class LichLamViecDto: LichLamViec
    {
        public List<NgayNghi> DanhSachNgayNghi { get; set; }
    }
    public class LichLamViecInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}
