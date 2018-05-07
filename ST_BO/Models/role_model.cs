using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ST_BO.Models
{
    #region Classes & Enum
    public class RoleActionModel
    {
        public ROLE_ACTION RoleAction { get; set; }
        public IList<ROLE_ACTION> ListRoleActionAssigned { get; set; }
        public RoleActionModel()
        {
            RoleAction = new ROLE_ACTION();
            ListRoleActionAssigned = new List<ROLE_ACTION>();
        }
    }
    public class ControllerAction
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class AssignController
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsAssigned { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }

    public enum Role
    {
        SuperAdmin = 1,
        //Admin = 2,
        //CRMExecutive = 4,
        //Account = 8,
        //Manager = 16,
        //Agent = 32,

    }

    //public enum RoleIds
    //{
    //    SuperAdmin = 1,
    //    Admin = 2,
    //    CRMExecutive = 3,
    //    Account = 4,
    //    Manager = 5,
    //    Agent = 6,
    //}

    #endregion
    #region Business
    public static class RoleUtil
    {
        private static ST_BASE_DATAEntities db = new ST_BASE_DATAEntities();
        private static Result result;


        public static List<string> GetRoles(Int64 RoleBit)
        {
            List<Int64> authlevels = new List<Int64>();
            List<string> roles = new List<string>();
            try
            {
                string roleBit = RoleBit == 0 ? BaseUtil.GetSessionValue(UserInfo.RoleBit.ToString()) : RoleBit.ToString();
                if (!string.IsNullOrEmpty(roleBit))
                {
                    Int64 Value = Convert.ToInt64(roleBit);
                    Int64 result = 0;
                    for (Int64 i = 0; Value >= (Int64)Math.Pow(2, i); i++)
                    {
                        result = Value & (Int64)Math.Pow(2, i);
                        authlevels.Add(result);
                    }

                    foreach (var item in authlevels)
                    {
                        var au = (from R in db.ROLEs
                                  where R.RoleBit == item
                                  select new
                                  {
                                      ROLE_BIT = R.RoleBit,
                                      ROLE_NAME = R.RoleName.Trim().ToUpper()
                                  }
                                  ).FirstOrDefault();

                        if (au != null)
                        {
                            roles.Add(au.ROLE_NAME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return roles;
        }

        public static bool IsInRole(Role role, Int64 RoleBit = 0, Boolean IsGetFromSession = true)
        {
            bool isRole = false;
            string roleValue = string.Empty;
            roleValue = IsGetFromSession ? BaseUtil.GetSessionValue(role.ToString()) : string.Empty;
            if (!string.IsNullOrEmpty(roleValue))
            {
                isRole = Convert.ToString(roleValue).ToUpper() == "TRUE" ? true : false;
                return isRole;
            }
            else
            {
                //return GetRoles(RoleBit).Where(x => x.Contains(role.ToString().ToUpper())).Count() > 0 ? true : false;
                var r = GetRoles(RoleBit);
                return GetRoles(RoleBit).Where(x => x == (role.ToString().ToUpper())).Count() > 0 ? true : false;
            }
        }

        public static List<SelectListItem> GetPublicRole()
        {
            return (from c in db.ROLEs.AsEnumerable()
                    where c.IsPublic
                    orderby c.RoleName
                    select new SelectListItem
                    {
                        Value = c.RoleBit.ToString(),
                        Text = c.RoleName
                    }).ToList();
        }
        public static List<SelectListItem> GetParentRoleByCompany(int company_id = 0)
        {
            db = new ST_BASE_DATAEntities();
            var list = (from c in db.ROLEs.AsEnumerable()
                        where c.CompanyId == company_id
                        orderby c.RoleName
                        select new SelectListItem
                        {
                            Value = c.RoleId.ToString(),
                            Text = c.RoleName
                        }).ToList();
            return list;
        }
        public static List<TreeNode> GetMenusOfRoleId(int roleId, int CompanyID)
        {
            var list = db.GetMenu(roleId, CompanyID).OrderBy(x => x.SequenceOrder).ToList();
            List<TreeNode> list1 = new List<TreeNode>();

            for (int i = 0; i < list.Count(); i++)
            {
                GetMenu_Result item = list[i] as GetMenu_Result;
                list1.Add(new TreeNode()
                {
                    Id = Convert.ToString(item.MenuId),
                    ParentId = Convert.ToString(item.MenuParentId),
                    Name = item.MenuName,
                    Icon = item.Icon,
                    Sequence = item.SequenceOrder,
                    Url = VirtualPathUtility.ToAbsolute(string.Format("~/{0}/{1}/{2}", item.ControllerName, item.ActionName, string.IsNullOrEmpty(item.Id) ? "" : item.Id)),
                });
            }

            //var list = (from me in db.menus.AsEnumerable()
            //            join rm in db.role_menu.AsEnumerable() on me.menu_id equals rm.menu_id 
            //            where rm.role_id == roleId
            //            select new TreeNode
            //            {
            //                Id = Convert.ToString(me.menu_id),
            //                ParentId = Convert.ToString(me.menu_parent_id),
            //                Name = me.menu_name,
            //                Url = string.Format("/{0}/{1}/{2}",me.controller_name, me.action_name, string.IsNullOrEmpty(me.id) ? "" : me.id)
            //            }).ToList();

            return list1;
        }

        public static String CheckUserFrofile(String LoginID, String PWD, Int32 userID = 0)
        {
            String result = "Invalid Login ID or Password ";

            var userInfo = (from U in db.USP_Login(LoginID, PWD, true)
                            select new
                            {
                                UserID = U.UserId,
                                IsActive = U.IsActive,
                                CompanyActive = U.CompanyActive,
                                FullName = U.FirstName,
                                LoginID = U.LoginId,
                                Email = U.EmailId,
                                gender = U.Gender,
                                Mobile = U.mobile,
                                RoleBit = U.RoleBit,
                                RoleName = U.RoleName,
                                RoleID = U.RoleId,

                                CompanyID = U.CompanyId,
                                CompanyName = "ST Technology",
                                Photo = U.UserPhoto,

                                last_login_date = U.LastLoginDate,
                                password_failed_attempt = U.PasswordFailedAttempt,
                                is_account_locked = Convert.ToBoolean(U.IsAccountLocked),
                            }).FirstOrDefault();
            #region AutoLogin By UserID


            if (userID > 0)
            {
                userInfo = (from U in db.USERs.AsEnumerable()
                            join C in db.COMPANies.AsEnumerable() on U.CompanyId equals C.CompanyId
                            join R in db.ROLEs.AsEnumerable() on U.RoleBit equals R.RoleBit
                            where U.UserId == userID
                            select new
                            {
                                UserID = U.UserId,
                                IsActive = U.IsActive,
                                CompanyActive = C.IsActive,
                                FullName = U.FirstName,
                                LoginID = U.LoginId,
                                Email = U.EmailId,
                                gender = U.Gender,
                                Mobile = U.Mobile,
                                RoleBit = U.RoleBit,
                                RoleName = R.RoleName,
                                RoleID = R.RoleId,

                                CompanyID = U.CompanyId,
                                CompanyName = "ST Technology",
                                Photo = U.UserPhoto,

                                last_login_date = U.LastLoginDate,
                                password_failed_attempt = U.PasswordFailedAttempt,
                               is_account_locked =Convert.ToBoolean(U.IsAccountLocked),
                            }).FirstOrDefault();    
            }
            #endregion
            if (userInfo == null)
            {
                var userRecord = (from U in db.USERs
                                  join C in db.COMPANies on U.CompanyId equals C.CompanyId
                                  where (U.LoginId == LoginID)
                                  select U).FirstOrDefault();
                if (userRecord != null)
                {
                    result = BaseUtil.UserLoginPolicy(userRecord.UserId, true);
                }
            }
            try
            {
                if (userInfo != null)
                {

                    if (userInfo.is_account_locked == true)
                    {
                        result = "Your acount has been locked. Due to multiple invalid password ! Please contact to Admin";
                    }
                    else
                    {
                        if (userInfo.IsActive==true)
                        {
                            BaseUtil.SetSessionValue(UserInfo.UserID.ToString(), Convert.ToString(userInfo.UserID));
                            BaseUtil.SetSessionValue(UserInfo.FullName.ToString(), Convert.ToString(userInfo.FullName));

                            BaseUtil.SetSessionValue(UserInfo.LoginID.ToString(), Convert.ToString(userInfo.LoginID));
                            BaseUtil.SetSessionValue(UserInfo.EmailID.ToString(), Convert.ToString(userInfo.Email));
                            BaseUtil.SetSessionValue(UserInfo.Mobile.ToString(), Convert.ToString(userInfo.Mobile));
                            BaseUtil.SetSessionValue(UserInfo.RoleBit.ToString(), Convert.ToString(userInfo.RoleBit));
                            BaseUtil.SetSessionValue(UserInfo.RoleID.ToString(), Convert.ToString(userInfo.RoleID));
                            BaseUtil.SetSessionValue(UserInfo.RoleName.ToString(), Convert.ToString(userInfo.RoleName));
                            BaseUtil.SetSessionValue(UserInfo.CompanyID.ToString(), Convert.ToString(userInfo.CompanyID));

                            BaseUtil.SetSessionValue(UserInfo.CompanyName.ToString(), Convert.ToString(userInfo.CompanyName));
                            BaseUtil.SetSessionValue(UserInfo.UserPhoto.ToString(), Convert.ToString(userInfo.Photo));
                            BaseUtil.SetSessionValue(UserInfo.Gender.ToString(), Convert.ToString(userInfo.gender.ToUpper()));
                            BaseUtil.SetSessionValue(UserInfo.SuperAdmin.ToString(), Convert.ToString(RoleUtil.IsInRole(Role.SuperAdmin, userInfo.RoleBit)));
                            BaseUtil.SetSessionValue(UserInfo.IsCompanySetup.ToString(), "1");
                            BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "1");
                            BaseUtil.UserLoginPolicy(userInfo.UserID, false);
                            result = "PASS";

                        }
                        else
                        {
                            result = !userInfo.IsActive ? "Your are Inactive! Please contact to Admin" : String.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        public static Result GetRoleList(string id = "")
        {
            try
            {
                result = new Result();
                string[] info = id.Split(',');
                if ((info[0]).Trim() == ("-1").Trim())
                {
                    int company_id = String.IsNullOrEmpty(info[1]) ? 0 : Convert.ToInt32(info[1]);
                    result.Object = (from R in db.ROLEs
                                     join R1 in db.ROLEs on R.RoleId equals R1.RoleId into b_temp
                                     from b_value in b_temp.DefaultIfEmpty()
                                     where R.CompanyId == company_id
                                     select new
                                     {
                                         role_id = R.RoleId,
                                         role_name = R.RoleName,
                                         is_public = R.IsPublic == true ? "True" : "False",
                                         is_active = R.IsActive,
                                         company_id = R.CompanyId,
                                         role_bit = R.RoleBit,
                                         parent_role_name = b_value.RoleName,
                                     });


                }
                else
                {
                    result.Object = (from R in db.ROLEs
                                     select new
                                     {
                                         role_id = R.RoleId,
                                         role_name = R.RoleName,
                                         role_bit = R.RoleBit,

                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }



    }
    #endregion
}