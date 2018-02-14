using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public static class OrderEnums
    {
        public enum OrderInStatusEnum
        {
            [Description("New order")]
            [Display(Name = "New order")]
            New = 0,
            [Description("Collected")]
            [Display(Name = "Collected")]
            Collected = 1,
            [Description("Received")]
            [Display(Name = "Received")]
            Received = 2,
            [Description("Closed")]
            [Display(Name = "Closed")]
            Closed = 3
        }

        public enum OrderOutStatusEnum
        {
            [Description("New order")]
            [Display(Name = "New order")]
            New = 0,
            [Description("Dispatched")]
            [Display(Name = "Dispatched")]
            Dispatched = 1,
            [Description("Delivered")]
            [Display(Name = "Delivered")]
            Delivered = 2,
            [Description("Closed")]
            [Display(Name = "Closed")]
            Closed = 3
        }
    }
}