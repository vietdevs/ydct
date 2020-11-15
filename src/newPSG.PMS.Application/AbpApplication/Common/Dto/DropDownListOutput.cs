using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Common.Dto
{
    public class DropDownListOutput
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long? parentId { get; set; }
        public bool? IsActive { get; set; }
        public DropDownListOutput()
        {
        }

        public DropDownListOutput(long _Id, string _Name, long? _parentId = null)
        {
            Id = _Id; Name = _Name; parentId = _parentId;
        }
    }
}
