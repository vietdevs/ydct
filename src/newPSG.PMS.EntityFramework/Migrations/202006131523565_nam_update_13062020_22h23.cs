namespace newPSG.PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nam_update_13062020_22h23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TT37_HoSoDoanThamDinh",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HoSoId = c.Long(),
                        ThuTucId = c.Int(),
                        UserId = c.Long(),
                        VaiTroEnum = c.Int(),
                        TenVaiTro = c.String(),
                        TrangThaiXuLy = c.Int(),
                        NoiDungYkien = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TT37_HoSoDoanThamDinh");
        }
    }
}
