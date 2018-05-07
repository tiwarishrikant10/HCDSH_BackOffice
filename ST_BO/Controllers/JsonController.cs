using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using ST_BO.Controllers;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;

namespace ST_BO.Controllers
{
    public class JsonController : BaseController
    {
     
        public Result Result { get; set; }
        private CountryUtil countryUtil;
        private StateUtil stateUtil;
        private UserUtil userUtil;
        private CompanyUtil companyUtil;
        
        public JsonController()
        {
           
            Result = new Result();
            countryUtil = new CountryUtil();
            stateUtil = new StateUtil();
            userUtil = new UserUtil();
           
        }

 
        #region
        public ActionResult GetOwnerId(string UserName)
        {
            //byte[] byteContent = LegalDoc_Completion.GenerateLegalDoc();
            //File(byteContent, string.Empty, "CompletionDocs.pdf");
            //Response.ClearHeaders();
            //Response.ClearContent();
            //Response.ContentType = "application/octet-stream";
            //Response.Charset = "UTF-8";
            //Response.AddHeader("content-disposition", "attachment; filename=" + string.Format("CompletionDocs_{0}.pdf", "1"));
            //Response.BinaryWrite(byteContent);
           
            return Json(1, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Email & Login ID
        public ActionResult IsEmailAddressExist(string id)
        {
            //id="country_name,country_id"
            string[] info = id.Split(',');
            string emailID = id;
            int RoleBit = 0;

            if (info.Length == 2)
            {
                emailID = info[0];
                RoleBit = info[1] != "" ? Convert.ToInt32(info[1]) : RoleBit;
            }
            return Json(userUtil.IsEmailAddressExist(emailID, RoleBit) == null ? false : true);
        }

        public ActionResult IsLoginIDExist(string id)
        {
            string LoginID = id;
            return Json(userUtil.IsLoginIDExist(LoginID) == null ? false : true);
        }
        #endregion

        #region User
        public ActionResult IsEmailIdExist(string id)
        {
            string[] info = id.Split(',');
            string emailID = id;
            int userId = 0;

            if (info.Length == 2)
            {
                emailID = info[0];
                userId = info[1] != "" ? Convert.ToInt32(info[1]) : userId;
            }
            return Json(userUtil.IsEmailExist(emailID, userId) == null ? false : true);
        }
        public ActionResult getRegPassword(string id)
        {
            int user_id = Convert.ToInt32(id);
            //int company_id = SessionUtil.GetCompanyID();
            var userData = db.USERs.Find(user_id);
            int company_id = userData.CompanyId;
            //string password = Convert.ToString(db.USP_Decrypt_TEXT(userData.password));
          //  string password = db.USP_GetUserPassword(user_id, company_id).ToList().FirstOrDefault().password;
            return Json("");
        }
        public ActionResult IsRegEmailIdExist(string id)
        {
            string[] info = id.Split(',');
            string emailID = id;
            int roleBit = 0;

            if (info.Length == 2)
            {
                emailID = info[0];
                roleBit = info[1] != "" ? Convert.ToInt32(info[1]) : roleBit;
            }
            return Json(userUtil.IsRegEmailExist(emailID, roleBit) == null ? false : true);
        }
        public ActionResult UsersList(string id)
        {
            int rolebit = Convert.ToInt32(db.ROLEs.Find(Convert.ToInt32(id)).RoleBit);
            IList<USER> list = db.USERs.AsEnumerable().Where(x => x.CompanyId == SessionUtil.GetCompanyID() && x.RoleBit == rolebit).ToList();
            var data = (from li in list
                        select new
                        {
                            user_id = li.UserId,
                            user_name = li.FirstName,
                            email_id = li.EmailId,
                            mobile = li.Mobile,
                            gender = li.Gender,
                            is_active = li.IsActive,
                            team_count = 0,
                            unit_count = 0,
                        }).ToList();
            return Json(data);
        }
        public ActionResult getPassword(string id)
        {
            int user_id = Convert.ToInt32(id);
            int company_id = SessionUtil.GetCompanyID();
           // string password = db.USP_GetUserPassword(user_id, company_id).ToList().FirstOrDefault().password;
            return Json("");
        }
        #endregion

        #region COUNTRY
        public ActionResult IsCountryNameExist(string id)
        {
            //id="country_name,country_id"
            string[] info = id.Split(',');
            string countryName = id;
            int countryId = 0;
            if (info.Length == 2)
            {
                countryName = info[0];
                countryId = info[1] != "" ? Convert.ToInt32(info[1]) : countryId;
            }
            return Json(countryUtil.GetCountryByCountryId(countryId, countryName) == null ? false : true);
        }

        #endregion

        #region STATE
        public ActionResult IsStateExist(string id)
        {
            //id="country_name,country_id"
            string[] info = id.Split(',');
            string stateName = id;
            int countryId = 0;
            int stateId = 0;
            if (info.Length == 3)
            {
                stateName = info[0];
                stateId = info[1] != "" ? Convert.ToInt32(info[1]) : stateId;
                countryId = info[2] != "" ? Convert.ToInt32(info[2]) : countryId;
            }
            return Json(stateUtil.GetStateByName(stateId, stateName, countryId) == null ? false : true);
        }
        public ActionResult IsCityExist(string id)
        {
            //id="country_name,country_id"
            string[] info = id.Split(',');
            string cityName = id;
            int countryId = 0;
            int stateId = 0;
            int cityId = 0;
            if (info.Length == 4)
            {
                cityName = info[0];
                stateId = info[1] != "" ? Convert.ToInt32(info[1]) : stateId;
                countryId = info[2] != "" ? Convert.ToInt32(info[2]) : countryId;
                cityId = info[3] != "" ? Convert.ToInt32(info[3]) : cityId;
            }
            return Json(new CityUtil().GetCityByName(stateId, cityName, countryId, cityId) == null ? false : true);
        }
        public ActionResult GetStateByCountry(int id)
        {
            var list = (from c in db.STATEs.AsEnumerable()
                        where c.CountryId == id
                        select new SelectListItem
                        {
                            Text = c.StateName,
                            Value = c.StateId.ToString(),
                        }).ToList();
            return Json(list);
        }

        //public ActionResult GetSubCatByCat(int id)
        //{
        //    var list = new TicketUtility().GetTicketSubCategoryList(id);
        //    return Json(list);
        //}
        #endregion
        public ActionResult GetCityByState(int id)
        {
            var list = (from c in db.CITies.AsEnumerable()
                        where c.StateId == id
                        select new SelectListItem
                        {
                            Text = c.CityName,
                            Value = c.CityId.ToString(),
                        }).ToList();
            return Json(list);
        }

      
        #region MENU CATEGORY
        public ActionResult GetMenuCategory(string id)
        {

            string[] treeobj = { "parent", "Text" };
            var snag_type_sla = db.MENU_CATEGORY.AsEnumerable().Where(x => x.CompanyId == SessionUtil.GetCompanyID()).ToList();
            var list = ( 
                        from sla in snag_type_sla
                        where sla.CompanyId == SessionUtil.GetCompanyID()
                        select new
                        {
                            id = sla.MenuCategoryId,
                            parent = (sla.PerentMenuCategoryId != null ? Convert.ToString(sla.PerentMenuCategoryId) : "#"),
                            text = sla != null ? sla.MenuCategoryName+"-"+sla.SequenceOrder:"",
                            selected =   false
                        }).Distinct().ToList();
            List<object> Arraylist = treeobj.ToList<object>();
            return Json(list);
        }

        public ActionResult MenuCategoryMoveNode(int menuCategoryId, string text, int parentId, int old_parent)
        {
            try
            {
                MenuCategoryDelete(menuCategoryId);
                MENU_CATEGORY MC = new MENU_CATEGORY();
                MC.MenuCategoryName = text;
                MC.PerentMenuCategoryId = parentId;
                MC.IsActive = true;
                db.MENU_CATEGORY.Add(MC);
                db.SaveChanges();
                Result.Message = MessageType.Success.ToString();
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(Result.Message);
        }

        public ActionResult MenuCategoryDelete(int id)
        {
            try
            {
                MENU_CATEGORY MC = new MENU_CATEGORY();
                MC = db.MENU_CATEGORY.Find(id);
                db.MENU_CATEGORY.Remove(MC);
                db.SaveChanges();
                Result.Message = MessageType.Success.ToString();
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(Result.Message);
        }
        public ActionResult CategoryCreateNewParent()
        {
            try
            {
                MENU_CATEGORY MC = new MENU_CATEGORY();
                MC.MenuCategoryName = "NEWLY ADDED";
                MC.IsActive = true;
                db.MENU_CATEGORY.Add(MC);
                db.SaveChanges();
                Result.Message = MessageType.Success.ToString();
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(Result.Message);
        }
        public ActionResult MenuCategoryNodeRename(int id, string text)
        {
            try
            {
                MENU_CATEGORY MC = new MENU_CATEGORY();
                MC = db.MENU_CATEGORY.Find(id);
                MC.MenuCategoryName = text;
                db.Entry(MC).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                Result.Message = MessageType.Success.ToString();
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(Result.Message);
        }
        public ActionResult MenuCategoryCreateNew(int parent_Id, string text)
        {
            try
            {
                MENU_CATEGORY MC = new MENU_CATEGORY();
                MC.MenuCategoryName = text;
                MC.PerentMenuCategoryId = parent_Id;
                MC.IsActive = true;
                db.MENU_CATEGORY.Add(MC);
                db.SaveChanges();
                Result.Message = MessageType.Success.ToString();
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(Result.Message);
        }
        #endregion


        #region Company
        public ActionResult IsCompanyNameExist(string id)
        {
            //id="country_name,country_id"
            string[] info = id.Split(',');
            string companyName = id;
            int companyId = 0;
            if (info.Length == 2)
            {
                companyName = info[0];
                companyId = info[1] != "" ? Convert.ToInt32(info[1]) : companyId;
            }
            return Json(companyUtil.GetCompanyByCountryId(companyId, companyName) == null ? false : true);
        }
        #endregion

        #region User
        public ActionResult IsLoginIdExist(string id)
        {
            string[] info = id.Split(',');
            string loginId = id;
            int userId = 0;
            if (info.Length == 2)
            {
                loginId = info[0];
                userId = info[1] != "" ? Convert.ToInt32(info[1]) : userId;
            }
            return Json(companyUtil.GetLoginByUserId(userId, loginId) == null ? false : true);
        }
        public ActionResult IsUserEmailIdExist(string id)
        {
            string[] info = id.Split(',');
            string emailId = id;
            int userId = 0;
            if (info.Length == 2)
            {
                emailId = info[0];
                userId = info[1] != "" ? Convert.ToInt32(info[1]) : userId;
            }
            return Json(companyUtil.GetEmailByUserId(userId, emailId) == null ? false : true);
        }

        #endregion

    }
}