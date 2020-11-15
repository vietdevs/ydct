using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace newPSG.PMS.Services
{
    public interface IXuLyPhanLoaiHoSoAppService : IApplicationService
    {
        List<ItemObj<int>> GetCustomLoaiHoSo(XuLyPhanLoaiHoSoInputDto input);
        Task<List<PhanLoaiHoSoDto>> GetLoaiHoSoFromThuThucId(int? thuTucId);
        Task<ServiceReturn<List<PhanLoaiHoSo_PhanCongDto>>> GetBienBan_SoLuongFromLoaiHoSoId(int setting_LoaiHoSoId);
        int? GetPhanLoaiHoSoId(XuLyPhanLoaiHoSoInputDto input);
    }

    public class XuLyPhanLoaiHoSoAppService : PMSAppServiceBase, IXuLyPhanLoaiHoSoAppService
    {
        private readonly IRepository<PhanLoaiHoSo> _setting_LoaiHoSoRepos;
        private readonly IRepository<PhanLoaiHoSo_PhanCong> _setting_LoaiHoSo_BienBanRepos;
        private readonly IRepository<PhanLoaiHoSo_Filter> _setting_LoaiHoSo_FilterRepos;
        private readonly IRepository<LoaiBienBanThamDinh> _loaiBienBanRepos;

        public XuLyPhanLoaiHoSoAppService(
            IRepository<PhanLoaiHoSo> setting_LoaiHoSoRepos,
            IRepository<PhanLoaiHoSo_PhanCong> setting_LoaiHoSo_BienBanRepos,
            IRepository<PhanLoaiHoSo_Filter> setting_LoaiHoSo_FilterRepos,
            IRepository<LoaiBienBanThamDinh> loaiBienBanRepos
        )
        {
            _setting_LoaiHoSoRepos = setting_LoaiHoSoRepos;
            _setting_LoaiHoSo_BienBanRepos = setting_LoaiHoSo_BienBanRepos;
            _setting_LoaiHoSo_FilterRepos = setting_LoaiHoSo_FilterRepos;
            _loaiBienBanRepos = loaiBienBanRepos;
        }

        public List<ItemObj<int>> GetCustomLoaiHoSo(XuLyPhanLoaiHoSoInputDto input)
        {
            var query = from data in _setting_LoaiHoSoRepos.GetAll()
                        where data.ThuTucId == input.ThuTucId && data.RoleLevel == input.RoleLevel
                        select new ItemObj<int>
                        {
                            Id = data.Id,
                            Name = data.Name
                        };

            return query.ToList();
        }

        public async Task<List<PhanLoaiHoSoDto>> GetLoaiHoSoFromThuThucId(int? thuTucId)
        {
            var query = from data in _setting_LoaiHoSoRepos.GetAll().WhereIf(thuTucId != null, x => x.ThuTucId == thuTucId)
                        orderby data.Id
                        select new PhanLoaiHoSoDto
                        {
                            Id = data.Id,
                            Name = data.Name,
                            ThuTucId = data.ThuTucId
                        };

            return await query.ToListAsync();
        }

        public async Task<ServiceReturn<List<PhanLoaiHoSo_PhanCongDto>>> GetBienBan_SoLuongFromLoaiHoSoId(int setting_LoaiHoSoId)
        {
            try
            {
                var query = (from dlDefault in _setting_LoaiHoSo_BienBanRepos.GetAll().Where(x => x.PhanLoaiHoSoId == setting_LoaiHoSoId)
                             select new PhanLoaiHoSo_PhanCongDto
                             {
                                 TieuBanEnum = dlDefault.TieuBanEnum,
                                 SoLuong = dlDefault.SoLuong,
                             });
                var res = await query.ToListAsync();

                if (res.Count > 0)
                {
                    return new ServiceReturn<List<PhanLoaiHoSo_PhanCongDto>>(res);
                }
                else
                {
                    return new ServiceReturn<List<PhanLoaiHoSo_PhanCongDto>>("Cảnh báo! Chưa cấu hình chuyên gia cho loại HS, YC Admin cấu hình!", Abp.Logging.LogSeverity.Warn);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public int? GetPhanLoaiHoSoId(XuLyPhanLoaiHoSoInputDto input)
        {
            try
            {
                var query = (from filter in _setting_LoaiHoSo_FilterRepos.GetAll()
                             join t_loaihoso in _setting_LoaiHoSoRepos.GetAll() on filter.PhanLoaiHoSoId equals t_loaihoso.Id into tb_loaihoso
                             from loaihoso in tb_loaihoso.DefaultIfEmpty()
                             where loaihoso.ThuTucId == input.ThuTucId && loaihoso.RoleLevel == input.RoleLevel
                             select filter);

                var dataGroup = query.GroupBy(x => x.PhanLoaiHoSoId)
                    .Select(x => new
                    {
                        PhanLoaiHoSoId = x.Key,
                        ListJsonFilter = x.Select(a => new
                        {
                            a.PhanLoaiHoSoId,
                            a.JsonFilter
                        })
                    }).ToList();

                List<XuLyPhanLoaiHoSo_CheckDto> listCheck = new List<XuLyPhanLoaiHoSo_CheckDto>();
                foreach (var data in dataGroup)
                {
                    var _deplicate = 0;
                    foreach (var item in data.ListJsonFilter)
                    {
                        if (input.ThuTucId == (int)CommonENum.THU_TUC_ID.THU_TUC_03)
                        {
                            if (input.RoleLevel == (int)CommonENum.ROLE_LEVEL.CHUYEN_VIEN)
                            {
                                var filter = JsonConvert.DeserializeObject<Filter_PhanLoaiHoSoDto>(item.JsonFilter);
                                if (DeepEquals_PDK(filter, input.Filter, ref _deplicate))
                                {
                                    listCheck.Add(new XuLyPhanLoaiHoSo_CheckDto()
                                    {
                                        PhanLoaiHoSoId = item.PhanLoaiHoSoId,
                                        Deplicate = _deplicate
                                    });
                                }
                            }
                            else if (input.RoleLevel == (int)CommonENum.ROLE_LEVEL.CHUYEN_GIA)
                            {
                                var filter = JsonConvert.DeserializeObject<Filter_PhanLoaiHoSoDto>(item.JsonFilter);
                                if (DeepEquals_PDK(filter, input.Filter, ref _deplicate))
                                {
                                    listCheck.Add(new XuLyPhanLoaiHoSo_CheckDto()
                                    {
                                        PhanLoaiHoSoId = item.PhanLoaiHoSoId,
                                        Deplicate = _deplicate
                                    });
                                }
                            }
                        }
                    }
                }

                var _check = listCheck.OrderByDescending(x => x.Deplicate).FirstOrDefault();
                if (_check == null)
                {
                    return null;
                }

                return _check.PhanLoaiHoSoId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Filter Loại hồ sơ - Chuyên gia - Thủ tục 89, 90 (Phòng đăng ký)

        private bool DeepEquals_PDK(object settingFilter, object _filter, ref int _soLuong)
        {
            if (settingFilter == null || _filter == null)
            {
                return false;
            }

            var result = true;
            foreach (var property in settingFilter.GetType().GetProperties())
            {
                var objValue = property.GetValue(settingFilter);
                if (objValue != null)
                {
                    var _objValueType = objValue.GetType();
                    if (_objValueType.IsClass)
                    {
                        var anotherValue = _filter.GetType().GetProperty(property.Name).GetValue(_filter);

                        if (DeepEquals_PDK(objValue, anotherValue, ref _soLuong) != true)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (_objValueType.Equals(typeof(bool)) && (bool)objValue == true)
                        {
                            var anotherValue = _filter.GetType().GetProperty(property.Name).GetValue(_filter);
                            if (objValue.Equals(anotherValue))
                            {
                                _soLuong++;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else if (_objValueType.Equals(typeof(int)))
                        {
                            var anotherValue = _filter.GetType().GetProperty(property.Name).GetValue(_filter);
                            if (objValue.Equals(anotherValue))
                            {
                                _soLuong++;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}