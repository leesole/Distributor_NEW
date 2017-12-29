using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.GroupEnums;

namespace Distributor.Models
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }

        [Required]
        [Display(Name = "Group name")]
        public string Name { get; set; }

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
        public DateTime RecordChangeOn { get; set; }
        public Guid RecordChangeBy { get; set; }

        //references to the listing originator
        [Required]
        public Guid GroupOriginatorAppUserId { get; set; }
        [Required]
        public Guid GroupOriginatorOrganisationId { get; set; }
        [Required]
        public DateTime GroupOriginatorDateTime { get; set; }
    }

    public class GroupMember
    {
        [Key]
        public Guid GroupMemberId { get; set; }

        public Guid GroupId { get; set; }

        [Display(Name = "Organisation reference Id")]
        public Guid OrganisationId { get; set; }

        [Display(Name = "Record added by")]
        public Guid AddedBy { get; set; }

        [Display(Name = "Record added on")]
        public DateTime AddedDateTime { get; set; }

        [Required]
        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }

        [Display(Name = "Group member status")]
        public GroupMemberStatusEnum Status { get; set; }

        public RecordChangeEnum RecordChange { get; set; }
        public DateTime RecordChangeOn { get; set; }
        public Guid RecordChangeBy { get; set; }
    }
}