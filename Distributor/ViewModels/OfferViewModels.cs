using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.OfferEnums;

namespace Distributor.ViewModels
{
    public class OfferManageViewModel
    {
        public bool EditableEntriesCreated { get; set; }
        public List<OfferManageViewOffersModel> OfferManageViewOffersCreated { get; set; }
        public bool EditableEntriesReceived { get; set; }
        public List<OfferManageViewOffersModel> OfferManageViewOffersReceived { get; set; }
    }

    public class OfferManageViewOffersModel
    {
        public Guid OfferId { get; set; }

        public Guid ListingId { get; set; }

        [Display(Name = "Listing type")]
        public ListingTypeEnum ListingType { get; set; }

        [Display(Name = "Listing organisation")]
        public string ListingOrganisation { get; set; }

        [Display(Name = "Offer status")]
        public OfferStatusEnum OfferStatus { get; set; }

        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Quantity outstanding")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Offer quantity")]
        public decimal? CurrentOfferQuantity { get; set; }  //set to 0 if rejected or returned

        [Display(Name = "Previous offer quantity")]
        public decimal? PreviousOfferQuantity { get; set; } //set this Current offer at time of currentofferqty entry so we have something to refer to if returned

        [Display(Name = "Counter offer quantity")]
        public decimal? CounterOfferQuantity { get; set; }  //if 'returned' status with a new offer, value is here

        [Display(Name = "Previous counter offer quantity")]
        public decimal? PreviousCounterOfferQuantity { get; set; }  //if 'returned' status with a new offer, value is here

        [Display(Name = "Rejected?")]
        public bool Rejected { get; set; }  //used in the history list view

        [Display(Name = "Order created?")]
        public bool OrderCreated { get; set; }  //set to true if an order was created from this offer (used for History screens)

        public Guid? OrderId { get; set; }

        public bool EditableQuantity { get; set; }  //set to true if this is a value that can be changed in the 'offer' screen - allows identification later to validate or not the offer
    }

    public class OfferViewModel : CallingFields
    {
        public bool DisplayOnly { get; set; }

        public string Breadcrumb { get; set; }  //Holds the breadcrumb list passed from the previous view

        public Dictionary<int, string> BreadcrumbDictionary { get; set; } //used to pass the build breadcrumb dictionary if 'RESET' button pressed as we have lost original details

        public string Type { get; set; }

        public bool EditableQuantity { get; set; }

        public Guid OfferId { get; set; }

        public Guid ListingId { get; set; }

        [Display(Name = "Listing type")]
        public ListingTypeEnum ListingType { get; set; }

        [Display(Name = "Status")]
        public OfferStatusEnum OfferStatus { get; set; }

        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Quantity outstanding")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Offer quantity")]
        public decimal CurrentOfferQuantity { get; set; }  //set to 0 if rejected or returned

        [Display(Name = "Previous offer quantity")]
        public decimal? PreviousOfferQuantity { get; set; } //set this Current offer at time of currentofferqty entry so we have something to refer to if returned

        [Display(Name = "Counter offer quantity")]
        public decimal? CounterOfferQuantity { get; set; }  //if 'returned' status with a new offer, value is here

        [Display(Name = "Previous counter offer quantity")]
        public decimal? PreviousCounterOfferQuantity { get; set; }  //if 'returned' status with a new offer, value is here

        [Display(Name = "Rejected by")]
        public string RejectedBy { get; set; }
        [Display(Name = "Rejected on")]
        public DateTime? RejectedOn { get; set; }

        //references to the offer originator
        [Display(Name = "User")]
        public string OfferOriginatorAppUser { get; set; }
        [Display(Name = "Organisation")]
        public string OfferOriginatorOrganisation { get; set; }
        [Display(Name = "Date")]
        public DateTime OfferOriginatorDateTime { get; set; }
        [Display(Name = "User")]
        public string LastOfferOriginatorAppUser { get; set; }
        [Display(Name = "Date")]
        public DateTime? LastOfferOriginatorDateTime { get; set; }

        [Display(Name = "User")]
        public string ListingOriginatorAppUser { get; set; }
        [Display(Name = "Organisation")]
        public string ListingOriginatorOrganisation { get; set; }
        [Display(Name = "Date")]
        public DateTime? ListingOriginatorDateTime { get; set; }

        [Display(Name = "User")]
        public string CounterOfferOriginatorAppUser { get; set; }
        [Display(Name = "Organisation")]
        public string CounterOfferOriginatorOrganisation { get; set; }
        [Display(Name = "Date")]
        public DateTime? CounterOfferOriginatorDateTime { get; set; }
        [Display(Name = "User")]
        public string LastCounterOfferOriginatorAppUser { get; set; }
        [Display(Name = "Date")]
        public DateTime? LastCounterOfferOriginatorDateTime { get; set; }

        public Guid? OrderId { get; set; }  //if this offer is accepted then an order is made and this is the reference to that order.
        [Display(Name = "User")]
        public string OrderOriginatorAppUser { get; set; }
        [Display(Name = "Organisation")]
        public string OrderOriginatorOrganisation { get; set; }
        [Display(Name = "Date")]
        public DateTime? OrderOriginatorDateTime { get; set; }
    }
}