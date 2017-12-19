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
            [Description("Awaiting friend request")]
            [Display(Name = "Awaiting friend request")]
            AwaitFriendRequest = 0,
            [Description("Awaiting group member acceptance")]
            [Display(Name = "Awaiting group member acceptance")]
            AwaitGroupMemberAcceptance = 1
        }
    }
}