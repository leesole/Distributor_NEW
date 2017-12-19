using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Distributor.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetAppUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AppUserId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetCurrentUserRole(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CurrentUserRole");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}