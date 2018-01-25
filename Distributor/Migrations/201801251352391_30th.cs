namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _30th : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserActions");
        }
    }
}
