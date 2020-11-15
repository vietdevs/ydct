namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nam_update_29062020_22h09 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TT37_HoSoXuLyHistory", "TrangThaiXuLy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TT37_HoSoXuLyHistory", "TrangThaiXuLy");
        }
    }
}
