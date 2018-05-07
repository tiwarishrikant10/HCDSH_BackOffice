using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using System.Net;
using System.IO;
namespace ST_BO.Models
{

    #region Classes & Enum
    public class CompanyUser
    {
        //public company company { get; set; }
        //public user user { get; set; }
        public int company_id { get; set; }
        public string admin_user_name { get; set; }
        public int language_id { get; set; }
        public string time_zone { get; set; }
        public string company_folder_name { get; set; }
        public string business_name { get; set; }
        public string country_code { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public int state_id { get; set; }
        public string company_logo { get; set; }
        public int parent_company_id { get; set; }
        public DateTime created_date { get; set; }
        public DateTime updated_date { get; set; }
        public int updated_by { get; set; }
        public bool is_active { get; set; }

        public int country_id { get; set; }


        public int user_id { get; set; }
        public string user_name { get; set; }
        public string login_id { get; set; }
        public string email_id { get; set; }
        public string mobile { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public int report_to { get; set; }
        public string user_photo { get; set; }
        public DateTime last_login_date { get; set; }
        public string activation_link { get; set; }
        public string reset_password_link { get; set; }
        public DateTime reset_password_link_expire_date_time { get; set; }
        public string activation_reset_key { get; set; }
        public int parent_user_id { get; set; }
        public int role_bit { get; set; }
        public bool is_account_locked { get; set; }
        public int password_failed_attempt { get; set; }
        public string imei_number { get; set; }
        public string token_number { get; set; }
        public string os_version { get; set; }
    }

    #endregion

    #region Business
    public class CompanyUtil
    {
        private Result result;
        private BaseEntities db;
        public CompanyUtil()
        {
            this.result = new Result();
            this.db = new BaseEntities();
        }
        public List<SelectListItem> GetRole()
        {
            var list = (from s in db.ROLEs.AsEnumerable().Where(x => x.IsPublic == true)
                        select new SelectListItem
                        {
                            Text = s.RoleName,
                            Value = s.RoleId.ToString(),
                        }).ToList();
            return list;
        }
        public IList<COMPANY> CompanyList()
        {
            int userID = 0, companyID = 0;
            IList<COMPANY> companyList = new List<COMPANY>();
            try
            {
                COMPANY companyCountRecord = new COMPANY();

                foreach (var item in db.COMPANies.Where(x => x.IsActive))
                {

                    companyCountRecord = new COMPANY();
                    companyCountRecord.IsActive = item.IsActive;
                    companyCountRecord.CompanyId = item.CompanyId;
                    companyList.Add(companyCountRecord);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return companyList;
        }

        public COMPANY GetCompanyByCountryId(int companyId, string companyName)
        {
            return companyId > 0 ?
                db.COMPANies.AsEnumerable().Where(x => x.CompanyName.ToUpper() == companyName.ToUpper() && x.CompanyId != companyId).FirstOrDefault()
                : db.COMPANies.AsEnumerable().Where(x => x.CompanyName.ToUpper() == companyName.ToUpper()).FirstOrDefault();
        }

        public USER GetLoginByUserId(int userId, string loginId)
        {
            return userId > 0 ?
                db.USERs.AsEnumerable().Where(x => x.LoginId.ToUpper() == loginId.ToUpper() && x.UserId != userId).FirstOrDefault()
                : db.USERs.AsEnumerable().Where(x => x.LoginId.ToUpper() == loginId.ToUpper()).FirstOrDefault();
        }
        public USER GetEmailByUserId(int userId, string emailId)
        {
            return userId > 0 ?
                db.USERs.AsEnumerable().Where(x => x.EmailId.ToUpper() == emailId.ToUpper() && x.CompanyId != userId).FirstOrDefault()
                : db.USERs.AsEnumerable().Where(x => x.EmailId.ToUpper() == emailId.ToUpper()).FirstOrDefault();
        }
        public CompanyUser GetCompanyData(string id)
        {
            int companyID = Convert.ToInt32(id);
            CompanyUser companyUserData = new CompanyUser();
            COMPANY company = db.COMPANies.Find(companyID);
            companyUserData.company_id = company.CompanyId;
            companyUserData.is_active = company.IsActive;

            USER user = db.USERs.Where(x => x.CompanyId == companyID).FirstOrDefault();
            companyUserData.user_name = user.FirstName;
            companyUserData.login_id = user.LoginId;
            companyUserData.password = Convert.ToString(user.Password);
            companyUserData.mobile = user.Mobile;
            companyUserData.email_id = user.EmailId;
            return companyUserData;
        }
        public Result PostCompanyCreateEdit(COMPANY company, string userName, string loginID, string emailID, string mobile, string gender, string password, HttpPostedFileBase company_logo)
        {
            try
            {
                int userID = SessionUtil.GetUserID();
                if (company.CompanyId > 0)
                {
                    var tempCompanyId = SessionUtil.GetCompanyID();

                    COMPANY data = new COMPANY();
                    data = db.COMPANies.Find(company.CompanyId);
                    data.CompanyName = company.CompanyName;
                    data.ContactNumber = company.ContactNumber;
                    data.CityId = company.CityId;
                    data.GSTN = company.GSTN;
                    data.CTSNumber = company.CTSNumber;
                    data.CompanyAddress = company.CompanyAddress;
                    data.ContactPerson = company.ContactPerson;
                    data.EmailId = company.EmailId;
                    data.PAN = company.PAN;
                    data.LogoImage = company_logo != null ? company.LogoImage : data.LogoImage;
                    data.IsActive = company.IsActive;

                    int sessionComId = (Int32)SessionUtil.GetCompanyID();

                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;

                    BaseUtil.SetSessionValue(UserInfo.CompanyID.ToString(), company.CompanyId.ToString());

                    db.SaveChanges();

                    BaseUtil.SetSessionValue(UserInfo.CompanyID.ToString(), sessionComId.ToString());
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "Company");
                    result.Id = company.CompanyId;
                }
                else
                {
                    db.COMPANies.Add(company);
                    db.SaveChanges();
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "Company");
                    result.MessageType = MessageType.Success;
                    result.Id = company.CompanyId;
                }
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }
        public Result PostUserEdit(USER user, string pass)
        {
            try
            {
                if (user.UserId > 0)
                {
                    var data = db.USP_CreateUser(user.UserId, user.FirstName, user.LastName, user.LoginId, user.EmailId, user.Mobile, pass, user.Address, user.CityId,
                        user.Gender, user.UserPhoto, 2, user.CompanyId, user.IsActive);
                }
                else
                {
                    var data = db.USP_CreateUser(user.UserId, user.FirstName, user.LastName, user.LoginId, user.EmailId, user.Mobile, pass, user.Address, user.CityId,
                    user.Gender, user.UserPhoto, 2, user.CompanyId, true);
                }
                result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "User");
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