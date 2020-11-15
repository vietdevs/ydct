using newPSG.PMS.Dto;

namespace newPSG.PMS.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
}