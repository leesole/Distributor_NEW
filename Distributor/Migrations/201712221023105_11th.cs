namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _11th : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Groups", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "Type", c => c.Int(nullable: false));
        }
    }
}
