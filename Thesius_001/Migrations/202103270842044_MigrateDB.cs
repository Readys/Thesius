namespace Thesius_001.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        LinkId = c.Int(nullable: false, identity: true),
                        Userid = c.String(),
                        Name = c.String(maxLength: 2048),
                        Body = c.String(maxLength: 2048),
                        CreateDate = c.DateTime(nullable: false),
                        ModifedDate = c.DateTime(nullable: false),
                        Deleted = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LinkId);
            
            CreateTable(
                "dbo.ThesisLinks",
                c => new
                    {
                        ThesisLinkId = c.Int(nullable: false, identity: true),
                        ThesisId = c.Int(nullable: false),
                        LinkId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ThesisLinkId);
            
            CreateTable(
                "dbo.ThesisTags",
                c => new
                    {
                        ThesisTagsId = c.Int(nullable: false, identity: true),
                        ThesisId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ThesisTagsId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ThesisTags");
            DropTable("dbo.ThesisLinks");
            DropTable("dbo.Links");
        }
    }
}
