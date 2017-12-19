namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTasks", "OrganisationId", c => c.Guid(nullable: false));
            AddColumn("dbo.UserTasks", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.UserTasks", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserTasks", "RecordChangeBy", c => c.Guid(nullable: false));
            DropColumn("dbo.UserTasks", "CreatedOn");
            DropColumn("dbo.UserTasks", "CreatedBy");
            DropTable("dbo.UserTaskAssignments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserTaskAssignments",
                c => new
                    {
                        UserTaskAssignmentId = c.Guid(nullable: false),
                        UserTaskId = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        UserRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTaskAssignmentId);
            
            AddColumn("dbo.UserTasks", "CreatedBy", c => c.Guid(nullable: false));
            AddColumn("dbo.UserTasks", "CreatedOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.UserTasks", "RecordChangeBy");
            DropColumn("dbo.UserTasks", "RecordChangeOn");
            DropColumn("dbo.UserTasks", "RecordChange");
            DropColumn("dbo.UserTasks", "OrganisationId");
        }
    }
}
