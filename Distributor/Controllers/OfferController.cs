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
    public class OfferController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
