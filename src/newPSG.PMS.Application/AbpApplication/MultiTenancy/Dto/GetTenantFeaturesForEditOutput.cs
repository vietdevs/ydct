using System.Collections.Generic;
using Abp.Application.Services.Dto;
using newPSG.PMS.Editions.Dto;

namespace newPSG.PMS.MultiTenancy.Dto
{
    public class GetTenantFeaturesForEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}