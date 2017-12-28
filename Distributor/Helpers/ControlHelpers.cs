using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            return new SelectList(allOrganisations, "OrganisationId", "OrganisationName");
        }

        #endregion
    }
}