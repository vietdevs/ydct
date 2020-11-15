using System;
using System.Collections.Generic;

namespace newPSG.PMS.Tenants.Dashboard.Dto
{
    public class DashboardSearchDTO
    {
        public long? doanhNghiepId { get; set; }
        public DateTime? DateSumary { get; set; }
        public long? tinhId { get; set; }
        public int? roleLevel { get; set; }
        public long userId { get; set; }
    }
    public class DashboardDTO
    {
        public GeneralSummary GeneralSummary { get; set; }
        public ExpertSummary ExpertSummary { get; set; }
        public ManagerSummary ManagerSummary { get; set; }
        public LeaderSummary LeaderSummary { get; set; }
        public ResultSummary ResultSummary { get; set; }
    }
    public class GeneralSummary
    {
        //thông tin hiển thị với quản lý
        public int hoSoDaTiepNhan { get; set; }
        public int hoSoDaKyDuyet { get; set; }
        public int hoSoDaGuiCvSDBS { get; set; }
        public int hoSoDaPhanCongChuaXuLy { get; set; }
        public int hoSoQuaHanXuLy { get; set; }
        public int hoSoDaCoSDBS { get; set; }
        public int hoSoQuaHanDnKhongSDBS { get; set; }
        //thông tin hiển thị với doanh nghiệp
        public int hoSoChuaHoanThien { get; set; }
        public int hoSoDangThanhToan { get; set; }
        public int hoSoDangTrongQuaTrinhXuLy { get; set; }
        public int hoSoCanBoSung { get; set; }
        public int hoSoDaHoanTat { get; set; }
    }

    public class ExpertSummary
    {
        public int hoSoDuocPhanCongCV1 { get; set; }
        public int hoSoDuocPhanCongCV2 { get; set; }
        public int hoSoDatTrinhLDP { get; set; }
        public int hoSoDatTrinhLDC { get; set; }
        public int hoSoTrinhLDP_SDBS { get; set; }
        public int hoSoCanThamDinhLai { get; set; }
    }

    public class ManagerSummary
    {
        //thông tin hiển thị với quản lý
        public int hoSoTrinhDat { get; set; }
        public int hoSoTrinhSDBS { get; set; }
        public int hoSoTrinhSDBSTiep { get; set; }
        public int hoSoLDC_TraTDL { get; set; }
        //thông tin hiển thị với Doanh Nghiệp
        public int hoSoDangChoThamDinh { get; set; }
        public int hoSoDangThamDinh { get; set; }
        public int hoSoDaThamDinh { get; set; }
        public int hoSoDangDuyet { get; set; }
        public int hoSoDaDuocDuyet { get; set; }
    }
    public class LeaderSummary
    {
        //thông tin hiển thị với quản lý
        public int hoSoPhongQLSP_TrinhDat { get; set; }
        public int hoSoPhongQLSP_TrinhYeuCauBoSung { get; set; }
        public int hoSoPhongQLTCVaKN_TrinhDat { get; set; }
        public int hoSoPhongQLTCVaKN_TrinhYeuCauBoSung { get; set; }
        //thông tin hiển thị với Doanh Nghiệp
        public int hoSoCucDangXuLy { get; set; }
        public int hoSoCucDaXuLy { get; set; }
    }
    public class ResultSummary
    {
        public int hoSoDuocCapBanCongBo { get; set; }
        public int hoSoCanBoSungThem { get; set; }
    }
}