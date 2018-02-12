using Distributor.Enums;
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

namespace Distributor.Helpers
{
    public static class RequiredListingHelpers
    {
        #region Get

        public static RequiredListing GetRequiredListing(ApplicationDbContext db, Guid listingId)
        {
            return db.RequiredListings.Find(listingId);
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
                QuantityRequired = model.QuantityRequired.Value,
                QuantityOutstanding = model.QuantityRequired.Value,
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

        public static RequiredListing UpdateRequiredListingQuantities(ApplicationDbContext db, Guid listingId, ListingQuantityChange changeOfValue, decimal changeQty, IPrincipal user)
        {
            RequiredListing listing = RequiredListingHelpers.GetRequiredListing(db, listingId);

            listing.RecordChange = RecordChangeEnum.RecordUpdated;
            listing.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
            listing.RecordChangeOn = DateTime.Now;

            if (changeOfValue == ListingQuantityChange.Subtract)
            {
                listing.QuantityFulfilled += changeQty;
                listing.QuantityOutstanding -= changeQty;

                if (listing.QuantityOutstanding == 0)
                {
                    listing.ListingStatus = ItemRequiredListingStatusEnum.Complete;
                    listing.RecordChange = RecordChangeEnum.ListingStatusChange;
                }
            }
            else
            {
                listing.QuantityFulfilled -= changeQty;
                listing.QuantityOutstanding += changeQty;
            }

            db.Entry(listing).State = EntityState.Modified;
            db.SaveChanges();

            return listing;
        }

        #endregion
    }

    public static class RequiredListingViewHelpers
    {
        #region Get

        #region General Info

