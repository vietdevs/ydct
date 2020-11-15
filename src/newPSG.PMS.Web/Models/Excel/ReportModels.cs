using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using FlexCel.Report;
namespace newPSG.PMS.Web.Models.Excel
{
    /// <summary>
    /// Hàm xử lý dữ liệu trên Report
    /// Author: PhuongLT15 (09/09/2014)
    /// </summary>
    public class ReportModels
    {
        #region "#"

        public static String FormatDateTimeToVietNames(DateTime dateTime)
        {

            String Ngay = "";
            int day = dateTime.Day;
            int month = dateTime.Month;
            int year = dateTime.Year;
            Ngay = " Ngày  " + day + " tháng  " + month + "  năm  " + year;
            return Ngay;
        }
        /// <summary>
        /// Format date time theo kiểu ngày tháng năm
        /// </summary>
        /// <returns></returns>
        public static String FormatDateTimeToVietNames(string strDateTime)
        {
            DateTime dateValue;
            String Ngay = "";
            int day, month, year;
            if (DateTime.TryParse(strDateTime, out dateValue))
            {
                day = dateValue.Day;
                month = dateValue.Month;
                year = dateValue.Year;
            }
            else
            {
                day = DateTime.Now.Day;
                month = DateTime.Now.Month;
                year = DateTime.Now.Year;
            }
            Ngay = " Ngày  " + day + " tháng  " + month + "  năm  " + year;
            return Ngay;
        }
        /// <summary>
        /// Lấy giá trị ngày tháng năm hiên tại
        /// </summary>
        /// <returns></returns>
        public static String Ngay_Thang_Nam_HienTai()
        {
            String Ngay = "";
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            Ngay = " Ngày  " + day + " tháng  " + month + "  năm  " + year;
            return Ngay;
        }
        /// <summary>
        /// Chọn khổ giấy in
        /// </summary>
        /// <returns></returns>
        public static DataTable LoaiKhoGiay()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaKhoGiay", typeof(String));
            dt.Columns.Add("TenKhoGiay", typeof(String));
            DataRow R1 = dt.NewRow();
            dt.Rows.Add(R1);
            R1[0] = "1";
            R1[1] = "In khổ giấy A3";
            DataRow R2 = dt.NewRow();
            dt.Rows.Add(R2);
            R2[0] = "2";
            R2[1] = "In khổ giấy A4";
            dt.Dispose();
            return dt;
        }

