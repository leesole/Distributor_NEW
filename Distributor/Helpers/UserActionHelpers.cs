using Distributor.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserActionEnums;

namespace Distributor.Helpers
{
    public static class UserActionHelpers
    {
        #region Get

        public static UserAction GetUserAction(ApplicationDbContext db, Guid userActionId)
        {
            return db.UserActions.Find(userActionId);
        }

        #endregion

        #region Create

        public static UserAction CreateUserAction(ApplicationDbContext db, ActionTypeEnum actionType, string actionDescription, Guid referenceKey, Guid referenceAppUserId, Guid referenceOrganisationId, IPrincipal user)
        {
            UserAction userAction = new UserAction()
            {
                UserActionId = Guid.NewGuid(),
                ActionType = actionType,
                ActionDescription = actionDescription,
                ReferenceKey = referenceKey,
                AppUserId = referenceAppUserId,
                OrganisationId = referenceOrganisationId,
                EntityStatus = EntityStatusEnum.Active,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user),
                RecordChangeOn = DateTime.Now
            };

            db.UserActions.Add(userAction);
            db.SaveChanges();

            return userAction;
        }

        #endregion

        #region Update

        public static UserAction UpdateUserActionEntityStatus(ApplicationDbContext db, Guid? userActionId, UserAction action, EntityStatusEnum newStatus, IPrincipal user)
        {
            if (action == null)
                action = UserActionHelpers.GetUserAction(db, userActionId.Value);

            action.EntityStatus = newStatus;
            action.RecordChange = RecordChangeEnum.StatusChange;
            action.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
            action.RecordChangeOn = DateTime.Now;

            db.Entry(action).State = EntityState.Modified;
            db.SaveChanges();

            return action;
        }

        #endregion

        #region Remove

        public static void RemoveUserActionsForOffer(ApplicationDbContext db, Guid offerId, IPrincipal user)
        {
            List<UserAction> actions = (from ua in db.UserActions
                                        where ua.ReferenceKey == offerId
                                        select ua).Distinct().ToList();

            foreach (UserAction action in actions)
                UpdateUserActionEntityStatus(db, null, action, EntityStatusEnum.Closed, user);
        }

        #endregion
    }
}