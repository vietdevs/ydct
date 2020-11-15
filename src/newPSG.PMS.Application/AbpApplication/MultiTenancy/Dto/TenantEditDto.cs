using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace newPSG.PMS.MultiTenancy.Dto
{
    [AutoMap(typeof (Tenant))]
    public class TenantEditDto : EntityDto
    {
        [Required]
        [StringLength(Tenant.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(Tenant.MaxNameLength)]
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public int? EditionId { get; set; }

        public long? TinhId { get; set; }

        public bool IsActive { get; set; }
    }
}