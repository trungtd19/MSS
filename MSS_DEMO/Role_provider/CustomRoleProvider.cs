using MSS_DEMO.Common;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MSS_DEMO.Role_provider
{
    public class CustomRoleProvider : RoleProvider
    {
        private MSSEntities db = new MSSEntities();
        public override string ApplicationName { get ; set ; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string UserName)
        {
            var userRoles = new string[] { };
            // tạo biến getrole, so sánh xem UserID đang đăng nhập có giống với tên trong db ko
            //User_Role account = db.User_Role.Single(x => x.Login.Equals("Admin"));
            //if (account != null) // Nếu giống
            //{
            userRoles= new String[] { "Admin" };
            //account.Role.Role_Name
            //}
            //else
            //    return new String[] { };
            return userRoles.ToArray();
        }





        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var userRoles = GetRolesForUser(username);
            return userRoles.Contains(roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}