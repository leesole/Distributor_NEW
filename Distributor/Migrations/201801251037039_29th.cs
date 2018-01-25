namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _29th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "LastOfferOriginatorDateTime", c => c.DateTime());
            AddColumn("dbo.Offers", "LastCounterOfferOriginatorDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "LastCounterOfferOriginatorDateTime");
            DropColumn("dbo.Offers", "LastOfferOriginatorDateTime");
        }
    }
}
