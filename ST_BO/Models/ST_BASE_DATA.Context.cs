﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ST_BO.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ST_BASE_DATAEntities : DbContext
    {
        public ST_BASE_DATAEntities()
            : base("name=ST_BASE_DATAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACADEMIC_LEVEL> ACADEMIC_LEVEL { get; set; }
        public virtual DbSet<APP_ERROR_LOG> APP_ERROR_LOG { get; set; }
        public virtual DbSet<APPLICATION_CONFIGURATION> APPLICATION_CONFIGURATION { get; set; }
        public virtual DbSet<AREA> AREAs { get; set; }
        public virtual DbSet<BANNER> BANNERs { get; set; }
        public virtual DbSet<CITY> CITies { get; set; }
        public virtual DbSet<COMPANY> COMPANies { get; set; }
        public virtual DbSet<CONTENT> CONTENTs { get; set; }
        public virtual DbSet<CONTENT_ATTACHMENT> CONTENT_ATTACHMENT { get; set; }
        public virtual DbSet<COUNTRY> COUNTRies { get; set; }
        public virtual DbSet<CUSTOMER_SETTING> CUSTOMER_SETTING { get; set; }
        public virtual DbSet<EVENT> EVENTs { get; set; }
        public virtual DbSet<EVENT_TYPE> EVENT_TYPE { get; set; }
        public virtual DbSet<GALLARY_TYPE> GALLARY_TYPE { get; set; }
        public virtual DbSet<MENU> MENUs { get; set; }
        public virtual DbSet<MENU_CATEGORY> MENU_CATEGORY { get; set; }
        public virtual DbSet<NEWS_TYPE> NEWS_TYPE { get; set; }
        public virtual DbSet<PIN_CODE> PIN_CODE { get; set; }
        public virtual DbSet<RECRUITMENT> RECRUITMENTs { get; set; }
        public virtual DbSet<ROLE> ROLEs { get; set; }
        public virtual DbSet<ROLE_ACTION> ROLE_ACTION { get; set; }
        public virtual DbSet<ROLE_MENU> ROLE_MENU { get; set; }
        public virtual DbSet<SMS_TRACKING> SMS_TRACKING { get; set; }
        public virtual DbSet<STATE> STATEs { get; set; }
        public virtual DbSet<TENURE> TENUREs { get; set; }
        public virtual DbSet<USER> USERs { get; set; }
        public virtual DbSet<WIDGET> WIDGETs { get; set; }
        public virtual DbSet<WIDGET_BUTTON> WIDGET_BUTTON { get; set; }
    
        public virtual ObjectResult<USP_CreateUser_Result> USP_CreateUser(Nullable<int> uSER_ID, string user_name, string last_name, string login_id, string email_id, string mobile, string password, string address, Nullable<int> cityId, string gender, string user_photo, Nullable<int> role_bit, Nullable<int> company_id, Nullable<bool> is_active)
        {
            var uSER_IDParameter = uSER_ID.HasValue ?
                new ObjectParameter("USER_ID", uSER_ID) :
                new ObjectParameter("USER_ID", typeof(int));
    
            var user_nameParameter = user_name != null ?
                new ObjectParameter("user_name", user_name) :
                new ObjectParameter("user_name", typeof(string));
    
            var last_nameParameter = last_name != null ?
                new ObjectParameter("last_name", last_name) :
                new ObjectParameter("last_name", typeof(string));
    
            var login_idParameter = login_id != null ?
                new ObjectParameter("login_id", login_id) :
                new ObjectParameter("login_id", typeof(string));
    
            var email_idParameter = email_id != null ?
                new ObjectParameter("email_id", email_id) :
                new ObjectParameter("email_id", typeof(string));
    
            var mobileParameter = mobile != null ?
                new ObjectParameter("mobile", mobile) :
                new ObjectParameter("mobile", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var addressParameter = address != null ?
                new ObjectParameter("Address", address) :
                new ObjectParameter("Address", typeof(string));
    
            var cityIdParameter = cityId.HasValue ?
                new ObjectParameter("cityId", cityId) :
                new ObjectParameter("cityId", typeof(int));
    
            var genderParameter = gender != null ?
                new ObjectParameter("gender", gender) :
                new ObjectParameter("gender", typeof(string));
    
            var user_photoParameter = user_photo != null ?
                new ObjectParameter("user_photo", user_photo) :
                new ObjectParameter("user_photo", typeof(string));
    
            var role_bitParameter = role_bit.HasValue ?
                new ObjectParameter("role_bit", role_bit) :
                new ObjectParameter("role_bit", typeof(int));
    
            var company_idParameter = company_id.HasValue ?
                new ObjectParameter("company_id", company_id) :
                new ObjectParameter("company_id", typeof(int));
    
            var is_activeParameter = is_active.HasValue ?
                new ObjectParameter("is_active", is_active) :
                new ObjectParameter("is_active", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_CreateUser_Result>("USP_CreateUser", uSER_IDParameter, user_nameParameter, last_nameParameter, login_idParameter, email_idParameter, mobileParameter, passwordParameter, addressParameter, cityIdParameter, genderParameter, user_photoParameter, role_bitParameter, company_idParameter, is_activeParameter);
        }
    
        public virtual ObjectResult<string> USP_Decrypt_TEXT(byte[] cipher)
        {
            var cipherParameter = cipher != null ?
                new ObjectParameter("Cipher", cipher) :
                new ObjectParameter("Cipher", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("USP_Decrypt_TEXT", cipherParameter);
        }
    
        public virtual ObjectResult<byte[]> USP_Encrypt_TEXT(string plaintext)
        {
            var plaintextParameter = plaintext != null ?
                new ObjectParameter("plaintext", plaintext) :
                new ObjectParameter("plaintext", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<byte[]>("USP_Encrypt_TEXT", plaintextParameter);
        }
    
        public virtual ObjectResult<USP_Login_Result> USP_Login(string loginID, string pWD, Nullable<bool> isWeb)
        {
            var loginIDParameter = loginID != null ?
                new ObjectParameter("LoginID", loginID) :
                new ObjectParameter("LoginID", typeof(string));
    
            var pWDParameter = pWD != null ?
                new ObjectParameter("PWD", pWD) :
                new ObjectParameter("PWD", typeof(string));
    
            var isWebParameter = isWeb.HasValue ?
                new ObjectParameter("IsWeb", isWeb) :
                new ObjectParameter("IsWeb", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_Login_Result>("USP_Login", loginIDParameter, pWDParameter, isWebParameter);
        }
    
        public virtual ObjectResult<GetMenu_Result> GetMenu(Nullable<int> roleId, Nullable<int> companyid)
        {
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            var companyidParameter = companyid.HasValue ?
                new ObjectParameter("companyid", companyid) :
                new ObjectParameter("companyid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetMenu_Result>("GetMenu", roleIdParameter, companyidParameter);
        }
    }
}
