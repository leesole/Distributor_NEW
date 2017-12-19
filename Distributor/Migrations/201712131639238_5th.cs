namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "EntityStatusChangeOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AppUsers", "EntityStatusChangeBy", c => c.Guid(nullable: false));
            AddColumn("dbo.AppUsers", "MaxDistanceFilter", c => c.Int());
            AddColumn("dbo.AppUsers", "MaxAgeFilter", c => c.Double());
            AddColumn("dbo.AppUsers", "SelectionLevelFilter", c => c.Int(nullable: false));
            AddColumn("dbo.AppUsers", "DisplayMyOrganisationListingsFilter", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "DisplayMyOrganisationListingsFilter");
            DropColumn("dbo.AppUsers", "SelectionLevelFilter");
            DropColumn("dbo.AppUsers", "MaxAgeFilter");
            DropColumn("dbo.AppUsers", "MaxDistanceFilter");
            DropColumn("dbo.AppUsers", "EntityStatusChangeBy");
            DropColumn("dbo.AppUsers", "EntityStatusChangeOn");
        }
    }
}
