using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public class OfferEnums
    {
        public enum OfferStatusEnum
        {
            [Description("New offer/request")]
            [Display(Name = "New offer/request")]
            New = 0,

            [Description("Accepted")]
            [Display(Name = "Accepted")]
            Accepted = 1,

            [Description("Rejected")]
            [Display(Name = "Rejected")]
            Rejected = 2,

            [Description("Countered")]
            [Display(Name = "Countered")]
            Countered = 3,

            [Description("Re-offer/request")]
            [Display(Name = "Re-offer/request")]
            Reoffer = 4,

            [Description("Closed - no stock")]
            [Display(Name = "Closed - no stock")]
            ClosedNoStock = 5
        }
    }
}