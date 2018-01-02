namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _15th : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AvailableListings",
                c => new
                    {
                        ListingId = c.Guid(nullable: false),
                        ItemDescription = c.String(nullable: false),
                        ItemCategory = c.Int(nullable: false),
                        ItemType = c.Int(nullable: false),
                        QuantityAvailable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityFulfilled = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityOutstanding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UoM = c.String(),
                        AvailableFrom = c.DateTime(),
                        AvailableTo = c.DateTime(),
                        ItemCondition = c.Int(nullable: false),
                        DisplayUntilDate = c.DateTime(),
                        SellByDate = c.DateTime(),
                        UseByDate = c.DateTime(),
                        DeliveryAvailable = c.Boolean(nullable: false),
                        ListingStatus = c.Int(nullable: false),
                        ListingOrganisationPostcode = c.String(),
                        ListingOriginatorAppUserId = c.Guid(nullable: false),
                        ListingOriginatorOrganisationId = c.Guid(nullable: false),
                        ListingOriginatorDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ListingId);
            
            CreateTable(
                "dbo.RequiredListings",
                c => new
                    {
                        ListingId = c.Guid(nullable: false),
                        ItemDescription = c.String(nullable: false),
                        ItemCategory = c.Int(nullable: false),
                        ItemType = c.Int(nullable: false),
                        QuantityRequired = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityFulfilled = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityOutstanding = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UoM = c.String(),
                        RequiredFrom = c.DateTime(),
                        RequiredTo = c.DateTime(),
                        AcceptDamagedItems = c.Boolean(nullable: false),
                        AcceptOutOfDateItems = c.Boolean(nullable: false),
                        CollectionAvailable = c.Boolean(nullable: false),
                        ListingStatus = c.Int(nullable: false),
                        ListingOrganisationPostcode = c.String(),
                        ListingOriginatorAppUserId = c.Guid(nullable: false),
                        ListingOriginatorOrganisationId = c.Guid(nullable: false),
                        ListingOriginatorDateTime = c.DateTime(nullable: false),
                        CampaignId = c.Guid(),
                    })
                .PrimaryKey(t => t.ListingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RequiredListings");
            DropTable("dbo.AvailableListings");
        }
    }
}
