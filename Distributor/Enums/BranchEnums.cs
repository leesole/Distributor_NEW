using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public static class BranchEnums
    {
        public enum BusinessTypeEnum
        {
            [Description("Caterer")]
            [Display(Name = "Caterer")]
            Caterer = 0,
            [Description("Charity")]
            [Display(Name = "Charity")]
            Charity = 1,
            [Description("Church")]
            [Display(Name = "Church")]
            Church = 2,
            [Description("Distributor")]
            [Display(Name = "Distributor")]
            Distributor = 3,
            [Description("Food bank")]
            [Display(Name = "Food bank")]
            FoodBank = 4,
            [Description("Hotelier/hostelry")]
            [Display(Name = "Hotelier/hostelry")]
            HotelierHostelry = 5,
            [Description("Producer")]
            [Display(Name = "Producer")]
            Producer = 6,
            [Description("Restaurant")]
            [Display(Name = "Restaurant")]
            Restaurant = 7,
            [Description("Retailer")]
            [Display(Name = "Retailer")]
            Retailer = 8,
            [Description("Supplier")]
            [Display(Name = "Supplier")]
            Supplier = 9,
            [Description("Takeaway")]
            [Display(Name = "Takeaway")]
            Takeaway = 10,
            [Description("Trader")]
            [Display(Name = "Trader")]
            Trader = 11,
            [Description("Other")]
            [Display(Name = "Other")]
            Other = 12
        }
    }
}