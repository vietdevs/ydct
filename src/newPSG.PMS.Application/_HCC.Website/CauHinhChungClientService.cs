using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using newPSG.PMS.Authorization.Users;
using newPSG.PMS.Common.Dto;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;

namespace newPSG.PMS.Services
{
    #region INTERFACE

    public interface ICauHinhChungClientAppService : IApplicationService
    {
        CauHinhChung GetCauHinhChungByKey(string settingKey);
    }

    #endregion INTERFACE

    #region MAIN

    public class CauHinhChungClientAppService : PMSAppServiceBase, ICauHinhChungClientAppService
    {
        private readonly IAbpSession _session;
        private readonly UserManager _userManager;
        private readonly IRepository<CauHinhChung> _systemConfigRepos;
        private readonly ICacheManager _cacheManager;

        public CauHinhChungClientAppService(UserManager userManager,
                                  IAbpSession session,
                                  IRepository<CauHinhChung> systemConfigRepos,
                                  ICacheManager cacheManager
        )
        {
            _session = session;
            _userManager = userManager;
            _systemConfigRepos = systemConfigRepos;
            _cacheManager = cacheManager;
        }

        public CauHinhChung GetCauHinhChungByKey(string settingKey)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    var obj = _systemConfigRepos.GetAll().Where(x => x.TuKhoa.Equals(settingKey)).FirstOrDefault();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    #endregion MAIN
}