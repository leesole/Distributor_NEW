using Distributor.Enums;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.EntityEnums;

namespace Distributor.Helpers
{
    public static class AvailableListingHelpers
    {
        #region Get

        public static AvailableListing GetAvailableListing(ApplicationDbContext db, Guid listingId)
        {
            return db.AvailableListings.Find(listingId);
        }

        public static List<AvailableListing> GetAvailableListingForOrganisation(ApplicationDbContext db, Guid organisationId, bool historyListing)
        {

            List<AvailableListing> list;

            if (historyListing)
            {
                list = (from al in db.AvailableListings
                        where (al.ListingOriginatorOrganisationId == organisationId && (al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Cancelled || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Complete || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Expired))
                        select al).Distinct().ToList();
            }
            else
            {
                list = (from al in db.AvailableListings
                        where (al.ListingOriginatorOrganisationId == organisationId && (al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Open || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Partial))
                        select al).Distinct().ToList();
            }

            return list;
        }

        #endregion

        #region Create

        public static AvailableListing CreateListing(ApplicationDbContext db, AvailableListingManageCreateViewModel model, IPrincipal user)
        {
            AvailableListing listing = new AvailableListing()
            {
                ListingId = Guid.NewGuid(),
                ItemDescription = model.ItemDescription,
                ItemCategory = model.ItemCategory,
                ItemType = model.ItemType,
                QuantityAvailable = model.QuantityAvailable,
                UoM = model.UoM,
                AvailableFrom = model.AvailableFrom,
                AvailableTo = model.AvailableTo,
                ItemCondition = model.ItemCondition,
                DisplayUntilDate = model.DisplayUntilDate,
                SellByDate = model.SellByDate,
                UseByDate = model.UseByDate,
                DeliveryAvailable = model.DeliveryAvailable

            };

            db.AvailableListings.Add(listing);
            db.SaveChanges();

            return listing;
        }

        #endregion
    }

    public static class AvailableListingViewHelpers
    {
        #region Get

        public static List<AvailableListingManageViewModel> GetAvailableListingManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<AvailableListing> available = AvailableListingHelpers.GetAvailableListingForOrganisation(db, organisationId, false);
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

        public static List<AvailableListingManageHistoryViewModel> GetAvailableListingManageHistoryViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<AvailableListing> history = AvailableListingHelpers.GetAvailableListingForOrganisation(db, organisationId, true);
            List<AvailableListingManageHistoryViewModel> list = new List<AvailableListingManageHistoryViewModel>();

            foreach (AvailableListing item in history)
            {
                AvailableListingManageHistoryViewModel listItem = new AvailableListingManageHistoryViewModel()
                {
                    ListingId = item.ListingId,
                    ItemDescription = item.ItemDescription,
                    ItemType = item.ItemType,
                    QuantityAvailable = item.QuantityAvailable,
                    QuantityOutstanding = item.QuantityOutstanding,
                    UoM = item.UoM,
                    ItemCondition = item.ItemCondition,
                    RecordChangeOn = item.RecordChangeOn,
                    ListingStatus = item.ListingStatus
                };

                list.Add(listItem);
            }

            return list;
        }

        #endregion
    }
}