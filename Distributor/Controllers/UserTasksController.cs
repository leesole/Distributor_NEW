using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Distributor.Models;
using Distributor.Helpers;
using Distributor.ViewModels;
using static Distributor.Enums.UserTaskEnums;
using static Distributor.Enums.EntityEnums;

namespace Distributor.Controllers
{
    [Authorize]
    public class UserTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserTasks
        public ActionResult Index()
        {
            List<UserTasksViewModel> model = UserTasksViewHelpers.GetUserTasksViewModelForOrganisationFromUser(db, User, false);
            return View(model);
        }
        
        // GET: UserTasks
        public ActionResult History()
        {
            List<UserTasksViewModel> model = UserTasksViewHelpers.GetUserTasksViewModelForOrganisationFromUser(db, User, true);
            return View(model);
        }

        // GET: UserTasks/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTasksViewModel model = UserTasksViewHelpers.GetUserTasksViewModel(db, id.Value);
            return View(model);
        }

        //// GET: UserTasks/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: UserTasks/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "UserTaskId,TaskType,TaskDescription,ReferenceKey,OrganisationId,EntityStatus,RecordChange,RecordChangeOn,RecordChangeBy")] UserTask userTask)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        userTask.UserTaskId = Guid.NewGuid();
        //        db.UserTasks.Add(userTask);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(userTask);
        //}

        //// GET: UserTasks/Edit/5
        //public ActionResult Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    UserTask userTask = db.UserTasks.Find(id);
        //    if (userTask == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(userTask);
        //}

        //// POST: UserTasks/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "UserTaskId,TaskType,TaskDescription,ReferenceKey,OrganisationId,EntityStatus,RecordChange,RecordChangeOn,RecordChangeBy")] UserTask userTask)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(userTask).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(userTask);
        //}

        //// GET: UserTasks/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    UserTask userTask = db.UserTasks.Find(id);
        //    if (userTask == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(userTask);
        //}

        //// POST: UserTasks/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    UserTask userTask = db.UserTasks.Find(id);
        //    db.UserTasks.Remove(userTask);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        #region data manipulation

        public ActionResult ApproveTask(Guid? userTaskId)
        {
            if (userTaskId.HasValue)
            {
                UserTask userTask = UserTasksHelpers.GetUserTask(db, userTaskId.Value);

                switch (userTask.TaskType)
                {
                    case TaskTypeEnum.UserOnHold:  //Make AppUser active
                        AppUserHelpers.UpdateAppUserEntityStatus(db, userTask.ReferenceKey, EntityStatusEnum.Active, User);
                        break;
                }

                //close the Task
                UserTasksHelpers.UpdateEntityStatus(db, userTask.UserTaskId, EntityStatusEnum.Closed);

                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }

        public ActionResult CancelTask(Guid? userTaskId)
        {
            if (userTaskId.HasValue)
            {
                UserTask userTask = UserTasksHelpers.GetUserTask(db, userTaskId.Value);

                switch (userTask.TaskType)
                {
                    case TaskTypeEnum.UserOnHold:  //Make AppUser inactive
                        AppUserHelpers.UpdateAppUserEntityStatus(db, userTask.ReferenceKey, EntityStatusEnum.Inactive, User);
                        break;
                }

                //close the Task
                UserTasksHelpers.UpdateEntityStatus(db, userTask.UserTaskId, EntityStatusEnum.Closed);

                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
