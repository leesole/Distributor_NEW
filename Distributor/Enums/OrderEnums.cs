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
        public enum OrderStatusEnum
        {
            [Description("New order")]
            [Display(Name = "New order")]
            New = 0,
            [Description("Despatched")]
            [Display(Name = "Despatched")]
            Despatched = 1,
            [Description("Delivered")]
            [Display(Name = "Delivered")]
            Delivered = 2,
            [Description("Collected")]
            [Display(Name = "Collected")]
            Collected = 3,
            [Description("Received")]
            [Display(Name = "Received")]
            Received = 4,
            [Description("Closed")]
            [Display(Name = "Closed")]
            Closed = 5
        }
    }
}