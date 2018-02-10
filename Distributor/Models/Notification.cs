using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserNotificationEnums;

namespace Distributor.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }

        [Required]
        [Display(Name = "Notification type")]
        public NotificationTypeEnum NotificationType { get; set; }

        [Display(Name = "Description")]
        public string NotificationDescription { get; set; }

        public Guid ReferenceKey { get; set; }  //this is used to hold the key of the reference file - i.e. OfferId for offers etc

        public Guid? AppUserId { get; set; }  //the AppUserId that this Notification is for (if any)

        public Guid OrganisationId { get; set; }  //the Organisation that this Notification is for

        [Display(Name = "Status")]
        public EntityStatusEnum EntityStatus { get; set; }
        public RecordChangeEnum RecordChange { get; set; }
        public DateTime RecordChangeOn { get; set; }
        public Guid RecordChangeBy { get; set; }
    }
}