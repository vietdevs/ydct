using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using newPSG.PMS.EntityDB;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System;
using Abp.Authorization;
using Abp.Application.Services;
using Newtonsoft.Json;
using System.Collections.Generic;

#region Class Riêng Cho Từng Thủ tục
using XHoSoVFA = newPSG.PMS.EntityDB.VFA_DangKyQuangCao;
using XHoSoLienThong = newPSG.PMS.Dto.DangKyQuangCaoLTDto;
using XCallApiInput = newPSG.PMS.Dto.CallApiDangKyQuangCaoInput;
using XPagingInput = newPSG.PMS.Dto.PagingDangKyQuangCaoLTInput;
using newPSG.PMS.Dto;
using newPSG.PMS.API;
using Castle.Core.Logging;
using newPSG.PMS.Configuration.Tenants;
using Abp.Runtime.Session;
#endregion

namespace newPSG.PMS.Services
{
    public interface IDangKyQuangCaoLTAppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input);

        Task<KetQuaLienThong> LienThongHoSo(List<XCallApiInput> input);

        Task<KetQuaLienThong> LienThongNhieuHoSo();
    }

    [AbpAuthorize]
    public class DangKyQuangCaoLTAppService : PMSAppServiceBase, IDangKyQuangCaoLTAppService
    {
        private readonly ILogger _logger;
        private readonly IAbpSession _session;
        private readonly IDangKyQuangCaoLTDomainService _dangKyQuangCaoLTDomainService;

        public DangKyQuangCaoLTAppService(ILogger logger,
                                    IAbpSession session,
                                          IDangKyQuangCaoLTDomainService dangKyQuangCaoLTDomainService)
        {
            _logger = logger;
            _session = session;
            _dangKyQuangCaoLTDomainService = dangKyQuangCaoLTDomainService;
        }

        public async Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input)
        {
            try
            {
                var query = await _dangKyQuangCaoLTDomainService.GetListHoSoPaging(input, _session.TenantId);
                return query;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }

        #region Call API Cục-ATTP
        public async Task<KetQuaLienThong> LienThongHoSo(List<XCallApiInput> input)
        {
            try
            {
                KetQuaLienThong KetQua = new KetQuaLienThong();
                KetQua = await _dangKyQuangCaoLTDomainService.LienThongHoSo(input, _session.TenantId.Value);

                return KetQua;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }
        public async Task<KetQuaLienThong> LienThongNhieuHoSo()
        {
            try
            {
                KetQuaLienThong KetQua = new KetQuaLienThong();
                KetQua = await _dangKyQuangCaoLTDomainService.AotuLienThongHoSo(_session.TenantId);

                return KetQua;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message);
                return null;
            }
        }
        #endregion
    }
}