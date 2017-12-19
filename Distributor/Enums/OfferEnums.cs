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
            [Description("New offer")]
            [Display(Name = "New offer")]
            New = 0,

            [Description("Accepted")]
            [Display(Name = "Accepted")]
            Accepted = 1,

            [Description("Rejected")]
            [Display(Name = "Rejected")]
            Rejected = 2,

            [Description("Countered")]
            [Display(Name = "Countered")]
            Countered = 3
        }
    }
}