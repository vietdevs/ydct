using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        public enum NHOM_BAI_VIET
        {
            [EnumDisplayString("Hướng dẫn sử dụng")]
            HUONG_DAN = 1,

            [EnumDisplayString("Quản trị dự án")]
            QUAN_TRI_DU_AN = 2,
        }

        public static List<ItemObj<int>> GetListNhomBaiViet()
        {
            var _list = new List<ItemObj<int>>();
            foreach (object iEnumItem in Enum.GetValues(typeof(NHOM_BAI_VIET)))
            {
                _list.Add(new ItemObj<int>
                {
                    Id = (int)iEnumItem,
                    Name = GetEnumDescription((NHOM_BAI_VIET)(int)iEnumItem)
                });
            }
            return _list;
        }
    }
}