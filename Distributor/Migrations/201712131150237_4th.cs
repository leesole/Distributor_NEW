namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupMembers", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.GroupMembers", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Groups", "EntityStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Groups", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Groups", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Organisations", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Organisations", "EntityStatusChangeBy", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "EntityStatusChangeBy");
            DropColumn("dbo.Organisations", "EntityStatusChangeOn");
            DropColumn("dbo.Groups", "EntityStatusChangeBy");
            DropColumn("dbo.Groups", "EntityStatusChangeOn");
            DropColumn("dbo.Groups", "EntityStatus");
            DropColumn("dbo.GroupMembers", "EntityStatusChangeBy");
            DropColumn("dbo.GroupMembers", "EntityStatusChangeOn");
        }
    }
}
