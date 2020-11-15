using Abp.Application.Services;
using Abp.Authorization;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using newPSG.PMS.Configuration.Tenants;
using newPSG.PMS.Dto;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Services
{
    public interface ICallApiATTPAppService : IApplicationService
    {
        Task<TokenDto> GetToken(TaiKhoanLienThongInput input);
        Task<TokenDto> AutoGetToKen();
        Task<ResultApi> CallApi(string input, string api, string token = "");
    }

    [AbpAuthorize]
    public class CallApiATTPAppService : ICallApiATTPAppService
    {
        private readonly ILogger _logger;
        private readonly IAbpSession _session;
        private readonly ITenantSettingsAppService _tenantSettingsr;
        private readonly ICallApiDomainService _callApiDomainService;

        public CallApiATTPAppService(ILogger logger,
            IAbpSession session,
            ITenantSettingsAppService tenantSettingsr,
            ICallApiDomainService callApiDomainService)
        {
            _logger = logger;
            _session = session;
            _tenantSettingsr = tenantSettingsr;
            _callApiDomainService = callApiDomainService;
        }

        public async Task<ResultApi> CallApi(string dataJson, string api, string token = "")
        {
            try
            {
                var obj = await _callApiDomainService.CallApi(_session.TenantId.Value, dataJson, api, token);
                return obj;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<TokenDto> GetToken(TaiKhoanLienThongInput input)
        {
            try
            {
                var obj = await _callApiDomainService.GetToken(input);
                return obj;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }

        public async Task<TokenDto> AutoGetToKen()
        {
            try
            {
                var obj = await _callApiDomainService.AutoGetToKen(_session.TenantId.Value);
                return obj;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }
    }
}
