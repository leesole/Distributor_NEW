namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _35th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "AcceptedBy", c => c.Guid());
            AddColumn("dbo.Offers", "AcceptedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "AcceptedOn");
            DropColumn("dbo.Offers", "AcceptedBy");
        }
    }
}
