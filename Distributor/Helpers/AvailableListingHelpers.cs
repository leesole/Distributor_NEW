using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributor.Helpers
{
    public static class AvailableListingHelpers
    {
        #region Get

        public static AvailableListing GetAvailableListing(ApplicationDbContext db, Guid listingId)
        {
            return db.AvailableListings.Find(listingId);
        }

        public static List<AvailableListing> GetAvailableListingForOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            List<AvailableListing> list = (from al in db.AvailableListings
                                           where al.ListingOriginatorOrganisationId == organisationId
                                           select al).Distinct().ToList();
            return list;
        }

        #endregion
    }

    public static class AvailableListingViewHelpers
    {

        #region Get

        public static List<AvailableListingManageViewModel> GetAvailableListingManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<AvailableListing> available = AvailableListingHelpers.GetAvailableListingForOrganisation(db, organisationId);
            List<AvailableListingManageViewModel> list = new List<AvailableListingManageViewModel>();

            foreach (AvailableListing item in available)
            {
                // set the expiry date to be sell by date, if this is null then set to use by date (which could also be null)
                DateTime? expiryDate = item.SellByDate ?? item.UseByDate;

                AvailableListingManageViewModel listItem = new AvailableListingManageViewModel()
                {
                    ListingId = item.ListingId,
                    ItemDescription = item.ItemDescription,
                    ItemType = item.ItemType,
                    QuantityOutstanding = item.QuantityOutstanding,
                    UoM = item.UoM,
                    AvailableTo = item.AvailableTo,
                    ItemCondition = item.ItemCondition,
                    ExpiryDate = expiryDate,
                    DeliveryAvailable = item.DeliveryAvailable,
                    ListingStatus = item.ListingStatus
                };

                list.Add(listItem);
            }

            return list;
        }

        #endregion
    }
}