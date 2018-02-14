namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _36th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "ClosedNoStockBy", c => c.Guid());
            AddColumn("dbo.Offers", "ClosedNoStockOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "ClosedNoStockOn");
            DropColumn("dbo.Offers", "ClosedNoStockBy");
        }
    }
}
