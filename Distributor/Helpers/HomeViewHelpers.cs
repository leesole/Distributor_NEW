using Distributor.Extensions;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Distributor.Helpers
{
    public static class HomeViewHelpers
    {
        public static HomeOrganisationDetailsView CreateHomeOrganisationDetailsView(IPrincipal user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return CreateHomeOrganisationDetailsView(db, user);
        }
        public static HomeOrganisationDetailsView CreateHomeOrganisationDetailsView(ApplicationDbContext db, IPrincipal user)
        {
            Guid appUserId;
            Guid.TryParse(user.Identity.GetAppUserId(), out appUserId);

            HomeOrganisationDetailsView view = new HomeOrganisationDetailsView()
            {
                AppUserId = appUserId
            };

            return view;
        }
    }
}