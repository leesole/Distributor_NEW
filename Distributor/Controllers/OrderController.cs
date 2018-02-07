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
    }
}