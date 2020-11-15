using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Services;
using Abp.Domain.Uow;
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
    public interface ICallApiDomainService : IDomainService
    {
        Task<TokenDto> GetToken(TaiKhoanLienThongInput input);
        Task<TokenDto> AutoGetToKen(int chiCucId);
        Task<ResultApi> CallApi(int chiCucId, string input, string api, string token = "");
        bool IsAccountLienThong(int tenanId);
    }

    public class CallApiDomainService : ICallApiDomainService
    {
        private readonly ILogger _logger;
        private readonly ITenantSettingsDomainService _tenantSettingsr;

        public CallApiDomainService(ILogger logger,
            ITenantSettingsDomainService tenantSettingsr)
        {
            _logger = logger;
            _tenantSettingsr = tenantSettingsr;
        }

        [UnitOfWork]
        public async Task<ResultApi> CallApi(int chiCucId, string dataJson, string api, string token = "")
        {
            try
            {
                var _lienThongSettings = await _tenantSettingsr.GetLienThongInfoSettingsAsync(chiCucId);
                if (_lienThongSettings != null && !string.IsNullOrEmpty(_lienThongSettings.DomainLienThong))
                {
                    ResultApi obj = new ResultApi();
                    using (var client = new HttpClient())
                    {
                        string domain = _lienThongSettings.DomainLienThong;

                        client.BaseAddress = new Uri(domain);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        HttpContent content = new StringContent(dataJson, Encoding.UTF8, "application/json");
                        var messge = await client.PostAsync(api, content);
                        if (messge.IsSuccessStatusCode)
                        {
                            string result = await messge.Content.ReadAsStringAsync();
                            obj = JsonConvert.DeserializeObject<ResultApi>(result);
                            return obj;
                        }
                    }
                    obj.Success = false;
                    return obj;
                }
                return null;
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
                if (!string.IsNullOrEmpty(input.UsernameOrEmailAddress) && !string.IsNullOrEmpty(input.Password) && !string.IsNullOrEmpty(input.DomainLienThong))
                {
                    using (var client = new HttpClient())
                    {
                        string domain = input.DomainLienThong;
                        string api = "api/Account";
                        if (string.IsNullOrEmpty(input.TenancyName))
                        {
                            input.TenancyName = "lienthong";
                        }
                        string dataJson = JsonConvert.SerializeObject(input);

                        client.BaseAddress = new Uri(domain);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpContent content = new StringContent(dataJson, Encoding.UTF8, "application/json");
                        var messge = await client.PostAsync(api, content);
                        if (messge.IsSuccessStatusCode)
                        {
                            string result = await messge.Content.ReadAsStringAsync();
                            TokenDto obj = JsonConvert.DeserializeObject<TokenDto>(result);
                            return obj;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }

        [UnitOfWork]
        public async Task<TokenDto> AutoGetToKen(int chiCucId)
        {
            try
            {
                var _lienThongSettings = await _tenantSettingsr.GetLienThongInfoSettingsAsync(chiCucId);
                if (_lienThongSettings != null && !string.IsNullOrEmpty(_lienThongSettings.UserLienThong))
                {
                    var account = new TaiKhoanLienThongInput
                    {
                        DomainLienThong = _lienThongSettings.DomainLienThong,
                        Password = _lienThongSettings.PassLienThong,
                        UsernameOrEmailAddress = _lienThongSettings.UserLienThong,
                        TenancyName = _lienThongSettings.TenantLienThong
                    };
                    var obj = await GetToken(account);
                    return obj;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }

        [UnitOfWork]
        public bool IsAccountLienThong(int tenanId)
        {
            return _tenantSettingsr.IsAccountLienThong(tenanId);
        }
    }
}
