using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.EntityDB
{
    [Table("Web_LienHe")]
    public class LienHe : Entity<int>, IHasCreationTime
    {
        [Required]
        [StringLength(200)]
        public string HoTen { get; set; }

        [StringLength(12)]
        [DataType(DataType.PhoneNumber)]
        public string DienThoai { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}")]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(250)]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(250)]
        public string TieuDe { get; set; }

        [StringLength(3000)]
        public string LoiNhan { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
    }
}