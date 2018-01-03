using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Distributor.Controllers
{
    [Authorize]
    public class ManageInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region AvailableListings

        public ActionResult Available()
        {
            List<AvailableListingManageViewModel> model = AvailableListingViewHelpers.GetAvailableListingManageViewModel(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        [HttpPost]
        public ActionResult Available(List<AvailableListingManageViewModel> model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("Available");
            }

            if (ModelState.IsValid)
            {
                if (Request.Form["savebutton"] != null)
                {
                    AvailableListingHelpers.UpdateAvailableListings(db, model, User);
                }

                return RedirectToAction("Available");
            }
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

        public ActionResult DisplayAvailable(Guid? id, string breadcrumb, string callingActionDisplayName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
            breadcrumbDictionary.Add(0, breadcrumb);
            AvailableListingDetailsViewModel model = AvailableListingViewHelpers.CreateAvailableListingDetailsViewModel(db, id.Value, Request, "ManageInfo", "Available", callingActionDisplayName, breadcrumbDictionary);
                        
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        #endregion

        #region RequiredListings
        #endregion

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