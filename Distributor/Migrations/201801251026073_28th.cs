namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _28th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "LastOfferOriginatorAppUserId", c => c.Guid());
            AddColumn("dbo.Offers", "CounterOfferOriginatorAppUserId", c => c.Guid());
            AddColumn("dbo.Offers", "CounterOfferOriginatorOrganisationId", c => c.Guid());
            AddColumn("dbo.Offers", "CounterOfferOriginatorDateTime", c => c.DateTime());
            DropColumn("dbo.Offers", "LastCounterOfferOriginatorOrganisationId");
            DropColumn("dbo.Offers", "LastCounterOfferOriginatorDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Offers", "LastCounterOfferOriginatorDateTime", c => c.DateTime());
            AddColumn("dbo.Offers", "LastCounterOfferOriginatorOrganisationId", c => c.Guid());
            DropColumn("dbo.Offers", "CounterOfferOriginatorDateTime");
            DropColumn("dbo.Offers", "CounterOfferOriginatorOrganisationId");
            DropColumn("dbo.Offers", "CounterOfferOriginatorAppUserId");
            DropColumn("dbo.Offers", "LastOfferOriginatorAppUserId");
        }
    }
}
