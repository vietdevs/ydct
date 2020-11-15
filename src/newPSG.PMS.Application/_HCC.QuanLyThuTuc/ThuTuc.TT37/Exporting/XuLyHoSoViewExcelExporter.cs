using Abp.Application.Services;
using Abp.Domain.Repositories;
using newPSG.PMS.DataExporting.Excel.EpPlus;
using newPSG.PMS.Dto;
using newPSG.PMS.EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS.Exporting
{
    public interface IXuLyHoSoViewExcelExporter : IApplicationService
    {
        FileDto ExportToFile(List<HoSoXuLyHistory37Dto> traCuuListDtos, long hoSoId);
    }
    public class XuLyHoSoViewExcelExporter: EpPlusExcelExporterBase, IXuLyHoSoViewExcelExporter
    {
        private readonly IRepository<HoSo37, long> _hoSoRepos;
        private readonly IRepository<LoaiHoSo> _loaiHoSoRepos;
        public XuLyHoSoViewExcelExporter(IRepository<HoSo37, long> hoSoRepos,
                                           IRepository<LoaiHoSo> loaiHoSoRepos)
        {
            _hoSoRepos = hoSoRepos;
            _loaiHoSoRepos = loaiHoSoRepos;
        }
        public FileDto ExportToFile(List<HoSoXuLyHistory37Dto> historyListDtos, long hoSoId)
        {
            if (hoSoId > 0)
            {
                var hoso = _hoSoRepos.Get(hoSoId);
                return CreateExcelPackage($"lichsuhoso_{hoso.MaHoSo}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add("Lịch sử hồ sơ");
                        sheet.OutLineApplyStyle = true;
                        AddHeader(
                            sheet, 1,
                            "STT",
                            "Tên người xử lý",
                            "Thời gian",
                            "Hành động",
                            "Kết quả",
                            "Ý kiến"
                            );
                        AddObjects(
                            sheet, 2, historyListDtos,
                            _ => _.STT,
                            _ => _.NguoiXuLy,
                            _ => _.StrNgayXuLy,
                            _ => _.HanhDongXuLy,
                            _ => _.StrKetQuaXuLy,
                            _ => _.NoiDungYKien
                            );
                        for (var i = 1; i <= 6; i++)
                        {
                            sheet.Column(i).AutoFit();
                        }
                    });
            }
            else
            {
                return null;
            }

        }
    }
}
