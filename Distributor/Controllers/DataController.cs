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
    }
}