using System;
using System.Collections.Generic;

#region Class Riêng Cho Từng Thủ tục
using XHoSoDto = newPSG.PMS.Dto.HoSo98Dto;
using XHoSoTepDinhKemDto = newPSG.PMS.Dto.HoSoTepDinhKem98Dto;
#endregion

namespace newPSG.PMS.Dto
{
    public class CreateOrUpdateHoSo98Input
    {
        public XHoSoDto HoSo { get; set; }
        public List<XHoSoTepDinhKemDto> Teps { get; set; }
    }

    public class HoSoInput98Dto : PagedAndSortedInputDto
    {
        public int? FormId { get; set; }
        public int? FormCase { get; set; } //Điều kiện lọc 1
        public int? FormCase2 { get; set; } //Điều kiện lọc 2

        public string Keyword { get; set; }
        public DateTime? NgayThanhToanTu { get; set; }
        public DateTime? NgayThanhToanToi { get; set; }
        public DateTime? NgayNopTu { get; set; }
        public DateTime? NgayNopToi { get; set; }

        public long? LoaiHoSoId { get; set; }
        public int? TinhId { get; set; }
        public int? DoanhNghiepId { get; set; }

        //Host filter
        public int? OnIsCA { get; set; }
        // flag only total
        public bool? IsOnlyToTal { get; set; }
    }    
}