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
using static Distributor.Enums.UserNotificationEnums;
using static Distributor.Enums.UserTaskEnums;

namespace Distributor.Helpers
{
    public static class OfferHelpers
    {
        #region Get

        public static Offer GetOffer(ApplicationDbContext db, Guid offerId)
        {
            return db.Offers.Find(offerId);
        }

        public static Offer GetOfferForListingByUser(ApplicationDbContext db, Guid listingId, Guid appUserId, Guid organisationId, LevelEnum listingPrivacyLevel)
        {
            Offer offer = new Offer();

            switch (listingPrivacyLevel)
            {
                case LevelEnum.User:
                    offer = (from o in db.Offers
                             where (o.ListingId == listingId && o.OfferOriginatorAppUserId == appUserId)
                             select o).Distinct().FirstOrDefault();
                    break;
                case LevelEnum.Organisation:
                    offer = (from o in db.Offers
                             where (o.ListingId == listingId && o.OfferOriginatorOrganisationId == organisationId)
                             select o).Distinct().FirstOrDefault();
                    break;
            }

            return offer;
        }

        public static List<Offer> GetOpenOffersCreatedForOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            List<Offer> list = (from o in db.Offers
                                join org in db.Organisations on o.ListingOriginatorOrganisationId equals org.OrganisationId
                                where (o.OfferOriginatorOrganisationId == organisationId && (o.OfferStatus == OfferStatusEnum.New || o.OfferStatus == OfferStatusEnum.Countered || o.OfferStatus == OfferStatusEnum.Reoffer))
                                select o).Distinct().ToList();

            return list;
        }

        public static List<Offer> GetAllOffersCreatedClosedForWeekForOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            DateTime weekAgo = DateTime.Now.AddDays(-7);

            List<Offer> list = (from o in db.Offers
                                join org in db.Organisations on o.ListingOriginatorOrganisationId equals org.OrganisationId
                                where (o.OfferOriginatorOrganisationId == organisationId && ((o.OfferStatus == OfferStatusEnum.Accepted && o.AcceptedOn >= weekAgo) || (o.OfferStatus == OfferStatusEnum.Rejected && o.RejectedOn >= weekAgo) || (o.OfferStatus == OfferStatusEnum.ClosedNoStock && o.ClosedNoStockOn >= weekAgo))) 
                                select o).Distinct().ToList();

