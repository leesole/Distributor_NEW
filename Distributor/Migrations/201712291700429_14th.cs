namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _14th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupMembers", "EntityStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupMembers", "EntityStatus");
        }
    }
}
