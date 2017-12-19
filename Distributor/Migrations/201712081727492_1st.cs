namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1st : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AppUserId", c => c.Guid(nullable: false));
            AddColumn("dbo.AspNetUsers", "CurrentUserRole", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CurrentUserRole");
            DropColumn("dbo.AspNetUsers", "AppUserId");
        }
    }
}
