using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.UserEnums;
using static Distributor.Enums.UserTaskEnums;

namespace Distributor.Helpers
{
    public static class UserTasksHelpers
    {
        #region Get

        public static UserTask GetUserTask(ApplicationDbContext db, Guid userTaskId)
        {
            return db.UserTasks.Find(userTaskId);
        }

        public static List<UserTask> GetUserTasksForOrganisationFromUser(IPrincipal user, bool getHistory)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<UserTask> userTask = GetUserTasksForOrganisationFromUser(db, user, getHistory);
            db.Dispose();
            return userTask;
        }
        public static List<UserTask> GetUserTasksForOrganisationFromUser(ApplicationDbContext db, IPrincipal user, bool getHistory)
        {
            AppUser appUser = AppUserHelpers.GetAppUser(db, user);
            EntityStatusEnum status = EntityStatusEnum.Active;
            if (getHistory)
                status = EntityStatusEnum.Inactive;

            List<UserTask> list = (from ut in db.UserTasks
                                   where (ut.OrganisationId == appUser.OrganisationId && ut.EntityStatus == status)
                                   orderby ut.RecordChangeBy ascending
                                   select ut).Distinct().ToList();

            return list;
        }

        #endregion

        #region Create

        public static UserTask CreateUserTask(TaskTypeEnum taskType, string taskDescription, Guid referenceKey, string referenceEmail, Guid referenceOrganisation, IPrincipal user)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            UserTask userTask = CreateUserTask(db, taskType, taskDescription, referenceKey, referenceEmail, referenceOrganisation, user);
            db.Dispose();
            return userTask;
        }

        public static UserTask CreateUserTask(ApplicationDbContext db, TaskTypeEnum taskType, string taskDescription, Guid referenceKey, string referenceEmail, Guid referenceOrganisationId, IPrincipal user)
        {
            UserTask userTask = new UserTask()
            {
                UserTaskId = Guid.NewGuid(),
                TaskType = taskType,
                TaskDescription = taskDescription,
                ReferenceKey = referenceKey,
                ReferenceEmail = referenceEmail,
                OrganisationId = referenceOrganisationId,
                EntityStatus = EntityStatusEnum.Active,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user),
                RecordChangeOn = DateTime.Now
        };

            db.UserTasks.Add(userTask);
            db.SaveChanges();

            return userTask;
        }

        #endregion

        #region Update

        public static UserTask UpdateEntityStatus(ApplicationDbContext db, Guid userTaskId, EntityStatusEnum entityStatus)
        {
            UserTask userTask = GetUserTask(db, userTaskId);
            userTask.EntityStatus = entityStatus;
            db.Entry(userTask).State = EntityState.Modified;
            db.SaveChanges();

            return userTask;
        }

        #endregion
    }

    public static class UserTasksViewHelpers
    {
        #region Get

        public static UserTasksViewModel GetUserTasksViewModel(ApplicationDbContext db, Guid userTaskId)
        {
            UserTask task = UserTasksHelpers.GetUserTask(db, userTaskId);

            //build view
            UserTasksViewModel view = CreateUserTasksViewModel(db, task);

            return view;
        }

        public static List<UserTasksViewModel> GetUserTasksViewModelForOrganisationFromUser(ApplicationDbContext db, IPrincipal user, bool getHistory)
        {
            List<UserTasksViewModel> list = new List<UserTasksViewModel>();

            foreach (UserTask task in UserTasksHelpers.GetUserTasksForOrganisationFromUser(db, user, getHistory))
            {
                //build view
                UserTasksViewModel view = CreateUserTasksViewModel(db, task);

                list.Add(view);
            }

            return list;
        }

        #endregion

        #region Create

        //Build a UserTasksViewModel record from a UserTask
        public static UserTasksViewModel CreateUserTasksViewModel(ApplicationDbContext db, UserTask task)
        {
            //set up the links to the relative areas
            AppUser appUser = null;

            //create the objects for the links to relative areas
            switch (task.TaskType)
            {
                case TaskTypeEnum.UserOnHold:
                    appUser = AppUserHelpers.GetAppUser(db, task.ReferenceKey);
                    break;
            }

            //build view
            UserTasksViewModel view = new UserTasksViewModel()
            {
                UserTaskId = task.UserTaskId,
                TaskType = task.TaskType,
                TaskDescription = task.TaskDescription,
                AppUser = appUser,
                ChangedOn = task.RecordChangeOn
            };

            return view;
        }

        #endregion
    }
}