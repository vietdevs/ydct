using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace newPSG.PMS
{
    public static class Utility
    {
        public static class KeyPaySetting
        {
            public static string KPAY_URL()
            {
                return ConfigurationManager.AppSettings["KPAY_URL"].ToString();
            }
            public static string RETURN_URL()
            {
                return ConfigurationManager.AppSettings["KPAY_RETURN_URL"].ToString();
            }
            public static string COMMAND_TYPE_PAY=  "pay";
            public static string COUNTRY_CODE = "+84";
            public static string CURRENT_CODE = "704";
            public static string CURRENT_LOCAL = "vn";
            public static string INTERNAL_BANK = "all_card";
            public static string SERVICE_CODE_MUA_HANG = "720";
            public static string SERVICE_CODE_THANH_TOAN_HOA_DON = "490";
            public static string VERSION = "1.0";
        }
        public static class StringExtensions
        {
            public static string FomatFilterText(string input)
            {
                if (string.IsNullOrEmpty(input)) return input;
                return input.Trim().Replace("  ", " ").ToLower();
            }
            public static string ConvertKhongDau(string s)
            {
                if (string.IsNullOrEmpty(s)) return s;
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = s.Normalize(NormalizationForm.FormD);
                return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            public static string FomatAndKhongDau(string s)
            {
                s = FomatFilterText(s);
                s = ConvertKhongDau(s);
                return s;
            }
        }
        public static class SqlExtensions
        {
            public static List<T> DataReaderToList<T>(SqlDataReader r, bool IsDipose = true) where T : new()
            {
                List<T> res = new List<T>();
                while (r.Read())
                {
                    T t = new T();

                    for (int inc = 0; inc < r.FieldCount; inc++)
                    {
                        try
                        {
                            Type type = t.GetType();
                            PropertyInfo prop = type.GetProperty(r.GetName(inc));
                            prop.SetValue(t, r.GetValue(inc), null);
                        }
                        catch { }
                    }

                    res.Add(t);
                }

                if (IsDipose)
                {
                    r.Close();
                    r.Dispose();
                }
                return res;

            }
            public static IEnumerable<Dictionary<string, object>> SerializeDataReader(SqlDataReader reader)
            {
                var results = new List<Dictionary<string, object>>();
                var cols = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                    cols.Add(reader.GetName(i));

                while (reader.Read())
                    results.Add(SerializeRow(cols, reader));

                reader.Close();
                reader.Dispose();

                return results;
            }
            private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                SqlDataReader reader)
            {
                var result = new Dictionary<string, object>();
                foreach (var col in cols)
                    result.Add(col, reader[col]);
                return result;
            }
        }
        private static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        /// <summary>
        /// Tạo ra một chuỗi ngẫu nhiên với độ dài cho trước
        /// </summary>
        /// <param name="size">Kích thước của chuỗi </param>
        /// <param name="lowerCase">Nếu đúng, tạo ra chuỗi chữ thường</param>
        /// <returns>Random string</returns>
        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        public static string GetCodeRandom()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, false));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
    }
}
