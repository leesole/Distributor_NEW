using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;

namespace Distributor.Helpers
{
    public static class ControlHelpers
    {
        #region DropDowns

        //get all organisations and poulate drop down
        public static SelectList AllOrganisationsListDropDown()
        {
            return new SelectList(OrganisationHelpers.GetAllOrganisations(), "OrganisationId", "OrganisationName");
        }

        //get all organisations and poulate drop down and select initial value
        public static SelectList AllOrganisationsListDropDown(Guid organisationId)
        {
            return new SelectList(OrganisationHelpers.GetAllOrganisations(), "OrganisationId", "OrganisationName", organisationId);
        }

        public static SelectList OrganisationsListForGroupDropDown(ApplicationDbContext db, Guid groupId)
        {
            List<Organisation> allOrganisations = OrganisationHelpers.GetAllOrganisations(db);
            List<GroupMember> members = GroupMembersHelpers.GetGroupMembersForGroup(db, groupId);

            //remove the group orgs from the allOrganisation list
            foreach (GroupMember member in members)
            {
                Organisation org = OrganisationHelpers.GetOrganisation(db, member.OrganisationId);
                allOrganisations.Remove(org);
            }

            //Build custom selectable data to hold org name and address
            return new SelectList(
                allOrganisations.Select(
                    o => new
                    {
                        OrganisationId = o.OrganisationId,
                        OrganisationDetails = o.OrganisationName + ": " + o.AddressLine1 + ", " + o.AddressTownCity
                    }), "OrganisationId", "OrganisationDetails");
        }

        public static SelectList EntityStatusEnumsForUsersDropDown(EntityStatusEnum status)
        {
            var enumList = (from EntityStatusEnum bt in Enum.GetValues(typeof(EntityStatusEnum))
                            select new
                            {
                                Id = bt,
                                Name = EnumHelpers.GetDescription((EntityStatusEnum)bt)
                            });

            SelectList list = new SelectList(enumList, "Id", "Name", status);

            //remove the non AppUser values...
            list = new SelectList(list
                            .Where(x => (x.Value != "Rejected") && (x.Value != "Closed") && (x.Value != "Removed"))
                            .ToList(),
                            "Value",
                            "Text",
                            status);

            return list;
        }

        public static SelectList ExternalSearchLevelEnumsDropDown(ExternalSearchLevelEnum level)
        { 
            var enumList = (from ExternalSearchLevelEnum bt in Enum.GetValues(typeof(ExternalSearchLevelEnum))
                            select new
                            {
                                Id = bt,
                                Name = EnumHelpers.GetDescription((ExternalSearchLevelEnum)bt)
                            });

            return new SelectList(enumList, "Id", "Name", level);
        }

        #endregion
    }
}