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
            return db.Organisations.OrderBy(x => x.OrganisationName).ToList();
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
        #endregion

        #region Delete
        #endregion
    }
}