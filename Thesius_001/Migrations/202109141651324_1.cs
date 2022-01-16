namespace Thesius_001.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserThesises", "MyMark", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserThesises", "MyMark");
        }
    }
}
