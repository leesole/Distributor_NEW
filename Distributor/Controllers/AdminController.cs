using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributor.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult UserAdmin()
        {
            return View();
        }

        public ActionResult OrganisationAdmin()
        {
            OrganisationAdminView model = OrganisationViewHelpers.GetOrganisationAdminView(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        [HttpPost]
        public ActionResult OrganisationAdmin([Bind(Include = "OrganisationId,OrganisationName,BusinessType,AddressLine1,AddressLine2,AddressLine3,AddressTownCity,AddressCounty,AddressPostcode,TelephoneNumber,Email,Website,ContactName,CompanyRegistrationDetails,CharityRegistrationDetails,VATRegistrationDetails,PrivacyLevel,GroupPrivacyLevel")] OrganisationAdminView model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("OrganisationAdmin");
            }

            if (ModelState.IsValid)
            {
                OrganisationHelpers.UpdateOrganisation(db, model, User);
                return RedirectToAction("Dashboard", "Home");
            }
            return View(model);
        }
    }
}