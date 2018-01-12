using Distributor.Extensions;
using Distributor.Helpers;
using Distributor.Models;
using Distributor.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.UserEnums;

namespace Distributor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //Show basic web page unless the user is logged in
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = AppUserHelpers.GetAppUser(User);

                //If user has not finished setting up user details (i.e. not linked to organisation) then force user to finish details else go to Dashboard
                switch (appUser.EntityStatus)
                {
                    case EntityStatusEnum.Active:
                        return RedirectToAction("Dashboard", "Home");
                    case EntityStatusEnum.AwaitingOrganisationDetails:
                        return RedirectToAction("OrganisationDetails", "Home");
                    case EntityStatusEnum.OnHold:
                        return RedirectToAction("Logout", "Account");
                    default: //other status would have been dealt with so just show home/index view
                        break;
                }
            }

            return View();
        }

        public ActionResult OrganisationDetails()
        {
            HomeOrganisationDetailsView model = HomeViewHelpers.CreateHomeOrganisationDetailsView(User);

            //DropDown
            ViewBag.OrganisationList = ControlHelpers.AllOrganisationsListDropDown();
            return View(model);
        }

        [HttpPost]
        public ActionResult OrganisationDetails([Bind(Include = "AppUserId,SelectedOrganisationId,OrganisationName,BusinessType,AddressLine1,AddressLine2,AddressLine3,AddressTownCity,AddressCounty,AddressPostcode,TelephoneNumber,Email,Website,ContactName,CompanyRegistrationDetails,CharityRegistrationDetails,VATRegistrationDetails,ListingPrivacyLevel,PrivacyLevel,GroupPrivacyLevel")] HomeOrganisationDetailsView model)
        {
            if (Request.Form["resetbutton"] != null)
                return RedirectToAction("OrganisationDetails", "Home");

            if (ModelState.IsValid)
            {
                //If the 'Submit' button pressed then update tables, else leave as are so that on reload it takes original values once again.
                if (Request.Form["submitbutton"] != null)
                {
                    if (model.SelectedOrganisationId == null)
                    {
                        //Add organisation update appUser with this organisationId
                        Organisation organisation = OrganisationHelpers.CreateOrganisation(model, User);
                        AppUserHelpers.UpdateAppUserOrganisationId(User, organisation.OrganisationId);
                        AppUserHelpers.UpdateAppUserRoleAndEntityStatus(User, UserRoleEnum.Admin ,EntityStatusEnum.Active, User);
                        ApplicationUser user = UserHelpers.UpdateUserRole(User, UserRoleEnum.Admin);
                    }
                    else
                    {
                        AppUserHelpers.UpdateAppUserOrganisationId(User, model.SelectedOrganisationId.Value);
                        AppUserHelpers.UpdateAppUserRoleAndEntityStatus(User, UserRoleEnum.User, EntityStatusEnum.OnHold, User);
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
            }

            //DropDown - rebuild and clear selected option
            ViewBag.OrganisationList = ControlHelpers.AllOrganisationsListDropDown();
            model.SelectedOrganisationId = null;
            return View(model);
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
    }
}