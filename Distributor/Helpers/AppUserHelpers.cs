using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserEnums;
using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Distributor.Extensions;
using System.Data.Entity;
using Distributor.ViewModels;
using static Distributor.Enums.UserTaskEnums;

namespace Distributor.Helpers
{
    public static class AppUserHelpers
    {
        #region Get

        public static AppUser GetAppUser(Guid appUserId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = GetAppUser(db, appUserId);
            db.Dispose();
            return appUser;
        }
        public static AppUser GetAppUser(ApplicationDbContext db, Guid appUserId)
        {
            return db.AppUsers.Find(appUserId);
        }
        public static AppUser GetAppUser(IPrincipal user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = GetAppUser(db, user);
            db.Dispose();
            return appUser;
        }
        public static AppUser GetAppUser(ApplicationDbContext db, IPrincipal user)
        {
            Guid appUserId;
            Guid.TryParse(user.Identity.GetAppUserId(), out appUserId);

            return GetAppUser(db, appUserId);
        }

        public static EntityStatusEnum GetAppUserEntityStatus(ApplicationUser user)
        {
            AppUser appUser = GetAppUser(user.AppUserId);

            return appUser.EntityStatus;
        }
        public static EntityStatusEnum GetAppUserEntityStatus(IPrincipal user)
        {
            AppUser appUser = GetAppUser(user);

            try
            {
                return appUser.EntityStatus;
            }
            catch
            {
                return EntityStatusEnum.Inactive;
            }
        }

        #endregion

        #region Create

        public static AppUser CreateAppUser(RegisterViewModel model, IPrincipal user, UserRoleEnum role)
        {
            Guid appUserId = GetAppUserIdFromUser(user);
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = CreateAppUser(db, model,appUserId, role);
            db.Dispose();
            return appUser;
        }
        public static AppUser CreateAppUser(ApplicationDbContext db, RegisterViewModel model, Guid appUserId, UserRoleEnum role)
        {
            AppUser appUser = new AppUser()
            {
                AppUserId = Guid.NewGuid(),
                FirstName = "",
                LastName = "",
                EntityStatus = EntityStatusEnum.AwaitingOrganisationDetails,
                OrganisationId = Guid.Empty,
                LoginEmail = model.Email,
                PrivacyLevel = PrivacyLevelEnum.None,
                UserRole = role,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeOn = DateTime.Now
            };
            //handle if this is a self made user
            if (appUserId == Guid.Empty)
                appUser.RecordChangeBy = appUser.AppUserId;
            else
                appUser.RecordChangeBy = appUserId;

            db.AppUsers.Add(appUser);
            db.SaveChanges();

            return appUser;
        }

        #endregion

        #region Update

        public static AppUser UpdateAppUserOrganisationId(IPrincipal user, Guid organisationId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = UpdateAppUserOrganisationId(db, user, organisationId);
            db.Dispose();
            return appUser;
        }
        public static AppUser UpdateAppUserOrganisationId(ApplicationDbContext db, IPrincipal user, Guid organisationId)
        {
            AppUser appUser = GetAppUser(db, user);
            appUser.OrganisationId = organisationId;
            appUser.RecordChange = RecordChangeEnum.NewRecord;
            appUser.RecordChangeBy = appUser.AppUserId;
            appUser.RecordChangeOn = DateTime.Now;

            db.Entry(appUser).State = EntityState.Modified;
            db.SaveChanges();

            return appUser;
        }

        public static AppUser UpdateAppUserEntityStatus(ApplicationDbContext db, Guid updatedUserId, EntityStatusEnum status, IPrincipal updatedByUser)
        {
            return UpdateAppUserRoleAndEntityStatus(db, updatedUserId, null, status, updatedByUser);
        }

        public static AppUser UpdateAppUserRoleAndEntityStatus(IPrincipal updatedUser, UserRoleEnum? role, EntityStatusEnum status, IPrincipal updatedByUser)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = UpdateAppUserRoleAndEntityStatus(db, updatedUser, role, status, updatedByUser);
            db.Dispose();
            return appUser;
        }
        public static AppUser UpdateAppUserRoleAndEntityStatus(ApplicationDbContext db, IPrincipal updatedUser, UserRoleEnum? role, EntityStatusEnum status, IPrincipal updatedByUser)
        {
            return UpdateAppUserRoleAndEntityStatus(db, GetAppUserIdFromUser(updatedUser), role, status, updatedByUser);
        }

        public static AppUser UpdateAppUserRoleAndEntityStatus(ApplicationDbContext db, Guid updatedUserId, UserRoleEnum? role, EntityStatusEnum status, IPrincipal updatedByUser)
        {
            AppUser updatedAppUser = GetAppUser(db, updatedUserId);

            if (role.HasValue)
                updatedAppUser.UserRole = role.Value;
            updatedAppUser.EntityStatus = status;
            updatedAppUser.RecordChange = RecordChangeEnum.StatusChange;
            updatedAppUser.RecordChangeBy = GetAppUserIdFromUser(updatedByUser);
            updatedAppUser.RecordChangeOn = DateTime.Now;

            db.Entry(updatedAppUser).State = EntityState.Modified;
            db.SaveChanges();

            //create Tasks for changes in status to On-Hold
            if (status == EntityStatusEnum.OnHold)
                UserTasksHelpers.CreateUserTask(db, TaskTypeEnum.UserOnHold, "New user on hold, awaiting administrator activation", updatedAppUser.AppUserId, updatedAppUser.LoginEmail, updatedByUser);

            return updatedAppUser;
        }

