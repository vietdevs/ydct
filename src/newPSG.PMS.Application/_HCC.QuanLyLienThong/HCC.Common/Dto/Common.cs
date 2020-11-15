using newPSG.PMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.API
{
    public static class MessageErorr
    {
        /// <summary>
        /// Erorr validation
        /// </summary>
        public static ErrorApi Er01 = new ErrorApi("ERR01_CODE", "Thông báo lỗi");

        /// <summary>
        /// 
        /// </summary>
        public static ErrorApi Er02 = new ErrorApi("ERR02_CODE", "Thông báo validation");

        /// <summary>
        /// Bản ghi đã tồn tại
        /// </summary>
        public static ErrorApi Er03 = new ErrorApi("ERR03_CODE", "Bản ghi đã tồn tại trên hệ thống");

        /// <summary>
        /// Thực hiện thành công
        /// </summary>
        public static ErrorApi Er100 = new ErrorApi("ERR100_CODE", "Thực hiện thành công");

    }
}
