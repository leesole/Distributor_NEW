using System;
using System.ComponentModel.DataAnnotations;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserEnums;

namespace Distributor.Models
{
    public class AppUser
    {
        [Key]
        public Guid AppUserId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }

        [Required]
        [Display(Name = "Current organisation")]
        public Guid OrganisationId { get; set; }

        [Required]
        [Display(Name = "Login email")]
        public string LoginEmail { get; set; }

        [Required]
        [Display(Name = "Privacy level")]
        public PrivacyLevelEnum PrivacyLevel { get; set; }

        [Required]
        [Display(Name = "User role")]
        public UserRoleEnum UserRole { get; set; }

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


        public RecordChangeEnum RecordChange { get; set; }
        public DateTime RecordChangeOn { get; set; }
        public Guid RecordChangeBy { get; set; }
    }
}