using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Common.Dto
{
    public class ItemDto<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
        public int? ThuTucId { get; set; }
        public long? ParentId { get; set; }
    }
}
