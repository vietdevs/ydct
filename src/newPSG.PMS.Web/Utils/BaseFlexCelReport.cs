using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
namespace newPSG.PMS.Web.Utils
{
    public class ToaDo 
    {
        public int X { get; set; }
        public int Y{ get; set; }
    }
    public class BaseFlexCelReport : FlexCelReport
    {
        public BaseFlexCelReport()
        { 
        }
        /// <summary>
        /// ThienDL:
        /// Lấy thời gian xuất  báo cáo
        /// </summary>
        /// <param name="startDate">từ ngày</param>
        /// <param name="endDate">đến ngày</param>
        /// <param name="months">tháng</param>
        /// <param name="quaters">quý</param>
        /// <param name="years">năm</param>
        /// <returns>chuỗi thời gian báo cáo</returns>
        public string getTimeReport(string startDate, string endDate, int? months, int? quaters, int? years)
        {
            string timeReport;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                timeReport = "Từ ngày " + startDate + " Đến ngày " + endDate;
            }
            else if (!string.IsNullOrEmpty(months.ToString()) && !string.IsNullOrEmpty(years.ToString()))
            {
                timeReport = "Tháng" + months + " Năm " + years;
            }
            else if (!string.IsNullOrEmpty(quaters.ToString()) && !string.IsNullOrEmpty(years.ToString()))
            {
                timeReport = "Quý " + quaters + " Năm " + years;
            }
            else
            {
                timeReport = "Năm " + years;
            }
            return timeReport;
        }

    
        public static ToaDo getPositionTableFlexExcel(XlsFile Result, string TextFil, int Sheet = 1)
        {  
            Result.ActiveSheet = Sheet;//we'll read sheet1. We could loop over the existing sheets by using xls.SheetCount and xls.ActiveSheet 
            for (int row = 1; row <= Result.RowCount; row++)
            {
                for (int colIndex = 1; colIndex <= Result.GetColCount(Sheet, false); colIndex++) //Don't use xls.ColCount as it is slow: See Performance.Pdf
                {
                    try
                    {
                        var valueCell = Result.GetCellValue(row, colIndex);
                        if (valueCell != null)
                        {
                            var dataString = valueCell.ToString();
                            if (!string.IsNullOrEmpty(dataString))
                            {
                                if (dataString.ToUpper().IndexOf(TextFil.ToUpper()) >= 0)//bat key ton tai table vidu: <#ChiTiet.TENDICHVU>
                                {
                                    return new ToaDo() { X = row, Y = colIndex };

                                }
                            }
                        }
                    }
                    catch (Exception )
                    {

                    }
                   
                }
            }
            return new ToaDo() { X = 0, Y = 0 };
        }
        public static double GetWidth(ExcelFile xls, int col1, int col2)
        {
            double Result = 0;
            for (int c = col1; c < col2; c++)
            {
                Result += xls.GetColWidth(c, true);
            }

            return Result / ExcelMetrics.ColMult(xls);
        }
    }
}