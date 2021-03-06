﻿using System;
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
    public class UserAdminView
    {
        public List<UserAdminDetailsView> UserAdminActiveView { get; set; }

        public List<UserAdminDetailsView> UserAdminPasswordChangeView { get; set; }

        public List<UserAdminDetailsView> UserAdminNonActiveView { get; set; }
    }
    public class UserAdminDetailsView
    {
        public Guid AppUserId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Login email")]
        public string LoginEmail { get; set; }

        [Required]
        [Display(Name = "Privacy level")]
        public PrivacyLevelEnum PrivacyLevel { get; set; }

        [Required]
        [Display(Name = "User role")]
        public UserRoleEnum UserRole { get; set; }

        [Required]
        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }
    }

    public class OrganisationAdminView
    {
        public Guid OrganisationId { get; set; }

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
        [Display(Name = "'General Info' listing privacy level")]
        public LevelEnum ListingPrivacyLevel { get; set; }

        [Required]
        [Display(Name = "Privacy level")]
        public PrivacyLevelEnum PrivacyLevel { get; set; }

        [Required]
        [Display(Name = "'Group' privacy level")]
        public PrivacyLevelEnum GroupPrivacyLevel { get; set; }
    }


    public class UserAdminAddUserView
    {
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Login email")]
        public string LoginEmail { get; set; }

        [Display(Name = "Login password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Re-enter password")]
        [Compare("LoginPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Privacy level")]
        public PrivacyLevelEnum PrivacyLevel { get; set; }

        [Display(Name = "User role")]
        public UserRoleEnum UserRole { get; set; }
    }
}