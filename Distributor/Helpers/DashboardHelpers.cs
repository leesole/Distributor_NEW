using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.OfferEnums;
using static Distributor.Enums.OrderEnums;

namespace Distributor.Helpers
{
    public class DashboardHelpers
    {
        public static HomeDashboardView CreateHomeDashboardView(ApplicationDbContext db, IPrincipal user)
        {
            HomeDashboardView view = new HomeDashboardView();

            Organisation currentOrg = OrganisationHelpers.GetOrganisation(db, AppUserHelpers.GetOrganisationIdFromUser(db, user));
            DateTime weekAgo = DateTime.Now.AddDays(-7);
            DateTime dayAgo = DateTime.Now.AddDays(-1);

            List<Notification> openNotifications = NotificationHelpers.GetNotificationsForOrganisationFromUser(db, user, false);
            view.OpenNotifications = openNotifications.Count;
            view.OpenNotificationsThisWeek = (from oN in openNotifications where oN.RecordChangeOn >= weekAgo select oN).Count();
            if (view.OpenNotificationsThisWeek == 0)
                view.OpenNotificationsThisWeekPercent = 0;
            else
                view.OpenNotificationsThisWeekPercent = (Convert.ToDecimal(view.OpenNotificationsThisWeek) / Convert.ToDecimal(view.OpenNotifications)) * 100;
            view.OpenNotificationsPastDay = (from oN in openNotifications where oN.RecordChangeOn >= dayAgo select oN).Count();
            if (view.OpenNotificationsPastDay == 0)
                view.OpenNotificationsPastDayPercent = 0;
            else
                view.OpenNotificationsPastDayPercent = (Convert.ToDecimal(view.OpenNotificationsPastDay) / Convert.ToDecimal(view.OpenNotifications)) * 100;

            List<UserTask> openUserTasks = UserTasksHelpers.GetUserTasksForOrganisationFromUser(db, user, false);
            view.OpenTasks = openUserTasks.Count;
            view.OpenTasksThisWeek = (from oT in openUserTasks where oT.RecordChangeOn >= weekAgo select oT).Count();
            if (view.OpenTasksThisWeek == 0)
                view.OpenTasksThisWeekPercent = 0;
            else
                view.OpenTasksThisWeekPercent = (Convert.ToDecimal(view.OpenTasksThisWeek) / Convert.ToDecimal(view.OpenTasks)) * 100;
            view.OpenTasksPastDay = (from oT in openUserTasks where oT.RecordChangeOn >= dayAgo select oT).Count();
            if (view.OpenTasksPastDay == 0)
                view.OpenTasksPastDayPercent = 0;
            else
                view.OpenTasksPastDayPercent = (Convert.ToDecimal(view.OpenTasksPastDay) / Convert.ToDecimal(view.OpenTasks)) * 100;

            List<Offer> offersCreated = OfferHelpers.GetOpenOffersCreatedForOrganisation(db, currentOrg.OrganisationId);
            view.OffersCreatedOpen = offersCreated.Count;
            view.OffersCreatedCountered = (from oC in offersCreated where oC.OfferStatus == OfferStatusEnum.Countered select oC).Count();
            if (view.OffersCreatedCountered == 0)
                view.OffersCreatedCounteredPercent = 0;
            else
                view.OffersCreatedCounteredPercent = (Convert.ToDecimal(view.OffersCreatedCountered) / Convert.ToDecimal(view.OffersCreatedOpen)) * 100;
            view.OffersCreatedReOffered = (from oC in offersCreated where oC.OfferStatus == OfferStatusEnum.Reoffer select oC).Count();
            if (view.OffersCreatedReOffered == 0)
                view.OffersCreatedReOfferedPercent = 0;
            else
                view.OffersCreatedReOfferedPercent = (Convert.ToDecimal(view.OffersCreatedReOffered) / Convert.ToDecimal(view.OffersCreatedOpen)) * 100;
            List<Offer> offersCreatedClosed = OfferHelpers.GetAllOffersCreatedClosedForWeekForOrganisation(db, currentOrg.OrganisationId);
            view.OffersCreatedAcceptedThisWeek = (from oCC in offersCreatedClosed where oCC.OfferStatus == OfferStatusEnum.Accepted select oCC).Count();
            view.OffersCreatedRejectedThisWeek = (from oCC in offersCreatedClosed where oCC.OfferStatus == OfferStatusEnum.Rejected select oCC).Count();
            view.OffersCreatedClosedThisWeek = (from oCC in offersCreatedClosed where oCC.OfferStatus == OfferStatusEnum.ClosedNoStock select oCC).Count();
            view.OffersCreatedClosedTotalThisWeek = view.OffersCreatedAcceptedThisWeek + view.OffersCreatedRejectedThisWeek + view.OffersCreatedClosedThisWeek;
            if (view.OffersCreatedAcceptedThisWeek == 0)
                view.OffersCreatedAcceptedThisWeekPercent = 0;
            else
                view.OffersCreatedAcceptedThisWeekPercent = (Convert.ToDecimal(view.OffersCreatedAcceptedThisWeek) / Convert.ToDecimal(view.OffersCreatedClosedTotalThisWeek)) * 100;
            if (view.OffersCreatedRejectedThisWeek == 0)
                view.OffersCreatedRejectedThisWeekPercent = 0;
            else
                view.OffersCreatedRejectedThisWeekPercent = (Convert.ToDecimal(view.OffersCreatedRejectedThisWeek) / Convert.ToDecimal(view.OffersCreatedClosedTotalThisWeek)) * 100;
            if (view.OffersCreatedClosedThisWeek == 0)
                view.OffersCreatedClosedThisWeekPercent = 0;
            else
                view.OffersCreatedClosedThisWeekPercent = (Convert.ToDecimal(view.OffersCreatedClosedThisWeek) / Convert.ToDecimal(view.OffersCreatedClosedTotalThisWeek)) * 100;

            List<Offer> offersReceived = OfferHelpers.GetOpenOffersReceivedForOrganisation(db, currentOrg.OrganisationId);
            view.OffersReceivedOpen = offersReceived.Count;
            view.OffersReceivedCountered = (from oR in offersReceived where oR.OfferStatus == OfferStatusEnum.Countered select oR).Count();
            if (view.OffersReceivedCountered == 0)
                view.OffersReceivedCounteredPercent = 0;
            else
                view.OffersReceivedCounteredPercent = (Convert.ToDecimal(view.OffersReceivedCountered) / Convert.ToDecimal(view.OffersReceivedOpen)) * 100;
            view.OffersReceivedReOffered = (from oR in offersReceived where oR.OfferStatus == OfferStatusEnum.Reoffer select oR).Count();
            if (view.OffersReceivedReOffered == 0)
                view.OffersReceivedReOfferedPercent = 0;
            else
                view.OffersReceivedReOfferedPercent = (Convert.ToDecimal(view.OffersReceivedReOffered) / Convert.ToDecimal(view.OffersReceivedOpen)) * 100;
            List<Offer> offersReceivedClosed = OfferHelpers.GetAllOffersReceivedClosedForWeekForOrganisation(db, currentOrg.OrganisationId);
            view.OffersReceivedAcceptedThisWeek = (from oRC in offersReceived where oRC.OfferStatus == OfferStatusEnum.Accepted select oRC).Count();
            view.OffersReceivedRejectedThisWeek = (from oRC in offersReceived where oRC.OfferStatus == OfferStatusEnum.Rejected select oRC).Count();
            view.OffersReceivedClosedThisWeek = (from oRC in offersReceived where oRC.OfferStatus == OfferStatusEnum.ClosedNoStock select oRC).Count();
            view.OffersReceivedClosedTotalThisWeek = view.OffersReceivedAcceptedThisWeek + view.OffersReceivedRejectedThisWeek + view.OffersReceivedClosedThisWeek;
            if (view.OffersReceivedAcceptedThisWeek == 0)
                view.OffersReceivedAcceptedThisWeekPercent = 0;
            else
                view.OffersReceivedAcceptedThisWeekPercent = (Convert.ToDecimal(view.OffersReceivedAcceptedThisWeek) / Convert.ToDecimal(view.OffersReceivedClosedTotalThisWeek)) * 100;
            if (view.OffersReceivedRejectedThisWeek == 0)
                view.OffersReceivedRejectedThisWeekPercent = 0;
            else
                view.OffersReceivedRejectedThisWeekPercent = (Convert.ToDecimal(view.OffersReceivedRejectedThisWeek) / Convert.ToDecimal(view.OffersReceivedClosedTotalThisWeek)) * 100;
            if (view.OffersReceivedClosedThisWeek == 0)
                view.OffersReceivedClosedThisWeekPercent = 0;
            else
                view.OffersReceivedClosedThisWeekPercent = (Convert.ToDecimal(view.OffersReceivedClosedThisWeek) / Convert.ToDecimal(view.OffersReceivedClosedTotalThisWeek)) * 100;

            view.OrdersInOpen = OrderHelpers.GetOrdersInForOganisationCount(db, currentOrg.OrganisationId);
            List<Order> ordersIn = OrderHelpers.GetOrdersInForWeekForOrganisation(db, currentOrg.OrganisationId);
            view.OrdersInCollectedThisWeek = (from oI in ordersIn where oI.OrderInStatus == OrderInStatusEnum.Collected select oI).Count();
            view.OrdersInReceivedThisWeek = (from oI in ordersIn where oI.OrderInStatus == OrderInStatusEnum.Received select oI).Count();
            view.OrdersInClosedThisWeek = (from oI in ordersIn where oI.OrderInStatus == OrderInStatusEnum.Closed select oI).Count();
            int ordersInClosedCount = view.OrdersInCollectedThisWeek + view.OrdersInReceivedThisWeek + view.OrdersInClosedThisWeek;
            if (view.OrdersInCollectedThisWeek == 0)
                view.OrdersInCollectedThisWeekPercent = 0;
            else
                view.OrdersInCollectedThisWeekPercent = (Convert.ToDecimal(view.OrdersInCollectedThisWeek) / Convert.ToDecimal(ordersInClosedCount)) * 100;
            if (view.OrdersInReceivedThisWeek == 0)
                view.OrdersInReceivedThisWeekPercent = 0;
            else
                view.OrdersInReceivedThisWeekPercent = (Convert.ToDecimal(view.OrdersInReceivedThisWeek) / Convert.ToDecimal(ordersInClosedCount)) * 100;
            if (view.OrdersInClosedThisWeek == 0)
                view.OrdersInClosedThisWeekPercent = 0;
            else
                view.OrdersInClosedThisWeekPercent = (Convert.ToDecimal(view.OrdersInClosedThisWeek) / Convert.ToDecimal(ordersInClosedCount)) * 100;

            view.OrdersOutOpen = OrderHelpers.GetOrdersOutForOganisationCount(db, currentOrg.OrganisationId);
            List<Order> ordersOut = OrderHelpers.GetOrdersOutForWeekForOganisation(db, currentOrg.OrganisationId);
            view.OrdersOutDespatchedThisWeek = (from oO in ordersOut where oO.OrderOutStatus == OrderOutStatusEnum.Dispatched select oO).Count();
            view.OrdersOutDeliveredThisWeek = (from oO in ordersOut where oO.OrderOutStatus == OrderOutStatusEnum.Delivered select oO).Count();
            view.OrdersOutClosedThisWeek = (from oO in ordersOut where oO.OrderOutStatus == OrderOutStatusEnum.Closed select oO).Count();
            int ordersOutClosedCount = view.OrdersOutDespatchedThisWeek + view.OrdersOutDeliveredThisWeek + view.OrdersOutClosedThisWeek;
            if (view.OrdersOutDespatchedThisWeek == 0)
                view.OrdersOutDespatchedThisWeekPercent = 0;
            else
                view.OrdersOutDespatchedThisWeekPercent = (Convert.ToDecimal(view.OrdersOutDespatchedThisWeek) / Convert.ToDecimal(ordersOutClosedCount)) * 100;
            if (view.OrdersOutDeliveredThisWeek == 0)
                view.OrdersOutDeliveredThisWeekPercent = 0;
            else
                view.OrdersOutDeliveredThisWeekPercent = (Convert.ToDecimal(view.OrdersOutDeliveredThisWeek) / Convert.ToDecimal(ordersOutClosedCount)) * 100;
            if (view.OrdersOutClosedThisWeek == 0)
                view.OrdersOutClosedThisWeekPercent = 0;
            else
                view.OrdersOutClosedThisWeekPercent = (Convert.ToDecimal(view.OrdersOutClosedThisWeek) / Convert.ToDecimal(ordersOutClosedCount)) * 100;

            return view;
        }
    }
}