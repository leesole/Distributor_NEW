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

        #endregion
    }
}