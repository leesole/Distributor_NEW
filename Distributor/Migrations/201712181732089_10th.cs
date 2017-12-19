namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTasks", "ReferenceEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTasks", "ReferenceEmail");
        }
    }
}
