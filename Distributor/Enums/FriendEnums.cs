using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public class FriendEnums
    {
        public enum FriendStatusEnum
        {
            [Description("Requested")]
            [Display(Name = "Requested")]
            Requested = 0,

            [Description("Accepted")]
            [Display(Name = "Accepted")]
            Accepted = 1,

            [Description("Rejected")]
            [Display(Name = "Rejected")]
            Rejected = 2,

            [Description("Closed")]
            [Display(Name = "Closed")]
            Closed = 2
        }
    }
}