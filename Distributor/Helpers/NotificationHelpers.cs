using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserNotificationEnums;

namespace Distributor.Helpers
{
    public static class NotificationHelpers
    {
        #region Get

        public static Notification GetNotification(ApplicationDbContext db, Guid notificationId)
        {
            return db.Notifications.Find(notificationId);
        }

        public static List<Notification> GetNotificationsForOrganisationFromUser(ApplicationDbContext db, IPrincipal user, bool getHistory)
        {
            AppUser appUser = AppUserHelpers.GetAppUser(db, user);
            EntityStatusEnum status = EntityStatusEnum.Active;
            if (getHistory)
                status = EntityStatusEnum.Inactive;

            List<Notification> list = (from n in db.Notifications
                                       where (n.OrganisationId == appUser.OrganisationId && n.EntityStatus == status)
                                       orderby n.RecordChangeBy ascending
                                       select n).Distinct().ToList();

            return list;
        }

        #endregion

        #region Create

        public static Notification CreateNotification(ApplicationDbContext db, NotificationTypeEnum notificationType, string notificationDescription, Guid referenceKey, Guid referenceAppUserId, Guid referenceOrganisationId, IPrincipal user)
        {
            Notification notification = new Notification()
            {
                NotificationId = Guid.NewGuid(),
                NotificationType = notificationType,
                NotificationDescription = notificationDescription,
                ReferenceKey = referenceKey,
                AppUserId = referenceAppUserId,
                OrganisationId = referenceOrganisationId,
                EntityStatus = EntityStatusEnum.Active,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user),
                RecordChangeOn = DateTime.Now
            };

            db.Notifications.Add(notification);
            db.SaveChanges();

            return notification;
        }

        #endregion

        #region Update

        public static Notification UpdateNotificationEntityStatus(ApplicationDbContext db, Guid? notificationId, Notification action, EntityStatusEnum newStatus, IPrincipal user)
        {
            if (action == null)
                action = NotificationHelpers.GetNotification(db, notificationId.Value);

            action.EntityStatus = newStatus;
            action.RecordChange = RecordChangeEnum.StatusChange;
            action.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
            action.RecordChangeOn = DateTime.Now;

            db.Entry(action).State = EntityState.Modified;
            db.SaveChanges();

            return action;
        }

        public static Notification UpdateEntityStatus(ApplicationDbContext db, Guid notificationId, EntityStatusEnum entityStatus, IPrincipal user)
        {
            Notification notification = GetNotification(db, notificationId);
            notification.EntityStatus = entityStatus;
            notification.RecordChange = RecordChangeEnum.StatusChange;
            notification.RecordChangeOn = DateTime.Now;
            notification.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);

            db.Entry(notification).State = EntityState.Modified;
            db.SaveChanges();

            return notification;
        }

        #endregion

        #region Remove

        public static void RemoveNotificationsForOffer(ApplicationDbContext db, Guid offerId, IPrincipal user)
        {
            List<Notification> notifications = (from n in db.Notifications
                                                where n.ReferenceKey == offerId
                                                select n).Distinct().ToList();

            foreach (Notification notification in notifications)
                UpdateNotificationEntityStatus(db, null, notification, EntityStatusEnum.Closed, user);
        }

        #endregion
    }

    public static class NotificationViewHelpers
    {
        #region Get

        public static List<NotificationViewModel> GetNotificationsViewModelForOrganisationFromUser(ApplicationDbContext db, IPrincipal user, bool getHistory)
        {
            List<NotificationViewModel> list = new List<NotificationViewModel>();

            foreach (Notification notification in NotificationHelpers.GetNotificationsForOrganisationFromUser(db, user, getHistory))
            {
                //build view
                NotificationViewModel view = CreateNotificationsViewModel(db, notification);

                list.Add(view);
            }

            return list;
        }

        #endregion

        #region Create

        //Build a NotificationsViewModel record from a Notification
        public static NotificationViewModel CreateNotificationsViewModel(ApplicationDbContext db, Notification notification)
        {
            string referenceInfo = "";

            switch (notification.NotificationType)
            {
                case NotificationTypeEnum.NewOfferReceived:
                    Offer offer1 = OfferHelpers.GetOffer(db, notification.ReferenceKey);
                    referenceInfo = offer1.ItemDescription + " x " + offer1.CurrentOfferQuantity.ToString();
                    break;
                case NotificationTypeEnum.CounterOfferReceived:
                    Offer offer2 = OfferHelpers.GetOffer(db, notification.ReferenceKey);
                    referenceInfo = offer2.ItemDescription + " x " + offer2.CounterOfferQuantity.ToString();
                    break;
                case NotificationTypeEnum.NewOrderReceived:
                    Order order = OrderHelpers.GetOrder(db, notification.ReferenceKey);
                    switch (order.ListingType)
                    {
                        case ListingTypeEnum.Available:
                            AvailableListing listingA = AvailableListingHelpers.GetAvailableListing(db, order.ListingId.Value);
                            referenceInfo = listingA.ItemDescription = " x " + order.OrderQuanity;
                            break;
                        case ListingTypeEnum.Requirement:
                            RequiredListing listingB = RequiredListingHelpers.GetRequiredListing(db, order.ListingId.Value);
                            referenceInfo = listingB.ItemDescription = " x " + order.OrderQuanity;
                            break;
                    }
                    break;
            }

            //build view
            NotificationViewModel view = new NotificationViewModel()
            {
                NotificationId = notification.NotificationId,
                NotificationType = notification.NotificationType,
                NotificationDescription = notification.NotificationDescription,
                ReferenceInformation = referenceInfo,
                AppUser = AppUserHelpers.GetAppUser(db, notification.AppUserId.Value),
                ChangedOn = notification.RecordChangeOn,
                ChangedBy = AppUserHelpers.GetAppUserName(db, notification.RecordChangeBy)
            };

            return view;
        }


        #endregion
    }
}