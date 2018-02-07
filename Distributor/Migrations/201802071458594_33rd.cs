namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _33rd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderInClosedDateTime", c => c.DateTime());
            AddColumn("dbo.Orders", "OrderInClosedBy", c => c.Guid());
            AddColumn("dbo.Orders", "OrderOutClosedDateTime", c => c.DateTime());
            AddColumn("dbo.Orders", "OrderOutClosedBy", c => c.Guid());
            DropColumn("dbo.Orders", "OrderClosedDateTime");
            DropColumn("dbo.Orders", "OrderClosedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "OrderClosedBy", c => c.Guid());
            AddColumn("dbo.Orders", "OrderClosedDateTime", c => c.DateTime());
            DropColumn("dbo.Orders", "OrderOutClosedBy");
            DropColumn("dbo.Orders", "OrderOutClosedDateTime");
            DropColumn("dbo.Orders", "OrderInClosedBy");
            DropColumn("dbo.Orders", "OrderInClosedDateTime");
        }
    }
}
