using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.BranchEnums;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserEnums;

namespace Distributor.ViewModels
{
    public class AppUserProfileView
    {
        public Guid AppUserId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }

        [Required]
        [Display(Name = "Login email")]
        public string LoginEmail { get; set; }

        [Required]
        [Display(Name = "Privacy level")]
        public PrivacyLevelEnum PrivacyLevel { get; set; }

        [Required]
        [Display(Name = "User role")]
        public UserRoleEnum UserRole { get; set; }

        [Display(Name = "Selected organisation")]
        public Guid? SelectedOrganisationId { get; set; }

        [Display(Name = "Organisation name")]
        public string OrganisationName { get; set; }

        [Display(Name = "Business type")]
        public BusinessTypeEnum BusinessType { get; set; }

        [Display(Name = "Address line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Address line 3")]
        public string AddressLine3 { get; set; }

        [Display(Name = "Address town/city")]
        public string AddressTownCity { get; set; }

        [Display(Name = "Address county")]
        public string AddressCounty { get; set; }

        [Display(Name = "Address postcode")]
        public string AddressPostcode { get; set; }
    }

    public class AppUserSettingsView
    {
        public Guid AppUserId { get; set; }

        //SETTINGS - Listings dafaults
        [Display(Name = "Max distance filter (miles)")]
        public int? MaxDistanceFilter { get; set; }

        [Display(Name = "Max age filter (days)")]
        public double? MaxAgeFilter { get; set; }

        [Required]
        [Display(Name = "Selection level filter")]
        public ExternalSearchLevelEnum SelectionLevelFilter { get; set; }

        [Required]
        [Display(Name = "Display my organisation listings filter")]
        public bool DisplayMyOrganisationListingsFilter { get; set; }

        //SETTINGS - Authorisation Levels  - LSLSLS Might not use, might just only allow ADMIN
    }
}