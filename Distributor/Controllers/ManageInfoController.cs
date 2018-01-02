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
    public class ManageInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageInfo
        public ActionResult Available()
        {
            List<AvailableListingManageViewModel> model = AvailableListingViewHelpers.GetAvailableListingManageViewModel(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        //public ActionResult AvailableHistory()
        //{
        //    return View(model);
        //}

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