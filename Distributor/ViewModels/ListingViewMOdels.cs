using Distributor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.ItemEnums;

namespace Distributor.ViewModels
{
    #region Available Listings

    #region Managed Views

    public class AvailableListingManageViewModel
    {
        public Guid ListingId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity available")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Available to")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableTo { get; set; }

        [Display(Name = "Item condition")]
        public ItemConditionEnum ItemCondition { get; set; }

        [Display(Name = "Expiry date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Can deliver?")]
        public string DeliveryAvailable { get; set; }

        [Display(Name = "Listing status")]
        public ItemRequiredListingStatusEnum ListingStatus { get; set; }
    }

    public class AvailableListingManageHistoryViewModel
    {
        public Guid ListingId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Original quantity available")]
        public decimal QuantityAvailable { get; set; }

        [Display(Name = "Final quantity available")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Item condition")]
        public ItemConditionEnum ItemCondition { get; set; }

        [Display(Name = "Last updated")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RecordChangeOn { get; set; }

        [Display(Name = "Listing status")]
        public ItemRequiredListingStatusEnum ListingStatus { get; set; }
    }

    public class AvailableListingManageCreateViewModel
    {
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item category")]
        public ItemCategoryEnum ItemCategory { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity available")]
        public decimal QuantityAvailable { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Available from")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableFrom { get; set; }

        [Display(Name = "Available to")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableTo { get; set; }

        [Display(Name = "Item condition")]
        public ItemConditionEnum ItemCondition { get; set; }

        [Display(Name = "Earliest display-until date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DisplayUntilDate { get; set; }

        [Display(Name = "Earliest sell-by date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? SellByDate { get; set; }

        [Display(Name = "Earliest use-by date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UseByDate { get; set; }

        [Display(Name = "Can deliver?")]
        public bool DeliveryAvailable { get; set; }
    }

    public class AvailableListingDetailsViewModel : CallingFields
    {
        public string Breadcrumb { get; set; }  //Holds the breadcrumb list passed from the previous view

        public Dictionary<int, string> BreadcrumbDictionary { get; set; } //used to pass the build breadcrumb dictionary if 'RESET' button pressed as we have lost original details

        public bool HistoryRecord { get; set; }  //hold the History record flag passed from previous view


        public Guid ListingId { get; set; }

        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item category")]
        public ItemCategoryEnum ItemCategory { get; set; }

        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity available")]
        public decimal? QuantityAvailable { get; set; }

        [Display(Name = "Quantity reserved")]
        public decimal? QuantityFulfilled { get; set; }

        [Display(Name = "Quantity outstanding")]
        public decimal? QuantityOutstanding { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Available from")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableFrom { get; set; }

        [Display(Name = "Available to")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AvailableTo { get; set; }

        [Display(Name = "Item condition")]
        public ItemConditionEnum ItemCondition { get; set; }

        [Display(Name = "Earliest display-until date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DisplayUntilDate { get; set; }

        [Display(Name = "Earliest sell-by date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? SellByDate { get; set; }

        [Display(Name = "Earliest use-by date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UseByDate { get; set; }

        [Display(Name = "Can deliver?")]
        public bool DeliveryAvailable { get; set; }

        [Display(Name = "Listing status")]
        public ItemRequiredListingStatusEnum ListingStatus { get; set; }

        [Display(Name = "Listing location")]
        public string ListingOrganisationPostcode { get; set; }  //Put here for quicker sorting in view screens  //LSLSLS Need to update this if changed by Admin


        [Display(Name = "Change")]
        public RecordChangeEnum RecordChange { get; set; }
        [Display(Name = "Date changed")]
        public DateTime RecordChangeOn { get; set; }
        [Display(Name = "Changed by")]
        public string RecordChangeByName { get; set; }
        [Display(Name = "Changed by (email)")]
        public string RecordChangeByEmail { get; set; }

        //references to the listing originator
        [Display(Name = "Created by")]
        public string ListingOriginatorAppUserName { get; set; }
        [Display(Name = "Created by (email)")]
        public string ListingOriginatorAppUserEmail { get; set; }
        [Display(Name = "Date created")]
        public DateTime ListingOriginatorDateTime { get; set; }
    }

    #endregion

    #region General Info Views
    #endregion

    #endregion

    #region RequiredListings

    #region Managed Views

    public class RequiredListingManageViewModel
    {
        public Guid ListingId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity available")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Required to")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RequiredTo { get; set; }

        [Display(Name = "Accept damaged items?")]
        public bool AcceptDamagedItems { get; set; }

        [Display(Name = "Accept out-of-date items?")]
        public bool AcceptOutOfDateItems { get; set; }

        [Display(Name = "Can collect?")]
        public bool CollectionAvailable { get; set; }

        [Display(Name = "Listing status")]
        public ItemRequiredListingStatusEnum ListingStatus { get; set; }
    }

    public class RequiredListingManageHistoryViewModel
    {
        public Guid ListingId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity required")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "Final quantity needed")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Last updated")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RecordChangeOn { get; set; }

        [Display(Name = "Listing status")]
        public ItemRequiredListingStatusEnum ListingStatus { get; set; }
    }

    public class RequiredListingManageCreateViewModel
    {
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item category")]
        public ItemCategoryEnum ItemCategory { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity required")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Required from")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RequiredFrom { get; set; }

        [Display(Name = "Required to")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RequiredTo { get; set; }

        [Display(Name = "Accept damaged items?")]
        public bool AcceptDamagedItems { get; set; }

        [Display(Name = "Accept out-of-date items?")]
        public bool AcceptOutOfDateItems { get; set; }

        [Display(Name = "Can collect?")]
        public bool CollectionAvailable { get; set; }
    }

    public class RequiredListingDetailsViewModel : CallingFields
    {
        public string Breadcrumb { get; set; }  //Holds the breadcrumb list passed from the previous view

        public Dictionary<int, string> BreadcrumbDictionary { get; set; } //used to pass the build breadcrumb dictionary if 'RESET' button pressed as we have lost original details

        public bool HistoryRecord { get; set; }  //hold the History record flag passed from previous view


        public Guid ListingId { get; set; }

        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item category")]
        public ItemCategoryEnum ItemCategory { get; set; }

        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Quantity required")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "Quantity fulfilled")]
        public decimal QuantityFulfilled { get; set; }

        [Display(Name = "Quantity needed")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Required from")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RequiredFrom { get; set; }

        [Display(Name = "Required to")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RequiredTo { get; set; }

        [Display(Name = "Accept damaged items?")]
        public bool AcceptDamagedItems { get; set; }

        [Display(Name = "Accept out-of-date items?")]
        public bool AcceptOutOfDateItems { get; set; }

        [Display(Name = "Can collect?")]
        public bool CollectionAvailable { get; set; }

        [Display(Name = "Listing status")]
        public ItemRequiredListingStatusEnum ListingStatus { get; set; }

        [Display(Name = "Listing location")]
        public string ListingOrganisationPostcode { get; set; }  //Put here for quicker sorting in view screens  //LSLSLS Need to update this if changed by Admin


        [Display(Name = "Change")]
        public RecordChangeEnum RecordChange { get; set; }
        [Display(Name = "Date changed")]
        public DateTime RecordChangeOn { get; set; }
        [Display(Name = "Changed by")]
        public string RecordChangeByName { get; set; }
        [Display(Name = "Changed by (email)")]
        public string RecordChangeByEmail { get; set; }

        //references to the listing originator
        [Display(Name = "Created by")]
        public string ListingOriginatorAppUserName { get; set; }
        [Display(Name = "Created by (email)")]
        public string ListingOriginatorAppUserEmail { get; set; }
        [Display(Name = "Date created")]
        public DateTime ListingOriginatorDateTime { get; set; }
    }

    #endregion

    #region General Info Views
    #endregion

    #endregion
}