using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserEnums;
using static Distributor.Enums.UserTaskEnums;

namespace Distributor.Models
{
    public class UserTask
    {
        [Key]
        public Guid UserTaskId { get; set; }

        [Required]
        [Display(Name = "Task type")]
        public TaskTypeEnum TaskType { get; set; }

        [Display(Name = "Description")]
        public string TaskDescription { get; set; }

        public Guid ReferenceKey { get; set; }  //this is used to hold the key of the reference file - i.e. AppUserId for user-on-hold etc

        public string ReferenceEmail { get; set; }

        public Guid OrganisationId { get; set; }  //organisation task is assigned to (all tasks are currently Org level)

        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }
        public RecordChangeEnum RecordChange { get; set; }
        public DateTime RecordChangeOn { get; set; }
        public Guid RecordChangeBy { get; set; }
    }
}