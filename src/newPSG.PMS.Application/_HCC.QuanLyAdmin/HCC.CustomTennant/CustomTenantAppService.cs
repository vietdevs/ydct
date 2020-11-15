using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.EntityDB;
using newPSG.PMS.MultiTenancy;
using System.Configuration;
using Abp.Runtime.Session;
using static newPSG.PMS.CommonENum;
using Abp.Domain.Uow;
using newPSG.PMS.MultiTenancy.Dto;

namespace newPSG.PMS.Services
{
    public interface ICustomTennantAppService : IApplicationService
    {
        int GetTenantIdCucHCC();
        int GetTenantIdDoanhNghiep();
        TENANT_TYPE GetTenantType();
        TENANT_TYPE GetTenantTypeFromId(int tenantId);
        Task<List<TenantListDto>> GetAllTenantChiCuc();
        Task<List<TenantListDto>> GetAllTenant();
    }

    public class CustomTennantAppService : PMSAppServiceBase, ICustomTennantAppService
    {
        private readonly IRepository<Tenant> _tenantRepos;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<Tinh> _tinhRepos;
        private readonly IAbpSession _session;

        public CustomTennantAppService(
            IRepository<Tenant> tenantRepos,
            TenantManager tenantManager,
            IRepository<Tinh> tinhRepos,
            IAbpSession session
        )
        {
            _tenantRepos = tenantRepos;
            _tenantManager = tenantManager;
            _tinhRepos = tinhRepos;
            _session = session;
        }

        public int GetTenantIdCucHCC()
        {
            try
            {
                var tenantCucId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_CUC_HCC"]);
                return tenantCucId;
            }
            catch { };
            return 0;
        }

        public int GetTenantIdDoanhNghiep()
        {
            try
            {
                var tenantDoanhNghiepId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"]);
                return tenantDoanhNghiepId;
            }
            catch { };
            return 0;
        }

        public TENANT_TYPE GetTenantType()
        {
            try
            {
                var tenantDoanhNghiepId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"]);
                var tenantCucId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_BAN_ATTP"]);
                if (_session.TenantId == tenantDoanhNghiepId)
                {
                    return TENANT_TYPE.DOANH_NGHIEP;
                }
                if (_session.TenantId == tenantCucId)
                {
                    return TENANT_TYPE.HCC_CUC;
                }
            }
            catch { };

            return TENANT_TYPE.HCC_CHI_CUC;
        }

        public TENANT_TYPE GetTenantTypeFromId(int tenantId)
        {
            try
            {
                var tenantDoanhNghiepId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"]);
                var tenantCucId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_BAN_ATTP"]);
                if (tenantId == tenantDoanhNghiepId)
                {
                    return TENANT_TYPE.DOANH_NGHIEP;
                }
                if (tenantId == tenantCucId)
                {
                    return TENANT_TYPE.HCC_CUC;
                }

            }
            catch { };
            return TENANT_TYPE.HCC_CHI_CUC;
        }

        public async Task<List<TenantListDto>> GetAllTenantChiCuc()
        {
            var tenantDoanhNghiepId = Convert.ToInt32(ConfigurationManager.AppSettings["TENANTCY_ID_DOANH_NGHIEP"]);
            var query = from tn in _tenantManager.Tenants.Where(x => x.Id != tenantDoanhNghiepId)
                        join r_tinh in _tinhRepos.GetAll() on tn.TinhId equals r_tinh.Id into tb_tinh //Left Join
                        from tinh in tb_tinh.DefaultIfEmpty()
                        orderby tn.Id
                        select new TenantListDto
                        {
                            TenancyName = tn.TenancyName,
                            Name = tn.Name,
                            Id = tn.Id,
                            TinhName=tinh.Ten,
                            TinhId = tn.TinhId,
                            IsActive = tn.IsActive
                        };

            return await query.ToListAsync();
        }

        public async Task<List<TenantListDto>> GetAllTenant()
        {
            var query = from tn in _tenantManager.Tenants
                        where (tn.IsActive == true)
                        orderby tn.Id
                        select new TenantListDto
                        {
                            TenancyName = tn.TenancyName,
                            Name = tn.Name,
                            Id = tn.Id,
                            TinhId = tn.TinhId,
                            IsActive = tn.IsActive
                        };

            return await query.ToListAsync();
        }
    }
}
