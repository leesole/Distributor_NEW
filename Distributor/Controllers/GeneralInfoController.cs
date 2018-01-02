using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributor.Controllers
{
    public class GeneralInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Available listing
        public ActionResult Available()
        {
            return View();
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