using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributor.ViewModels;
using static Distributor.Enums.EntityEnums;
using System.Security.Principal;
using Distributor.Extensions;
using static Distributor.Enums.GeneralEnums;
using System.Data.Entity;

namespace Distributor.Helpers
{
    public static class OrganisationHelpers
    {
        #region Get

        public static Organisation GetOrganisation(Guid organisationId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Organisation organisation = GetOrganisation(db, organisationId);
            db.Dispose();
            return organisation;
        }
        public static Organisation GetOrganisation(ApplicationDbContext db, Guid organisationId)
        {
            return db.Organisations.Find(organisationId);
        }

        public static List<Organisation> GetAllOrganisations()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Organisation> list = GetAllOrganisations(db);
            db.Dispose();
            return list;
        }
        public static List<Organisation> GetAllOrganisations(ApplicationDbContext db)
        {
            List<Organisation> list = (from o in db.Organisations
                                       where o.EntityStatus == EntityStatusEnum.Active
                                       orderby o.OrganisationName
                                       select o).ToList();
            return list;
        }

        #endregion

        #region Create

        public static Organisation CreateOrganisation(HomeOrganisationDetailsView model, IPrincipal user)
        {
            Guid appUserId = AppUserHelpers.GetAppUserIdFromUser(user);
            ApplicationDbContext db = new ApplicationDbContext();
            Organisation org = CreateOrganisation(db, model, appUserId);
            db.Dispose();
            return org;
        }
        public static Organisation CreateOrganisation(ApplicationDbContext db, HomeOrganisationDetailsView model, Guid appUserId)
        {
            Organisation organisation = new Organisation()
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = model.OrganisationName,
                BusinessType = model.BusinessType,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2 ?? "",
                AddressLine3 = model.AddressLine3 ?? "",
                AddressTownCity = model.AddressTownCity,
                AddressCounty = model.AddressCounty ?? "",
                AddressPostcode = model.AddressPostcode,
                TelephoneNumber = model.TelephoneNumber,
                Email = model.Email,
                Website = model.Website ?? "",
                ContactName = model.ContactName,
                CompanyRegistrationDetails = model.CompanyRegistrationDetails ?? "",
                CharityRegistrationDetails = model.CharityRegistrationDetails ?? "",
                VATRegistrationDetails = model.VATRegistrationDetails ?? "",
                ListingPrivacyLevel = model.ListingPrivacyLevel,
                PrivacyLevel = model.PrivacyLevel,
                GroupPrivacyLevel = model.GroupPrivacyLevel,
                EntityStatus = EntityStatusEnum.Active,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeBy = appUserId,
                RecordChangeOn = DateTime.Now
            };

            db.Organisations.Add(organisation);
            db.SaveChanges();

            return organisation;
        }

        #endregion

        #region Update

        public static Organisation UpdateOrganisation(ApplicationDbContext db, OrganisationAdminView view, IPrincipal user)
        {
            Organisation organisation = GetOrganisation(db, view.OrganisationId);
            organisation.OrganisationName = view.OrganisationName;
            organisation.BusinessType = view.BusinessType;
            organisation.AddressLine1 = view.AddressLine1;
            organisation.AddressLine2 = view.AddressLine2;
            organisation.AddressLine3 = view.AddressLine3;
            organisation.AddressTownCity = view.AddressTownCity;
            organisation.AddressCounty = view.AddressCounty;
            organisation.AddressPostcode = view.AddressPostcode;
            organisation.TelephoneNumber = view.TelephoneNumber;
            organisation.Email = view.Email;
            organisation.Website = view.Website;
            organisation.ContactName = view.ContactName;
            organisation.CompanyRegistrationDetails = view.CompanyRegistrationDetails;
            organisation.CharityRegistrationDetails = view.CharityRegistrationDetails;
            organisation.VATRegistrationDetails = view.VATRegistrationDetails;
            organisation.ListingPrivacyLevel = view.ListingPrivacyLevel;
            organisation.PrivacyLevel = view.PrivacyLevel;
            organisation.GroupPrivacyLevel = view.GroupPrivacyLevel;
            organisation.RecordChange = RecordChangeEnum.RecordUpdated;
            organisation.RecordChangeOn = DateTime.Now;
            organisation.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);

            db.Entry(organisation).State = EntityState.Modified;
            db.SaveChanges();

            return organisation;
        }

        #endregion

        #region Delete
        #endregion
    }

    public static class OrganisationViewHelpers
    {
        #region Get

        public static OrganisationAdminView GetOrganisationAdminView(ApplicationDbContext db, Guid organisationId)
        {
            Organisation org = OrganisationHelpers.GetOrganisation(db, organisationId);

            OrganisationAdminView view = new OrganisationAdminView()
            {
                OrganisationId = org.OrganisationId,
                OrganisationName = org.OrganisationName,
                BusinessType = org.BusinessType,
                AddressLine1 = org.AddressLine1,
                AddressLine2 = org.AddressLine2,
                AddressLine3 = org.AddressLine3,
                AddressTownCity = org.AddressTownCity,
                AddressCounty = org.AddressCounty,
                AddressPostcode = org.AddressPostcode,
                TelephoneNumber = org.TelephoneNumber,
                Email = org.Email,
                Website = org.Website,
                ContactName = org.ContactName,
                CompanyRegistrationDetails = org.CompanyRegistrationDetails,
                CharityRegistrationDetails = org.CharityRegistrationDetails,
                VATRegistrationDetails = org.VATRegistrationDetails,
                ListingPrivacyLevel = org.ListingPrivacyLevel,
                PrivacyLevel = org.PrivacyLevel,
                GroupPrivacyLevel = org.GroupPrivacyLevel
            };

            return view;
        }

        #endregion
    }
}