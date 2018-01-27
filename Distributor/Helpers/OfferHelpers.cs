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
using static Distributor.Enums.UserActionEnums;
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
            UserActionHelpers.CreateUserAction(db, ActionTypeEnum.NewOfferReceived, "New offer received from " + org.OrganisationName, offer.OfferId, currentUser.AppUserId, org.OrganisationId, user);

            return offer;
        }

        public static List<Offer> CreateOffers(ApplicationDbContext db, AvailableListingGeneralViewListModel model, ListingTypeEnum listingType, IPrincipal user)
        {
            AppUser currentUser = AppUserHelpers.GetAppUser(db, user);

            List<Offer> createdOffers = new List<Offer>();

            //go through the records, if there are offer qty's > 0 then create an offer from these
            foreach (AvailableListingGeneralViewModel item in model.Listing)
            {
                if (item.OfferQty.HasValue)
                    if (item.OfferQty > 0)
                        //only create offers if they don't already exist
                        if (OfferHelpers.GetOfferForListingByUser(db, item.ListingId, currentUser.AppUserId, currentUser.OrganisationId, LevelEnum.Organisation) == null)
                            createdOffers.Add(CreateOffer(db, item.ListingId, item.OfferQty, listingType, currentUser, user));
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

                        //Create Action
                        Organisation org = OrganisationHelpers.GetOrganisation(db, offer.CounterOfferOriginatorOrganisationId.Value);
                        UserActionHelpers.CreateUserAction(db, ActionTypeEnum.NewOfferReceived, "New offer received from " + org.OrganisationName, offer.OfferId, AppUserHelpers.GetAppUserIdFromUser(user), org.OrganisationId, user);
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

                        //Create Action
                        Organisation org = OrganisationHelpers.GetOrganisation(db, offer.OfferOriginatorOrganisationId);
                        UserActionHelpers.CreateUserAction(db, ActionTypeEnum.NewOfferReceived, "New offer received from " + org.OrganisationName, offer.OfferId, AppUserHelpers.GetAppUserIdFromUser(user), org.OrganisationId, user);
                    }
                    break;
            }

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
            offer.OrderId = order.OrderId;
            offer.OrderOriginatorAppUserId = appUser.AppUserId;
            offer.OrderOriginatorOrganisationId = appUser.OrganisationId;
            offer.OrderOriginatorDateTime = DateTime.Now;

            db.Entry(offer).State = EntityState.Modified;
            db.SaveChanges();

            //set any related actions to Closed
            UserActionHelpers.RemoveUserActionsForOffer(db, offerId, user);

            //Create Action to show order ready
            Organisation org = OrganisationHelpers.GetOrganisation(db, offer.OfferOriginatorOrganisationId);
            UserActionHelpers.CreateUserAction(db, ActionTypeEnum.NewOrderReceived, "New order received from " + org.OrganisationName, offer.OfferId, appUser.AppUserId, org.OrganisationId, user);

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
            UserActionHelpers.RemoveUserActionsForOffer(db, offerId, user);

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

        public static OfferViewModel GetOfferViewModel(ApplicationDbContext db, Guid offerId)
        {
            OfferViewModel model = (from o in db.Offers
                                    where o.OfferId == offerId
                                    select new OfferViewModel()
                                    {
                                        OfferId = o.OfferId,
                                        ListingId = o.ListingId,
                                        ListingType = o.ListingType,
                                        OfferStatus = o.OfferStatus,
                                        ItemDescription = o.ItemDescription,
                                        CurrentOfferQuantity = o.CurrentOfferQuantity,
                                        PreviousOfferQuantity = o.PreviousOfferQuantity,
                                        CounterOfferQuantity = o.CounterOfferQuantity,
                                        PreviousCounterOfferQuantity = o.PreviousCounterOfferQuantity,
                                        RejectedBy = o.RejectedBy,
                                        RejectedOn = o.RejectedOn,
                                        OfferOriginatorAppUserId = o.OfferOriginatorAppUserId,
                                        OfferOriginatorOrganisationId = o.OfferOriginatorOrganisationId,
                                        OfferOriginatorDateTime = o.OfferOriginatorDateTime,
                                        LastOfferOriginatorAppUserId = o.LastOfferOriginatorAppUserId,
                                        LastOfferOriginatorDateTime = o.LastOfferOriginatorDateTime,
                                        ListingOriginatorAppUserId = o.ListingOriginatorAppUserId,
                                        ListingOriginatorOrganisationId = o.ListingOriginatorOrganisationId,
                                        ListingOriginatorDateTime = o.ListingOriginatorDateTime,
                                        CounterOfferOriginatorAppUserId = o.CounterOfferOriginatorAppUserId,
                                        CounterOfferOriginatorOrganisationId = o.CounterOfferOriginatorOrganisationId,
                                        CounterOfferOriginatorDateTime = o.CounterOfferOriginatorDateTime,
                                        LastCounterOfferOriginatorAppUserId = o.LastCounterOfferOriginatorAppUserId,
                                        LastCounterOfferOriginatorDateTime = o.LastCounterOfferOriginatorDateTime,
                                        OrderId = o.OrderId,
                                        OrderOriginatorAppUserId = o.OrderOriginatorAppUserId,
                                        OrderOriginatorOrganisationId = o.OrderOriginatorOrganisationId,
                                        OrderOriginatorDateTime = o.OrderOriginatorDateTime
                                    }).FirstOrDefault();

            return model;
        }

        #endregion

        #region Create

        public static List<OfferManageViewOffersModel> CreateActiveOffersCreatedByOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            List<OfferManageViewOffersModel> list = (from o in db.Offers
                                                     where (o.OfferOriginatorOrganisationId == organisationId && o.OfferStatus != OfferStatusEnum.Accepted && o.OfferStatus != OfferStatusEnum.Rejected)
                                                     select new OfferManageViewOffersModel()
                                                     {
                                                         OfferId = o.OfferId,
                                                         ListingId = o.ListingId,
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
                                                        where (o.ListingOriginatorOrganisationId == organisationId && o.OfferStatus != OfferStatusEnum.Accepted && o.OfferStatus != OfferStatusEnum.Rejected)
                                                        select new OfferManageViewOffersModel()
                                                        {
                                                            OfferId = o.OfferId,
                                                            ListingId = o.ListingId,
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
            {
                switch (item.ListingType)
                {
                    case ListingTypeEnum.Available:
                        item.QuantityOutstanding = AvailableListingHelpers.GetAvailableListing(db, item.ListingId).QuantityOutstanding;
                        break;
                    case ListingTypeEnum.Requirement:
                        item.QuantityOutstanding = RequiredListingHelpers.GetRequiredListing(db, item.ListingId).QuantityOutstanding;
                        break;
                }
            }

            return list;
        }

        #endregion
    }
}