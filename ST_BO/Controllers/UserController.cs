using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeveloperCRM.Models;
using DeveloperCRM.Base.Models;
using System.IO;

namespace DeveloperCRM.Controllers
{
    public class UserController : BaseController
    {
        UserUtil userUtil;
        Result result;
        public UserController()
        {
            userUtil = new UserUtil();
            result = new Result();
        }
        // GET: User
        public ActionResult Index(string id)
        {
            int roleID = Convert.ToInt32(id);
            role r = db.roles.Find(roleID);

            // ViewBag.Title = "User / " + r.role_name;
            ViewBag.rol_id = id;
            //ViewBag.project_id = userUtil.ProjectList();
            //ViewBag.project_block_id = userUtil.BlockList();
            //ViewBag.project_block_unit_id = userUtil.UnitList();
            //ViewBag.snag_type_id = userUtil.SnagTypeList();
            ViewBag.project_list = userUtil.GetProjectList();
            //ViewBag.user_id = new SelectList(userUtil.GetGroupWiseUser(),"Value","Text","Group.Name",0);
            return View();
        }

        public ActionResult CreateEdit(string id, string rolid)
        {
            int role_bit = Convert.ToInt32(db.roles.Find(Convert.ToInt32(rolid)).role_bit);
            role parentrole = new role();
            role parentrole2 = new role();
            List<user> userlist = new List<user>();
            int company = Convert.ToInt32(SessionUtil.GetCompanyID());
            parentrole = db.roles.SingleOrDefault(x => x.role_bit == role_bit && x.company_id == company);
            parentrole2 = db.roles.SingleOrDefault(x => x.role_id == parentrole.parent_id && x.company_id == company);
            //userlist = db.users.Where(x => x.role_bit == parentrole2.role_bit && x.company_id == company).ToList();
            user u = new user();
            u.role_bit = role_bit;
            if (parentrole2 != null)
            {
                var data = (from c in db.users.AsEnumerable()
                            where c.company_id == company && c.role_bit == parentrole2.role_bit
                            select new SelectListItem
                            {
                                Text = c.user_name,
                                Value = c.user_id.ToString(),

                            }).OrderBy(x => x.Text).ToList();
                if (id != null && id != "")
                {
                    ViewBag.Title = "User Edit";
                    u = db.users.Find(Convert.ToInt32(id));
                    string Pass = db.USP_Decrypt_TEXT(u.password).ToString();
                    ViewBag.Password = Pass;
                    ViewBag.parent_user_id = new SelectList(data, "Value", "Text", u.parent_user_id);
                }
                else
                {
                    ViewBag.parent_user_id = new SelectList(data, "Value", "Text", 0);
                }
            }
            else
            {
                var data = (from c in db.users.AsEnumerable()
                            where c.company_id == company
                            select new SelectListItem
                            {
                                Text = c.user_name,
                                Value = c.user_id.ToString(),

                            }).OrderBy(x => x.Text).ToList();
                if (id != null && id != "")
                {
                    ViewBag.Title = "User Edit";
                    u = db.users.Find(Convert.ToInt32(id));
                    string Pass = db.USP_Decrypt_TEXT(u.password).ToString();
                    ViewBag.Password = Pass;
                    ViewBag.parent_user_id = new SelectList(data, "Value", "Text", u.parent_user_id);
                }
                else
                {
                    ViewBag.parent_user_id = new SelectList(data, "Value", "Text", 0);
                }
                //ViewBag.parent_user_id = (from c in db.users.AsEnumerable()
                //                          where c.company_id == company && c.role_bit == parentrole2.role_bit
                //                          select new SelectListItem
                //                          {
                //                              Value = c.user_id.ToString(),
                //                              Text = c.user_name

                //                          }).OrderBy(x => x.Text).ToList();
            }
            ViewBag.rol_id = rolid;
            //ViewBag.parent_user_id = new SelectList(userlist, "Value", "Text");
            int roleBit = 0;

             //List<SelectListItem> plist = (from c in db.users.AsEnumerable()
             //                         where c.company_id == company && c.role_bit == parentrole2.role_bit
             //                         select new SelectListItem
             //                         {
             //                             Value = c.user_id.ToString(),
             //                             Text = c.user_name

             //                         }).OrderBy(x => x.Text).ToList();
             //ViewBag.parent_user_id = new SelectList(plist, "Value", "Text", u.parent_user_id);
            //else
            //{
            //    if (rolid != null && rolid != "")
            //    {
            //        u.role_bit = Convert.ToInt32(db.roles.Find(rol_id).role_bit);
            //        roleBit = u.role_bit;
            //    }
            //    ViewBag.Title = "User Create";
            //}
            //ViewBag.rol_id = rolid;
            //ViewBag.IsParentDDL = userUtil.IsParentDDL(rol_id);
            //if (userUtil.IsParentDDL(rol_id))
            //{
            //    ViewBag.parent_user_id = new SelectList(userUtil.ParentUserList(roleBit), "Value", "Text", u != null ? u.parent_user_id : 0);
            //}
            return View(u);
        }

        [HttpPost]
        public ActionResult CreateEdit(user user, FormCollection frm, HttpPostedFileBase user_photo)
        {
            try
            {
                string rol_id = frm["rol_id"];
                if (user_photo != null)
                {
                    string AWSProfileName = BaseUtil.GetWebConfigValue("AWSProfileName");
                    string GenFileName = BaseUtil.GetTodayDate().ToString("yyyyMMdd") + "_" + SessionUtil.GetCompanyID().ToString() + "_" + Path.GetFileName(user_photo.FileName).Replace(" ", "_");
                    String companyFolderName = BaseUtil.GetSessionValue(UserInfo.CompanyFolderName.ToString()).ToString().Replace("/", "");
                    AWSUtil.UploadFile(user_photo.InputStream, AWSProfileName, companyFolderName, GenFileName);

                    user.user_photo = GenFileName;
                }
                else
                {
                    user.user_photo = user.gender + ".JPG";
                }
                result = userUtil.PostCreateEdit(user, frm);
                ViewBag.Title = user == null ? "User Create" : "User Edit";
                ViewBag.action_name = BaseUtil.GetListAllActionByController("");
                switch (result.MessageType)
                {
                    case MessageType.Success:
                        return RedirectToAction("Index", "User", new { id = rol_id, Result = result.Message, MessageType = result.MessageType });

                    default:
                        return RedirectToAction("CreateEdit", "User", new { id = user.user_id, Result = result.Message, MessageType = result.MessageType });
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View(user);
            }
        }
        public ActionResult UserIndex(string id)
        {
            int company_id = SessionUtil.GetCompanyID();
            var roleList = db.roles.Where(x => x.is_public && x.is_active && x.company_id == company_id).ToList();
            return View(roleList);
        }

        [HttpPost]
        public ActionResult AssingProject(string []property_listing_id , int user_id, int project_id)
        {
            result = userUtil.SaveAssignedProjectToUser(property_listing_id, user_id, project_id);
            ViewBag.action_name = BaseUtil.GetListAllActionByController("");
            return Json(result);
        }
        public ActionResult Teamlist(int id)
        {
            var user = db.users.AsEnumerable().Where(x => x.parent_user_id == id).ToList();
            return View(user);
        }
        [HttpPost]
        public ActionResult AddToUserTeam(string [] user_id,int owner_id)
        {
            result = userUtil.AddUserToTeam(user_id, owner_id);
            return Json(result);
        }
    }
}
