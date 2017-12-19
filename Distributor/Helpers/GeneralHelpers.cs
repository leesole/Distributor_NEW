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


    }
}