﻿using System;
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

    public class OfferViewModel
    {
        public bool DisplayOnly { get; set; }

        public string Type { get; set; }

        public Guid OfferId { get; set; }

        public Guid ListingId { get; set; }

        [Display(Name = "Listing type")]
        public ListingTypeEnum ListingType { get; set; }

        [Display(Name = "Offer status")]
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

        public string RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }

        //references to the offer originator
        public string OfferOriginatorAppUser { get; set; }
        public string OfferOriginatorOrganisation { get; set; }
        public DateTime OfferOriginatorDateTime { get; set; }
        public string LastOfferOriginatorAppUser { get; set; }
        public DateTime? LastOfferOriginatorDateTime { get; set; }

        public string ListingOriginatorAppUser { get; set; }
        public string ListingOriginatorOrganisation { get; set; }
        public DateTime? ListingOriginatorDateTime { get; set; }

        public string CounterOfferOriginatorAppUser { get; set; }
        public string CounterOfferOriginatorOrganisation { get; set; }
        public DateTime? CounterOfferOriginatorDateTime { get; set; }
        public string LastCounterOfferOriginatorAppUser { get; set; }
        public DateTime? LastCounterOfferOriginatorDateTime { get; set; }

        public Guid? OrderId { get; set; }  //if this offer is accepted then an order is made and this is the reference to that order.
        public string OrderOriginatorAppUser { get; set; }
        public string OrderOriginatorOrganisation { get; set; }
        public DateTime? OrderOriginatorDateTime { get; set; }
    }
}