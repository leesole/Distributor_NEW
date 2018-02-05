using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
        public DateTime? OrderClosedDateTime { get; set; }
        public bool OrderClosed { get; set; }
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
        public DateTime? OrderClosedDateTime { get; set; }
        public bool OrderClosed { get; set; }
    }
}