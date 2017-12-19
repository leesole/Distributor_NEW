namespace Distributor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        AppUserId = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EntityStatus = c.Int(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        LoginEmail = c.String(nullable: false),
                        PrivacyLevel = c.Int(nullable: false),
                        AdminUser = c.Boolean(nullable: false),
                        SuperUser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AppUserId);
            
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        GroupMemberId = c.Guid(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        OrganisationId = c.Guid(nullable: false),
                        AddedBy = c.Guid(nullable: false),
                        AddedDateTime = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupMemberId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        VisibilityLevel = c.Int(nullable: false),
                        InviteLevel = c.Int(nullable: false),
                        AcceptanceLevel = c.Int(nullable: false),
                        GroupOriginatorAppUserId = c.Guid(nullable: false),
                        GroupOriginatorOrganisationId = c.Guid(nullable: false),
                        GroupOriginatorDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        OrganisationId = c.Guid(nullable: false),
                        OrganisationName = c.String(nullable: false),
                        BusinessType = c.Int(nullable: false),
                        AddressLine1 = c.String(nullable: false),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressTownCity = c.String(nullable: false),
                        AddressCounty = c.String(),
                        AddressPostcode = c.String(nullable: false),
                        TelephoneNumber = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        ContactName = c.String(nullable: false),
                        CompanyRegistrationDetails = c.String(),
                        CharityRegistrationDetails = c.String(),
                        VATRegistrationDetails = c.String(),
                        PrivacyLevel = c.Int(nullable: false),
                        EntityStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrganisationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Organisations");
            DropTable("dbo.Groups");
            DropTable("dbo.GroupMembers");
            DropTable("dbo.AppUsers");
        }
    }
}
