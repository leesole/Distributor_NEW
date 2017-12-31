using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public static class EntityEnums
    {
        public enum EntityStatusEnum
        {
            [Description("Inactive")]
            [Display(Name = "Inactive")]
            Inactive = 0,
            [Description("Active")]
            [Display(Name = "Active")]
            Active = 1,
            [Description("On hold")]
            [Display(Name = "On hold")]
            OnHold = 2,
            [Description("Awaiting organisation details")]
            [Display(Name = "Awaiting organisation details")]
            AwaitingOrganisationDetails = 3,
            [Description("Rejected")]
            [Display(Name = "Rejected")]
            Rejected = 4,
            [Description("Closed")]
            [Display(Name = "Closed")]
            Closed = 5,
            [Description("Removed")]
            [Display(Name = "Removed")]
            Removed = 6,
            [Description("Password reset required")]
            [Display(Name = "Password reset required")]
            PasswordResetRequired = 7
        }
    }
}