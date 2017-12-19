using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Distributor.Models;
using Distributor.Helpers;
using Distributor.ViewModels;

namespace Distributor.Controllers
{
    [Authorize]
    public class AppUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AppUsers
        public ActionResult Index()
        {
            return View(db.AppUsers.ToList());
        }

        // GET: AppUsers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // GET: AppUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppUserId,FirstName,LastName,EntityStatus,OrganisationId,LoginEmail,PrivacyLevel,UserRole,MaxDistanceFilter,MaxAgeFilter,SelectionLevelFilter,DisplayMyOrganisationListingsFilter,RecordChange,RecordChangeOn,RecordChangeBy")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                appUser.AppUserId = Guid.NewGuid();
                db.AppUsers.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public ActionResult Edit()
        {
            Guid id = AppUserHelpers.GetAppUserIdFromUser(User);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppUserId,FirstName,LastName,EntityStatus,OrganisationId,LoginEmail,PrivacyLevel,UserRole,MaxDistanceFilter,MaxAgeFilter,SelectionLevelFilter,DisplayMyOrganisationListingsFilter,RecordChange,RecordChangeOn,RecordChangeBy")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AppUser appUser = db.AppUsers.Find(id);
            db.AppUsers.Remove(appUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: AppUsers/Profile/5
        public ActionResult UserProfile()
        {
            string errorMessage = "Your current user appears to be corrupt, please contact your system administrator.";
            Guid id = AppUserHelpers.GetAppUserIdFromUser(User);
            if (id == null)
                return RedirectToAction("Error", "Home", new { errorMessage = errorMessage });

            AppUserProfileView view = AppUserViewHelpers.CreateAppUserProfileView(id);
            if (view == null)
                return RedirectToAction("Error", "Home", new { errorMessage = errorMessage });

            //DropDown
            if (view.SelectedOrganisationId == Guid.Empty)
            {
                ViewBag.OrganisationList = ControlHelpers.AllOrganisationsListDropDown();  //no selected item as nothing to select
                ViewBag.OrganisationSelected = false;
            }
            else
            {
                ViewBag.OrganisationList = ControlHelpers.AllOrganisationsListDropDown(view.SelectedOrganisationId.Value); //select the organisation as initial value
                ViewBag.OrganisationSelected = true;
            }

            return View(view);
        }

        // POST: AppUsers/Settings/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile([Bind(Include = "AppUserId,FirstName,LastName,EntityStatus,LoginEmail,PrivacyLevel,UserRole,SelectedOrganisationId,OrganisationName,BusinessType,AddressLine1,AddressLine2,AddressLine3,AddressTownCity,AddressCounty,AddressPostcode")] AppUserProfileView view)
        {
            if (ModelState.IsValid)
            {
                AppUserHelpers.UpdateAppUser(db, view, User);

                return RedirectToAction("Index", "Home");
            }
            return View(view);
        }

        // GET: AppUsers/Settings/5
        public ActionResult Settings()
        {
            string errorMessage = "Your current user appears to be corrupt, please contact your system administrator.";
            Guid id = AppUserHelpers.GetAppUserIdFromUser(User);
            if (id == null)
                return RedirectToAction("Error", "Home", new { errorMessage = errorMessage });

            AppUserSettingsView view = AppUserViewHelpers.CreateAppUserSettingsView(id);
            if (view == null)
                return RedirectToAction("Error", "Home", new { errorMessage = errorMessage });

            return View(view);
        }

        // POST: AppUsers/Settings/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings([Bind(Include = "AppUserId,MaxDistanceFilter,MaxAgeFilter,SelectionLevelFilter,DisplayMyOrganisationListingsFilter")] AppUserSettingsView view)
        {
            if (ModelState.IsValid)
            {
                AppUserHelpers.UpdateAppUser(db, view, User);

                return RedirectToAction("Index", "Home");
            }
            return View(view);
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
