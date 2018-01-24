using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.OfferEnums;

namespace Distributor.Helpers
{
    public static class OfferHelpers
    {
        #region Get

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

        public static Offer CreateOffer(ApplicationDbContext db, Guid listingId, decimal? offerQty, ListingTypeEnum listingType, AppUser currentUser)
        {
            Guid listingOrigAppUserId = Guid.Empty;
            Guid listingOrigOrgId = Guid.Empty;
            DateTime listingOrigDateTime = DateTime.MinValue;

            //Get originator information for the correct listing
            if (listingType == ListingTypeEnum.Available)
            {
                AvailableListing availableListing = AvailableListingHelpers.GetAvailableListing(db, listingId);
                listingOrigAppUserId = availableListing.ListingOriginatorAppUserId;
                listingOrigOrgId = availableListing.ListingOriginatorOrganisationId;
                listingOrigDateTime = availableListing.ListingOriginatorDateTime;
            }
            else
            {
                RequiredListing requiredListing = RequiredListingHelpers.GetRequiredListing(db, listingId);
                listingOrigAppUserId = requiredListing.ListingOriginatorAppUserId;
                listingOrigOrgId = requiredListing.ListingOriginatorOrganisationId;
                listingOrigDateTime = requiredListing.ListingOriginatorDateTime;
            }

            //create offer
            Offer offer = new Offer()
            {
                OfferId = Guid.NewGuid(),
                ListingId = listingId,
                ListingType = listingType,
                OfferStatus = OfferStatusEnum.New,
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
                            createdOffers.Add(CreateOffer(db, item.ListingId, item.OfferQty, listingType, currentUser));
            }

            return createdOffers;
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
                                                         CurrentOfferQuantity = o.CurrentOfferQuantity,
                                                         PreviousOfferQuantity = o.PreviousOfferQuantity,
                                                         CounterOfferQuantity = o.CounterOfferQuantity,
                                                         Rejected = o.RejectedBy.HasValue
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
                                                            CurrentOfferQuantity = o.CurrentOfferQuantity,
                                                            PreviousOfferQuantity = o.PreviousOfferQuantity,
                                                            CounterOfferQuantity = o.CounterOfferQuantity,
                                                            Rejected = o.RejectedBy.HasValue
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