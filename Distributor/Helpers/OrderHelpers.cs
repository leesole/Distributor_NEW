using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.OfferEnums;
using static Distributor.Enums.OrderEnums;

namespace Distributor.Helpers
{
    public static class OrderHelpers
    {
        public static Order CreateOrder(ApplicationDbContext db, Offer offer, IPrincipal user)
        {
            AppUser appUser = AppUserHelpers.GetAppUser(db, user);

            decimal orderQty = 0.00M;

            switch (offer.OfferStatus)
            {
                case OfferStatusEnum.New:
                case OfferStatusEnum.Reoffer:
                    orderQty = offer.CurrentOfferQuantity;
                    break;
                case OfferStatusEnum.Countered:
                    orderQty = offer.CounterOfferQuantity.Value;
                    break;
            }

            //Update Listing values
            switch (offer.ListingType)
            {
                case ListingTypeEnum.Available:
                    AvailableListingHelpers.UpdateAvailableListingQuantities(db, offer.ListingId, ListingQuantityChange.Subtract, orderQty, user);
                    break;
                case ListingTypeEnum.Requirement:
                    RequiredListingHelpers.UpdateRequiredListingQuantities(db, offer.ListingId, ListingQuantityChange.Subtract, orderQty, user);
                    break;
            }

            Order order = new Order()
            {
                OrderId = Guid.NewGuid(),
                ListingType = offer.ListingType,
                OrderQuanity = orderQty,
                OrderInStatus = OrderInStatusEnum.New,
                OrderOutStatus = OrderOutStatusEnum.New,
                OrderCreationDateTime = DateTime.Now,
                OrderOriginatorAppUserId = appUser.AppUserId,
                OrderOriginatorOrganisationId = appUser.OrganisationId,
                OrderOriginatorDateTime = DateTime.Now,
                OfferId = offer.OfferId,
                OfferOriginatorAppUserId = offer.LastOfferOriginatorAppUserId ?? offer.OfferOriginatorAppUserId,
                OfferOriginatorOrganisationId = offer.OfferOriginatorOrganisationId,
                ListingId = offer.ListingId,
                ListingOriginatorAppUserId = offer.ListingOriginatorAppUserId,
                ListingOriginatorOrganisationId = offer.ListingOriginatorOrganisationId
            };

            db.Orders.Add(order);
            db.SaveChanges();

            return order;
        }
    }

    public static class OrderViewHelpers
    {
        #region Get

        public static OrderManageViewModel GetOrderManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            OrderManageViewModel model = new OrderManageViewModel()
            {
                OrdersInViewModel = CreateOrdersInViewModel(db, organisationId),
                OrdersOutViewModel = CreateOrdersOutViewModel(db, organisationId)
            };

            return model;
        }

        #endregion

        #region Create

        public static List<OrderInViewModel> CreateOrdersInViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<OrderInViewModel> list = (from o in db.Orders
                                           where ((o.ListingType == ListingTypeEnum.Available && o.OfferOriginatorOrganisationId == organisationId) || 
                                                  (o.ListingType == ListingTypeEnum.Requirement && o.ListingOriginatorOrganisationId == organisationId))
                                           select new OrderInViewModel()
                                           {
                                               OrderId = o.OrderId,
                                               OrderQuanity = o.OrderQuanity,
                                               OrderInStatus = o.OrderInStatus,
                                               OrderCreationDateTime = o.OrderCreationDateTime,
                                               OrderCollectedDateTime = o.OrderCollectedDateTime,
                                               OrderCollected = o.OrderCollectedDateTime.HasValue,
                                               OrderReceivedDateTime = o.OrderReceivedDateTime,
                                               OrderReceived = o.OrderReceivedDateTime.HasValue,
                                               OrderClosedDateTime = o.OrderClosedDateTime,
                                               OrderClosed = o.OrderClosedDateTime.HasValue
                                           }).ToList();

            return list;
        }

        public static List<OrderOutViewModel> CreateOrdersOutViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<OrderOutViewModel> list = (from o in db.Orders
                                           where ((o.ListingType == ListingTypeEnum.Available && o.ListingOriginatorOrganisationId == organisationId) ||
                                                  (o.ListingType == ListingTypeEnum.Requirement && o.OfferOriginatorOrganisationId == organisationId))
                                           select new OrderOutViewModel()
                                           {
                                               OrderId = o.OrderId,
                                               OrderQuanity = o.OrderQuanity,
                                               OrderOutStatus = o.OrderOutStatus,
                                               OrderCreationDateTime = o.OrderCreationDateTime,
                                               OrderDistributionDateTime = o.OrderDistributionDateTime,
                                               OrderDistributed = o.OrderDistributionDateTime.HasValue,
                                               OrderDeliveredDateTime = o.OrderDeliveredDateTime,
                                               OrderDelivered = o.OrderDeliveredDateTime.HasValue,
                                               OrderClosedDateTime = o.OrderClosedDateTime,
                                               OrderClosed = o.OrderClosedDateTime.HasValue
                                           }).ToList();

            return list;
        }

        #endregion
    }
}