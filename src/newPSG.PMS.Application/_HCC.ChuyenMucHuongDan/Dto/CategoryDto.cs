using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(Category))]
    public class CategoryDto : Category
    {
        public List<CategoryDto> Items { get; set; }
        public List<ArticleDto> ArrArticles { get; set; }

        public string StrGroupEnum
        {
            get
            {
                return GroupEnum == null ? null : CommonENum.GetEnumDescription((CommonENum.NHOM_BAI_VIET)(int)GroupEnum);
            }
        }

        public string StrRoleLevel
        {
            get
            {
                return RoleLevel == null ? null : CommonENum.GetEnumDescription((CommonENum.ROLE_LEVEL)(int)RoleLevel);
            }
        }
    }

    public class CategoryInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? RoleLevel { get; set; }
        public int? GroupEnum { get; set; }
        public int? CurrentId { get; set; }
    }
}