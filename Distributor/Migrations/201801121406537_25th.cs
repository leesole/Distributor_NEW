namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _25th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "OfferOriginatorOrganisationId", c => c.Guid(nullable: false));
            AddColumn("dbo.Offers", "ListingOriginatorOrganisationId", c => c.Guid());
            AddColumn("dbo.Offers", "LastCounterOfferOriginatorOrganisationId", c => c.Guid());
            AddColumn("dbo.Offers", "OrderOriginatorOrganisationId", c => c.Guid());
            AddColumn("dbo.Orders", "OrderOriginatorOrganisationId", c => c.Guid());
            AddColumn("dbo.Orders", "OfferOriginatorOrganisationId", c => c.Guid());
            AddColumn("dbo.Orders", "ListingOriginatorOrganisationId", c => c.Guid());
            DropColumn("dbo.Offers", "OfferOriginatorCompanyId");
            DropColumn("dbo.Offers", "ListingOriginatorCompanyId");
            DropColumn("dbo.Offers", "LastCounterOfferOriginatorCompanyId");
            DropColumn("dbo.Offers", "OrderOriginatorCompanyId");
            DropColumn("dbo.Orders", "OrderOriginatorCompanyId");
            DropColumn("dbo.Orders", "OfferOriginatorCompanyId");
            DropColumn("dbo.Orders", "ListingOriginatorCompanyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ListingOriginatorCompanyId", c => c.Guid());
            AddColumn("dbo.Orders", "OfferOriginatorCompanyId", c => c.Guid());
            AddColumn("dbo.Orders", "OrderOriginatorCompanyId", c => c.Guid());
            AddColumn("dbo.Offers", "OrderOriginatorCompanyId", c => c.Guid());
            AddColumn("dbo.Offers", "LastCounterOfferOriginatorCompanyId", c => c.Guid());
            AddColumn("dbo.Offers", "ListingOriginatorCompanyId", c => c.Guid());
            AddColumn("dbo.Offers", "OfferOriginatorCompanyId", c => c.Guid(nullable: false));
            DropColumn("dbo.Orders", "ListingOriginatorOrganisationId");
            DropColumn("dbo.Orders", "OfferOriginatorOrganisationId");
            DropColumn("dbo.Orders", "OrderOriginatorOrganisationId");
            DropColumn("dbo.Offers", "OrderOriginatorOrganisationId");
            DropColumn("dbo.Offers", "LastCounterOfferOriginatorOrganisationId");
            DropColumn("dbo.Offers", "ListingOriginatorOrganisationId");
            DropColumn("dbo.Offers", "OfferOriginatorOrganisationId");
        }
    }
}