        //updates AppUser from the AppUserProfileView (AppUser/UserProfile)
        public static AppUser UpdateAppUser(AppUserProfileView view, IPrincipal user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = UpdateAppUser(db, view, user);
            db.Dispose();
            return appUser;
        }
        //updates AppUser from the AppUserProfileView (AppUser/UserProfile)
        public static AppUser UpdateAppUser(ApplicationDbContext db, AppUserProfileView view, IPrincipal user)
        {
            AppUser appUser = GetAppUser(db, view.AppUserId);
            appUser.FirstName = view.FirstName;
            appUser.LastName = view.LastName;
            appUser.EntityStatus = view.EntityStatus;
            appUser.LoginEmail = view.LoginEmail;
            appUser.PrivacyLevel = view.PrivacyLevel;
            appUser.UserRole = view.UserRole;
            appUser.OrganisationId = view.SelectedOrganisationId.Value;
            appUser.RecordChange = RecordChangeEnum.RecordUpdated;
            appUser.RecordChangeBy = GetAppUserIdFromUser(user);
            appUser.RecordChangeOn = DateTime.Now;

            db.Entry(appUser).State = EntityState.Modified;
            db.SaveChanges();

            return appUser;
        }

        //updates AppUser from the AppUserSettingsView (AppUser/Settings)
        public static AppUser UpdateAppUser(AppUserSettingsView view, IPrincipal user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUser appUser = UpdateAppUser(db, view, user);
            db.Dispose();
            return appUser;
        }
        //updates AppUser from the AppUserSettingsView (AppUser/Settings)
        public static AppUser UpdateAppUser(ApplicationDbContext db, AppUserSettingsView view, IPrincipal user)
        {
            AppUser appUser = GetAppUser(db, view.AppUserId);
            appUser.MaxDistanceFilter = view.MaxDistanceFilter;
            appUser.MaxAgeFilter = view.MaxAgeFilter;
            appUser.SelectionLevelFilter = view.SelectionLevelFilter;
            appUser.DisplayMyOrganisationListingsFilter = view.DisplayMyOrganisationListingsFilter;
            appUser.RecordChange = RecordChangeEnum.RecordUpdated;
            appUser.RecordChangeBy = GetAppUserIdFromUser(user);
            appUser.RecordChangeOn = DateTime.Now;

            db.Entry(appUser).State = EntityState.Modified;
            db.SaveChanges();

            return appUser;
        }

        #endregion

        #region Delete
        
        public static void DeleteAppUser(Guid appUserId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            DeleteAppUser(db, appUserId);
            db.Dispose();
        }
        public static void DeleteAppUser(ApplicationDbContext db, Guid appUserId)
        {
            db.AppUsers.Remove(db.AppUsers.Find(appUserId));
            db.SaveChanges();
        }

        #endregion

        #region Processes

        public static Guid GetAppUserIdFromUser(IPrincipal user)
        {
            return GeneralHelpers.StringToGuid(user.Identity.GetAppUserId());
        }

        public static Guid GetOrganisationIdFromUser(IPrincipal user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Guid organisationId = GetOrganisationIdFromUser(db, user);
            db.Dispose();
            return organisationId;
        }
        public static Guid GetOrganisationIdFromUser(ApplicationDbContext db, IPrincipal user)
        {
            try
            {
                return GetAppUser(db, user).OrganisationId;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        #endregion
    }

    public static class AppUserViewHelpers
    {
        #region Create

        //AppUser/Profile
        public static AppUserProfileView CreateAppUserProfileView(Guid appUserId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUserProfileView view = CreateAppUserProfileView(db, appUserId);
            db.Dispose();
            return view;
        }
        //AppUser/Profile
        public static AppUserProfileView CreateAppUserProfileView(ApplicationDbContext db, Guid appUserId)
        {
            AppUser appUser = AppUserHelpers.GetAppUser(db, appUserId);

            if (appUser == null)
                return null;

            AppUserProfileView view = new AppUserProfileView()
            {
                AppUserId = appUser.AppUserId,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                EntityStatus = appUser.EntityStatus,
                LoginEmail = appUser.LoginEmail,
                PrivacyLevel = appUser.PrivacyLevel,
                UserRole = appUser.UserRole,
                SelectedOrganisationId = appUser.OrganisationId
            };

            if (appUser.OrganisationId != Guid.Empty)
            {
                Organisation org = OrganisationHelpers.GetOrganisation(db, appUser.OrganisationId);
                view.OrganisationName = org.OrganisationName;
                view.BusinessType = org.BusinessType;
                view.AddressLine1 = org.AddressLine1;
                view.AddressLine2 = org.AddressLine2;
                view.AddressLine3 = org.AddressLine3;
                view.AddressTownCity = org.AddressTownCity;
                view.AddressCounty = org.AddressCounty;
                view.AddressPostcode = org.AddressPostcode;
            }

            return view;
        }

        //AppUser/Settings
        public static AppUserSettingsView CreateAppUserSettingsView(Guid appUserId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            AppUserSettingsView view = CreateAppUserSettingsView(db, appUserId);
            db.Dispose();
            return view;
        }
        //AppUser/Settings
        public static AppUserSettingsView CreateAppUserSettingsView(ApplicationDbContext db, Guid appUserId)
        {
            AppUser appUser = AppUserHelpers.GetAppUser(db, appUserId);

            if (appUser == null)
                return null;

            AppUserSettingsView view = new AppUserSettingsView()
            {
                AppUserId = appUser.AppUserId,
                MaxDistanceFilter = appUser.MaxDistanceFilter,
                MaxAgeFilter = appUser.MaxAgeFilter,
                SelectionLevelFilter = appUser.SelectionLevelFilter,
                DisplayMyOrganisationListingsFilter = appUser.DisplayMyOrganisationListingsFilter
            };

            return view;
        }

        #endregion
    }
}