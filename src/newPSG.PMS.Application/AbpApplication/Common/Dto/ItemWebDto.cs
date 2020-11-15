using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Common.Dto
{
    public class ItemWebDto<T>
    {
        public T Id { get; set; }
        public int? GroupEnum { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public int? Level { get; set; }
        public int? RoleLevel { get; set; }
        public int? SortOrder { get; set; }
    }
}