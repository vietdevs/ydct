using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.ThuTucCommon.Dto
{
    public class GetAllCountThuTucDashBoardDto
    {
        public string ThuTucEnum { get; set; }
        public List<FormThuTucDashBoardDto> ListForm { get; set; }
        public int? Total { get; set; }
    }
    public class FormThuTucDashBoardDto
    {
        public int? FormId { get; set; }
        public List<int?> ListFormCase { get; set; }
        public int? Total { get; set; }
    }
    public class GetCountFormCaseThuTucSqlInput
    {
        public int ThuTuc { get; set; }
        public int FormId { get; set; }
    }
    public class GetCountThuTucDashBoardDto
    {
        public int ThuTuc { get; set; }
        public int FormId { get; set; }
        public List<int?> ListFormCase { get; set; }
        public int? Total { get; set; }
    }

}
