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
using Castle.Core.Logging;
using newPSG.PMS.Dto;
using newPSG.PMS.API;

#region Class Riêng Cho Từng Thủ tục
using XHoSoVFA = newPSG.PMS.EntityDB.VFADangKyCongBo;
using XHoSoLienThong = newPSG.PMS.Dto.DangKyCongBoLTDto;
using XCallApiInput = newPSG.PMS.Dto.CallApiDangKyCongBoInput;
using XPagingInput = newPSG.PMS.Dto.PagingDangKyCongBoLTInput;
using Abp.Runtime.Session;
#endregion

namespace newPSG.PMS.Services
{
    public interface IDangKyCongBoLTAppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input);

        Task<KetQuaLienThong> LienThongHoSo(List<XCallApiInput> input);

        Task<KetQuaLienThong> LienThongNhieuHoSo();
    }

    [AbpAuthorize]
    public class DangKyCongBoLTAppService : PMSAppServiceBase, IDangKyCongBoLTAppService
    {
        private readonly ILogger _logger;
        private readonly IAbpSession _session;
        private readonly IDangKyCongBoLTDomainService _dangKyCongBoLTDomainService;

        public DangKyCongBoLTAppService(ILogger logger,
                                    IAbpSession session,
                                    IDangKyCongBoLTDomainService dangKyCongBoLTDomainService)
        {
            _logger = logger;
            _session = session;
            _dangKyCongBoLTDomainService = dangKyCongBoLTDomainService;
        }

        public async Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input)
        {
            try
            {
                var query = await _dangKyCongBoLTDomainService.GetListHoSoPaging(input, _session.TenantId);
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
                KetQua = await _dangKyCongBoLTDomainService.LienThongHoSo(input, _session.TenantId.Value);

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
                KetQua = await _dangKyCongBoLTDomainService.AotuLienThongHoSo(_session.TenantId);

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