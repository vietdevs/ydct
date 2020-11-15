using Abp.AutoMapper;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System.Collections.Generic;

namespace newPSG.PMS.Dto
{
    [AutoMap(typeof(Article))]
    public class ArticleDto : Article
    {
        public string CategoryName { get; set; }
        public string sRoleLevel { get; set; }
    }

    public class ArticleInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        public int? CategoryId { get; set; }
        public int? RoleLevel { get; set; }
    }
}