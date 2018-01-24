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
        public List<OfferManageViewOffersModel> OfferManageViewOffersCreated { get; set; }
        public List<OfferManageViewOffersModel> OfferManageViewOffersReceived { get; set; }
    }

    public class OfferManageViewOffersModel
    {
        public Guid OfferId { get; set; }

        public Guid ListingId { get; set; }

        [Display(Name = "Listing type")]
        public ListingTypeEnum ListingType { get; set; }

        [Display(Name = "Offer status")]
        public OfferStatusEnum OfferStatus { get; set; }

        [Display(Name = "Quantity outstanding")]
        public decimal QuantityOutstanding { get; set; }

        [Display(Name = "Offer quantity")]
        public decimal CurrentOfferQuantity { get; set; }  //set to 0 if rejected or returned

        [Display(Name = "Previous offer quantity")]
        public decimal? PreviousOfferQuantity { get; set; } //set this Current offer at time of currentofferqty entry so we have something to refer to if returned

        [Display(Name = "Counter offer quantity")]
        public decimal? CounterOfferQuantity { get; set; }  //if 'returned' status with a new offer, value is here

        [Display(Name = "Rejected?")]
        public bool Rejected { get; set; }
    }
}