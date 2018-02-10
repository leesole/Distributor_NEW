namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _34th : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationId = c.Guid(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        NotificationDescription = c.String(),
                        ReferenceKey = c.Guid(nullable: false),
                        AppUserId = c.Guid(),
                        OrganisationId = c.Guid(nullable: false),
                        EntityStatus = c.Int(nullable: false),
                        RecordChange = c.Int(nullable: false),
                        RecordChangeOn = c.DateTime(nullable: false),
                        RecordChangeBy = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationId);
            
            DropTable("dbo.UserActions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserActions",
                c => new
                    {
                        UserActionId = c.Guid(nullable: false),
                        ActionType = c.Int(nullable: false),
                        ActionDescription = c.String(),
                        ReferenceKey = c.Guid(nullable: false),
                        AppUserId = c.Guid(),
                        OrganisationId = c.Guid(nullable: false),
                        EntityStatus = c.Int(nullable: false),
                        RecordChange = c.Int(nullable: false),
                        RecordChangeOn = c.DateTime(nullable: false),
                        RecordChangeBy = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.UserActionId);
            
            DropTable("dbo.Notifications");
        }
    }
}
