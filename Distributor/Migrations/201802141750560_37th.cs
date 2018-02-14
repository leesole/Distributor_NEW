namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _37th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ItemDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ItemDescription");
        }
    }
}
