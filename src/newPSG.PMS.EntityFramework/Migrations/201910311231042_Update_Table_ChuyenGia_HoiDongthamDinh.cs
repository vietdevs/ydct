namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Table_ChuyenGia_HoiDongthamDinh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TT01_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT02_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT03_HoSoXuLy", "IsAllChuyenGiaDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT03_HoSoXuLy", "IsAllHoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT03_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT03_HoSoXuLy_ChuyenGia", "TrangThaiChuyenGiaDuyetHoSo", c => c.Int());
            AddColumn("dbo.TT03_HoSoXuLy_ChuyenGia", "YKienThamXet", c => c.String(storeType: "ntext"));
            AddColumn("dbo.TT03_HoSoXuLy_HoiDongThamDinh", "TrangThaiHoiDongThamDinhDuyetHoSo", c => c.Int());
            AddColumn("dbo.TT03_HoSoXuLy_HoiDongThamDinh", "YKienThamXet", c => c.String(storeType: "ntext"));
            AddColumn("dbo.TT04_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT05_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT06_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT07_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT08_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT09_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT10_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
            AddColumn("dbo.TT99_HoSoXuLy", "HoiDongThamDinhDaDuyet", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TT99_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT10_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT09_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT08_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT07_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT06_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT05_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT04_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT03_HoSoXuLy_HoiDongThamDinh", "YKienThamXet");
            DropColumn("dbo.TT03_HoSoXuLy_HoiDongThamDinh", "TrangThaiHoiDongThamDinhDuyetHoSo");
            DropColumn("dbo.TT03_HoSoXuLy_ChuyenGia", "YKienThamXet");
            DropColumn("dbo.TT03_HoSoXuLy_ChuyenGia", "TrangThaiChuyenGiaDuyetHoSo");
            DropColumn("dbo.TT03_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT03_HoSoXuLy", "IsAllHoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT03_HoSoXuLy", "IsAllChuyenGiaDaDuyet");
            DropColumn("dbo.TT02_HoSoXuLy", "HoiDongThamDinhDaDuyet");
            DropColumn("dbo.TT01_HoSoXuLy", "HoiDongThamDinhDaDuyet");
        }
    }
}
