using Distributor.Helpers;
using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributor.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region Organisation

        //GET organisation object for organisationid
        [HttpPost]
        public ActionResult GetOrganisationDetailsForOrganisation(Guid organisationId)
        {
            if (organisationId != null)
            {
                Organisation organisationDetails = OrganisationHelpers.GetOrganisation(organisationId);

                if (organisationDetails != null)
                {
                    string businessTypeText = EnumHelpers.GetDescription(organisationDetails.BusinessType);
                    return Json(new { organisationDetails, businessTypeText, success = true });
                }
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }

        #endregion

        #region Groups

        // This will (set the status to) remove (for) the 'Block' from a given blockId
        [HttpPost]
        public ActionResult RemoveGroup(Guid groupId)
        {
            GroupHelpers.RemoveGroup(db, groupId, User);
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddOrganisationToGroup(Guid groupId, Guid organisationId)
        {
            GroupMembersHelpers.CreateGroupMember(db, groupId, organisationId, User);
            return Json(new { success = true });
        }

        #endregion
    }
}