using Distributor.Enums;
using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.ItemEnums;

namespace Distributor.Helpers
{
    public static class AvailableListingHelpers
    {
        #region Get

        public static AvailableListing GetAvailableListing(ApplicationDbContext db, Guid listingId)
        {
            return db.AvailableListings.Find(listingId);
        }

        public static List<AvailableListing> GetAvailableListingForOrganisation(ApplicationDbContext db, Guid organisationId, ExternalSearchLevelEnum? selectionLevelFilter, int? maxDistanceFilter, double? maxAgeFilter, bool generalInfo, bool historyListing)
        {
            List<AvailableListing> list;

            //history is currently only for ManageInfo (if this changes then need to do similar check on 'generalInfo' as below
            if (historyListing)
            {
                list = (from al in db.AvailableListings
                        where (al.ListingOriginatorOrganisationId == organisationId && (al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Cancelled || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Complete || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Expired || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Closed))
                        select al).Distinct().ToList();
            }
            else if (generalInfo)  //bring back those that DO NOT belong to this user's organisation
            {
                //build the age filter to apply when building list
                double negativeDays = 0 - maxAgeFilter.Value;
                var dateCheck = DateTime.Now.AddDays(negativeDays);

                list = (from al in db.AvailableListings
                        where (al.ListingOriginatorOrganisationId != organisationId && (al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Open || al.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Partial)
                            && al.ListingOriginatorDateTime >= dateCheck)
                        select al).Distinct().ToList();

                selectionLevelFilter = selectionLevelFilter ?? ExternalSearchLevelEnum.All;

                //filter the list by group if required
                if (selectionLevelFilter.Value == ExternalSearchLevelEnum.Group)
                    list = GroupFilters.FilterAvailableListingsByGroup(db, list, organisationId);

                //LSLSLS TODO? - Extra Filters  (probably add ages, types, etc..)
                //filter the list by distance
                list = SearchHelpers.FilterAvailableListingsByDistance(db, list, organisationId, maxDistanceFilter.Value);
            }
            else //bring back those that ONLY belong to this user's organisation
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
            AppUser thisUser = AppUserHelpers.GetAppUser(db, user);
            Organisation thisOrg = OrganisationHelpers.GetOrganisation(db, thisUser.OrganisationId);

            AvailableListing listing = new AvailableListing()
            {
                ListingId = Guid.NewGuid(),
                ItemDescription = model.ItemDescription,
                ItemCategory = model.ItemCategory,
                ItemType = model.ItemType,
                QuantityAvailable = model.QuantityAvailable.Value,
                QuantityOutstanding = model.QuantityAvailable.Value,
                UoM = model.UoM,
                AvailableFrom = model.AvailableFrom,
                AvailableTo = model.AvailableTo,
                ItemCondition = model.ItemCondition,
                DisplayUntilDate = model.DisplayUntilDate,
                SellByDate = model.SellByDate,
                UseByDate = model.UseByDate,
                DeliveryAvailable = model.DeliveryAvailable,
                ListingStatus = ItemEnums.ItemRequiredListingStatusEnum.Open,
                ListingOrganisationPostcode = thisOrg.AddressPostcode,
                RecordChange = GeneralEnums.RecordChangeEnum.NewRecord,
                RecordChangeBy = thisUser.AppUserId,
                RecordChangeOn = DateTime.Now,
                ListingOriginatorAppUserId = thisUser.AppUserId,
                ListingOriginatorOrganisationId = thisOrg.OrganisationId,
                ListingOriginatorDateTime = DateTime.Now
            };

            db.AvailableListings.Add(listing);
            db.SaveChanges();

            return listing;
        }

        #endregion

        #region Update

