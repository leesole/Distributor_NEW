using Distributor.Models;
using Distributor.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using static Distributor.Enums.EntityEnums;
using static Distributor.Enums.GeneralEnums;
using static Distributor.Enums.GroupEnums;

namespace Distributor.Helpers
{
    public static class GroupHelpers
    {
        #region Get

        //Get all groups that have been created by a specific Organisation
        public static List<Group> GetGroupsCreatedByOrg(ApplicationDbContext db, Guid organisationId)
        {
            List<Group> groups = (from g in db.Groups
                                  where g.GroupOriginatorOrganisationId == organisationId
                                  select g).Distinct().ToList();

            return groups;
        }

        //Get all groups that have members belonging to an organisation but have not themselves been set up by that organisation
        public static List<Group> GetGroupsContainingOrg(ApplicationDbContext db, Guid organisationId)
        {
            List<Group> groups = (from g in db.Groups
                                  join gm in db.GroupMembers on g.GroupId equals gm.GroupId
                                  where (gm.OrganisationId == organisationId && g.GroupOriginatorOrganisationId != organisationId)
                                  select g).Distinct().ToList();

            return groups;
        }

        #endregion

        #region Create

        public static Group CreateGroup(ApplicationDbContext db, GroupViewCreateModel model, IPrincipal user)
        {
            Group group = new Group()
            {
                GroupId = Guid.NewGuid(),
                Name = model.Name,
                VisibilityLevel = model.VisibilityLevel,
                InviteLevel = model.InviteLevel,
                AcceptanceLevel = model.AcceptanceLevel,
                EntityStatus = EntityStatusEnum.Active,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeOn = DateTime.Now,
                RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user),
                GroupOriginatorAppUserId = AppUserHelpers.GetAppUserIdFromUser(user),
                GroupOriginatorOrganisationId = AppUserHelpers.GetOrganisationIdFromUser(user),
                GroupOriginatorDateTime = DateTime.Now
            };

            db.Groups.Add(group);
            db.SaveChanges();

            if (model.GroupMembers != null)
                GroupMembersHelpers.CreateGroupMembers(db, model.GroupMembers, group.GroupId, user);

            return group;
        }

        #endregion

        #region Update

        public static Group UpdateEntityStatus(ApplicationDbContext db, Guid groupId, EntityStatusEnum entityStatus, IPrincipal user)
        {
            try
            {
                Group group = db.Groups.Find(groupId);

                group.EntityStatus = entityStatus;
                group.RecordChange = RecordChangeEnum.StatusChange;
                group.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
                group.RecordChangeOn = DateTime.Now;
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();

                return group;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Remove

        public static Group RemoveGroup(ApplicationDbContext db, Guid groupId, IPrincipal user)
        {
            //To remove we just change status
            return UpdateEntityStatus(db, groupId, EntityStatusEnum.Removed, user);
        }

        #endregion
    }

    public static class GroupMembersHelpers
    {
        #region Get

        public static List<GroupMember> GetGroupMembersForGroup(ApplicationDbContext db, Guid groupId)
        {
            List<GroupMember> list = (from gm in db.GroupMembers
                                      where gm.GroupId == groupId
                                      select gm).Distinct().ToList();

            return list;
        }

        #endregion

        #region Create

        //Create a group member record from the GroupMemberViewCreateModel
        public static GroupMember CreateGroupMember(ApplicationDbContext db, GroupMemberViewCreateModel memberModel, Guid groupId, IPrincipal user)
        {
            GroupMember member = new GroupMember()
            {
                GroupMemberId = Guid.NewGuid(),
                GroupId = groupId,
                OrganisationId = memberModel.SelectedOrganisationId,
                AddedBy = AppUserHelpers.GetAppUserIdFromUser(user),
                AddedDateTime = DateTime.Now,
                Status = GroupMemberStatusEnum.Accepted,
                RecordChange = RecordChangeEnum.NewRecord,
                RecordChangeOn = DateTime.Now,
                RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user)
            };

            db.GroupMembers.Add(member);
            db.SaveChanges();

            return member;
        }

        //to create the members from the List<view> page through the List and call the single creation of the GroupMember
        public static List<GroupMember> CreateGroupMembers(ApplicationDbContext db, List<GroupMemberViewCreateModel> membersModel, Guid groupId, IPrincipal user)
        {
            List<GroupMember> list = new List<GroupMember>();

            foreach (GroupMemberViewCreateModel memberModel in membersModel)
            {
                GroupMember item = CreateGroupMember(db, memberModel, groupId, user);
                list.Add(item);
            }

            return list;
        }

        #endregion
    }

    public static class GroupViewHelpers
    {
        #region Get

        public static GroupIndexViewModel GetGroupIndexViewModel(ApplicationDbContext db, IPrincipal user)
        {
            List<GroupViewModel> GroupsCreatedByOrg = GetGroupsViewCreatedByOrg(db, user);
            List<GroupViewModel> GroupsContainingOrg = GetGroupsViewContainingOrg(db, user);

            GroupIndexViewModel model = new GroupIndexViewModel()
            {
                GroupsCreatedByOrg = GroupsCreatedByOrg,
                GroupsContainingOrg = GroupsContainingOrg
            };

            return model;
        }

