using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public class UserActionEnums
    {
        public enum ActionTypeEnum
        {
            [Description("New offer received")]
            [Display(Name = "New offer received")]
            NewOfferReceived = 0,

            [Description("Counter offer received")]
            [Display(Name = "Counter offer received")]
            CounterOfferReceived = 1,

            [Description("New order received")]
            [Display(Name = "New order received")]
            NewOrderReceived = 0,
        }
    }
}