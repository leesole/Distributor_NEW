using System;
using System.ComponentModel.DataAnnotations;
using static Distributor.Enums.BranchEnums;
using static Distributor.Enums.GeneralEnums;

namespace Distributor.ViewModels
{
    public class HomeOrganisationDetailsView
    {
        [Required]
        public Guid AppUserId { get; set; }

        [Display(Name = "Selected organisation")]
        public Guid? SelectedOrganisationId { get; set; }

        [Required]
        [Display(Name = "Organisation name")]
        public string OrganisationName { get; set; }

        [Required]
        [Display(Name = "Business type")]
        public BusinessTypeEnum BusinessType { get; set; }

        [Required]
        [Display(Name = "Address line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Address line 3")]
        public string AddressLine3 { get; set; }

        [Required]
        [Display(Name = "Address town/city")]
        public string AddressTownCity { get; set; }

        [Display(Name = "Address county")]
        public string AddressCounty { get; set; }

        [Required]
        [Display(Name = "Address postcode")]
        public string AddressPostcode { get; set; }

        [Required]
        [Display(Name = "Telephone number")]
        public string TelephoneNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Website")]
        public string Website { get; set; }

        [Required]
        [Display(Name = "Contact name")]
        public string ContactName { get; set; }

        [Display(Name = "Company registration details")]
        public string CompanyRegistrationDetails { get; set; }

        [Display(Name = "Charity registration details")]
        public string CharityRegistrationDetails { get; set; }

        [Display(Name = "VAT registration details")]
        public string VATRegistrationDetails { get; set; }

        [Required]
        [Display(Name = "Privacy level")]
        public PrivacyLevelEnum PrivacyLevel { get; set; }

        [Required]
        [Display(Name = "'Group' privacy level")]
        public PrivacyLevelEnum GroupPrivacyLevel { get; set; }
    }
}