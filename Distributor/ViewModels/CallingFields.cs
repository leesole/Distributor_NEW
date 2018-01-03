
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Distributor.ViewModels
{
    public class CallingFields
    {
        public string CallingController { get; set; }

        public string CallingAction { get; set; }

        public string CallingActionDisplayName { get; set; }

        public Dictionary<int, string> BreadcrumbTrail { get; set; }
    }
}