namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _32nd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderInStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "OrderOutStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "OrderStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "OrderStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "OrderOutStatus");
            DropColumn("dbo.Orders", "OrderInStatus");
        }
    }
}
