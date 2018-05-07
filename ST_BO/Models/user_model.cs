using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace ST_BO.Models
{
    #region Classes & Enum

    #endregion

    #region Business

    public class UserUtil
    {

        private Result result;
        private BaseEntities db;
        public UserUtil()
        {
            this.result = new Result();
            this.db = new BaseEntities();
        }

        public String userRegistrion(String fullName, String mobileNumber, String emailaddress)
        {
            String result = "";

            try
            {

                //var userReg = db.USP_Process_Registration(fullName, mobileNumber, emailaddress, 1).FirstOrDefault();
                //if (userReg != null)
                //{
                //    string AWSProfileName = BaseUtil.GetWebConfigValue("AWSProfileName");
                //    string SNAG_AWS_S3 = BaseUtil.GetWebConfigValue("SNAG_AWS_S3");
                //    string EMAIL_TEMPALTE_FOLDER = BaseUtil.GetWebConfigValue("EMAIL_TEMPALTE_FOLDER");
                //    AWSUtil.FolderCreation(AWSProfileName, userReg.company_folder_name);
                //    var emailTempRecord = db.email_template.AsEnumerable().Where(x => x.email_template_type_id == 1).OrderByDescending(x => x.email_template_id).FirstOrDefault();
                //    if (emailTempRecord != null)
                //    {

                //        String templateName = SNAG_AWS_S3 + EMAIL_TEMPALTE_FOLDER + "/" + emailTempRecord.email_template_file_name;
                //        var filePath = (new WebClient()).DownloadString(templateName);
                //        filePath = filePath.Replace("{UserName}", userReg.user_name);
                //        filePath = filePath.Replace("{ACTIVATE_ACCOUNT_LINK}", userReg.activation_link);

                //        BaseUtil.SendMail(userReg.email_id, "Verify your Agency CRM email address ", filePath, null);


                //    }
                //}


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        public USER IsEmailAddressExist(string emailID, int RoleBit)
        {

            return db.USERs.AsEnumerable().Where(x => x.EmailId.ToUpper() == emailID.ToUpper() && x.RoleBit  == RoleBit).FirstOrDefault();

        }

        public USER IsLoginIDExist(string LoginID)
        {

            return db.USERs.AsEnumerable().Where(x => x.LoginId.ToUpper() == LoginID.ToUpper()).FirstOrDefault();

        }

        public List<SelectListItem> GetRole(int id)
        {
            var list = (from s in db.ROLEs.Where(x => x.IsActive == true)
                        select new SelectListItem
                        {
                            Text = s.RoleName,
                            Value = s.RoleId.ToString(),
                        }).ToList();
            return list;
        }

    

        #region USER
        public USER IsEmailExist(string emailID, int userId)
        {

            var list = db.USERs.AsEnumerable().Where(x => x.CompanyId == SessionUtil.GetCompanyID()).ToList();
            return userId > 0 ?
                 list.AsEnumerable().Where(x => x.EmailId.ToUpper() == emailID.ToUpper() && x.UserId != userId && x.CompanyId == SessionUtil.GetCompanyID()).FirstOrDefault()
                : list.AsEnumerable().Where(x => x.EmailId.ToUpper() == emailID.ToUpper() && x.CompanyId == SessionUtil.GetCompanyID()).FirstOrDefault();
        }
        public USER IsRegEmailExist(string emailID, int roleBit)
        {
            var list = db.USERs.AsEnumerable().Where(x => x.IsActive == true && x.EmailId.ToUpper() == emailID.ToUpper() && x.RoleBit == roleBit).FirstOrDefault();
            return list;
        }
        public IList<SelectListItem> ParentUserList(int roleBit)
        {
            return (from c in db.USERs.AsEnumerable()
                    where c.CompanyId == SessionUtil.GetCompanyID() && c.RoleBit == roleBit
                    select new SelectListItem
                    {
                        Value = c.UserId.ToString(),
                        Text = c.FirstName,

                    }).OrderBy(x => x.Text).ToList();
        }

      

        public Result PostCreateEdit(USER user, FormCollection frm)
        {
            try
            {
                string password = frm["userpassword"];
                db = new BaseEntities();
                //if (user.user_id > 0)
                //{
                //    var msg = db.USP_CreateUser(user.user_id, user.user_name, user.login_id, user.email_id, user.mobile, password, user.gender, user.user_photo, user.parent_user_id, user.role_bit, SessionUtil.GetCompanyID(), SessionUtil.GetUserID(), user.is_account_access, user.is_account_add_payment, user.is_property_edit, user.is_active).FirstOrDefault().MSG;
                //    result.Message = msg == "Success" ? string.Format(BaseConst.MSG_SUCCESS_CREATE, "User") : msg;
                //}
                //else
                //{
                //    user.is_active = true;
                //    var msg = db.USP_CreateUser(user.user_id, user.user_name, user.login_id, user.email_id, user.mobile, password, user.gender, user.user_photo, user.parent_user_id, user.role_bit, SessionUtil.GetCompanyID(), SessionUtil.GetUserID(), user.is_account_access, user.is_account_add_payment, user.is_property_edit, user.is_active).FirstOrDefault().MSG;
                //    result.Message = msg == "Success" ? string.Format(BaseConst.MSG_SUCCESS_UPDATE, "User") : msg;
                //}
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }
        public Result PostProfileEdit(USER user)
        {
            try
            {
                db = new BaseEntities();
                USER userdata = db.USERs.Find(user.UserId);
                if (user.UserId > 0)
                {
                    userdata.FirstName = user.FirstName;
                    userdata.Mobile = user.Mobile;
                    userdata.Gender = user.Gender;
                    userdata.UserPhoto = user.UserPhoto;
                    db.Entry(userdata).State = System.Data.Entity.EntityState.Modified;
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "User Profile");
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }
        public Result PostChangePassword(int userid, string oldpasword, string newpassword, string conformpassword)
        {
            try
            {
                db = new BaseEntities();
                USER userdata = db.USERs.Find(userid);
                if (userdata.UserId > 0)
                {
                    var msg = "";
                  //  var msg = db.USP_ChangePassword(userid, oldpasword, newpassword, conformpassword).FirstOrDefault().MSG;
                    result.Message = msg;
                    result.MessageType = msg == "Success" ? MessageType.Success : MessageType.Warning;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }

            return result;
        }
        #endregion
    }
    #endregion
}