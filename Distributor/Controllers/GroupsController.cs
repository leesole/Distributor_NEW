using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Distributor.Models;
using Distributor.ViewModels;
using Distributor.Helpers;

namespace Distributor.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Groups
        public ActionResult Index()
        {
            GroupIndexViewModel model = GroupViewHelpers.GetGroupIndexViewModel(db, User);
            return View(model);
        }

        public ActionResult PastGroups()
        {
            List<GroupViewModel> model = GroupViewHelpers.GetPastGroupsViewModel(db, User);
            return View(model);
        }

        // GET: Groups/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,VisibilityLevel,InviteLevel,AcceptanceLevel")] GroupViewCreateModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form["resetbutton"] != null)
                {
                    return RedirectToAction("Create");
                }

                //Save the group before going to add members as to be here you have pressed either 'Save' or 'Add Members'
                Group newGroup = GroupHelpers.CreateGroup(db, model, User);

                if (Request.Form["addmembersbutton"] != null)
                {
                    return RedirectToAction("AddMembers", "Groups", new { groupId = newGroup.GroupId });
                }

                //all done, go back to initial list
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,Name,Type,VisibilityLevel,InviteLevel,AcceptanceLevel,EntityStatus,RecordChange,RecordChangeOn,RecordChangeBy,GroupOriginatorAppUserId,GroupOriginatorOrganisationId,GroupOriginatorDateTime")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddMembers(Guid? groupId)
        {
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Group group = db.Groups.Find(groupId);
            ViewBag.GroupName = group.Name;
            ViewBag.GroupId = group.GroupId;
            //DropDown
            ViewBag.OrganisationList = ControlHelpers.OrganisationsListForGroupDropDown(db, group.GroupId);

            List<GroupMemberViewCreateModel> model = GroupMembersViewHelpers.GetGroupMembersViewCreateForGroup(db, groupId.Value);

            return View(model);
        }

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
