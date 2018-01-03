namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _18th : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AvailableListings", "AvailableFrom", c => c.DateTime());
            AlterColumn("dbo.AvailableListings", "AvailableTo", c => c.DateTime());
            AlterColumn("dbo.AvailableListings", "DisplayUntilDate", c => c.DateTime());
            AlterColumn("dbo.AvailableListings", "SellByDate", c => c.DateTime());
            AlterColumn("dbo.AvailableListings", "UseByDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AvailableListings", "UseByDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AvailableListings", "SellByDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AvailableListings", "DisplayUntilDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AvailableListings", "AvailableTo", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AvailableListings", "AvailableFrom", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
    }
}
