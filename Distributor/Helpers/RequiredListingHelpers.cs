using Distributor.Enums;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.ItemEnums;

namespace Distributor.Helpers
{
    public static class RequiredListingHelpers
    {
        #region Get

        public static RequiredListing GetRequiredListing(ApplicationDbContext db, Guid listingId)
        {
            return db.RequiredListings.Find(listingId);
        }

        public static List<RequiredListing> GetRequiredListingForOrganisation(ApplicationDbContext db, Guid organisationId, bool historyListing)
        {

            List<RequiredListing> list;

            if (historyListing)
            {
                list = (from al in db.RequiredListings
                        where (al.ListingOriginatorOrganisationId == organisationId && (al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Cancelled || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Complete || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Expired || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Closed))
                        select al).Distinct().ToList();
            }
            else
            {
                list = (from al in db.RequiredListings
                        where (al.ListingOriginatorOrganisationId == organisationId && (al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Open || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Partial))
                        select al).Distinct().ToList();
            }

            return list;
        }

        #endregion

        #region Create

        public static RequiredListing CreateListing(ApplicationDbContext db, RequiredListingManageCreateViewModel model, IPrincipal user)
        {
            AppUser thisUser = AppUserHelpers.GetAppUser(db, user);
            Organisation thisOrg = OrganisationHelpers.GetOrganisation(db, thisUser.OrganisationId);

            RequiredListing listing = new RequiredListing()
            {
                ListingId = Guid.NewGuid(),
                ItemDescription = model.ItemDescription,
                ItemCategory = model.ItemCategory,
                ItemType = model.ItemType,
                QuantityRequired = model.QuantityRequired,
                QuantityOutstanding = model.QuantityRequired,
                UoM = model.UoM,
                RequiredFrom = model.RequiredFrom,
                RequiredTo = model.RequiredTo,
                AcceptDamagedItems = model.AcceptDamagedItems,
                AcceptOutOfDateItems = model.AcceptOutOfDateItems,
                CollectionAvailable = model.CollectionAvailable,
                ListingStatus = ItemEnums.ItemRequiredListingStatusEnum.Open,
                ListingOrganisationPostcode = thisOrg.AddressPostcode,
                RecordChange = GeneralEnums.RecordChangeEnum.NewRecord,
                RecordChangeBy = thisUser.AppUserId,
                RecordChangeOn = DateTime.Now,
                ListingOriginatorAppUserId = thisUser.AppUserId,
                ListingOriginatorOrganisationId = thisOrg.OrganisationId,
                ListingOriginatorDateTime = DateTime.Now
            };

            db.RequiredListings.Add(listing);
            db.SaveChanges();

            return listing;
        }

        #endregion

        #region Update

        public static RequiredListing UpdateRequiredListing(ApplicationDbContext db, RequiredListingDetailsViewModel model, IPrincipal user)
        {
            RequiredListing listing = GetRequiredListing(db, model.ListingId);

            decimal qtyRequired;
            decimal.TryParse(model.QuantityRequired.ToString(), out qtyRequired);
            decimal qtyFulfilled;
            decimal.TryParse(model.QuantityFulfilled.ToString(), out qtyFulfilled);
            decimal qtyOutstanding;
            decimal.TryParse(model.QuantityOutstanding.ToString(), out qtyOutstanding);

            if (listing != null)
            {
                listing.ItemDescription = model.ItemDescription;
                listing.ItemCategory = model.ItemCategory;
                listing.ItemType = model.ItemType;
                listing.QuantityRequired = qtyRequired;
                listing.QuantityFulfilled = qtyFulfilled;
                listing.QuantityOutstanding = qtyOutstanding;
                listing.UoM = model.UoM;
                listing.RequiredFrom = model.RequiredFrom;
                listing.RequiredTo = model.RequiredTo;
                listing.AcceptDamagedItems = model.AcceptDamagedItems;
                listing.AcceptOutOfDateItems = model.AcceptOutOfDateItems;
                listing.CollectionAvailable = model.CollectionAvailable;
                listing.ListingStatus = model.ListingStatus;
                listing.ListingOrganisationPostcode = model.ListingOrganisationPostcode;
                listing.RecordChange = GeneralEnums.RecordChangeEnum.ListingStatusChange;
                listing.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
                listing.RecordChangeOn = DateTime.Now;

                db.Entry(listing).State = EntityState.Modified;
                db.SaveChanges();
            }

            return listing;
        }

        public static RequiredListing UpdateRequiredListingListingStatus(ApplicationDbContext db, Guid listingId, ItemRequiredListingStatusEnum listingStatus, IPrincipal user)
        {
            RequiredListing listing = GetRequiredListing(db, listingId);

            if (listing != null)
            {
                listing.ListingStatus = listingStatus;
                listing.RecordChange = GeneralEnums.RecordChangeEnum.ListingStatusChange;
                listing.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
                listing.RecordChangeOn = DateTime.Now;

                db.Entry(listing).State = EntityState.Modified;
                db.SaveChanges();
            }

            return listing;
        }

