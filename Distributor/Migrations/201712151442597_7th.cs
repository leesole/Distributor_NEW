namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7th : DbMigration
    {
        public override void Up()
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
            
            CreateTable(
                "dbo.UserTasks",
                c => new
                    {
                        UserTaskId = c.Guid(nullable: false),
                        TaskType = c.Int(nullable: false),
                        TaskDescription = c.String(),
                        ReferenceKey = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Guid(nullable: false),
                        EntityStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTaskId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserTasks");
            DropTable("dbo.UserTaskAssignments");
        }
    }
}
