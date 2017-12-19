using Distributor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.UserTaskEnums;

namespace Distributor.ViewModels
{
    public class UserTasksViewModel
    {
        public Guid UserTaskId { get; set; }
        
        [Display(Name = "Task type")]
        public TaskTypeEnum TaskType { get; set; }

        [Display(Name = "Description")]
        public string TaskDescription { get; set; }
        
        public AppUser AppUser { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ChangedOn { get; set; }
    }
}