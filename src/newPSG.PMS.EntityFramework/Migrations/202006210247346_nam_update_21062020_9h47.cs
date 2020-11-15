namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nam_update_21062020_9h47 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TT37_PhamViHoatDong", "Ten", c => c.String());
            DropColumn("dbo.TT37_PhamViHoatDong", "PhamViHoatDong");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TT37_PhamViHoatDong", "PhamViHoatDong", c => c.String());
            DropColumn("dbo.TT37_PhamViHoatDong", "Ten");
        }
    }
}
