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

#region Class Riêng Cho Từng Thủ tục
using XHoSoVFA = newPSG.PMS.EntityDB.VFA_CoSoDuDieuKien;
using XHoSoLienThong = newPSG.PMS.Dto.CoSoDuDieuKienLTDto;
using XCallApiInput = newPSG.PMS.Dto.CallApiCoSoDuDieuKienInput;
using XPagingInput = newPSG.PMS.Dto.PagingCoSoDuDieuKienLTInput;
using newPSG.PMS.Common;
using System.Collections.Generic;
using Castle.Core.Logging;
using newPSG.PMS.Dto;
using newPSG.PMS.API;
using Abp.Runtime.Session;
#endregion

namespace newPSG.PMS.Services
{
    public interface ICoSoDuDieuKienLTAppService : IApplicationService
    {
        Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input);

        Task<KetQuaLienThong> LienThongHoSo(List<XCallApiInput> input);

        Task<KetQuaLienThong> LienThongNhieuHoSo();
    }

    [AbpAuthorize]
    public class CoSoDuDieuKienLTAppService : PMSAppServiceBase, ICoSoDuDieuKienLTAppService
    {
        private readonly ILogger _logger;
        private readonly IAbpSession _session;
        private readonly ICoSoDuDieuKienLTDomainService _coSoDuDieuKienLTAppService;

        public CoSoDuDieuKienLTAppService(ILogger logger,
                                          IAbpSession session,
                                          ICoSoDuDieuKienLTDomainService coSoDuDieuKienLTAppService)
        {
            _logger = logger;
            _session = session;
            _coSoDuDieuKienLTAppService = coSoDuDieuKienLTAppService;
        }

        public async Task<PagedResultDto<XHoSoLienThong>> GetListHoSoPaging(XPagingInput input)
        {
            try
            {
                var query = await _coSoDuDieuKienLTAppService.GetListHoSoPaging(input, _session.TenantId);
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
                KetQua = await _coSoDuDieuKienLTAppService.LienThongHoSo(input, _session.TenantId.Value);

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
                KetQua = await _coSoDuDieuKienLTAppService.AotuLienThongHoSo(_session.TenantId);

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