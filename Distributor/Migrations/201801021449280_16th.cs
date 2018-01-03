namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _16th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AvailableListings", "RecordChange", c => c.Int(nullable: false));
            AddColumn("dbo.AvailableListings", "RecordChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AvailableListings", "RecordChangeBy", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AvailableListings", "RecordChangeBy");
            DropColumn("dbo.AvailableListings", "RecordChangeOn");
            DropColumn("dbo.AvailableListings", "RecordChange");
        }
    }
}