            return list;
        }

        public static List<Offer> GetOpenOffersReceivedForOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            List<Offer> list = (from o in db.Offers
                                join org in db.Organisations on o.OfferOriginatorOrganisationId equals org.OrganisationId
                                where (o.ListingOriginatorOrganisationId == organisationId && (o.OfferStatus == OfferStatusEnum.New || o.OfferStatus == OfferStatusEnum.Countered || o.OfferStatus == OfferStatusEnum.Reoffer))
                                select o).Distinct().ToList();

            return list;
        }

        public static List<Offer> GetAllOffersReceivedClosedForWeekForOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            DateTime weekAgo = DateTime.Now.AddDays(-7);

            List<Offer> list = (from o in db.Offers
                                join org in db.Organisations on o.OfferOriginatorOrganisationId equals org.OrganisationId
                                where (o.ListingOriginatorOrganisationId == organisationId && ((o.OfferStatus == OfferStatusEnum.Accepted && o.AcceptedOn >= weekAgo) || (o.OfferStatus == OfferStatusEnum.Rejected && o.RejectedOn >= weekAgo) || (o.OfferStatus == OfferStatusEnum.ClosedNoStock && o.ClosedNoStockOn >= weekAgo)))
                                select o).Distinct().ToList();

            return list;
        }

        #endregion

        #region Create

        public static Offer CreateOffer(ApplicationDbContext db, Guid listingId, decimal? offerQty, ListingTypeEnum listingType, AppUser currentUser, IPrincipal user)
        {
            if (currentUser == null)
                currentUser = AppUserHelpers.GetAppUser(db, user);

            Guid listingOrigAppUserId = Guid.Empty;
            Guid listingOrigOrgId = Guid.Empty;
            DateTime listingOrigDateTime = DateTime.MinValue;
            string itemDescription = "";

            //Get originator information for the correct listing
            if (listingType == ListingTypeEnum.Available)
            {
                AvailableListing availableListing = AvailableListingHelpers.GetAvailableListing(db, listingId);
                listingOrigAppUserId = availableListing.ListingOriginatorAppUserId;
                listingOrigOrgId = availableListing.ListingOriginatorOrganisationId;
                listingOrigDateTime = availableListing.ListingOriginatorDateTime;
                itemDescription = availableListing.ItemDescription;
            }
            else
            {
                RequiredListing requiredListing = RequiredListingHelpers.GetRequiredListing(db, listingId);
                listingOrigAppUserId = requiredListing.ListingOriginatorAppUserId;
                listingOrigOrgId = requiredListing.ListingOriginatorOrganisationId;
                listingOrigDateTime = requiredListing.ListingOriginatorDateTime;
                itemDescription = requiredListing.ItemDescription;
            }

            //create offer
            Offer offer = new Offer()
            {
                OfferId = Guid.NewGuid(),
                ListingId = listingId,
                ListingType = listingType,
                OfferStatus = OfferStatusEnum.New,
                ItemDescription = itemDescription,
                CurrentOfferQuantity = offerQty.Value,
                OfferOriginatorAppUserId = currentUser.AppUserId,
                OfferOriginatorOrganisationId = currentUser.OrganisationId,
                OfferOriginatorDateTime = DateTime.Now,
                ListingOriginatorAppUserId = listingOrigAppUserId,
                ListingOriginatorOrganisationId = listingOrigOrgId,
                ListingOriginatorDateTime = listingOrigDateTime
            };

            db.Offers.Add(offer);
            db.SaveChanges();

            //Create Action
            Organisation org = OrganisationHelpers.GetOrganisation(db, currentUser.OrganisationId);
            NotificationHelpers.CreateNotification(db, NotificationTypeEnum.NewOfferReceived, "New offer received from " + org.OrganisationName, offer.OfferId, listingOrigAppUserId, listingOrigOrgId, user);

            return offer;
        }

        public static List<Offer> CreateOffers(ApplicationDbContext db, AvailableListingGeneralViewListModel availableModel, RequiredListingGeneralViewListModel requiredModel, ListingTypeEnum listingType, IPrincipal user)
        {
            AppUser currentUser = AppUserHelpers.GetAppUser(db, user);

            List<Offer> createdOffers = new List<Offer>();

            if (listingType == ListingTypeEnum.Available)
            {
                //go through the records, if there are offer qty's > 0 then create an offer from these
                foreach (AvailableListingGeneralViewModel item in availableModel.Listing)
                {
                    if (item.OfferQty.HasValue)
                        if (item.OfferQty > 0)
                            //only create offers if they don't already exist
                            if (OfferHelpers.GetOfferForListingByUser(db, item.ListingId, currentUser.AppUserId, currentUser.OrganisationId, LevelEnum.Organisation) == null)
                                createdOffers.Add(CreateOffer(db, item.ListingId, item.OfferQty, listingType, currentUser, user));
                }
            }

            if (listingType == ListingTypeEnum.Requirement)
            {
                //go through the records, if there are offer qty's > 0 then create an offer from these
                foreach (RequiredListingGeneralViewModel item in requiredModel.Listing)
                {
                    if (item.RequiredQty.HasValue)
                        if (item.RequiredQty > 0)
                            //only create offers if they don't already exist
                            if (OfferHelpers.GetOfferForListingByUser(db, item.ListingId, currentUser.AppUserId, currentUser.OrganisationId, LevelEnum.Organisation) == null)
                                createdOffers.Add(CreateOffer(db, item.ListingId, item.RequiredQty, listingType, currentUser, user));
                }
            }

            return createdOffers;
        }

        #endregion

        #region Update

        public static Offer UpdateOffer(ApplicationDbContext db, Guid offerid, OfferStatusEnum offerStatus,  decimal? currentOfferQuantity, decimal? counterOfferQuantity, IPrincipal user)
        {
            Offer offer = OfferHelpers.GetOffer(db, offerid);

            switch (offerStatus)
            {
                case OfferStatusEnum.Reoffer: //update offer value and move counter value to previous
                    if (currentOfferQuantity.HasValue)
                    {
                        offer.CurrentOfferQuantity = currentOfferQuantity.Value;
                        offer.PreviousCounterOfferQuantity = offer.CounterOfferQuantity;
                        offer.CounterOfferQuantity = null;
                        offer.LastOfferOriginatorAppUserId = AppUserHelpers.GetAppUserIdFromUser(user);
                        offer.LastOfferOriginatorDateTime = DateTime.Now;
                        offer.OfferStatus = offerStatus;

                        db.Entry(offer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    break;
                case OfferStatusEnum.Countered: //update counter value and move current offer to previous offer
                    if (counterOfferQuantity.HasValue)
                    {
                        offer.CounterOfferQuantity = counterOfferQuantity;
                        offer.PreviousOfferQuantity = offer.CurrentOfferQuantity;
                        offer.CurrentOfferQuantity = 0.00M;
                        if (!offer.CounterOfferOriginatorOrganisationId.HasValue)
                        {
                            AppUser appUser = AppUserHelpers.GetAppUser(db, AppUserHelpers.GetAppUserIdFromUser(user));
                            offer.CounterOfferOriginatorAppUserId = appUser.AppUserId;
                            offer.CounterOfferOriginatorDateTime = DateTime.Now;
                            offer.CounterOfferOriginatorOrganisationId = appUser.OrganisationId;
                        }
                        else
                        {
                            offer.LastCounterOfferOriginatorAppUserId = AppUserHelpers.GetAppUserIdFromUser(user);
                            offer.LastCounterOfferOriginatorDateTime = DateTime.Now;
                        }
                        offer.OfferStatus = offerStatus;

                        db.Entry(offer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    break;
            }
            //Create Action
            Organisation org = OrganisationHelpers.GetOrganisation(db, AppUserHelpers.GetAppUser(db, AppUserHelpers.GetAppUserIdFromUser(user)).OrganisationId);
            NotificationHelpers.CreateNotification(db, NotificationTypeEnum.NewOfferReceived, "New offer received from " + org.OrganisationName, offer.OfferId, offer.ListingOriginatorAppUserId.Value, offer.ListingOriginatorOrganisationId.Value, user);

            return offer;
        }

        public static Offer UpdateOffer(ApplicationDbContext db, OfferViewModel model, IPrincipal user)
        {
            Offer offer = new Offer();

            if (model.Type == "created")
                if (model.CurrentOfferQuantity > 0 && model.EditableQuantity)
                    offer = UpdateOffer(db, model.OfferId, OfferStatusEnum.Reoffer, model.CurrentOfferQuantity, null, user);

            if (model.Type == "received")
                if (model.CounterOfferQuantity > 0 && model.EditableQuantity)
                    offer = UpdateOffer(db, model.OfferId, OfferStatusEnum.Countered, null, model.CounterOfferQuantity, user);

            return offer;
        }

        public static void UpdateOffers(ApplicationDbContext db, List<OfferManageViewOffersModel> list, IPrincipal user)
        {
            //Update Offer value - remove counter
            foreach (OfferManageViewOffersModel item in list)
                if (item.CurrentOfferQuantity != null && item.CurrentOfferQuantity > 0 && item.EditableQuantity)
                    UpdateOffer(db, item.OfferId, OfferStatusEnum.Reoffer, item.CurrentOfferQuantity, null, user);
        }

        public static void UpdateCounterOffers(ApplicationDbContext db, List<OfferManageViewOffersModel> list, IPrincipal user)
        {
            //Update Counter Offer value, move current offer to previous
            foreach (OfferManageViewOffersModel item in list)
                if (item.CounterOfferQuantity != null && item.CounterOfferQuantity > 0 && item.EditableQuantity)
                    UpdateOffer(db, item.OfferId, OfferStatusEnum.Countered, null, item.CounterOfferQuantity, user);
        }

        public static Offer AcceptOffer(ApplicationDbContext db, Guid offerId, IPrincipal user)
        {
            Offer offer = db.Offers.Find(offerId);
            AppUser appUser = AppUserHelpers.GetAppUser(db, user);

            Order order = OrderHelpers.CreateOrder(db, offer, user);

            offer.OfferStatus = OfferStatusEnum.Accepted;
            offer.AcceptedBy = appUser.AppUserId;
            offer.AcceptedOn = DateTime.Now;
            offer.OrderId = order.OrderId;
            offer.OrderOriginatorAppUserId = appUser.AppUserId;
            offer.OrderOriginatorOrganisationId = appUser.OrganisationId;
            offer.OrderOriginatorDateTime = DateTime.Now;

            db.Entry(offer).State = EntityState.Modified;
            db.SaveChanges();

            //set any related actions to Closed
            NotificationHelpers.RemoveNotificationsForOffer(db, offerId, user);

            //set any related current offers to closed if there is no stock left (currentOfferQuantity = 0)
            if (offer.CurrentOfferQuantity == 0)
                OfferHelpers.CloseOffersRelatedToListing(db, offer.ListingId, user);

            //Create Action to show order ready
            Organisation org = OrganisationHelpers.GetOrganisation(db, offer.OfferOriginatorOrganisationId);
            NotificationHelpers.CreateNotification(db, NotificationTypeEnum.NewOrderReceived, "New order received from " + org.OrganisationName, order.OrderId, appUser.AppUserId, org.OrganisationId, user);

            return offer;
        }

        public static Offer RejectOffer(ApplicationDbContext db, Guid offerId, IPrincipal user)
        {
            Offer offer = db.Offers.Find(offerId);
            AppUser appUser = AppUserHelpers.GetAppUser(db, user);

            offer.OfferStatus = OfferStatusEnum.Rejected;
            offer.RejectedBy = appUser.AppUserId;
            offer.RejectedOn = DateTime.Now;

            db.Entry(offer).State = EntityState.Modified;
            db.SaveChanges();

            //set any related actions to Closed
            NotificationHelpers.RemoveNotificationsForOffer(db, offerId, user);

            return offer;
        }

        public static void CloseOffersRelatedToListing(ApplicationDbContext db, Guid listingId, IPrincipal user)
        {
            List<Offer> list = (from o in db.Offers
                                where o.ListingId == listingId && (o.OfferStatus == OfferStatusEnum.New || o.OfferStatus == OfferStatusEnum.Countered || o.OfferStatus == OfferStatusEnum.Reoffer)
                                select o).Distinct().ToList();

            foreach (Offer offer in list)
                UpdateStatus(db, offer.OfferId, OfferStatusEnum.ClosedNoStock, user);
        }

        public static Offer UpdateStatus(ApplicationDbContext db, Guid offerId, OfferStatusEnum newStatus, IPrincipal user)
        {
            Offer offer = OfferHelpers.GetOffer(db, offerId);

            offer.OfferStatus = newStatus;

            if (newStatus == OfferStatusEnum.ClosedNoStock)
            {
                offer.ClosedNoStockBy = AppUserHelpers.GetAppUserIdFromUser(user);
                offer.ClosedNoStockOn = DateTime.Now;
            }

            db.Entry(offer).State = EntityState.Modified;
            db.SaveChanges();

            return offer;
        }
        
        #endregion
    }

    public static class OfferViewHelpers
    {
        #region Get

        public static OfferManageViewModel GetOfferManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            OfferManageViewModel model = new OfferManageViewModel()
            {
                OfferManageViewOffersCreated = CreateActiveOffersCreatedByOrganisation(db, organisationId),
                OfferManageViewOffersReceived = CreateActiveOffersReceivedByOrganisation(db, organisationId)
            };

            //these flags are set to true if any single entry in the list has an editable quantity (ie. the user can enter a value).
            //If these flags are false then we use these to remove the uneccesary footer parts to the table that allows submission and reset of offers
            model.EditableEntriesCreated = model.OfferManageViewOffersCreated.Any(x => x.EditableQuantity);
            model.EditableEntriesReceived = model.OfferManageViewOffersReceived.Any(x => x.EditableQuantity);

            return model;
        }

        public static OfferViewModel GetOfferViewModel(ApplicationDbContext db, HttpRequestBase request, Guid offerId, string breadcrumb, string callingActionDisplayName, bool displayOnly, string type, bool? recalled, string controllerValue, string actionValue, IPrincipal user)
        {
            AppUser currentUser = AppUserHelpers.GetAppUser(db, user);
            Dictionary<int, string> breadcrumbDictionary = new Dictionary<int, string>();
            breadcrumbDictionary.Add(0, breadcrumb);

            if (!recalled.HasValue || recalled.Value != true)
                GeneralHelpers.GetCallingDetailsFromRequest(request, controllerValue, actionValue, out controllerValue, out actionValue);

            Offer offer = OfferHelpers.GetOffer(db, offerId);

            OfferViewModel model = new OfferViewModel()
            {
                DisplayOnly = displayOnly,
                Breadcrumb = breadcrumb,
                BreadcrumbDictionary = breadcrumbDictionary,
                Type = type,
                OfferId = offer.OfferId,
                ListingId = offer.ListingId,
                ListingType = offer.ListingType,
                OfferStatus = offer.OfferStatus,
                ItemDescription = offer.ItemDescription,
                QuantityOutstanding = OfferViewHelpers.GetListingQuantityForListingIdListingType(db, offer.ListingId, offer.ListingType),
                CurrentOfferQuantity = offer.CurrentOfferQuantity,
                PreviousOfferQuantity = offer.PreviousOfferQuantity,
                CounterOfferQuantity = offer.CounterOfferQuantity,
                PreviousCounterOfferQuantity = offer.PreviousCounterOfferQuantity,
                OfferOriginatorAppUser = AppUserHelpers.GetAppUserName(db, offer.OfferOriginatorAppUserId),
                OfferOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, offer.OfferOriginatorOrganisationId),
                OfferOriginatorDateTime = offer.OfferOriginatorDateTime,
                YourOrganisationId = currentUser.OrganisationId,
                OfferOriginatorOrganisationId = offer.OfferOriginatorOrganisationId,
                RejectedBy = AppUserHelpers.GetAppUserName(db, offer.RejectedBy),
                RejectedOn = offer.RejectedOn,
                LastOfferOriginatorAppUser = AppUserHelpers.GetAppUserName(db, offer.LastOfferOriginatorAppUserId),
                LastOfferOriginatorDateTime = offer.LastOfferOriginatorDateTime,
                ListingOriginatorAppUser = AppUserHelpers.GetAppUserName(db, offer.ListingOriginatorAppUserId),
                ListingOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, offer.ListingOriginatorOrganisationId),
                ListingOriginatorDateTime = offer.ListingOriginatorDateTime,
                CounterOfferOriginatorAppUser = AppUserHelpers.GetAppUserName(db, offer.CounterOfferOriginatorAppUserId),
                CounterOfferOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, offer.CounterOfferOriginatorOrganisationId),
                CounterOfferOriginatorDateTime = offer.CounterOfferOriginatorDateTime,
                CounterOfferOriginatorOrganisationId = offer.CounterOfferOriginatorOrganisationId,
                LastCounterOfferOriginatorAppUser = AppUserHelpers.GetAppUserName(db, offer.LastCounterOfferOriginatorAppUserId),
                LastCounterOfferOriginatorDateTime = offer.LastCounterOfferOriginatorDateTime,
                OrderOriginatorAppUser = AppUserHelpers.GetAppUserName(db, offer.OrderOriginatorAppUserId),
                OrderOriginatorOrganisation = OrganisationHelpers.GetOrganisationName(db, offer.OrderOriginatorOrganisationId),
                OrderOriginatorDateTime = offer.OrderOriginatorDateTime,
                CallingAction = actionValue,
                CallingActionDisplayName = callingActionDisplayName,
                CallingController = controllerValue,
                BreadcrumbTrail = breadcrumbDictionary
            };

            if (displayOnly)
                model.EditableQuantity = false;
            else
            {
                if (type == "created")
                    model.EditableQuantity = offer.CurrentOfferQuantity == 0;
                else if (type == "received")
                    model.EditableQuantity = offer.CurrentOfferQuantity > 0;
            }

            return model;
        }

        #endregion

        #region Create

        public static List<OfferManageViewOffersModel> CreateActiveOffersCreatedByOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            List<OfferManageViewOffersModel> list = (from o in db.Offers
                                                     join org in db.Organisations on o.ListingOriginatorOrganisationId equals org.OrganisationId
                                                     where (o.OfferOriginatorOrganisationId == organisationId && o.OfferStatus != OfferStatusEnum.Accepted && o.OfferStatus != OfferStatusEnum.Rejected && o.OfferStatus != OfferStatusEnum.ClosedNoStock)
                                                     select new OfferManageViewOffersModel()
                                                     {
                                                         OfferId = o.OfferId,
                                                         ListingId = o.ListingId,
                                                         ListingOrganisation = org.OrganisationName,
                                                         ListingType = o.ListingType,
                                                         OfferStatus = o.OfferStatus,
                                                         ItemDescription = o.ItemDescription,
                                                         CurrentOfferQuantity = o.CurrentOfferQuantity,
                                                         PreviousOfferQuantity = o.PreviousOfferQuantity,
                                                         CounterOfferQuantity = o.CounterOfferQuantity,
                                                         PreviousCounterOfferQuantity = o.PreviousCounterOfferQuantity,
                                                         Rejected = o.RejectedBy.HasValue,
                                                         EditableQuantity = o.CurrentOfferQuantity == 0
                                                     }).ToList();

            return AddListingQuantityToOfferManageViewOffersModel(db, list);
        }

        public static List<OfferManageViewOffersModel> CreateActiveOffersReceivedByOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            List<OfferManageViewOffersModel> list = (from o in db.Offers
                                                     join org in db.Organisations on o.OfferOriginatorOrganisationId equals org.OrganisationId
                                                     where (o.ListingOriginatorOrganisationId == organisationId && o.OfferStatus != OfferStatusEnum.Accepted && o.OfferStatus != OfferStatusEnum.Rejected && o.OfferStatus != OfferStatusEnum.ClosedNoStock)
                                                        select new OfferManageViewOffersModel()
                                                        {
                                                            OfferId = o.OfferId,
                                                            ListingId = o.ListingId,
                                                            ListingOrganisation = org.OrganisationName,
                                                            ListingType = o.ListingType,
                                                            OfferStatus = o.OfferStatus,
                                                            ItemDescription = o.ItemDescription,
                                                            CurrentOfferQuantity = o.CurrentOfferQuantity,
                                                            PreviousOfferQuantity = o.PreviousOfferQuantity,
                                                            CounterOfferQuantity = o.CounterOfferQuantity,
                                                            PreviousCounterOfferQuantity = o.PreviousCounterOfferQuantity,
                                                            Rejected = o.RejectedBy.HasValue,
                                                            EditableQuantity = o.CurrentOfferQuantity > 0
                                                        }).ToList();

            return AddListingQuantityToOfferManageViewOffersModel(db, list);
        }

        public static List<OfferManageViewOffersModel> AddListingQuantityToOfferManageViewOffersModel(ApplicationDbContext db, List<OfferManageViewOffersModel> list)
        {
            foreach (OfferManageViewOffersModel item in list)
                item.QuantityOutstanding = GetListingQuantityForListingIdListingType(db, item.ListingId, item.ListingType);

            return list;
        }

        public static decimal GetListingQuantityForListingIdListingType(ApplicationDbContext db, Guid listingId, ListingTypeEnum listingType)
        {
            decimal quantityOutstanding = 0.00M;

            switch (listingType)
            {
                case ListingTypeEnum.Available:
                    quantityOutstanding = AvailableListingHelpers.GetAvailableListing(db, listingId).QuantityOutstanding;
                    break;
                case ListingTypeEnum.Requirement:
                    quantityOutstanding = RequiredListingHelpers.GetRequiredListing(db, listingId).QuantityOutstanding;
                    break;
            }

            return quantityOutstanding;
        }

        public static List<OfferManageViewOffersModel> CreateOffersCreatedHistoryManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<OfferManageViewOffersModel> list = (from o in db.Offers
                                                     join org in db.Organisations on o.ListingOriginatorOrganisationId equals org.OrganisationId
                                                     where (o.OfferOriginatorOrganisationId == organisationId && (o.OfferStatus == OfferStatusEnum.Accepted || o.OfferStatus == OfferStatusEnum.Rejected || o.OfferStatus == OfferStatusEnum.ClosedNoStock))
                                                     select new OfferManageViewOffersModel()
                                                     {
                                                         OfferId = o.OfferId,
                                                         ListingId = o.ListingId,
                                                         ListingOrganisation = org.OrganisationName,
                                                         ListingType = o.ListingType,
                                                         OfferStatus = o.OfferStatus,
                                                         ItemDescription = o.ItemDescription,
                                                         CurrentOfferQuantity = o.CurrentOfferQuantity,
                                                         PreviousOfferQuantity = o.PreviousOfferQuantity,
                                                         CounterOfferQuantity = o.CounterOfferQuantity,
                                                         PreviousCounterOfferQuantity = o.PreviousCounterOfferQuantity,
                                                         Rejected = o.RejectedBy.HasValue,
                                                         EditableQuantity = false,
                                                         OrderId = o.OrderId,
                                                         OrderCreated = o.OrderId.HasValue
                                                     }).ToList();

            return list;
        }

        public static List<OfferManageViewOffersModel> CreateOffersReceivedHistoryManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<OfferManageViewOffersModel> list = (from o in db.Offers
                                                     join org in db.Organisations on o.OfferOriginatorOrganisationId equals org.OrganisationId
                                                     where (o.ListingOriginatorOrganisationId == organisationId && (o.OfferStatus == OfferStatusEnum.Accepted || o.OfferStatus == OfferStatusEnum.Rejected || o.OfferStatus == OfferStatusEnum.ClosedNoStock))
                                                     select new OfferManageViewOffersModel()
                                                     {
                                                         OfferId = o.OfferId,
                                                         ListingId = o.ListingId,
                                                         ListingOrganisation = org.OrganisationName,
                                                         ListingType = o.ListingType,
                                                         OfferStatus = o.OfferStatus,
                                                         ItemDescription = o.ItemDescription,
                                                         CurrentOfferQuantity = o.CurrentOfferQuantity,
                                                         PreviousOfferQuantity = o.PreviousOfferQuantity,
                                                         CounterOfferQuantity = o.CounterOfferQuantity,
                                                         PreviousCounterOfferQuantity = o.PreviousCounterOfferQuantity,
                                                         Rejected = o.RejectedBy.HasValue,
                                                         EditableQuantity = false,
                                                         OrderId = o.OrderId,
                                                         OrderCreated = o.OrderId.HasValue
                                                     }).ToList();

            return list;
        }

        #endregion
    }
}