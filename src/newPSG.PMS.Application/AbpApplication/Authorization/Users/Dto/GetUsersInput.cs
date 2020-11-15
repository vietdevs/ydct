using Abp.Runtime.Validation;
using newPSG.PMS.Dto;

namespace newPSG.PMS.Authorization.Users.Dto
{
    public class GetUsersInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public string Permission { get; set; }

        public int? Role { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name,Surname";
            }
        }
    }

    public class GetAdminsInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }

        public int? TenantId { get; set; }
    }


}