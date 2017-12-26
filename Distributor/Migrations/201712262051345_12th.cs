namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "GroupPrivacyLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "GroupPrivacyLevel");
        }
    }
}
