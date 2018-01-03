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
    public class ManageInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageInfo
        public ActionResult Available()
        {
            List<AvailableListingManageViewModel> model = AvailableListingViewHelpers.GetAvailableListingManageViewModel(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        public ActionResult AvailableHistory()
        {
            List<AvailableListingManageHistoryViewModel> model = AvailableListingViewHelpers.GetAvailableListingManageHistoryViewModel(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        public ActionResult CreateAvailable()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAvailable([Bind(Include = "ItemDescription,ItemCategory,ItemType,QuantityAvailable,UoM,AvailableFrom,AvailableTo,ItemCondition,DisplayUntilDate,SellByDate,UseByDate,DeliveryAvailable")] AvailableListingManageCreateViewModel model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("CreateAvailable");
            }

            if (ModelState.IsValid)
            {
                AvailableListingHelpers.CreateListing(db, model, User);
                return RedirectToAction("Available");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}