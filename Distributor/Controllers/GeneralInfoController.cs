using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
            List<AvailableListingGeneralViewModel> model = AvailableListingViewHelpers.GetAvailableListingGeneralViewModel(db, User, maxDistance, maxAge);
            
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