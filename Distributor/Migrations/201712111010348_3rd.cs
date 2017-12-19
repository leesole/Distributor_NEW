namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3rd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "UserRole", c => c.Int(nullable: false));
            DropColumn("dbo.AppUsers", "AdminUser");
            DropColumn("dbo.AppUsers", "SuperUser");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AppUsers", "SuperUser", c => c.Boolean(nullable: false));
            AddColumn("dbo.AppUsers", "AdminUser", c => c.Boolean(nullable: false));
            DropColumn("dbo.AppUsers", "UserRole");
        }
    }
}
