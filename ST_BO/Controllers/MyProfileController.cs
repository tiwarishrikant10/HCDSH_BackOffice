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
    public class MyProfileController : BaseController
    {
        UserUtil userUtil;
        EmailSettingUtil emailSettingUtil;
        Result result;
        public MyProfileController()
        {
            userUtil = new UserUtil();
            emailSettingUtil = new EmailSettingUtil();
            result = new Result();
        }
        // GET: MyProfile
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProfileIndex()
        {
            ViewBag.Title = "Profile";
            int userid = SessionUtil.GetUserID();
            user user = db.users.Find(userid);
            return View(user);
        }
        [HttpPost]
        public ActionResult EditProfile(user user, HttpPostedFileBase user_photo)
        {

            try
            {
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

                }
                result = userUtil.PostProfileEdit(user);
                ViewBag.action_name = BaseUtil.GetListAllActionByController("");
                switch (result.MessageType)
                {
                    case MessageType.Success:
                        BaseUtil.SetSessionValue(UserInfo.FullName.ToString(), Convert.ToString(user.user_name));
                        BaseUtil.SetSessionValue(UserInfo.Mobile.ToString(), Convert.ToString(user.mobile));
                        BaseUtil.SetSessionValue(UserInfo.UserPhoto.ToString(), Convert.ToString(user.user_photo));
                        BaseUtil.SetSessionValue(UserInfo.Gender.ToString(), Convert.ToString(user.gender));

                        return RedirectToAction("ProfileIndex", "MyProfile", new { Result = result.Message, MessageType = result.MessageType });
                    default:
                        return RedirectToAction("ProfileIndex", "MyProfile", new { Result = result.Message, MessageType = result.MessageType });
                }
                return View(user);
            }
            catch
            {
                return View(user);
            }
        }

        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Change Password";
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection frm)
        {
            try
            {
                int User_id = SessionUtil.GetUserID();
                string Old_password = frm["oldPassword"];
                string New_password = frm["newpassword"];
                string conform_password = frm["conformpassword"];
                result = userUtil.PostChangePassword(User_id, Old_password, New_password, conform_password);

                switch (result.MessageType)
                {
                    case MessageType.Success:
                        return RedirectToAction("ChangePassword", "MyProfile", new { Result = result.Message, MessageType = result.MessageType });
                    default:
                        return RedirectToAction("ChangePassword", "MyProfile", new { Result = result.Message, MessageType = result.MessageType });
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        public ActionResult EmailSetting()
        {
            ViewBag.email_setting_type_id = emailSettingUtil.EmailSettingTypeSelectList();
            return View(new email_setting());
        }
        public ActionResult PostEmailSetting(email_setting email_setting)
        {
            result = new Result();
            try
            {
                result = emailSettingUtil.PostEmailSetting(email_setting);
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = "Error";
            }
            return Json(result);
        }
        public ActionResult EmailSettingsList(int id)
        {
            var list = db.email_setting.AsEnumerable().ToList();
            //list = list.Where(x => x.email_setting_type.is_public).ToList();
            var data = (from li in list
                        where li.company_id == SessionUtil.GetCompanyID() && li.user_id == SessionUtil.GetUserID()
                        select new
                        {
                            email_setting_id = li.email_setting_id,
                            from_email_id = li.from_email_id,
                            from_email_name = li.from_email_name,
                            email_user_name = li.email_user_name,
                            email_setting_type_name = li.email_setting_type.email_setting_type_name,
                            is_public = li.email_setting_type.is_public,
                            is_active = li.is_active,
                            company_id = li.company_id,
                            created_date = li.created_date,
                            created_by = li.created_by,
                            updated_date = li.updated_date,
                            updated_by = li.updated_by,
                        }).ToList();
            return Json(data);
        }
        public ActionResult GetEmailSettings(string id)
        {
            var data = (from li in db.email_setting.AsEnumerable()
                        where li.email_setting_id == Convert.ToInt32(id)
                        select new
                        {
                            email_setting_id = li.email_setting_id,
                            email_setting_type_idList = emailSettingUtil.EmailSettingTypeAllList(),
                            from_email_id = li.from_email_id,
                            from_email_name = li.from_email_name,
                            email_user_name = li.email_user_name,
                            email_config_set = li.email_config_set,
                            email_port = li.email_port,
                            email_host = li.email_host,
                            email_password = li.email_password,
                            email_setting_type_id = li.email_setting_type_id,
                            email_setting_type_name = li.email_setting_type.email_setting_type_name,
                            is_public = li.email_setting_type.is_public,
                            is_active = li.is_active,
                            company_id = li.company_id,
                            created_date = li.created_date,
                            created_by = li.created_by,
                            updated_date = li.updated_date,
                            updated_by = li.updated_by,
                        }).FirstOrDefault();

            return Json(data);
        }

    }
}
