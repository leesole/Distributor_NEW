namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _26th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "ListingPrivacyLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "ListingPrivacyLevel");
        }
    }
}
