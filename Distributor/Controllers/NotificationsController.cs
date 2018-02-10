using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.UserNotificationEnums;

namespace Distributor.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notifications
        public ActionResult Index()
        {
            List<NotificationViewModel> model = NotificationViewHelpers.GetNotificationsViewModelForOrganisationFromUser(db, User, false);
            return View(model);
        }
        
        // GET: Notification History
        public ActionResult History()
        {
            List<NotificationViewModel> model = NotificationViewHelpers.GetNotificationsViewModelForOrganisationFromUser(db, User, true);
            return View(model);
        }

        #region data manipulation

        public ActionResult RemoveNotification(Guid? notificationId)
        {
            if (notificationId.HasValue)
            {
                //close the Task
                NotificationHelpers.UpdateEntityStatus(db, notificationId.Value, EntityStatusEnum.Inactive, User);

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