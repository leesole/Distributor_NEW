using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
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

            Order order = new Order()
            {
                OrderId = Guid.NewGuid(),
                ListingType = offer.ListingType,
                OrderQuanity = orderQty,
                OrderStatus = OrderStatusEnum.New,
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

            return order;
        }
    }
}