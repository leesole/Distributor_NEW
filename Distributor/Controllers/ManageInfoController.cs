﻿using Distributor.Helpers;
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

        public ActionResult DisplayAvailable(Guid? id, string breadcrumb, string callingActionDisplayName, bool historyDisplay, bool? recalled, string defaultController, string defaultAction)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
            breadcrumbDictionary.Add(0, breadcrumb);

            if (!recalled.HasValue)
            {
                defaultController = "ManageInfo";
                defaultAction = "Available";
            }

            AvailableListingDetailsViewModel model = AvailableListingViewHelpers.CreateAvailableListingDetailsViewModel(db, id.Value, breadcrumb, historyDisplay, Request, defaultController, defaultAction, callingActionDisplayName, breadcrumbDictionary, recalled);
            
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DisplayAvailable([Bind(Include = "Breadcrumb,HistoryRecord,CallingAction,CallingController,CallingActionDisplayName,ListingId,ItemDescription,ItemCategory,ItemType,QuantityAvailable,QuantityFulfilled,QuantityOutstanding,UoM,AvailableFrom,AvailableTo,ItemCondition,DisplayUntilDate,SellByDate,UseByDate,DeliveryAvailable,ListingStatus,ListingOrganisationPostcode")] AvailableListingDetailsViewModel model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("DisplayAvailable", "ManageInfo", new { id = model.ListingId, breadcrumb = model.Breadcrumb, callingActionDisplayName = model.CallingActionDisplayName, historyDisplay = model.HistoryRecord, recalled = true, defaultController = model.CallingController, defaultAction = model.CallingAction });
            }

            if (ModelState.IsValid)
            {
                AvailableListingHelpers.UpdateAvailableListing(db, model, User);
                return RedirectToAction(model.CallingAction, model.CallingController);
            }

            return View(model);
        }

        #endregion

        #region RequiredListings

        public ActionResult Required()
        {
            List<RequiredListingManageViewModel> model = RequiredListingViewHelpers.GetRequiredListingManageViewModel(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        [HttpPost]
        public ActionResult Required(List<RequiredListingManageViewModel> model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("Required");
            }

            if (ModelState.IsValid)
            {
                if (Request.Form["savebutton"] != null)
                {
                    RequiredListingHelpers.UpdateRequiredListings(db, model, User);
                }

                return RedirectToAction("Required");
            }
            return View(model);
        }

        //public ActionResult RequiredHistory()
        //{
        //    List<AvailableListingManageHistoryViewModel> model = AvailableListingViewHelpers.GetAvailableListingManageHistoryViewModel(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
        //    return View(model);
        //}

        //public ActionResult RequiredAvailable()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult RequiredAvailable([Bind(Include = "ItemDescription,ItemCategory,ItemType,QuantityAvailable,UoM,AvailableFrom,AvailableTo,ItemCondition,DisplayUntilDate,SellByDate,UseByDate,DeliveryAvailable")] AvailableListingManageCreateViewModel model)
        //{
        //    if (Request.Form["resetbutton"] != null)
        //    {
        //        return RedirectToAction("RequiredAvailable");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        AvailableListingHelpers.CreateListing(db, model, User);
        //        return RedirectToAction("Required");
        //    }

        //    return View(model);
        //}

        //public ActionResult DisplayRequired(Guid? id, string breadcrumb, string callingActionDisplayName, bool historyDisplay, bool? recalled, string defaultController, string defaultAction)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
        //    breadcrumbDictionary.Add(0, breadcrumb);

        //    if (!recalled.HasValue)
        //    {
        //        defaultController = "ManageInfo";
        //        defaultAction = "Available";
        //    }

        //    AvailableListingDetailsViewModel model = AvailableListingViewHelpers.CreateAvailableListingDetailsViewModel(db, id.Value, breadcrumb, historyDisplay, Request, defaultController, defaultAction, callingActionDisplayName, breadcrumbDictionary, recalled);

        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult DisplayRequired([Bind(Include = "Breadcrumb,HistoryRecord,CallingAction,CallingController,CallingActionDisplayName,ListingId,ItemDescription,ItemCategory,ItemType,QuantityAvailable,QuantityFulfilled,QuantityOutstanding,UoM,AvailableFrom,AvailableTo,ItemCondition,DisplayUntilDate,SellByDate,UseByDate,DeliveryAvailable,ListingStatus,ListingOrganisationPostcode")] AvailableListingDetailsViewModel model)
        //{
        //    if (Request.Form["resetbutton"] != null)
        //    {
        //        return RedirectToAction("DisplayAvailable", "ManageInfo", new { id = model.ListingId, breadcrumb = model.Breadcrumb, callingActionDisplayName = model.CallingActionDisplayName, historyDisplay = model.HistoryRecord, recalled = true, defaultController = model.CallingController, defaultAction = model.CallingAction });
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        AvailableListingHelpers.UpdateAvailableListing(db, model, User);
        //        return RedirectToAction(model.CallingAction, model.CallingController);
        //    }

        //    return View(model);
        //}

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