        public static RequiredListingGeneralViewListModel GetRequiredListingGeneralViewListModel(ApplicationDbContext db, IPrincipal user, int? maxDistance, double? maxAge)
        {
            List<RequiredListingGeneralViewModel> list = new List<RequiredListingGeneralViewModel>();

            //Get user so we can get the settings to initialise the search on the screen
            AppUser currentUser = AppUserHelpers.GetAppUser(db, user);
            Organisation currentOrg = OrganisationHelpers.GetOrganisation(db, currentUser.OrganisationId);

            //set the search criteria.  If nothing passed in then take the values from the settings.  If values past in then this is the dynamic changes made on the list screen and resubmitted
            int? maxDistanceFilter = maxDistance ?? currentUser.MaxDistanceFilter ?? 1500;
            double? maxAgeFilter = maxAge ?? currentUser.MaxAgeFilter ?? 9999;

            //Get the group Member IDs from groups that this user/organisation are part of, so we can remove them from the list
            List<Guid> groupMemberOrgIds = null;
            if (currentUser.SelectionLevelFilter == ExternalSearchLevelEnum.Group)
                groupMemberOrgIds = GroupMembersHelpers.GetGroupsMembersOrgGuidsForGroupsFromOrg(db, currentOrg.OrganisationId);

            //build the age filter to apply when building list
            double negativeDays = 0 - maxAgeFilter.Value;
            var dateCheck = DateTime.Now.AddDays(negativeDays);

            //build list depending on whether to filter on groups or not (settings, search level = groups)
            if (groupMemberOrgIds == null)
            {
                list = (from rl in db.RequiredListings
                        join org in db.Organisations on rl.ListingOriginatorOrganisationId equals org.OrganisationId
                        where (rl.ListingOriginatorOrganisationId != currentUser.OrganisationId && (rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Open || rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Partial)
                            && rl.ListingOriginatorDateTime >= dateCheck)
                        select new RequiredListingGeneralViewModel()
                        {
                            ListingId = rl.ListingId,
                            ItemDescription = rl.ItemDescription,
                            ItemType = rl.ItemType,
                            QuantityOutstanding = rl.QuantityOutstanding,
                            UoM = rl.UoM,
                            RequiredTo = rl.RequiredTo,
                            AcceptDamagedItems = rl.AcceptDamagedItems,
                            AcceptOutOfDateItems = rl.AcceptOutOfDateItems,
                            CollectionAvailable = rl.CollectionAvailable,
                            RequesterDetails = org.OrganisationName,
                            ListingOriginatorOrganisationId = rl.ListingOriginatorOrganisationId,
                            ListingOrganisationPostcode = rl.ListingOrganisationPostcode
                        }).Distinct().ToList();
            }
            else
            {
                list = (from rl in db.RequiredListings
                        join org in db.Organisations on rl.ListingOriginatorOrganisationId equals org.OrganisationId
                        join grpmem in groupMemberOrgIds on rl.ListingOriginatorOrganisationId equals grpmem
                        where (rl.ListingOriginatorOrganisationId != currentUser.OrganisationId && (rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Open || rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Partial)
                            && rl.ListingOriginatorDateTime >= dateCheck)
                        select new RequiredListingGeneralViewModel()
                        {
                            ListingId = rl.ListingId,
                            ItemDescription = rl.ItemDescription,
                            ItemType = rl.ItemType,
                            QuantityOutstanding = rl.QuantityOutstanding,
                            UoM = rl.UoM,
                            RequiredTo = rl.RequiredTo,
                            AcceptDamagedItems = rl.AcceptDamagedItems,
                            AcceptOutOfDateItems = rl.AcceptOutOfDateItems,
                            CollectionAvailable = rl.CollectionAvailable,
                            RequesterDetails = org.OrganisationName,
                            ListingOriginatorOrganisationId = rl.ListingOriginatorOrganisationId,
                            ListingOrganisationPostcode = rl.ListingOrganisationPostcode
                        }).Distinct().ToList();
            }

            //Filter by DISTANCE and add OFFER info also.

            //hold list of organisationIds already checked - set to true if within range
            Dictionary<Guid, bool> listingOrgIds = new Dictionary<Guid, bool>();

            //hold new list from old
            List<RequiredListingGeneralViewModel> newList = new List<RequiredListingGeneralViewModel>();

            foreach (RequiredListingGeneralViewModel item in list)
            {
                //if we have checked this org before then just add or not depending on flag
                if (listingOrgIds.ContainsKey(item.ListingOriginatorOrganisationId))
                {
                    if (listingOrgIds[item.ListingOriginatorOrganisationId])
                    {
                        //quick check for offer
                        Offer offer = OfferHelpers.GetOfferForListingByUser(db, item.ListingId, currentUser.AppUserId, currentOrg.OrganisationId, currentOrg.ListingPrivacyLevel);

                        if (offer == null)
                            item.RequiredQty = 0.00M;
                        else
                        {
                            item.OfferId = offer.OfferId;
                            item.RequiredQty = offer.CurrentOfferQuantity;
                        }

                        newList.Add(item);
                    }
                }
                else  //add the org to the dictionary with the flag set and add to new list if within range
                {
                    int distanceValue = DistanceHelpers.GetDistance(currentOrg.AddressPostcode, item.ListingOrganisationPostcode);
                    if (distanceValue <= maxDistanceFilter)
                    {
                        listingOrgIds.Add(item.ListingOriginatorOrganisationId, true);

                        //quick check for offer
                        Offer offer = OfferHelpers.GetOfferForListingByUser(db, item.ListingId, currentUser.AppUserId, currentOrg.OrganisationId, currentOrg.ListingPrivacyLevel);

                        if (offer == null)
                            item.RequiredQty = 0.00M;
                        else
                        {
                            item.OfferId = offer.OfferId;
                            item.RequiredQty = offer.CurrentOfferQuantity;
                        }

                        newList.Add(item);
                    }
                    else
                        listingOrgIds.Add(item.ListingOriginatorOrganisationId, false);
                }
            }

            RequiredListingGeneralViewListModel model = new RequiredListingGeneralViewListModel()
            {
                MaxDistance = maxDistanceFilter,
                MaxAge = maxAgeFilter,
                EditableFields = newList.Any(x => x.OfferId == null),  //only set if there are no offers in the list
                Listing = newList
            };

            return model;
        }