        public static AvailableListing UpdateAvailableListing(ApplicationDbContext db, AvailableListingDetailsViewModel model, IPrincipal user)
        {
            AvailableListing listing = GetAvailableListing(db, model.ListingId);

            decimal qtyAvailable;
            decimal.TryParse(model.QuantityAvailable.ToString(), out qtyAvailable);
            decimal qtyFulfilled;
            decimal.TryParse(model.QuantityFulfilled.ToString(), out qtyFulfilled);
            decimal qtyOutstanding;
            decimal.TryParse(model.QuantityOutstanding.ToString(), out qtyOutstanding);

            if (listing != null)
            {
                listing.ItemDescription = model.ItemDescription;
                listing.ItemCategory = model.ItemCategory;
                listing.ItemType = model.ItemType;
                listing.QuantityAvailable = qtyAvailable;
                listing.QuantityFulfilled = qtyFulfilled;
                listing.QuantityOutstanding = qtyOutstanding;
                listing.UoM = model.UoM;
                listing.AvailableFrom = model.AvailableFrom;
                listing.AvailableTo = model.AvailableTo;
                listing.ItemCondition = model.ItemCondition;
                listing.DisplayUntilDate = model.DisplayUntilDate;
                listing.SellByDate = model.SellByDate;
                listing.UseByDate = model.UseByDate;
                listing.DeliveryAvailable = model.DeliveryAvailable;
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

        public static AvailableListing UpdateAvailableListingListingStatus(ApplicationDbContext db, Guid listingId, ItemRequiredListingStatusEnum listingStatus, IPrincipal user)
        {
            AvailableListing listing = GetAvailableListing(db, listingId);

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

        public static void UpdateAvailableListings(ApplicationDbContext db, List<AvailableListingManageViewModel> model, IPrincipal user)
        {
            if (model != null)
                foreach (AvailableListingManageViewModel modelItem in model)
                    UpdateAvailableListingListingStatus(db, modelItem.ListingId, modelItem.ListingStatus, user);
        }

        #endregion
    }

    public static class AvailableListingViewHelpers
    {
        #region Get

        #region General Info

        public static List<AvailableListingGeneralViewModel> GetAvailableListingGeneralViewModel(ApplicationDbContext db, IPrincipal user, int? maxDistance, double? maxAge)
        {
            //Get user so we can get the settings to initialise the search on the screen
            AppUser currentUser = AppUserHelpers.GetAppUser(db, user);
            Organisation currentOrg = OrganisationHelpers.GetOrganisation(db, currentUser.OrganisationId);

            //set the search criteria.  If nothing passed in then take the values from the settings.  If values past in then this is the dynamic changes made on the list screen and resubmitted
            int? maxDistanceFilter = maxDistance ?? currentUser.MaxDistanceFilter ?? 1500;
            double? maxAgeFilter = maxAge ?? currentUser.MaxAgeFilter ?? 9999;

            List<AvailableListing> available = AvailableListingHelpers.GetAvailableListingForOrganisation(db, currentUser.OrganisationId, currentUser.SelectionLevelFilter, maxDistanceFilter, maxAgeFilter, true, false);
            List<AvailableListingGeneralViewModel> list = new List<AvailableListingGeneralViewModel>();

            foreach (AvailableListing item in available)
            {
                // set the expiry date to be sell by date, if this is null then set to use by date (which could also be null)
                DateTime? expiryDate = item.SellByDate ?? item.UseByDate;

                Organisation supplier = OrganisationHelpers.GetOrganisation(db, item.ListingOriginatorOrganisationId);
                
                AvailableListingGeneralViewModel listItem = new AvailableListingGeneralViewModel()
                {
                    MaxDistance = maxDistanceFilter,
                    MaxAge = maxAgeFilter,
                    ListingId = item.ListingId,
                    ItemDescription = item.ItemDescription,
                    ItemType = item.ItemType,
                    QuantityOutstanding = item.QuantityOutstanding,
                    UoM = item.UoM,
                    AvailableTo = item.AvailableTo,
                    ItemCondition = item.ItemCondition,
                    ExpiryDate = expiryDate,
                    DeliveryAvailable = item.DeliveryAvailable,
                    SupplierDetails = supplier.OrganisationName,
                    Distance = DistanceHelpers.GetDistance(currentOrg.AddressPostcode, item.ListingOrganisationPostcode)
                };

                list.Add(listItem);
            }

            return list;
        }

        #endregion

        #region Manage Info

        public static List<AvailableListingManageViewModel> GetAvailableListingManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<AvailableListing> available = AvailableListingHelpers.GetAvailableListingForOrganisation(db, organisationId, null, null, null, false, false);
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
            List<AvailableListing> history = AvailableListingHelpers.GetAvailableListingForOrganisation(db, organisationId, null, null, null, false, true);
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

        #endregion

        #region Create

        public static AvailableListingDetailsViewModel CreateAvailableListingDetailsViewModel(ApplicationDbContext db, Guid listingId, string breadcrumb, bool displayOnly, HttpRequestBase request, string controllerValue, string actionValue, string callingActionDisplayName, Dictionary<int, string> breadcrumbDictionary, bool? recalled)
        {
            AvailableListing listing = AvailableListingHelpers.GetAvailableListing(db, listingId);

            if (listing != null)
            {
                //Set up the calling fields
                if (!recalled.HasValue)
                    GeneralHelpers.GetCallingDetailsFromRequest(request, controllerValue, actionValue, out controllerValue, out actionValue);

                AppUser recordChangedBy = AppUserHelpers.GetAppUser(db, listing.RecordChangeBy);
                AppUser listingOriginatorAppUser = AppUserHelpers.GetAppUser(db, listing.ListingOriginatorAppUserId);

                AvailableListingDetailsViewModel model = new AvailableListingDetailsViewModel()
                {
                    Breadcrumb = breadcrumb,
                    DisplayOnly = displayOnly,
                    ListingId = listing.ListingId,
                    ItemDescription = listing.ItemDescription,
                    ItemCategory = listing.ItemCategory,
                    ItemType = listing.ItemType,
                    QuantityAvailable = listing.QuantityAvailable,
                    QuantityFulfilled = listing.QuantityFulfilled,
                    QuantityOutstanding = listing.QuantityOutstanding,
                    UoM = listing.UoM,
                    AvailableFrom = listing.AvailableFrom,
                    AvailableTo = listing.AvailableTo,
                    ItemCondition = listing.ItemCondition,
                    DisplayUntilDate = listing.DisplayUntilDate,
                    SellByDate = listing.SellByDate,
                    UseByDate = listing.UseByDate,
                    DeliveryAvailable = listing.DeliveryAvailable,
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