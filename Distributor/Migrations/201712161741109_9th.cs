namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "Website", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "Website");
        }
    }
}
