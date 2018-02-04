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
    public class OfferController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Display(Guid? id, string breadcrumb, string callingActionDisplayName, bool displayOnly, string type, bool? recalled, string controllerValue, string actionValue)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfferViewModel model = OfferViewHelpers.GetOfferViewModel(db, Request, id.Value, breadcrumb, callingActionDisplayName, displayOnly, type, recalled, controllerValue, actionValue, User);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Display([Bind(Include = "DisplayOnly,Breadcrumb,Type,EditableQuantity,OfferId,ListingId,ListingType,OfferStatus,ItemDescription,QuantityOutstanding,CurrentOfferQuantity,PreviousOfferQuantity,CounterOfferQuantity,PreviousCounterOfferQuantity,RejectedBy,RejectedOn,YourOrganisationId,OfferOriginatorOrganisationId,CounterOfferOriginatorOrganisationId,OfferOriginatorAppUser,OfferOriginatorOrganisation,OfferOriginatorDateTime,LastOfferOriginatorAppUser,LastOfferOriginatorDateTime,ListingOriginatorAppUser,ListingOriginatorOrganisation,ListingOriginatorDateTime,CounterOfferOriginatorAppUser,CounterOfferOriginatorOrganisation,CounterOfferOriginatorDateTime,LastCounterOfferOriginatorAppUser,LastCounterOfferOriginatorDateTime,OrderId,OrderOriginatorAppUser,OrderOriginatorOrganisation,OrderOriginatorDateTime,CallingController,CallingAction,CallingActionDisplayName")] OfferViewModel model)
        {
            //LSLSLS - the model is missing lots of info.....CHECK!
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("Display", "Offer", new { id = model.OfferId, breadcrumb = model.Breadcrumb, callingActionDisplayName = model.CallingActionDisplayName, displayOnly = model.DisplayOnly, type = model.Type, recalled = true, controllerValue = model.CallingController, actionValue = model.CallingAction });
            }

            if (ModelState.IsValid)
            {
                if (Request.Form["saveofferbutton"] != null)
                {
                    if (model.Type == "created")
                        if (model.CurrentOfferQuantity > 0)
                            //Update offer
                            OfferHelpers.UpdateOffer(db, model, User);

                    if (model.Type == "received")
                        if (model.CounterOfferQuantity > 0)
                            //Update offer
                            OfferHelpers.UpdateOffer(db, model, User);
                }

                return RedirectToAction(model.CallingAction, model.CallingController);
            }

            return View(model);
        }

        #region data manipulation

        public ActionResult AcceptOffer(Guid? offerId)
        {
            if (offerId.HasValue)
            {
                OfferHelpers.AcceptOffer(db, offerId.Value, User);

                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }

        public ActionResult RejectOffer(Guid? offerId)
        {
            if (offerId.HasValue)
            {
                OfferHelpers.RejectOffer(db, offerId.Value, User);

                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }

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
