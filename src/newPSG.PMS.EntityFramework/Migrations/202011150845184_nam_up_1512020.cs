namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nam_up_1512020 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TT37_PhamViHoatDong", "Chung_Name", c => c.String());
            AddColumn("dbo.TT37_PhamViHoatDong", "Chung_BirthDay", c => c.DateTime());
            AddColumn("dbo.TT37_PhamViHoatDong", "Chung_Province", c => c.String());
            AddColumn("dbo.TT37_PhamViHoatDong", "Chung_Age", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TT37_PhamViHoatDong", "Chung_Age");
            DropColumn("dbo.TT37_PhamViHoatDong", "Chung_Province");
            DropColumn("dbo.TT37_PhamViHoatDong", "Chung_BirthDay");
            DropColumn("dbo.TT37_PhamViHoatDong", "Chung_Name");
        }
    }
}
