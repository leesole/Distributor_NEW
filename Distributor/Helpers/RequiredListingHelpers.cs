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

        #region Create

        public static RequiredListingDetailsViewModel CreateRequiredListingDetailsViewModel(ApplicationDbContext db, Guid listingId, string breadcrumb, bool displayOnly, HttpRequestBase request, string controllerValue, string actionValue, string callingActionDisplayName, Dictionary<int, string> breadcrumbDictionary, bool? recalled)
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

                return model;
            }
            else
                return null;

        }

        #endregion
    }
}