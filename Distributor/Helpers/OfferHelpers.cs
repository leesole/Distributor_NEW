using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Distributor.Enums.GeneralEnums;

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
    }
}