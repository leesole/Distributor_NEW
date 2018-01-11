namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _22nd : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AppUsers", "DisplayMyOrganisationListingsFilter");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AppUsers", "DisplayMyOrganisationListingsFilter", c => c.Boolean(nullable: false));
        }
    }
}
