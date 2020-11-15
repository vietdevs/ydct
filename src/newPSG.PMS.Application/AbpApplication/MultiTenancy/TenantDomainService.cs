using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using newPSG.PMS.MultiTenancy.Dto;
using Newtonsoft.Json;

namespace newPSG.PMS.MultiTenancy
{
    public interface ITenantDomainService : IDomainService
    {
        List<TenantListDto> GetListTenants();
    }

    public class TenantDomainService : PMSDomainServiceBase, ITenantDomainService
    {
        private readonly IRepository<Tenant> _tenantRepos;
        private ILogger _logger { get; set; }

        public TenantDomainService(IRepository<Tenant> tenantRepos)
        {
            _tenantRepos = tenantRepos;
            _logger = NullLogger.Instance;
        }

        [UnitOfWork]
        public List<TenantListDto> GetListTenants()
        {
            try
            {
                var tenants = _tenantRepos.GetAllList();
                var listTenant = tenants.MapTo<List<TenantListDto>>();

                return listTenant;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }
    }
}