using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using ST_BO.Controllers;
using System.IO;

namespace ST_BO.Controllers
{
    public class CompanyController : BaseController
    {
        private Result result;
        private CompanyUtil companyUtil;
        private CountryUtil countryUtil;
        private UserUtil userUtil;
        public CompanyController()
        {
            result = new Result();
            companyUtil = new CompanyUtil();
            countryUtil = new CountryUtil();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Company List";
            return View();
        }
        public ActionResult CompanyList()
        {

            //// var path = BaseUtil.GetWebConfigValue("SNAG_AWS_S3");
            int role = SessionUtil.GetRoleBit();
            int companyId = SessionUtil.GetCompanyID();
            var list = db.COMPANies.AsEnumerable().ToList();
            if (role != Convert.ToInt32(Role.SuperAdmin))
            {
                list = list.AsEnumerable().Where(x => x.CompanyId == companyId).ToList();
            }
            var data = (from li in list
                        select new
                        {
                            company_id = li.CompanyId,
                            CompanyName = li.CompanyName,
                            EmailId = li.EmailId,
                            address = li.CompanyAddress,
                            image = string.IsNullOrEmpty(li.LogoImage) ? "~/Base/Content/images/NA.jpg" : li.LogoImage,
                            is_active = li.IsActive,
                        }).ToList();
            return Json(data);

        }
        public ActionResult Create(string id)
        {
            //CompanyUser companyUserData = new Models.CompanyUser();
            COMPANY company = new COMPANY();
            if (id != null && id != "")
            {
                ViewBag.Title = "Company Edit";
                //companyUserData = companyUtil.GetCompanyData(id);
                company = db.COMPANies.Find(Convert.ToInt32(id));
            }
            else
            {
                ViewBag.Title = "Company Create";
            }
            countryUtil = new CountryUtil();

            ViewBag.country_id = new SelectList(countryUtil.GetCountry(), "Value", "Text", company.CompanyId > 0 ? company.CITY.STATE.CountryId : 0);
            ViewBag.state_id = new SelectList(new StateUtil().GetStateSelectList(company.CityId > 0 ? company.CITY.STATE.CountryId : 0), "Value", "Text", company.CityId > 0 ? company.CITY.StateId : 0);
            ViewBag.CityId = new SelectList(new CityUtil().GetCitySelectList(company.CityId > 0 ? company.CITY.StateId : 0), "Value", "Text", company.CityId);
            ViewBag.TimeZone = BaseUtil.GetTimeZoneInfo();
            return View(company);
        }
        [HttpPost]
        public ActionResult AddCompany(COMPANY company, FormCollection frmUser, HttpPostedFileBase company_logo)
        {
            try
            {
                string userName = frmUser["user_name"];
                string loginID = frmUser["login_id"];
                string emailID = frmUser["email_id"];
                string mobile = frmUser["mobile"];
                string password = frmUser["password"];
                string gender = frmUser["gender"];
                if (company_logo != null)
                {
                    #region Attache Documents
                    string fileName = string.Empty;
                    List<String> arrfileName = new List<String>();
                    int loop = 0;
                    // Verify that the user selected a file
                    if (company_logo != null && company_logo.ContentLength > 0)
                    {
                        // extract only the fielname
                        fileName = Guid.NewGuid() + "_" + SessionUtil.GetUserID() + "_" + Path.GetFileName(company_logo.FileName);
                        fileName = System.DateTime.Now.Millisecond.ToString() + Path.GetFileName(company_logo.FileName);
                        var path = Path.Combine(Server.MapPath("~/Files/"), fileName);
                        company_logo.SaveAs(path);
                        arrfileName.Add(fileName);
                    }
                    else
                    {
                        arrfileName.Add("NA.JPG");
                    }
                    loop++;

                    company.LogoImage = arrfileName[0].ToString();
                    #endregion
                    string GenFileName = BaseUtil.GetTodayDate().ToString("yyyyMMdd") + "_" + SessionUtil.GetRoleBit().ToString() + "_" + Path.GetFileName(company_logo.FileName).Replace(" ", "_");

                    ///  CompanyInformation.company_logo = GenFileName;
                }
                BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "1");
                Session["a"] = 1;
                result = companyUtil.PostCompanyCreateEdit(company, userName, loginID, emailID, mobile, gender, password, company_logo);
                BaseUtil.SetSessionValue(UserInfo.IsCompanySetup.ToString(), "1");
                return RedirectToAction("CompanyTabs", new { id = result.Id, Result = result.Message, MessageType = result.MessageType });

            }
            catch (Exception ex)
            {
                BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "1");
                result.Message = ex.Message;
            }
            return View();

        }

        #region Tabular Company
        public ActionResult CompanyTabs(string id)
        {
            COMPANY company = new COMPANY();
            try
            {
                if (id != null && id != "")
                {
                    company = db.COMPANies.Find(Convert.ToInt32(id));
                }
            }
            catch (Exception ex)
            {

            }
            return View(company);
        }
        public ActionResult CompanyInfo(string id)
        {
            COMPANY company = new COMPANY();
            if (id != null && id != "")
            {
                //ViewBag.Title = "Company Edit";
                company = db.COMPANies.Find(Convert.ToInt32(id));
            }
            //else
            //{
            //    ViewBag.Title = "Company Create";
            //}
            countryUtil = new CountryUtil();
            ViewBag.country_id = new SelectList(countryUtil.GetCountry(), "Value", "Text", company != null ? (company.CompanyId > 0 ? company.CITY.STATE.CountryId : 0) : 0);
            ViewBag.state_id = new SelectList(new StateUtil().GetStateSelectList(company != null ? (company.CityId > 0 ? company.CITY.STATE.CountryId : 0) : 0), "Value", "Text", company != null ? (company.CityId > 0 ? company.CITY.StateId : 0) : 0);
            ViewBag.CityId = new SelectList(new CityUtil().GetCitySelectList(company != null ? (company.CityId > 0 ? company.CITY.StateId : 0) : 0), "Value", "Text", company != null ? (company.CityId) : 0);
            ViewBag.TimeZone = BaseUtil.GetTimeZoneInfo();
            return PartialView(company);
        }
        public ActionResult CompanyUsersInfo(string id)
        {
            ViewBag.CompanyId = id;
            ViewBag.RoleBit = new SelectList(companyUtil.GetRole(), "Value", "Text", 0);
            ViewBag.CountryId = new SelectList(countryUtil.GetCountry(), "Value", "Text", 0);
            ViewBag.StateId = new SelectList(new StateUtil().GetStateSelectList(0), "Value", "Text", 0);
            ViewBag.CityId = new SelectList(new CityUtil().GetCitySelectList(0), "Value", "Text", 0);

            return PartialView("CompanyUsersInfo");
        }
        #endregion

        #region User
        public ActionResult UserListByCompanyId(string id)
        {
            int CompanyId = (id != null && id != "") ? Convert.ToInt32(id) : 0;
            USER user = new USER();
            var list = (from li in db.USERs
                        where li.CompanyId == CompanyId
                        select new
                        {
                            UserPhoto = string.IsNullOrEmpty(li.UserPhoto) ? "/Base/Content/images/NA.jpg" : "/Files/UsersPhoto/" + li.UserPhoto,
                            UserNmae = li.FirstName + " " + li.LastName,
                            LoginId = li.LoginId,
                            EmailId = li.EmailId,
                            Mobile = li.Mobile,
                            UserId = li.UserId,
                            IsActive = li.IsActive,
                        }).ToList();
            return Json(list);
        }
        public ActionResult GetUserCreateEdit(string id)
        {
            var data = (from u in db.USERs.AsEnumerable()
                        where u.UserId == Convert.ToInt32(id)
                        select new
                        {
                            UserId = u.UserId,
                            CompanyId = u.CompanyId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            EmployeeCode = u.EmployeeCode,
                            LoginId = u.LoginId,
                            Password = db.USP_Decrypt_TEXT(u.Password).FirstOrDefault(),
                            EmailId = u.EmailId,
                            Mobile = u.Mobile,
                            Gender = u.Gender,
                            Dob = u.Dob,
                            Address = u.Address,
                            CountryId = u.CITY.STATE.COUNTRY.CountryId,
                            StateIdList = new StateUtil().GetStateSelectList(u.CITY.STATE.COUNTRY.CountryId),
                            StateId = u.CITY.STATE.StateId,
                            CityIdList = new CityUtil().GetCitySelectList(u.CITY.STATE.StateId),
                            CityId = u.CityId,
                            RoleBitList = companyUtil.GetRole(),
                            RoleBit = u.RoleBit,
                            IsAccountLocked = u.IsAccountLocked,
                            IsActive = u.IsActive,
                        }).FirstOrDefault();
            return Json(data);
        }
        [HttpPost]
        public ActionResult UserCreateEdit(USER user, FormCollection frm, HttpPostedFileBase UserPhoto)
        {
            if (UserPhoto != null)
            {
                #region Attache Documents
                string fileName = string.Empty;
                List<String> arrfileName = new List<String>();
                int loop = 0;
                // Verify that the user selected a file
                if (UserPhoto != null && UserPhoto.ContentLength > 0)
                {
                    // extract only the fielname
                    fileName = Guid.NewGuid() + "_" + SessionUtil.GetUserID() + "_" + Path.GetFileName(UserPhoto.FileName);
                    fileName = System.DateTime.Now.Millisecond.ToString() + Path.GetFileName(UserPhoto.FileName);
                    var path = Path.Combine(Server.MapPath("~/Files/UsersPhoto/"), fileName);
                    UserPhoto.SaveAs(path);
                    arrfileName.Add(fileName);
                }
                else
                {
                    arrfileName.Add("NA.JPG");
                }
                loop++;

                user.UserPhoto = arrfileName[0].ToString();
                #endregion
            }
            string pass = frm["pass"] != null ? frm["pass"] : "";
            result = companyUtil.PostUserEdit(user, pass);
            switch (result.MessageType)
            {
                case MessageType.Success:
                    return RedirectToAction("CompanyTabs", "Company", new { id = user.CompanyId, Result = result.Message, MessageType = result.MessageType });

                default:
                    return RedirectToAction("CompanyTabs", "Company", new { id = user.CompanyId, Result = result.Message, MessageType = result.MessageType });
            }
        }
        #endregion
    }

}
