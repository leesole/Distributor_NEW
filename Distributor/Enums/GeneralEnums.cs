using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributor.Enums
{
    public static class GeneralEnums
    {
        public enum RecordChangeEnum
        {
            [Description("New record added")]
            [Display(Name = "New record added")]
            NewRecord = 0,

            [Description("Record details updated")]
            [Display(Name = "Record details updated")]
            RecordUpdated = 1,

            [Description("Entity status change")]
            [Display(Name = "Entity status change")]
            StatusChange = 2,

            [Description("Listing status change")]
            [Display(Name = "Listing status change")]
            ListingStatusChange = 3
        }



        public enum ListingTypeEnum
        {
            [Description("Requirement listing")]
            [Display(Name = "Requirement listing")]
            Requirement = 0,

            [Description("Available listing")]
            [Display(Name = "Available listing")]
            Available = 1
        }

        /// <summary>
        /// Internal Search levels to be used for selecting records for ManageView and 'my' information
        /// </summary>
        public enum InternalSearchLevelEnum
        {
            [Description("User")]
            [Display(Name = "User")]
            User = 0,

            [Description("Organisation")]
            [Display(Name = "Organisation")]
            Organisation = 1,

            [Description("Group")]
            [Display(Name = "Group")]  //These are user built closed groups
            Group = 2
        }

        /// <summary>
        /// External Search levels to be used for selecting records for GeneralInfo and other user listing information
        /// </summary>
        public enum ExternalSearchLevelEnum
        {
            [Description("All")]
            [Display(Name = "All")]
            All = 0,

            [Description("Organisation")]
            [Display(Name = "Organisation")]
            Organisation = 1,

            [Description("Group")]
            [Display(Name = "Group")]  //These are user built closed groups
            Group = 2
        }

        /// <summary>
        /// The Level of which an ID may refer, i.e. User = AppUserId
        /// </summary>
        public enum LevelEnum
        {
            [Description("User")]
            [Display(Name = "User")]
            User = 0,

            [Description("Organisation")]
            [Display(Name = "Organisation")]
            Organisation = 1
        }

        /// <summary>
        /// The level of privacy
        /// </summary>
        public enum PrivacyLevelEnum
        {
            [Description("Public")]
            [Display(Name = "Public")]
            Public = 0,

            [Description("Private")]
            [Display(Name = "Private")]
            Private = 1
        }
    }
}