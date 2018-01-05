using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.ItemEnums;

namespace Distributor.Models
{
    public class AvailableListing
    {
        [Key]
        public Guid ListingId { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Item category")]
        public ItemCategoryEnum ItemCategory { get; set; }

        [Required]
        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Required]
        [Display(Name = "Quantity available")]
        public decimal QuantityAvailable { get; set; }

        [Display(Name = "Quantity reserved")]
        public decimal QuantityFulfilled { get; set; }

        [Display(Name = "Quantity available")]
        public decimal QuantityOutstanding { get; set; }

        [Required]
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


        public RecordChangeEnum RecordChange { get; set; }
        public DateTime RecordChangeOn { get; set; }
        public Guid RecordChangeBy { get; set; }

        //references to the listing originator
        public Guid ListingOriginatorAppUserId { get; set; }
        public Guid ListingOriginatorOrganisationId { get; set; }
        public DateTime ListingOriginatorDateTime { get; set; }
    }

    public class RequiredListing
    {
        [Key]
        public Guid ListingId { get; set; }

        [Required]
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
        public string ListingOrganisationPostcode { get; set; }  //Put here for quicker sorting in view screens

        //references to the listing originator
        public Guid ListingOriginatorAppUserId { get; set; }
        public Guid ListingOriginatorOrganisationId { get; set; }
        public DateTime ListingOriginatorDateTime { get; set; }

        //other references
        public Guid? CampaignId { get; set; }
    }
}