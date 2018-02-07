using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.ItemEnums;
using static Distributor.Enums.OrderEnums;

namespace Distributor.ViewModels
{
    public class OrderManageViewModel
    {
        public List<OrderInViewModel> OrdersInViewModel { get; set; }
        public List<OrderOutViewModel> OrdersOutViewModel { get; set; }
    }

    public class OrderInViewModel
    {
        public Guid OrderId { get; set; }

        [Display(Name = "Order quantity")]
        public decimal OrderQuanity { get; set; }

        [Display(Name = "Order status")]
        public OrderInStatusEnum OrderInStatus { get; set; }

        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderCreationDateTime { get; set; }
        
        [Display(Name = "Collection date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderCollectedDateTime { get; set; }
        public bool OrderCollected { get; set; }

        [Display(Name = "Received date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderReceivedDateTime { get; set; }
        public bool OrderReceived { get; set; }

        [Display(Name = "Closed date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderInClosedDateTime { get; set; }
        public bool OrderInClosed { get; set; }
    }

    public class OrderOutViewModel
    {
        public Guid OrderId { get; set; }

        [Display(Name = "Order quantity")]
        public decimal OrderQuanity { get; set; }

        [Display(Name = "Order status")]
        public OrderOutStatusEnum OrderOutStatus { get; set; }

        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderCreationDateTime { get; set; }

        [Display(Name = "Distribution date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDistributionDateTime { get; set; }
        public bool OrderDistributed { get; set; }

        [Display(Name = "Delivered date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDeliveredDateTime { get; set; }
        public bool OrderDelivered { get; set; }

        [Display(Name = "Closed date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderOutClosedDateTime { get; set; }
        public bool OrderOutClosed { get; set; }
    }

    public class OrderViewModel : CallingFields
    {
        public bool DisplayOnly { get; set; }

        public string Breadcrumb { get; set; }  //Holds the breadcrumb list passed from the previous view

        public Dictionary<int, string> BreadcrumbDictionary { get; set; } //used to pass the build breadcrumb dictionary if 'RESET' button pressed as we have lost original details

        public string Type { get; set; }

        public Guid OrderId { get; set; }

        [Display(Name = "Listing type")]
        public ListingTypeEnum ListingType { get; set; }

        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Unit of measure")]
        public string UoM { get; set; }

        [Display(Name = "Order quantity")]
        public decimal OrderQuanity { get; set; }

        [Display(Name = "Order status")]
        public OrderInStatusEnum OrderInStatus { get; set; }

        [Display(Name = "Order status")]
        public OrderOutStatusEnum OrderOutStatus { get; set; }

        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderCreationDateTime { get; set; }

        [Display(Name = "Distribution date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDistributionDateTime { get; set; }
        public string OrderDistributedBy { get; set; }

        [Display(Name = "Delivered date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDeliveredDateTime { get; set; }
        public string OrderDeliveredBy { get; set; }


        [Display(Name = "Collection date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderCollectedDateTime { get; set; }
        public string OrderCollectedBy { get; set; }

        [Display(Name = "Received date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderReceivedDateTime { get; set; }
        public string OrderReceivedBy { get; set; }

        [Display(Name = "Closed date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderInClosedDateTime { get; set; }
        public string OrderInClosedBy { get; set; }

        [Display(Name = "Closed date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderOutClosedDateTime { get; set; }
        public string OrderOutClosedBy { get; set; }

        public string OrderOriginatorAppUser { get; set; }
        public string OrderOriginatorOrganisation { get; set; }
        public DateTime? OrderOriginatorDateTime { get; set; }


        //Reference keys
        //references to the offer originator
        public Guid? OfferId { get; set; }
        public string OfferOriginatorAppUser { get; set; }
        public string OfferOriginatorOrganisation { get; set; }

        //references to the listing originator
        public Guid? ListingId { get; set; }
        public string ListingOriginatorAppUser { get; set; }
        public string ListingOriginatorOrganisation { get; set; }
    }
}