namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21st : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequiredListings", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.RequiredListings", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.RequiredListings", "RecordChangeBy", c => c.Guid(nullable: false));
            AlterColumn("dbo.RequiredListings", "UoM", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequiredListings", "UoM", c => c.String());
            DropColumn("dbo.RequiredListings", "RecordChangeBy");
            DropColumn("dbo.RequiredListings", "RecordChangeOn");
            DropColumn("dbo.RequiredListings", "RecordChange");
        }
    }
}
