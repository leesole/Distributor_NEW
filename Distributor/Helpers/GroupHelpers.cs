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

        public static Group GetGroup(ApplicationDbContext db, Guid groupId)
        {
            return db.Groups.Find(groupId);
        }

        //Get all groups that have been created by a specific Organisation
        public static List<Group> GetGroupsCreatedByOrg(ApplicationDbContext db, Guid organisationId)
        {
            List<Group> groups = (from g in db.Groups
                                  where (g.GroupOriginatorOrganisationId == organisationId && g.EntityStatus == EntityStatusEnum.Active)
                                  select g).Distinct().ToList();

            return groups;
        }

        //Get all groups that have members belonging to an organisation but have not themselves been set up by that organisation
        public static List<Group> GetGroupsContainingOrg(ApplicationDbContext db, Guid organisationId, EntityStatusEnum memberStatus)
        {
            List<Group> groups = (from g in db.Groups
                                  join gm in db.GroupMembers on g.GroupId equals gm.GroupId
                                  where (gm.OrganisationId == organisationId && g.GroupOriginatorOrganisationId != organisationId && g.EntityStatus == EntityStatusEnum.Active && gm.EntityStatus == memberStatus)
                                  select g).Distinct().ToList();

            return groups;
        }

        //Get all groups that have members belonging to an organisation 
        public static List<Group> GetGroupsForOrg(ApplicationDbContext db, Guid organisationId)
        {
            List<Group> groups = (from g in db.Groups
                                  join gm in db.GroupMembers on g.GroupId equals gm.GroupId
                                  where (gm.OrganisationId == organisationId && g.EntityStatus == EntityStatusEnum.Active && gm.EntityStatus == EntityStatusEnum.Active)
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

            //Add user Organisation as the initial group member
            GroupMembersHelpers.CreateGroupMember(db, group.GroupId, group.GroupOriginatorOrganisationId, user);

            return group;
        }

        #endregion

        #region Update

        public static Group UpdateGroup(ApplicationDbContext db, GroupViewEditModel model, IPrincipal user)
        {
            Group group = GetGroup(db, model.GroupId);

            group.Name = model.Name;
            group.VisibilityLevel = model.VisibilityLevel;
            group.InviteLevel = model.InviteLevel;
            group.AcceptanceLevel = model.AcceptanceLevel;
            group.RecordChange = RecordChangeEnum.NewRecord;
            group.RecordChangeOn = DateTime.Now;
            group.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);

            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();

            return group;
        }

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

        public static GroupMember GetGroupMember(ApplicationDbContext db, Guid groupMemberId)
        {
            GroupMember member = (from gm in db.GroupMembers
                                  where gm.GroupMemberId == groupMemberId
                                  select gm).FirstOrDefault();

            return member;
        }

        public static GroupMember GetGroupMemberFromGroup(ApplicationDbContext db, Guid groupId, Guid organisationId)
        {
            GroupMember member = (from gm in db.GroupMembers
                                  where (gm.GroupId == groupId && gm.OrganisationId == organisationId)
                                  select gm).FirstOrDefault();

            return member;
        }

        public static List<GroupMember> GetGroupMembersForGroup(ApplicationDbContext db, Guid groupId)
        {
            List<GroupMember> list = (from gm in db.GroupMembers
                                      where (gm.GroupId == groupId && gm.EntityStatus == EntityStatusEnum.Active)
                                      select gm).Distinct().ToList();

            return list;
        }

        //Get all groupmembers IDs from all groups that have members belonging to an organisation 
        public static List<Guid> GetGroupsMembersOrgGuidsForGroupsFromOrg(ApplicationDbContext db, Guid organisationId)
        {
            List<Group> groupsForOrg = GroupHelpers.GetGroupsForOrg(db, organisationId);

            List<Guid> groupMembersOrgId = (from gm in db.GroupMembers
                                            join g in db.Groups on gm.GroupId equals g.GroupId
                                            select gm.OrganisationId).Distinct().ToList();

            return groupMembersOrgId;
        }

        //Get all groupmembers IDs from all groups that have members NOT belonging to an organisation 
        public static List<Guid> GetGroupsMembersOrgGuidsForGroupsNotFromOrg(ApplicationDbContext db, Guid organisationId)
        {
            List<Guid> groupMembersOrgId = (from gm in db.GroupMembers
                                            join g in db.Groups on gm.GroupId equals g.GroupId
                                            where (gm.OrganisationId != organisationId && g.EntityStatus == EntityStatusEnum.Active)
                                            select gm.OrganisationId).Distinct().ToList();

            return groupMembersOrgId;
        }

        #endregion

        #region Create

        //Create a group member record from the GroupMemberViewCreateModel
        public static GroupMember CreateGroupMember(ApplicationDbContext db, Guid groupId, Guid organisationId, IPrincipal user)
        {
            GroupMember member = new GroupMember()
            {
                GroupMemberId = Guid.NewGuid(),
                GroupId = groupId,
                OrganisationId = organisationId,
                AddedBy = AppUserHelpers.GetAppUserIdFromUser(user),
                AddedDateTime = DateTime.Now,
                EntityStatus = EntityStatusEnum.Active,
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
        public static List<GroupMember> CreateGroupMembers(ApplicationDbContext db, List<GroupMemberViewCreateModel> membersModel, IPrincipal user)
        {
            List<GroupMember> list = new List<GroupMember>();

            foreach (GroupMemberViewCreateModel memberModel in membersModel)
            {
                GroupMember item = CreateGroupMember(db, memberModel.GroupId, memberModel.OrganisationId, user);
                list.Add(item);
            }

            return list;
        }

        #endregion

        #region Remove

        public static void RemoveMember(ApplicationDbContext db, Guid groupMemberId)
        {
            GroupMember member = GetGroupMember(db, groupMemberId);
            db.GroupMembers.Remove(member);
            db.SaveChanges();
        }

        public static void LeaveGroup(ApplicationDbContext db, Guid groupId, Guid groupMemberId, IPrincipal user)
        {
            GroupMember member = GetGroupMemberFromGroup(db, groupId, groupMemberId);
            member.EntityStatus = EntityStatusEnum.Inactive;
            member.RecordChange = RecordChangeEnum.StatusChange;
            member.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
            member.RecordChangeOn = DateTime.Now;

            db.Entry(member).State = EntityState.Modified;
            db.SaveChanges();
        }

        public static void RejoinGroup(ApplicationDbContext db, Guid groupId, Guid groupMemberId, IPrincipal user)
        {
            GroupMember member = GetGroupMemberFromGroup(db, groupId, groupMemberId);
            member.EntityStatus = EntityStatusEnum.Active;
            member.RecordChange = RecordChangeEnum.StatusChange;
            member.RecordChangeBy = AppUserHelpers.GetAppUserIdFromUser(user);
            member.RecordChangeOn = DateTime.Now;

            db.Entry(member).State = EntityState.Modified;
            db.SaveChanges();
        }

        #endregion
    }

    public static class GroupViewHelpers
    {
        #region Get

        public static GroupIndexViewModel GetGroupIndexViewModel(ApplicationDbContext db, IPrincipal user)
        {
            List<GroupViewModel> GroupsCreatedByOrg = GetGroupsViewCreatedByOrg(db, user);
            List<GroupViewModel> GroupsContainingOrg = GetGroupsViewContainingOrg(db, EntityStatusEnum.Active, user); //only for Active members

            GroupIndexViewModel model = new GroupIndexViewModel()
            {
                GroupsCreatedByOrg = GroupsCreatedByOrg,
                GroupsContainingOrg = GroupsContainingOrg
            };

            return model;
        }

        public static GroupViewEditModel GetGroupViewEditModel(ApplicationDbContext db, Guid groupId)
        {
            Group group = db.Groups.Find(groupId);

            GroupViewEditModel model = new GroupViewEditModel()
            {
                GroupId = group.GroupId,
                Name = group.Name,
                VisibilityLevel = group.VisibilityLevel,
                InviteLevel = group.InviteLevel,
                AcceptanceLevel = group.AcceptanceLevel
            };

            return model;
        }

        public static List<GroupViewModel> GetPastGroupsViewModel(ApplicationDbContext db, IPrincipal user)
        {
            List<GroupViewModel> model = GetGroupsViewContainingOrg(db, EntityStatusEnum.Inactive, user); //Only for Inactive members

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

        public static List<GroupViewModel> GetGroupsViewContainingOrg(ApplicationDbContext db, EntityStatusEnum memberStatus, IPrincipal user)
        {
            List<GroupViewModel> groupsViewContainingOrg = new List<GroupViewModel>();

            //get organisation from User
            Organisation organisation = OrganisationHelpers.GetOrganisation(db, AppUserHelpers.GetOrganisationIdFromUser(db, user));

            //get list of groups containing this organisation
            List<Group> groupsContainingOrg = GroupHelpers.GetGroupsContainingOrg(db, organisation.OrganisationId, memberStatus);

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

        public static List<GroupMemberViewCreateModel> GetGroupMembersViewCreateForGroup(ApplicationDbContext db, Guid groupId)
        {
            List<GroupMember> groupMembersForGroup = GroupMembersHelpers.GetGroupMembersForGroup(db, groupId);
            List<GroupMemberViewCreateModel> list = BuildGroupMemberViewCreateListFromGroupMemberList(db, groupMembersForGroup);

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

        public static List<GroupMemberViewCreateModel> BuildGroupMemberViewCreateListFromGroupMemberList(ApplicationDbContext db, List<GroupMember> groupMemberList)
        {
            List<GroupMemberViewCreateModel> list = new List<GroupMemberViewCreateModel>();

            foreach (GroupMember member in groupMemberList)
            {
                Organisation organisaion = OrganisationHelpers.GetOrganisation(db, member.OrganisationId);

                GroupMemberViewCreateModel item = new GroupMemberViewCreateModel()
                {
                    GroupMemberId = member.GroupMemberId,
                    GroupId = member.GroupId,
                    OrganisationId = member.OrganisationId,
                    OrganisationName = organisaion.OrganisationName,
                    BusinessType = organisaion.BusinessType,
                    AddressLine1 = organisaion.AddressLine1,
                    AddressTownCity = organisaion.AddressTownCity,
                    AddressPostcode = organisaion.AddressPostcode
                };

                list.Add(item);
            }

            return list;
        }

        #endregion
    }

    public static class GroupFilters
    {
        public static List<AvailableListing> FilterAvailableListingsByGroup(ApplicationDbContext db, List<AvailableListing> currentList, Guid organisationId)
        {
            //Get the group Member IDs from groups that this user/organisation are part of, so we can remove them from the list
            List<Guid> groupMemberOrgIds = GroupMembersHelpers.GetGroupsMembersOrgGuidsForGroupsFromOrg(db, organisationId);

            if (groupMemberOrgIds.Count == 0)
                return new List<AvailableListing>();
            else
                //Select from currentList only those records containing the list ouf found orgIds
                return currentList.Where(x => groupMemberOrgIds.Contains(x.ListingOriginatorOrganisationId)).ToList();
        }
    }
}