using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributor.ViewModels
{
    public class HomeDashboardView
    {
        public int OpenNotifications { get; set; }
        public int OpenNotificationsThisWeek { get; set; }
        public decimal OpenNotificationsThisWeekPercent { get; set; }
        public int OpenNotificationsPastDay { get; set; }
        public decimal OpenNotificationsPastDayPercent { get; set; }

        public int OpenTasks { get; set; }
        public int OpenTasksThisWeek { get; set; }
        public decimal OpenTasksThisWeekPercent { get; set; }
        public int OpenTasksPastDay { get; set; }
        public decimal OpenTasksPastDayPercent { get; set; }

        public int OffersCreatedOpen { get; set; }
        public int OffersCreatedCountered { get; set; }
        public decimal OffersCreatedCounteredPercent { get; set; }
        public int OffersCreatedReOffered { get; set; }
        public decimal OffersCreatedReOfferedPercent { get; set; }
        public int OffersCreatedClosedTotalThisWeek { get; set; }
        public int OffersCreatedAcceptedThisWeek { get; set; }
        public decimal OffersCreatedAcceptedThisWeekPercent { get; set; }
        public int OffersCreatedRejectedThisWeek { get; set; }
        public decimal OffersCreatedRejectedThisWeekPercent { get; set; }
        public int OffersCreatedClosedThisWeek { get; set; }
        public decimal OffersCreatedClosedThisWeekPercent { get; set; }

        public int OffersReceivedOpen { get; set; }
        public int OffersReceivedCountered { get; set; }
        public decimal OffersReceivedCounteredPercent { get; set; }
        public int OffersReceivedReOffered { get; set; }
        public decimal OffersReceivedReOfferedPercent { get; set; }
        public int OffersReceivedClosedTotalThisWeek { get; set; }
        public int OffersReceivedAcceptedThisWeek { get; set; }
        public decimal OffersReceivedAcceptedThisWeekPercent { get; set; }
        public int OffersReceivedRejectedThisWeek { get; set; }
        public decimal OffersReceivedRejectedThisWeekPercent { get; set; }
        public int OffersReceivedClosedThisWeek { get; set; }
        public decimal OffersReceivedClosedThisWeekPercent { get; set; }

        public int OrdersInOpen { get; set; }
        public int OrdersInCollectedThisWeek { get; set; }
        public decimal OrdersInCollectedThisWeekPercent { get; set; }
        public int OrdersInReceivedThisWeek { get; set; }
        public decimal OrdersInReceivedThisWeekPercent { get; set; }
        public int OrdersInClosedThisWeek { get; set; }
        public decimal OrdersInClosedThisWeekPercent { get; set; }

        public int OrdersOutOpen { get; set; }
        public int OrdersOutDespatchedThisWeek { get; set; }
        public decimal OrdersOutDespatchedThisWeekPercent { get; set; }
        public int OrdersOutDeliveredThisWeek { get; set; }
        public decimal OrdersOutDeliveredThisWeekPercent { get; set; }
        public int OrdersOutClosedThisWeek { get; set; }
        public decimal OrdersOutClosedThisWeekPercent { get; set; }
    }
}