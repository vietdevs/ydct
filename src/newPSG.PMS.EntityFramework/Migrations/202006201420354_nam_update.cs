namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nam_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DanhMucHuyen", "MaLGSP", c => c.String(maxLength: 2000));
            AddColumn("dbo.DanhMucXa", "MaLGSP", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DanhMucXa", "MaLGSP");
            DropColumn("dbo.DanhMucHuyen", "MaLGSP");
        }
    }
}
