using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public static string GetEnumDescription(Enum en)
        {
            Type type = en.GetType();

            try
            {
                MemberInfo[] memInfo = type.GetMember(en.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(EnumDisplayString), false);

                    if (attrs != null && attrs.Length > 0)
                        return ((EnumDisplayString)attrs[0]).DisplayString;
                }
            }
            catch { }

            return en.ToString();
        }

        public static ThucTucEnumDisplayString GetThucEnumAttriblue(Enum en)
        {
            Type type = en.GetType();

            try
            {
                MemberInfo[] memInfo = type.GetMember(en.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(ThucTucEnumDisplayString), false);

                    if (attrs != null && attrs.Length > 0)
                        return ((ThucTucEnumDisplayString)attrs[0]);
                }
            }
            catch { }

            return null;
        }



        //Loại tài khoản (Chỉ dùng để truy vấn/Không lưu trữ) 
        public enum LOAI_TAI_KHOAN
        {
            [EnumDisplayString("Tài khoản trong cục")]
            TAI_KHOAN_CUC = 1,
            [EnumDisplayString("Tài khoản chuyên gia")]
            TAI_KHOAN_CHUYEN_GIA = 2,
            [EnumDisplayString("Tài khoản sở y tế",true)]
            TAI_KHOAN_SO_Y_TE = 3
        }
    }

    public class ThucTucEnumDisplayString : Attribute
    {
        public string DisplayName { get; set; }
        public bool IsForXuLy { get; set; }
        public ThucTucEnumDisplayString(string text,bool isForXuLy = false)
        {
            this.DisplayName = text;
            this.IsForXuLy = isForXuLy;
        }
    }

    public class EnumDisplayString : Attribute
    {
        public string DisplayString;
        public bool IsXuLy { get; set; }

        public EnumDisplayString(string text,bool IsXuLy = false)
        {
            this.DisplayString = text;
            this.IsXuLy = IsXuLy;
        }
    }
    public class ItemObj<T>
    {
        public T Id { get; set; }

        public string Name { get; set; }

        public string Ids
        {
            get
            {
                if (IdTTs != null)
                    return String.Join(",", IdTTs);
                else
                    return string.Empty;
            }
        }
        public List<string> IdTTs { get; set; }
        public bool Checked { get; set; }
        public int? ThuTu { get; set; }
        public long? TotalCount { get; set; }
        public bool IsForXuLy { get; set; }
    }
}
