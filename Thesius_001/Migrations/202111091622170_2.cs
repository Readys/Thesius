namespace Thesius_001.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Book", "MainThesisId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Book", "MainThesisId", c => c.Int(nullable: false));
        }
    }
}
