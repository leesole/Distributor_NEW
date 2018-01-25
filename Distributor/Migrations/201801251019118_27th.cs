namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _27th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "PreviousCounterOfferQuantity", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "PreviousCounterOfferQuantity");
        }
    }
}
