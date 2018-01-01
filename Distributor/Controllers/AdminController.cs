using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Distributor.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult UserAdmin()
        {
            UserAdminView model = AppUserViewHelpers.GetUserAdminView(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        [HttpPost]
        public ActionResult UserAdmin(UserAdminView model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("UserAdmin");
            }

            if (ModelState.IsValid)
            {
                if (Request.Form["savebutton"] != null)
                {
                    AppUserHelpers.UpdateAppUsers(db, model.UserAdminActiveView, true, User);
                    return RedirectToAction("Dashboard", "Home");
                }

                if (Request.Form["addusersbutton"] != null)
                {
                    AppUserHelpers.UpdateAppUsers(db, model.UserAdminActiveView, true, User);
                    return RedirectToAction("AddUser");
                }

                if (Request.Form["saveinactivebutton"] != null)
                {
                    AppUserHelpers.UpdateAppUsers(db, model.UserAdminNonActiveView, false, User);
                    return RedirectToAction("Dashboard", "Home");
                }

                return RedirectToAction("Dashboard", "Home");
            }
            return View(model);
        }

        public ActionResult OrganisationAdmin()
        {
            OrganisationAdminView model = OrganisationViewHelpers.GetOrganisationAdminView(db, AppUserHelpers.GetOrganisationIdFromUser(db, User));
            return View(model);
        }

        [HttpPost]
        public ActionResult OrganisationAdmin([Bind(Include = "OrganisationId,OrganisationName,BusinessType,AddressLine1,AddressLine2,AddressLine3,AddressTownCity,AddressCounty,AddressPostcode,TelephoneNumber,Email,Website,ContactName,CompanyRegistrationDetails,CharityRegistrationDetails,VATRegistrationDetails,PrivacyLevel,GroupPrivacyLevel")] OrganisationAdminView model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("OrganisationAdmin");
            }

            if (ModelState.IsValid)
            {
                OrganisationHelpers.UpdateOrganisation(db, model, User);
                return RedirectToAction("Dashboard", "Home");
            }
            return View(model);
        }

        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser([Bind(Include = "FirstName,LastName,LoginEmail,LoginPassword,ConfirmPassword,PrivacyLevel,UserRole")] UserAdminAddUserView model)
        {
            if (Request.Form["resetbutton"] != null)
            {
                return RedirectToAction("AddUser");
            }

            if (ModelState.IsValid)
            {
                //Create a new AppUser
                AppUser appUser = AppUserHelpers.CreateAppUser(db, model, User);

                var user = new ApplicationUser { UserName = model.LoginEmail, Email = model.LoginEmail, AppUserId = appUser.AppUserId, CurrentUserRole = appUser.UserRole };
                var result = await UserManager.CreateAsync(user, model.LoginPassword);
                if (result.Succeeded)
                {
                    if (Request.Form["adduserbutton"] != null)
                    {
                        return RedirectToAction("AddUser");
                    }

                    return RedirectToAction("UserAdmin");
                }

                //Delete the appUser account as this has not gone through
                AppUserHelpers.DeleteAppUser(db, appUser.AppUserId);
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[HttpPost]
        //public ActionResult AddUser([Bind(Include = "FirstName,LastName,LoginEmail,LoginPassword,ConfirmPassword,PrivacyLevel,UserRole")] UserAdminAddUserView model)
        //{
        //    if (Request.Form["resetbutton"] != null)
        //    {
        //        return RedirectToAction("UserAdmin");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        AppUser appUser = AppUserHelpers.CreateAppUser(db, model, User);

        //        var user = new ApplicationUser { UserName = model.LoginEmail, Email = model.LoginEmail, AppUserId = appUser.AppUserId, CurrentUserRole = appUser.UserRole };
        //        var result = UserManager.CreateAsync(user, model.LoginPassword);
        //        if (result.Succeeded)
        //        {
        //        }

        //        //Delete the appUser account as this has not gone through
        //        AppUserHelpers.DeleteAppUser(appUser.AppUserId);


        //        //if (Request.Form["adduserbutton"] != null)
        //        //{
        //        //    return RedirectToAction("AddUser");
        //        //}

        //        //return RedirectToAction("UserAdmin");
        //    }
        //    return View(model);
        //}
    }
}