        #endregion

        #region Manage Info

        public static List<RequiredListingManageViewModel> GetRequiredListingManageViewModel(ApplicationDbContext db, Guid organisationId)
        {
           List<RequiredListingManageViewModel> list = (from rl in db.RequiredListings
                                                        where (rl.ListingOriginatorOrganisationId == organisationId && (rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Open || rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Partial))
                                                        select new RequiredListingManageViewModel()
                                                        {
                                                            ListingId = rl.ListingId,
                                                            ItemDescription = rl.ItemDescription,
                                                            ItemType = rl.ItemType,
                                                            QuantityOutstanding = rl.QuantityOutstanding,
                                                            UoM = rl.UoM,
                                                            RequiredTo = rl.RequiredTo,
                                                            AcceptDamagedItems = rl.AcceptDamagedItems,
                                                            AcceptOutOfDateItems = rl.AcceptOutOfDateItems,
                                                            CollectionAvailable = rl.CollectionAvailable,
                                                            ListingStatus = rl.ListingStatus
                                                        }).Distinct().ToList();
            return list;
        }

        public static List<RequiredListingManageHistoryViewModel> GetRequiredListingManageHistoryViewModel(ApplicationDbContext db, Guid organisationId)
        {
            List<RequiredListingManageHistoryViewModel> list = (from rl in db.RequiredListings
                                                                where (rl.ListingOriginatorOrganisationId == organisationId && (rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Cancelled || rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Complete || rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Expired || rl.ListingStatus == ItemEnums.ItemRequiredListingStatusEnum.Closed))
                                                                select new RequiredListingManageHistoryViewModel()
                                                                {
                                                                    ListingId = rl.ListingId,
                                                                    ItemDescription = rl.ItemDescription,
                                                                    ItemType = rl.ItemType,
                                                                    QuantityRequired = rl.QuantityRequired,
                                                                    QuantityOutstanding = rl.QuantityOutstanding,
                                                                    UoM = rl.UoM,
                                                                    RecordChangeOn = rl.RecordChangeOn,
                                                                    ListingStatus = rl.ListingStatus
                                                                }).Distinct().ToList();
            return list;
        }

        #endregion

        #endregion

        #region Create

        public static RequiredListingDetailsViewModel CreateRequiredListingDetailsViewModel(ApplicationDbContext db, Guid listingId, string breadcrumb, bool displayOnly, HttpRequestBase request, string controllerValue, string actionValue, string callingActionDisplayName, Dictionary<int, string> breadcrumbDictionary, bool? recalled, IPrincipal user, int? maxDistance, double? maxAge)
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
                    DisplayOnly = displayOnly,
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

                if (controllerValue == "GeneralInfo" && actionValue == "Required")
                {
                    //Get user info for the offer side of the display
                    AppUser currentUser = AppUserHelpers.GetAppUser(db, user);
                    Organisation currentOrg = OrganisationHelpers.GetOrganisation(db, currentUser.OrganisationId);

                    Offer offer = OfferHelpers.GetOfferForListingByUser(db, listing.ListingId, currentUser.AppUserId, currentOrg.OrganisationId, currentOrg.ListingPrivacyLevel);

                    if (offer == null)
                    {
                        model.OfferDescription = "Make a request";
                    }
                    else
                    {
                        model.OfferDescription = "Current request";
                        model.OfferId = offer.OfferId;
                        model.OfferQty = offer.CurrentOfferQuantity;
                        model.OfferCounterQty = offer.CounterOfferQuantity;
                        model.OfferStatus = offer.OfferStatus;
                    }
                }

                return model;
            }
            else
                return null;

        }

        #endregion
    }
}