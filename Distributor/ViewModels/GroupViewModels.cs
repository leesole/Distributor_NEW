using Distributor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.GroupEnums;

namespace Distributor.ViewModels
{
    public class GroupViewModel
    {
        public Guid GroupId { get; set; }

        [Required]
        [Display(Name = "Group name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Type of group")]
        public LevelEnum Type { get; set; }

        [Required]
        [Display(Name = "Group visibility")]
        public GroupVisibilityEnum VisibilityLevel { get; set; }

        [Required]
        [Display(Name = "Group invite level")]
        public GroupInviteLevelEnum InviteLevel { get; set; }

        [Required]
        [Display(Name = "Group invite acceptance level")]
        public GroupInviteAcceptanceLevelEnum AcceptanceLevel { get; set; }

        [Required]
        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }

        public RecordChangeEnum RecordChange { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RecordChangeOn { get; set; }
        public AppUser RecordChangeBy { get; set; }

        //references to the listing originator
        public AppUser GroupOriginatorAppUser { get; set; }
        public Organisation GroupOriginatorOrganisation { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime GroupOriginatorDateTime { get; set; }

        public List<GroupMemberViewModel> GroupMembers { get; set; }
    }

    public class GroupMemberViewModel
    {
        public Guid GroupMemberId { get; set; }

        public Guid GroupId { get; set; }

        [Display(Name = "Organisation reference Id")]
        public Organisation OrganisationDetails { get; set; }

        [Display(Name = "Record added by")]
        public AppUser AddedBy { get; set; }

        [Display(Name = "Record added on")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedDateTime { get; set; }

        [Display(Name = "Group member status")]
        public GroupMemberStatusEnum Status { get; set; }

        public RecordChangeEnum RecordChange { get; set; }
        public DateTime RecordChangeOn { get; set; }
        public AppUser RecordChangeBy { get; set; }
    }

    public class GroupViewIndexModel
    {
        public List<GroupViewModel> GroupsCreatedByOrg { get; set; }

        public List<GroupViewModel> GroupsContainingOrg { get; set; }
    }
}