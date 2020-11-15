namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nam_update_13062020_21h15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TT37_HoSoXuLy", "NgayTraGiayTiepNhan", c => c.DateTime());
            AddColumn("dbo.TT37_HoSoXuLy", "SoGiayTiepNhan", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "NgayHenCap", c => c.DateTime());
            AddColumn("dbo.TT37_HoSoXuLy", "HinhThucCapCTJson", c => c.String(storeType: "ntext"));
            AddColumn("dbo.TT37_HoSoXuLy", "TaiLieuDaNhanJson", c => c.String(storeType: "ntext"));
            AddColumn("dbo.TT37_HoSoXuLy", "TrangThaiXuLy", c => c.Int());
            AddColumn("dbo.TT37_HoSoXuLy", "LyDoTraLai", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "SoCongVan", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "NgayYeuCauBoSung", c => c.DateTime());
            AddColumn("dbo.TT37_HoSoXuLy", "NoiDungYeuCauGiaiQuyet", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "LyDoYeuCauBoSung", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "TenCanBoHoTro", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "DienThoaiCanBo", c => c.String());
            AddColumn("dbo.TT37_HoSoXuLy", "BienBanTongHopUrl", c => c.String(maxLength: 2000));
            AddColumn("dbo.TT37_HoSoXuLy", "NgayLapDoanThamDinh", c => c.DateTime());
            AddColumn("dbo.TT37_HoSoXuLy", "NguoiLapDoanThamDinhId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TT37_HoSoXuLy", "NguoiLapDoanThamDinhId");
            DropColumn("dbo.TT37_HoSoXuLy", "NgayLapDoanThamDinh");
            DropColumn("dbo.TT37_HoSoXuLy", "BienBanTongHopUrl");
            DropColumn("dbo.TT37_HoSoXuLy", "DienThoaiCanBo");
            DropColumn("dbo.TT37_HoSoXuLy", "TenCanBoHoTro");
            DropColumn("dbo.TT37_HoSoXuLy", "LyDoYeuCauBoSung");
            DropColumn("dbo.TT37_HoSoXuLy", "NoiDungYeuCauGiaiQuyet");
            DropColumn("dbo.TT37_HoSoXuLy", "NgayYeuCauBoSung");
            DropColumn("dbo.TT37_HoSoXuLy", "SoCongVan");
            DropColumn("dbo.TT37_HoSoXuLy", "LyDoTraLai");
            DropColumn("dbo.TT37_HoSoXuLy", "TrangThaiXuLy");
            DropColumn("dbo.TT37_HoSoXuLy", "TaiLieuDaNhanJson");
            DropColumn("dbo.TT37_HoSoXuLy", "HinhThucCapCTJson");
            DropColumn("dbo.TT37_HoSoXuLy", "NgayHenCap");
            DropColumn("dbo.TT37_HoSoXuLy", "SoGiayTiepNhan");
            DropColumn("dbo.TT37_HoSoXuLy", "NgayTraGiayTiepNhan");
        }
    }
}
