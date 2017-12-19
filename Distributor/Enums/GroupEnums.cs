using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public static class GroupEnums
    {
        //Groups are private at present so this is not used
        public enum GroupVisibilityEnum
        {
            [Description("Private")]
            [Display(Name = "Private")]
            Private = 0,
            [Description("Public")]
            [Display(Name = "Public")]
            Public = 1
        }

        //Level of who can invite someone to the group
        public enum GroupInviteLevelEnum
        {
            [Description("Group owner")]
            [Display(Name = "Group owner")]
            Owner = 0,
            [Description("Group member")]
            [Display(Name = "Group member")]
            Member = 1
        }

        //Level of acceptance steps for new users
        public enum GroupInviteAcceptanceLevelEnum
        {
            [Description("Automatic acceptance")]
            [Display(Name = "Automatic acceptance")]
            Automatic = 0,
            [Description("Group member acceptance")]
            [Display(Name = "Group member acceptance")]
            Member = 1,
            [Description("Group invitee acceptance")]
            [Display(Name = "Group invitee acceptance")]
            Invitee = 2,
            [Description("Group owner acceptance")]
            [Display(Name = "Group owner acceptance")]
            Owner = 3
        }

        public enum GroupMemberStatusEnum
        {
            [Description("Awaiting acceptance")]
            [Display(Name = "Awaiting acceptance")]
            Awaiting = 0,
            [Description("Accepted")]
            [Display(Name = "Accepted")]
            Accepted = 1,
            [Description("Rejected")]
            [Display(Name = "Rejected")]
            Rejected = 2
        }
    }
}