        /// <summary>
        /// Thay đổi ngon ngữ để hiện thị dấu "," thành "."
        /// </summary>
        public static void Language()
        {
            string lang = "vi-VN";
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
        }
        /// <summary>
        /// Lọc từ dữ liệu Datatable ra các trường hợp trùng nhau
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="SourceTable"></param>
        /// <param name="FieldName"></param>
        /// <param name="sFieldAdd"></param>
        /// <param name="sField_DK"></param>
        /// <param name="sField_DK_Value"></param>
        /// <param name="strSort"></param>
        /// <returns></returns>
        public static DataTable SelectDistinct(string TableName, DataTable SourceTable, string FieldName, String sFieldAdd, String sField_DK = "", String sField_DK_Value = "", String strSort = "")
        {
            DataTable dt = new DataTable(TableName);
            String[] arrFieldAdd = sFieldAdd.Split(',');
            String[] arrFieldName = FieldName.Split(',');
            for (int i = 0; i < arrFieldAdd.Length; i++)
            {
                dt.Columns.Add(arrFieldAdd[i], SourceTable.Columns[arrFieldAdd[i]].DataType);
            }
            if (SourceTable.Rows.Count > 0)
            {
                object[] LastValue = new object[arrFieldName.Length];
                for (int i = 0; i < LastValue.Length; i++)
                {
                    LastValue[i] = null;
                }


                foreach (DataRow dr in SourceTable.Select("", FieldName + " " + strSort))
                {
                    Boolean ok = true;
                    for (int i = 0; i < arrFieldName.Length; i++)
                    {
                        if (LastValue[i] != null && (ColumnEqual(LastValue[i], dr[arrFieldName[i]])))
                        {
                            ok = false;
                        }
                        else
                        {
                            ok = true;
                            break;
                        }
                    }
                    for (int i = 0; i < arrFieldName.Length; i++)
                    {
                        if (ok)
                        {
                            LastValue[i] = dr[arrFieldName[i]];
                        }
                    }
                    if (ok)
                    {
                        DataRow R = dt.NewRow();
                        for (int j = 0; j < arrFieldAdd.Length; j++)
                        {
                            R[arrFieldAdd[j]] = dr[arrFieldAdd[j]];
                        }
                        if ((String.IsNullOrEmpty(sField_DK) == false && arrFieldAdd[arrFieldAdd.Length - 1] == "sMoTa") || String.IsNullOrEmpty(sField_DK_Value) == false)
                            R[arrFieldAdd[arrFieldAdd.Length - 1]] = "";
                        dt.Rows.Add(R);
                    }
                }
            }
            return dt;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static bool ColumnEqual(object A, object B)
        {

            // Compares two values to see if they are equal. Also compares DBNULL.Value.
            // Note: If your DataTable contains object fields, then you must extend this
            // function to handle them in a meaningful way if you intend to group on them.

            if ((A == DBNull.Value || A == null) && (B == DBNull.Value || B == null)) //  both are DBNull.Value
                return true;
            if ((A == DBNull.Value || A == null) || (B == DBNull.Value || B == null)) //  only one is DBNull.Value
                return false;
            return (A.Equals(B));  // value type standard comparison
        }
        #endregion
        #region "Chuyển tiền kiểu số thành chữ - phục vụ báo cáo"
        
        public static String TienRaChu(long Tien)
        {
            String vR = "";
            if (Tien < 0)
                return vR;
            String vR1 = "";
            long d = 0, So1, So2, So3;
            long lRound = (long)Math.Round((decimal)Tien);
            String ChuSo = "không,một,hai,ba,bốn,năm,sáu,bảy,tám,chín";
            String DonViTien = ",nghìn,triệu,tỉ,nghìn tỉ, triệu tỉ, tỉ tỉ";
            String[] arr1 = ChuSo.Split(',');
            String[] arr2 = DonViTien.Split(',');
            do
            {
                So1 = lRound % 10;
                lRound = (lRound - So1) / 10;
                So2 = lRound % 10;
                lRound = (lRound - So2) / 10;
                So3 = lRound % 10;
                lRound = (lRound - So3) / 10;
                if (!(So1 == 0 && So2 == 0 && So3 == 0))
                {
                    vR1 = "";
                    if (So3 != 0 || lRound != 0)
                    {
                        vR1 = arr1[So3] + " trăm";
                    }
                    if (So2 == 0)
                    {
                        if (vR1 != "" && So1 != 0)
                        {
                            vR1 += " linh";
                        }
                    }
                    else if (So2 == 1)
                    {
                        vR1 += " mười";
                    }
                    else
                    {
                        vR1 += " " + arr1[So2] + " mươi";
                    }
                    if (So1 != 0)
                    {
                        if (So1 == 1 && So2 >= 2)
                        {
                            vR1 += " mốt";
                        }
                        else if (So1 == 5 && So2 >= 1)
                        {
                            vR1 += " lăm";
                        }
                        else
                        {
                            vR1 += " " + arr1[So1];
                        }
                    }
                    vR1 = vR1.Trim();
                    if (vR1 != "")
                    {
                        vR = vR1 + " " + arr2[d] + " " + vR.Trim();
                    }
                }
                d = d + 1;
            } while (lRound != 0);
            vR = vR.Trim();
            if (vR == "")
            {
                vR = "không";
            }
            vR = vR.Substring(0, 1).ToUpper() + vR.Substring(1);
            return vR + " đồng";
        }
        public static string TienRaChu(string TienString)
        {
            long tienLong = long.Parse(TienString.Split('.', ',')[0]);
            return TienRaChu(tienLong);
        }
        public static String TienRaChu(long Tien, bool isChan = true, string themSoTienChanLe = "")
        {
            String vR = "";
            var checkSoAm = false;
            if (Tien < 0)
            {
                checkSoAm = true;
                Tien *= -1;
            }
            String vR1 = "";
            long d = 0, So1, So2, So3;
            long lRound = (long)Math.Round((decimal)Tien);
            String ChuSo = "không,một,hai,ba,bốn,năm,sáu,bảy,tám,chín";
            String DonViTien = ",nghìn,triệu,tỉ,nghìn tỉ, triệu tỉ, tỉ tỉ";
            String[] arr1 = ChuSo.Split(',');
            String[] arr2 = DonViTien.Split(',');
            do
            {
                So1 = lRound % 10;
                lRound = (lRound - So1) / 10;
                So2 = lRound % 10;
                lRound = (lRound - So2) / 10;
                So3 = lRound % 10;
                lRound = (lRound - So3) / 10;
                if (!(So1 == 0 && So2 == 0 && So3 == 0))
                {
                    vR1 = "";
                    if (So3 != 0 || lRound != 0)
                    {
                        vR1 = arr1[So3] + " trăm";
                    }
                    if (So2 == 0)
                    {
                        if (vR1 != "" && So1 != 0)
                        {
                            vR1 += " linh";
                        }
                    }
                    else if (So2 == 1)
                    {
                        vR1 += " mười";
                    }
                    else
                    {
                        vR1 += " " + arr1[So2] + " mươi";
                    }
                    if (So1 != 0)
                    {
                        if (So1 == 1 && So2 >= 2)
                        {
                            vR1 += " mốt";
                        }
                        else if (So1 == 5 && So2 >= 1)
                        {
                            vR1 += " lăm";
                        }
                        else
                        {
                            vR1 += " " + arr1[So1];
                        }
                    }
                    vR1 = vR1.Trim();
                    if (vR1 != "")
                    {
                        vR = vR1 + " " + arr2[d] + " " + vR.Trim();
                    }
                }
                d = d + 1;
            } while (lRound != 0);
            vR = vR.Trim();
            if (vR == "")
            {
                vR = "không";
            }
            if (isChan && !checkSoAm)
            {
                vR = vR.Substring(0, 1).ToUpper() + vR.Substring(1);
            }
            return checkSoAm ? ("Âm " + vR + " đồng" + themSoTienChanLe) : (vR + " đồng" + themSoTienChanLe);
        }
        public static string TienRaChuCoLe(string TienString)
        {
            long tienChan = long.Parse(TienString.Split('.', ',')[0]);
            long tienLe = 0;
            string docThanhTien = TienRaChuKhongCoDong(tienChan, true);
            if (TienString.Split('.', ',').Length > 1)
            {
                tienLe = long.Parse(TienString.Split('.', ',')[1]);
                if (tienLe > 0)
                {
                    docThanhTien += " phẩy " + TienRaChuKhongCoDong(tienLe, false);
                }
            }
            return docThanhTien + " đồng";
        }
        public static string TienRaChuKhongCoDongCoLe(string TienString)
        {
            long tienChan = long.Parse(TienString.Split('.', ',')[0]);
            long tienLe = 0;
            string docThanhTien = TienRaChuKhongCoDong(tienChan, true);
            if (TienString.Split('.', ',').Length > 1)
            {
                tienLe = long.Parse(TienString.Split('.', ',')[1]);
                if (tienLe > 0)
                {
                    docThanhTien += " phẩy " + TienRaChuKhongCoDong(tienLe, false);
                }
            }
            return docThanhTien;
        }
        public static string TienRaChu(string TienString, bool isChan = true, string themSoTienChanLe = "")
        {
            long tienLong = long.Parse(TienString.Replace(",", ".").Split('.')[0]);
            return TienRaChu(tienLong, isChan, themSoTienChanLe);
        }
        private static string TienRaChuKhongCoDong(long tienChan, bool isChan)
        {
            String vR = "";
            if (tienChan < 0)
                return vR;
            String vR1 = "";
            long d = 0, So1, So2, So3;
            long lRound = (long)Math.Round((decimal)tienChan);
            String ChuSo = "không,một,hai,ba,bốn,năm,sáu,bảy,tám,chín";
            String DonViTien = ",nghìn,triệu,tỉ,nghìn tỉ, triệu tỉ, tỉ tỉ";
            String[] arr1 = ChuSo.Split(',');
            String[] arr2 = DonViTien.Split(',');
            do
            {
                So1 = lRound % 10;
                lRound = (lRound - So1) / 10;
                So2 = lRound % 10;
                lRound = (lRound - So2) / 10;
                So3 = lRound % 10;
                lRound = (lRound - So3) / 10;
                if (!(So1 == 0 && So2 == 0 && So3 == 0))
                {
                    vR1 = "";
                    if (So3 != 0 || lRound != 0)
                    {
                        vR1 = arr1[So3] + " trăm";
                    }
                    if (So2 == 0)
                    {
                        if (vR1 != "" && So1 != 0)
                        {
                            vR1 += " linh";
                        }
                    }
                    else if (So2 == 1)
                    {
                        vR1 += " mười";
                    }
                    else
                    {
                        vR1 += " " + arr1[So2] + " mươi";
                    }
                    if (So1 != 0)
                    {
                        if (So1 == 1 && So2 >= 2)
                        {
                            vR1 += " mốt";
                        }
                        else if (So1 == 5 && So2 >= 1)
                        {
                            vR1 += " lăm";
                        }
                        else
                        {
                            vR1 += " " + arr1[So1];
                        }
                    }
                    vR1 = vR1.Trim();
                    if (vR1 != "")
                    {
                        vR = vR1 + " " + arr2[d] + " " + vR.Trim();
                    }
                }
                d = d + 1;
            } while (lRound != 0);
            vR = vR.Trim();
            if (vR == "")
            {
                vR = "không";
            }
            if (isChan)
            {
                vR = vR.Substring(0, 1).ToUpper() + vR.Substring(1);
            }
            return vR;
        }
        #endregion
    }
    /// <summary>
    /// Lớp đối tượng để xử lý xuất dữ liệu ra file Excel
    /// Author: PhuongLT15 (09/09/2014)
    /// </summary>
    public class clsExcelResult : ActionResult
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public MemoryStream ms { get; set; }
        public String type { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            try
            {
                context.HttpContext.Response.Buffer = true;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + FileName);
                context.HttpContext.Response.ContentType = "application/vnd." + type;
                if (string.IsNullOrEmpty(Path) == false)
                {
                    context.HttpContext.Response.WriteFile(Path);
                }
                else
                {
                    context.HttpContext.Response.BinaryWrite(ms.ToArray());
                }
                context.HttpContext.Response.End();
            }
            catch { }
        }
    }
}