        public static List<GroupViewModel> GetGroupsViewCreatedByOrg(ApplicationDbContext db, IPrincipal user)
        {
            List<GroupViewModel> groupsViewCreatedByOrg = new List<GroupViewModel>();

            //get organisation from User
            Organisation organisation = OrganisationHelpers.GetOrganisation(db, AppUserHelpers.GetOrganisationIdFromUser(db, user));

            //get list of groups created by this organisation
            List<Group> groupsCreatedByOrg = GroupHelpers.GetGroupsCreatedByOrg(db, organisation.OrganisationId);

            //build view
            foreach (Group group in groupsCreatedByOrg)
            {
                //Build members view for this group
                List<GroupMemberViewModel> membersView = GroupMembersViewHelpers.GetGroupMembersViewForGroup(db, group.GroupId);

                //Build objects of guid details
                AppUser recordChangedBy = AppUserHelpers.GetAppUser(db, group.RecordChangeBy);
                AppUser groupOriginatorAppUser = AppUserHelpers.GetAppUser(db, group.GroupOriginatorAppUserId);
                Organisation groupOriginatorOrganisation = OrganisationHelpers.GetOrganisation(db, group.GroupOriginatorOrganisationId);

                //Build group view
                GroupViewModel groupView = new GroupViewModel()
                {
                    GroupId = group.GroupId,
                    Name = group.Name,
                    InviteLevel = group.InviteLevel,
                    VisibilityLevel = group.VisibilityLevel,
                    AcceptanceLevel = group.AcceptanceLevel,
                    EntityStatus = group.EntityStatus,
                    RecordChange = group.RecordChange,
                    RecordChangeOn = group.RecordChangeOn,
                    RecordChangeBy = recordChangedBy,
                    GroupOriginatorAppUser = groupOriginatorAppUser,
                    GroupOriginatorOrganisation = groupOriginatorOrganisation,
                    GroupOriginatorDateTime = group.GroupOriginatorDateTime,
                    GroupMembers = membersView
                };

                groupsViewCreatedByOrg.Add(groupView);
            }

            return groupsViewCreatedByOrg;
        }

        public static List<GroupViewModel> GetGroupsViewContainingOrg(ApplicationDbContext db, IPrincipal user)
        {
            List<GroupViewModel> groupsViewContainingOrg = new List<GroupViewModel>();

            //get organisation from User
            Organisation organisation = OrganisationHelpers.GetOrganisation(db, AppUserHelpers.GetOrganisationIdFromUser(db, user));

            //get list of groups containing this organisation
            List<Group> groupsContainingOrg = GroupHelpers.GetGroupsContainingOrg(db, organisation.OrganisationId);
            
            //build view
            foreach (Group group in groupsContainingOrg)
            {
                //Build members view for this group (NOTE, this will not be added to for this list as this is just a list of groups we are part of, no need currently to show the members.
                List<GroupMemberViewModel> membersView = new List<GroupMemberViewModel>();  //If we do need to show the members then change this to "List<GroupMemberViewModel> membersView = GroupMembersViewHelpers.GetGroupMembersViewForGroup(db, group.GroupId);"

                //Build objects of guid details
                AppUser recordChangedBy = AppUserHelpers.GetAppUser(db, group.RecordChangeBy);
                AppUser groupOriginatorAppUser = AppUserHelpers.GetAppUser(db, group.GroupOriginatorAppUserId);
                Organisation groupOriginatorOrganisation = OrganisationHelpers.GetOrganisation(db, group.GroupOriginatorOrganisationId);

                //Build group view
                GroupViewModel groupView = new GroupViewModel()
                {
                    GroupId = group.GroupId,
                    Name = group.Name,
                    InviteLevel = group.InviteLevel,
                    VisibilityLevel = group.VisibilityLevel,
                    AcceptanceLevel = group.AcceptanceLevel,
                    EntityStatus = group.EntityStatus,
                    RecordChange = group.RecordChange,
                    RecordChangeOn = group.RecordChangeOn,
                    RecordChangeBy = recordChangedBy,
                    GroupOriginatorAppUser = groupOriginatorAppUser,
                    GroupOriginatorOrganisation = groupOriginatorOrganisation,
                    GroupOriginatorDateTime = group.GroupOriginatorDateTime,
                    GroupMembers = membersView
                };

                groupsViewContainingOrg.Add(groupView);
            }

            return groupsViewContainingOrg;
        }

        #endregion
    }

    public static class GroupMembersViewHelpers
    {
        #region Get

        public static List<GroupMemberViewModel> GetGroupMembersViewForGroup(ApplicationDbContext db, Guid groupId)
        {
            List<GroupMember> groupMembersForGroup = GroupMembersHelpers.GetGroupMembersForGroup(db, groupId);
            List<GroupMemberViewModel> list = BuildGroupMemberViewListFromGroupMemberList(db, groupMembersForGroup);

            return list;
        }

        #endregion

        #region Create

        public static List<GroupMemberViewModel> BuildGroupMemberViewListFromGroupMemberList(ApplicationDbContext db, List<GroupMember> groupMemberList)
        {
            List<GroupMemberViewModel> list = new List<GroupMemberViewModel>();

            foreach (GroupMember member in groupMemberList)
            {
                Organisation organisaion = OrganisationHelpers.GetOrganisation(db, member.OrganisationId);
                AppUser addedBy = AppUserHelpers.GetAppUser(db, member.AddedBy);
                AppUser recordChangedBy = AppUserHelpers.GetAppUser(db, member.RecordChangeBy);

                GroupMemberViewModel item = new GroupMemberViewModel()
                {
                    GroupMemberId = member.GroupMemberId,
                    GroupId = member.GroupId,
                    OrganisationDetails = organisaion,
                    AddedBy = addedBy,
                    AddedDateTime = member.AddedDateTime,
                    Status = member.Status,
                    RecordChange = member.RecordChange,
                    RecordChangeOn = member.RecordChangeOn,
                    RecordChangeBy = recordChangedBy
                };

                list.Add(item);
            }

            return list;
        }

        #endregion
    }
}