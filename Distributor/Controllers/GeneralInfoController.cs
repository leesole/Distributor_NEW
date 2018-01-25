using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Distributor.Enums.GeneralEnums;

namespace Distributor.Controllers
{
    public class GeneralInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region AvailableListings

        public ActionResult Available(int? maxDistance, double? maxAge)
        {
            AvailableListingGeneralViewListModel model = AvailableListingViewHelpers.GetAvailableListingGeneralViewListModel(db, User, maxDistance, maxAge);

            return View(model);
        }

        [HttpPost]
        public ActionResult Available(AvailableListingGeneralViewListModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form["savebutton"] != null)
                    //Create offers
                    OfferHelpers.CreateOffers(db, model, ListingTypeEnum.Available, User);

                return RedirectToAction("Available", "GeneralInfo", new { maxDistance = model.MaxDistance, maxAge = model.MaxAge });
            }

            return View(model);
        }

        public ActionResult DisplayAvailable(Guid? id, string breadcrumb, string callingActionDisplayName, bool displayOnly, bool? recalled, string defaultController, string defaultAction, int? maxDistance, double? maxAge)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
            breadcrumbDictionary.Add(0, breadcrumb);

            if (!recalled.HasValue)
            {
                defaultController = "GeneralInfo";
                defaultAction = "Available";
            }

            AvailableListingDetailsViewModel model = AvailableListingViewHelpers.CreateAvailableListingDetailsViewModel(db, id.Value, breadcrumb, displayOnly, Request, defaultController, defaultAction, callingActionDisplayName, breadcrumbDictionary, recalled, User, maxDistance, maxAge);

            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.ListingType = "General Information";
            return View(model);
        }

        [HttpPost]
        public ActionResult DisplayAvailable([Bind(Include = "MaxDistance,MaxAge,Breadcrumb,DisplayOnly,CallingAction,CallingController,CallingActionDisplayName,ListingId,ItemDescription,ItemCategory,ItemType,QuantityAvailable,QuantityFulfilled,QuantityOutstanding,UoM,AvailableFrom,AvailableTo,ItemCondition,DisplayUntilDate,SellByDate,UseByDate,DeliveryAvailable,ListingStatus,ListingOrganisationPostcode,OfferDescription,OfferId,OfferQty,OfferCounterQty,OfferStatus")] AvailableListingDetailsViewModel model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("DisplayAvailable", "GeneralInfo", new { id = model.ListingId, breadcrumb = model.Breadcrumb, callingActionDisplayName = model.CallingActionDisplayName, displayOnly = model.DisplayOnly, recalled = true, defaultController = model.CallingController, defaultAction = model.CallingAction, maxDistance = model.MaxDistance, maxAge = model.MaxAge });
            }

            if (ModelState.IsValid)
            {
                if (Request.Form["saveofferbutton"] != null)
                    OfferHelpers.CreateOffer(db, model.ListingId, model.OfferQty, ListingTypeEnum.Available, null, User);

                if (Request.Form["savebutton"] != null)
                    AvailableListingHelpers.UpdateAvailableListing(db, model, User);

                return RedirectToAction(model.CallingAction, model.CallingController);
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