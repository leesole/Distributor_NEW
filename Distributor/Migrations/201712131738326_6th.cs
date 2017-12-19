namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.AppUsers", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AppUsers", "RecordChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.GroupMembers", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.GroupMembers", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.GroupMembers", "RecordChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Groups", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.Groups", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Groups", "RecordChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Organisations", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.Organisations", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Organisations", "RecordChangeBy", c => c.Guid(nullable: false));
            DropColumn("dbo.AppUsers", "EntityStatusChangeOn");
            DropColumn("dbo.AppUsers", "EntityStatusChangeBy");
            DropColumn("dbo.GroupMembers", "EntityStatusChangeOn");
            DropColumn("dbo.GroupMembers", "EntityStatusChangeBy");
            DropColumn("dbo.Groups", "EntityStatusChangeOn");
            DropColumn("dbo.Groups", "EntityStatusChangeBy");
            DropColumn("dbo.Organisations", "EntityStatusChangeOn");
            DropColumn("dbo.Organisations", "EntityStatusChangeBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Organisations", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Organisations", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Groups", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.Groups", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.GroupMembers", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.GroupMembers", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AppUsers", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.AppUsers", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Organisations", "RecordChangeBy");
            DropColumn("dbo.Organisations", "RecordChangeOn");
            DropColumn("dbo.Organisations", "RecordChange");
            DropColumn("dbo.Groups", "RecordChangeBy");
            DropColumn("dbo.Groups", "RecordChangeOn");
            DropColumn("dbo.Groups", "RecordChange");
            DropColumn("dbo.GroupMembers", "RecordChangeBy");
            DropColumn("dbo.GroupMembers", "RecordChangeOn");
            DropColumn("dbo.GroupMembers", "RecordChange");
            DropColumn("dbo.AppUsers", "RecordChangeBy");
            DropColumn("dbo.AppUsers", "RecordChangeOn");
            DropColumn("dbo.AppUsers", "RecordChange");
        }
    }
}
