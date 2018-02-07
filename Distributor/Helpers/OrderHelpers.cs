using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        #region Get

        public static Order GetOrder(ApplicationDbContext db, Guid orderId)
        {
            return db.Orders.Find(orderId);
        }

        #endregion
        
        #region Create

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

        #endregion

        #region Update

        public static void UpdateOrders(ApplicationDbContext db, OrderManageViewModel model, IPrincipal user)
        {
            //Update Orders In
            if (model.OrdersInViewModel != null)
                foreach (OrderInViewModel item in model.OrdersInViewModel)
                    UpdateOrderDates(db, item, null, user);

            //Update Orders Out
            if (model.OrdersOutViewModel != null)
                foreach (OrderOutViewModel item in model.OrdersOutViewModel)
                    UpdateOrderDates(db, null, item, user);
        }

        public static void UpdateOrderDates(ApplicationDbContext db, OrderInViewModel orderIn, OrderOutViewModel orderOut, IPrincipal user)
        {
            Guid currentAppUserId = AppUserHelpers.GetAppUserIdFromUser(user);

            if (orderIn != null)
            {
                Order order = GetOrder(db, orderIn.OrderId);
                if (orderIn.OrderCollected)
                {
                    order.OrderCollectedBy = currentAppUserId;
                    order.OrderCollectedDateTime = DateTime.Now;
                    order.OrderInStatus = OrderInStatusEnum.Collected;
                }
                if (orderIn.OrderReceived)
                {
                    order.OrderReceivedBy = currentAppUserId;
                    order.OrderReceivedDateTime = DateTime.Now;
                    order.OrderInStatus = OrderInStatusEnum.Received;
                }
                if (orderIn.OrderClosed)
                {
                    order.OrderClosedBy = currentAppUserId;
                    order.OrderClosedDateTime = DateTime.Now;
                    order.OrderInStatus = OrderInStatusEnum.Closed;
                }

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (orderOut != null)
            {
                Order order = GetOrder(db, orderOut.OrderId);
                if (orderOut.OrderDistributed)
                {
                    order.OrderDistributedBy = currentAppUserId;
                    order.OrderDistributionDateTime = DateTime.Now;
                    order.OrderOutStatus = OrderOutStatusEnum.Despatched;
                }
                if (orderOut.OrderDelivered)
                {
                    order.OrderDeliveredBy = currentAppUserId;
                    order.OrderDeliveredDateTime = DateTime.Now;
                    order.OrderOutStatus = OrderOutStatusEnum.Delivered;
                }
                if (orderOut.OrderClosed)
                {
                    order.OrderClosedBy = currentAppUserId;
                    order.OrderClosedDateTime = DateTime.Now;
                    order.OrderOutStatus = OrderOutStatusEnum.Closed;
                }

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        #endregion
    }

    public static class OrderViewHelpers
    {
        #region Get

        public static OrderManageViewModel GetOrderManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            OrderManageViewModel model = new OrderManageViewModel()
            {
                OrdersInViewModel = CreateOrdersInViewModel(db, organisationId, false),
                OrdersOutViewModel = CreateOrdersOutViewModel(db, organisationId, false)
            };

            return model;
        }

        public static OrderManageViewModel GetOrderManageViewModelHistory(ApplicationDbContext db, string type, Guid organisationId)
        {
            OrderManageViewModel model = new OrderManageViewModel();

            if (type == "in")
                model.OrdersInViewModel = CreateOrdersInViewModel(db, organisationId, true);
            if (type == "out")
                model.OrdersOutViewModel = CreateOrdersOutViewModel(db, organisationId, true);

            return model;
        }

        #endregion

        #region Create

        public static List<OrderInViewModel> CreateOrdersInViewModel(ApplicationDbContext db, Guid organisationId, bool history)
        {
            List<OrderInViewModel> list;

            if (history)
                list = (from o in db.Orders
                        where (((o.ListingType == ListingTypeEnum.Available && o.OfferOriginatorOrganisationId == organisationId) ||
                                (o.ListingType == ListingTypeEnum.Requirement && o.ListingOriginatorOrganisationId == organisationId)) &&
                                o.OrderInStatus == OrderInStatusEnum.Closed)
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
            else
                list = (from o in db.Orders
                        where (((o.ListingType == ListingTypeEnum.Available && o.OfferOriginatorOrganisationId == organisationId) || 
                                (o.ListingType == ListingTypeEnum.Requirement && o.ListingOriginatorOrganisationId == organisationId)) &&
                                o.OrderInStatus != OrderInStatusEnum.Closed)
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

        public static List<OrderOutViewModel> CreateOrdersOutViewModel(ApplicationDbContext db, Guid organisationId, bool history)
        {
            List<OrderOutViewModel> list;

            if (history)
                list = (from o in db.Orders
                        where (((o.ListingType == ListingTypeEnum.Available && o.ListingOriginatorOrganisationId == organisationId) ||
                                (o.ListingType == ListingTypeEnum.Requirement && o.OfferOriginatorOrganisationId == organisationId)) &&
                                o.OrderOutStatus == OrderOutStatusEnum.Closed)
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
            else
                list = (from o in db.Orders
                        where (((o.ListingType == ListingTypeEnum.Available && o.ListingOriginatorOrganisationId == organisationId) ||
                                (o.ListingType == ListingTypeEnum.Requirement && o.OfferOriginatorOrganisationId == organisationId)) &&
                                o.OrderOutStatus != OrderOutStatusEnum.Closed)
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