using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributor.Helpers
{
    public static class GeneralHelpers
    {
        public static Guid StringToGuid(string stringGuid)
        {
            Guid guidId = Guid.Empty;
            Guid.TryParse(stringGuid, out guidId);

            return guidId;
        }

        public static void GetCallingDetailsFromRequest(HttpRequestBase request, string controllerInitialValue, string actionInitialValue, out string controller, out string action)
        {
            controller = controllerInitialValue;
            action = actionInitialValue;

            try
            {
                string[] callingUrlSegments = request.UrlReferrer.Segments.Select(x => x.TrimEnd('/')).ToArray();
                controller = callingUrlSegments[callingUrlSegments.Count() - 2];
                action = callingUrlSegments[callingUrlSegments.Count() - 1];
            }
            catch { }
        }
    }
}