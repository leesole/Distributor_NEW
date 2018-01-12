namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24th : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        OfferId = c.Guid(nullable: false),
                        ListingId = c.Guid(nullable: false),
                        ListingType = c.Int(nullable: false),
                        OfferStatus = c.Int(nullable: false),
                        CurrentOfferQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousOfferQuantity = c.Decimal(precision: 18, scale: 2),
                        CounterOfferQuantity = c.Decimal(precision: 18, scale: 2),
                        RejectedBy = c.Guid(),
                        RejectedOn = c.DateTime(),
                        OfferOriginatorAppUserId = c.Guid(nullable: false),
                        OfferOriginatorCompanyId = c.Guid(nullable: false),
                        OfferOriginatorDateTime = c.DateTime(nullable: false),
                        ListingOriginatorAppUserId = c.Guid(),
                        ListingOriginatorCompanyId = c.Guid(),
                        ListingOriginatorDateTime = c.DateTime(),
                        LastCounterOfferOriginatorAppUserId = c.Guid(),
                        LastCounterOfferOriginatorCompanyId = c.Guid(),
                        LastCounterOfferOriginatorDateTime = c.DateTime(),
                        OrderId = c.Guid(),
                        OrderOriginatorAppUserId = c.Guid(),
                        OrderOriginatorCompanyId = c.Guid(),
                        OrderOriginatorDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.OfferId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Guid(nullable: false),
                        ListingType = c.Int(nullable: false),
                        OrderQuanity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderStatus = c.Int(nullable: false),
                        OrderCreationDateTime = c.DateTime(),
                        OrderDistributionDateTime = c.DateTime(),
                        OrderDistributedBy = c.Guid(),
                        OrderDeliveredDateTime = c.DateTime(),
                        OrderDeliveredBy = c.Guid(),
                        OrderCollectedDateTime = c.DateTime(),
                        OrderCollectedBy = c.Guid(),
                        OrderReceivedDateTime = c.DateTime(),
                        OrderReceivedBy = c.Guid(),
                        OrderClosedDateTime = c.DateTime(),
                        OrderClosedBy = c.Guid(),
                        OrderOriginatorAppUserId = c.Guid(),
                        OrderOriginatorCompanyId = c.Guid(),
                        OrderOriginatorDateTime = c.DateTime(),
                        OfferId = c.Guid(),
                        OfferOriginatorAppUserId = c.Guid(),
                        OfferOriginatorCompanyId = c.Guid(),
                        ListingId = c.Guid(),
                        ListingOriginatorAppUserId = c.Guid(),
                        ListingOriginatorCompanyId = c.Guid(),
                    })
                .PrimaryKey(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Orders");
            DropTable("dbo.Offers");
        }
    }
}
