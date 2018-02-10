using Distributor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.UserNotificationEnums;

namespace Distributor.ViewModels
{
    public class NotificationViewModel
    {
        public Guid NotificationId { get; set; }

        [Display(Name = "Notification type")]
        public NotificationTypeEnum NotificationType { get; set; }

        [Display(Name = "Description")]
        public string NotificationDescription { get; set; }

        [Display(Name = "Reference info")]
        public string ReferenceInformation { get; set; }

        public AppUser AppUser { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ChangedOn { get; set; }

        [Display(Name = "Changed by")]
        public string ChangedBy { get; set; }
    }
}