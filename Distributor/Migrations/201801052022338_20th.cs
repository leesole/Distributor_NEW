namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20th : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AvailableListings", "UoM", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AvailableListings", "UoM", c => c.String());
        }
    }
}