        public static void UpdateRequiredListings(ApplicationDbContext db, List<RequiredListingManageViewModel> model, IPrincipal user)
        {
            if (model != null)
                foreach (RequiredListingManageViewModel modelItem in model)
                    UpdateRequiredListingListingStatus(db, modelItem.ListingId, modelItem.ListingStatus, user);
        }

        #endregion
    }

    public static class RequiredListingViewHelpers
    {
        #region Get

        public static List<RequiredListingManageViewModel> GetRequiredListingManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<RequiredListing> required = RequiredListingHelpers.GetRequiredListingForOrganisation(db, organisationId, false);
            List<RequiredListingManageViewModel> list = new List<RequiredListingManageViewModel>();

            foreach (RequiredListing item in required)
            {
                RequiredListingManageViewModel listItem = new RequiredListingManageViewModel()
                {
                    ListingId = item.ListingId,
                    ItemDescription = item.ItemDescription,
                    ItemType = item.ItemType,
                    QuantityOutstanding = item.QuantityOutstanding,
                    UoM = item.UoM,
                    RequiredTo = item.RequiredTo,
                    AcceptDamagedItems = item.AcceptDamagedItems,
                    AcceptOutOfDateItems = item.AcceptOutOfDateItems,
                    CollectionAvailable = item.CollectionAvailable,
                    ListingStatus = item.ListingStatus
                };

                list.Add(listItem);
            }

            return list;
        }

        public static List<RequiredListingManageHistoryViewModel> GetRequiredListingManageHistoryViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<RequiredListing> history = RequiredListingHelpers.GetRequiredListingForOrganisation(db, organisationId, true);
            List<RequiredListingManageHistoryViewModel> list = new List<RequiredListingManageHistoryViewModel>();

            foreach (RequiredListing item in history)
            {
                RequiredListingManageHistoryViewModel listItem = new RequiredListingManageHistoryViewModel()
                {
                    ListingId = item.ListingId,
                    ItemDescription = item.ItemDescription,
                    ItemType = item.ItemType,
                    QuantityRequired = item.QuantityRequired,
                    QuantityOutstanding = item.QuantityOutstanding,
                    UoM = item.UoM,
                    RecordChangeOn = item.RecordChangeOn,
                    ListingStatus = item.ListingStatus
                };

                list.Add(listItem);
            }

            return list;
        }


        #endregion

        #region Create

        public static RequiredListingDetailsViewModel CreateRequiredListingDetailsViewModel(ApplicationDbContext db, Guid listingId, string breadcrumb, bool historyDisplay, HttpRequestBase request, string controllerValue, string actionValue, string callingActionDisplayName, Dictionary<int, string> breadcrumbDictionary, bool? recalled)
        {
            RequiredListing listing = RequiredListingHelpers.GetRequiredListing(db, listingId);

            if (listing != null)
            {
                //Set up the calling fields
                if (!recalled.HasValue)
                    GeneralHelpers.GetCallingDetailsFromRequest(request, controllerValue, actionValue, out controllerValue, out actionValue);

                AppUser recordChangedBy = AppUserHelpers.GetAppUser(db, listing.RecordChangeBy);
                AppUser listingOriginatorAppUser = AppUserHelpers.GetAppUser(db, listing.ListingOriginatorAppUserId);

                RequiredListingDetailsViewModel model = new RequiredListingDetailsViewModel()
                {
                    Breadcrumb = breadcrumb,
                    HistoryRecord = historyDisplay,
                    ListingId = listing.ListingId,
                    ItemDescription = listing.ItemDescription,
                    ItemCategory = listing.ItemCategory,
                    ItemType = listing.ItemType,
                    QuantityRequired = listing.QuantityRequired,
                    QuantityFulfilled = listing.QuantityFulfilled,
                    QuantityOutstanding = listing.QuantityOutstanding,
                    UoM = listing.UoM,
                    RequiredFrom = listing.RequiredFrom,
                    RequiredTo = listing.RequiredTo,
                    AcceptDamagedItems = listing.AcceptDamagedItems,
                    AcceptOutOfDateItems = listing.AcceptOutOfDateItems,
                    CollectionAvailable = listing.CollectionAvailable,
                    ListingStatus = listing.ListingStatus,
                    ListingOrganisationPostcode = listing.ListingOrganisationPostcode,
                    RecordChange = listing.RecordChange,
                    RecordChangeOn = listing.RecordChangeOn,
                    RecordChangeByName = recordChangedBy.FirstName + " " + recordChangedBy.LastName,
                    RecordChangeByEmail = recordChangedBy.LoginEmail,
                    ListingOriginatorAppUserName = listingOriginatorAppUser.FirstName + " " + listingOriginatorAppUser.LastName,
                    ListingOriginatorAppUserEmail = listingOriginatorAppUser.LoginEmail,
                    ListingOriginatorDateTime = listing.ListingOriginatorDateTime,
                    CallingController = controllerValue,
                    CallingAction = actionValue,
                    CallingActionDisplayName = callingActionDisplayName,
                    BreadcrumbTrail = breadcrumbDictionary
                };

                return model;
            }
            else
                return null;

        }

        #endregion
    }
}