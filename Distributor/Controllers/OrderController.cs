using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Distributor.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Display(Guid? id, string breadcrumb, string callingActionDisplayName, bool displayOnly, string type, bool? recalled, string controllerValue, string actionValue)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderViewModel model = OrderViewHelpers.GetOfferViewModel(db, Request, id.Value, breadcrumb, callingActionDisplayName, displayOnly, type, recalled, controllerValue, actionValue, User);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Display([Bind(Include = "DisplayOnly,Breadcrumb,Type,OrderId,OrderDistributionDateTime,OrderDistributed,OrderDistributedBy,OrderDeliveredDateTime,OrderDelivered,OrderDeliveredBy,OrderCollectedDateTime,OrderCollected,OrderCollectedBy,OrderReceivedDateTime,OrderReceived,OrderReceivedBy,OrderInClosedDateTime,OrderInClosed,OrderInClosedBy,OrderOutClosedDateTime,OrderOutClosed,OrderOutClosedBy,CallingController,CallingAction,CallingActionDisplayName")] OrderViewModel model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("Display", "Order", new { id = model.OrderId, breadcrumb = model.Breadcrumb, callingActionDisplayName = model.CallingActionDisplayName, displayOnly = model.DisplayOnly, type = model.Type, recalled = true, controllerValue = model.CallingController, actionValue = model.CallingAction });
            }

            if (ModelState.IsValid)
            {
                if (Request.Form["savebutton"] != null)
                    //Update order
                    OrderHelpers.UpdateOrder(db, model, User);

                return RedirectToAction(model.CallingAction, model.CallingController);
            }

            Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
            breadcrumbDictionary.Add(0, model.Breadcrumb);
            model.BreadcrumbDictionary = breadcrumbDictionary;
            model.BreadcrumbTrail = breadcrumbDictionary;
            return View(model);
        }
    }
}