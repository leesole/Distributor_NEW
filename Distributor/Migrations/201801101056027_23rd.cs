namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _23rd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "DisplayMyOrganisationListingsFilter", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "DisplayMyOrganisationListingsFilter");
        }
    }
}
