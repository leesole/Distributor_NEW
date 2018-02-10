using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.ItemEnums;
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

        public static Order UpdateOrder(ApplicationDbContext db, OrderViewModel model, IPrincipal user)
        {
            Order order = null;
            //Only update order if a flag is set to Y
            if (model.Type == "in" && (model.OrderCollected || model.OrderReceived || model.OrderInClosed))
                order = UpdateOrderDates(db, model.Type, model.OrderId, model.OrderCollected, model.OrderReceived, model.OrderInClosed, false, false, false, user);

            if (model.Type == "out" && (model.OrderDistributed || model.OrderDelivered || model.OrderOutClosed))
                order = UpdateOrderDates(db, model.Type, model.OrderId, false, false, false, model.OrderDistributed, model.OrderDelivered, model.OrderOutClosed, user);

            return order;
        }

        public static void UpdateOrders(ApplicationDbContext db, OrderManageViewModel model, IPrincipal user)
        {
            //Update Orders In
            if (model.OrdersInViewModel != null)
                foreach (OrderInViewModel item in model.OrdersInViewModel)
                    UpdateOrderDates(db, "in", item.OrderId, item.OrderCollected, item.OrderReceived, item.OrderInClosed, false, false, false, user);

            //Update Orders Out
            if (model.OrdersOutViewModel != null)
                foreach (OrderOutViewModel item in model.OrdersOutViewModel)
                    UpdateOrderDates(db, "out", item.OrderId, false, false, false, item.OrderDistributed, item.OrderDelivered, item.OrderOutClosed, user);
        }

        public static Order UpdateOrderDates(ApplicationDbContext db, string type, Guid orderId, bool orderCollected, bool orderReceived, bool orderInClosed, bool orderDistributed, bool orderDelivered, bool orderOutClosed, IPrincipal user)
        {
            Order order = OrderHelpers.GetOrder(db, orderId);
            Guid currentAppUserId = AppUserHelpers.GetAppUserIdFromUser(user);

            if (type == "in")
            {
                if (orderCollected)
                {
                    order.OrderCollectedBy = currentAppUserId;
                    order.OrderCollectedDateTime = DateTime.Now;
                    order.OrderInStatus = OrderInStatusEnum.Collected;
                }
                if (orderReceived)
                {
                    order.OrderReceivedBy = currentAppUserId;
                    order.OrderReceivedDateTime = DateTime.Now;
                    order.OrderInStatus = OrderInStatusEnum.Received;
                }
                if (orderInClosed)
                {
                    order.OrderInClosedBy = currentAppUserId;
                    order.OrderInClosedDateTime = DateTime.Now;
                    order.OrderInStatus = OrderInStatusEnum.Closed;
                }

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                //If order closed then add Action to Organisation of order out if they haven't closed their side yet saying this is closed  //LSLSLS

                //Organisation org = OrganisationHelpers.GetOrganisation(db, offer.OfferOriginatorOrganisationId);
                //UserActionHelpers.CreateUserAction(db, ActionTypeEnum.NewOrderReceived, "New order received from " + org.OrganisationName, offer.OfferId, appUser.AppUserId, org.OrganisationId, user);
            }

            if (type == "out")
            {
                if (orderDistributed)
                {
                    order.OrderDistributedBy = currentAppUserId;
                    order.OrderDistributionDateTime = DateTime.Now;
                    order.OrderOutStatus = OrderOutStatusEnum.Despatched;
                }
                if (orderDelivered)
                {
                    order.OrderDeliveredBy = currentAppUserId;
                    order.OrderDeliveredDateTime = DateTime.Now;
                    order.OrderOutStatus = OrderOutStatusEnum.Delivered;
                }
                if (orderOutClosed)
                {
                    order.OrderOutClosedBy = currentAppUserId;
                    order.OrderOutClosedDateTime = DateTime.Now;
                    order.OrderOutStatus = OrderOutStatusEnum.Closed;
                }

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                //If order closed then add Action to Organisation of order out if they haven't closed their side yet saying this is closed    //LSLSLS
            }

            return order;
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

        public static OrderViewModel GetOfferViewModel(ApplicationDbContext db, HttpRequestBase request, Guid orderId, string breadcrumb, string callingActionDisplayName, bool displayOnly, string type, bool? recalled, string controllerValue, string actionValue, IPrincipal user)
        {
            AppUser currentUser = AppUserHelpers.GetAppUser(db, user);
            
            Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
            breadcrumbDictionary.Add(0, breadcrumb);

            if (!recalled.HasValue || recalled.Value != true)
                GeneralHelpers.GetCallingDetailsFromRequest(request, controllerValue, actionValue, out controllerValue, out actionValue);

            Order order = OrderHelpers.GetOrder(db, orderId);

            string itemDescription = "";
            ItemTypeEnum itemType = ItemTypeEnum.Canned;
            string uoM = "";

            if (order.ListingType == ListingTypeEnum.Available)
            {
                AvailableListing listing = AvailableListingHelpers.GetAvailableListing(db, order.ListingId.Value);
                itemDescription = listing.ItemDescription;
                itemType = listing.ItemType;
                uoM = listing.UoM;
            }
            else
            {
                RequiredListing listing = RequiredListingHelpers.GetRequiredListing(db, order.ListingId.Value);
                itemDescription = listing.ItemDescription;
                itemType = listing.ItemType;
                uoM = listing.UoM;
            }

            OrderViewModel model = new OrderViewModel()
            {
                DisplayOnly = displayOnly,
                Breadcrumb = breadcrumb,
                BreadcrumbDictionary = breadcrumbDictionary,
                Type = type,
                OrderId = order.OrderId,
                ListingType = order.ListingType,
                ItemDescription = itemDescription,
                ItemType = itemType,
                UoM = uoM,
                OrderQuanity = order.OrderQuanity,
                OrderInStatus = order.OrderInStatus,
                OrderOutStatus = order.OrderOutStatus,
                OrderCreationDateTime = order.OrderCreationDateTime,
                OrderDistributionDateTime = order.OrderDistributionDateTime,
                OrderDistributedBy = AppUserHelpers.GetAppUserName(db, order.OrderDistributedBy),
                OrderDeliveredDateTime = order.OrderDeliveredDateTime,
                OrderDeliveredBy = AppUserHelpers.GetAppUserName(db, order.OrderDeliveredBy),
                OrderCollectedDateTime = order.OrderCollectedDateTime,
                OrderCollectedBy = AppUserHelpers.GetAppUserName(db, order.OrderCollectedBy),
                OrderReceivedDateTime = order.OrderReceivedDateTime,
                OrderReceivedBy = AppUserHelpers.GetAppUserName(db, order.OrderReceivedBy),
                OrderInClosedDateTime = order.OrderInClosedDateTime,
                OrderInClosedBy = AppUserHelpers.GetAppUserName(db, order.OrderInClosedBy),
                OrderOutClosedDateTime = order.OrderOutClosedDateTime,
                OrderOutClosedBy = AppUserHelpers.GetAppUserName(db, order.OrderOutClosedBy),
                OrderOriginatorAppUser = AppUserHelpers.GetAppUserName(db, order.OrderOriginatorAppUserId),
                OrderOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, order.OrderOriginatorOrganisationId),
                OrderOriginatorDateTime = order.OrderOriginatorDateTime,
                OfferId = order.OfferId,
                OfferOriginatorAppUser = AppUserHelpers.GetAppUserName(db, order.OfferOriginatorAppUserId),
                OfferOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, order.OfferOriginatorOrganisationId),
                ListingId = order.ListingId,
                ListingOriginatorAppUser = AppUserHelpers.GetAppUserName(db, order.ListingOriginatorAppUserId),
                ListingOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, order.ListingOriginatorOrganisationId),
                CallingAction = actionValue,
                CallingActionDisplayName = callingActionDisplayName,
                CallingController = controllerValue,
                BreadcrumbTrail = breadcrumbDictionary
            };

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
                            OrderInClosedDateTime = o.OrderInClosedDateTime,
                            OrderInClosed = o.OrderInClosedDateTime.HasValue
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
                            OrderInClosedDateTime = o.OrderInClosedDateTime,
                            OrderInClosed = o.OrderInClosedDateTime.HasValue
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
                            OrderOutClosedDateTime = o.OrderOutClosedDateTime,
                            OrderOutClosed = o.OrderOutClosedDateTime.HasValue
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
                            OrderOutClosedDateTime = o.OrderOutClosedDateTime,
                            OrderOutClosed = o.OrderOutClosedDateTime.HasValue
                        }).ToList();

            return list;
        }

        #endregion
    }
}