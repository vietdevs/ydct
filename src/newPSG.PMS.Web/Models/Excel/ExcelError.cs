using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace newPSG.PMS.Web.Models.Excel
{
    public class ExcelError
    {
        public ExcelError(string sheet, int row, string info, string  error)
        {
            SheetName = sheet;
            RowNumber = row;
            RowInfo = info;
            Error = error;
        }

        public string SheetName { get; set; }
        public int RowNumber { get; set; }
        public string RowInfo { get; set; }
        public string Error { get; set; }
    }
}