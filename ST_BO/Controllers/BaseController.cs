using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;  

namespace ST_BO.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        public BaseEntities db = null;
        public BaseModel BaseModel { get; set; }
        public BaseController()
        {
            db = new BaseEntities();
            this.BaseModel = new BaseModel();
            this.BaseModel.ControllerName = this.ToString().Split('.')[this.ToString().Split('.').Length - 1];
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filterContext.ActionDescriptor.ActionName.ToUpper().Equals("INDEX") &&

            String ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper();
            String ActionName = filterContext.ActionDescriptor.ActionName.ToUpper();

            if (BaseUtil.ListControllerExcluded().Contains(ControllerName))
            {
                if (ControllerName == "ACCOUNT" && ActionName == "LOGIN" && BaseUtil.IsAuthenticated())
                {

                    filterContext.Result = new RedirectResult("/DashBoard/Index/");
                }

                return;
            }
            else
            {
                if (BaseUtil.GetSessionValue(UserInfo.UserID.ToString()) == "")
                {
                    filterContext.Result = null;
                    filterContext.Result = new RedirectResult("/Account/Login/");
                    return;
                }
                if (!BaseUtil.CheckAuthentication(filterContext))
                {
                    filterContext.Result = null;
                    filterContext.Result = new RedirectResult("/Home/AccessDenied/");
                    return;
                }

                //if (BaseUtil.IsAuthenticated() && BaseUtil.GetSessionValue(UserInfo.IsCompanySetup.ToString()) == "0" && ActionName != "COMPANYINDEX" && ActionName != "COMPANYCREATEEDIT")
                if (BaseUtil.IsAuthenticated() && BaseUtil.GetSessionValue(UserInfo.IsCompanySetup.ToString()) == "0" && ActionName != "COMPANYACCOUNT")
                {
                    filterContext.Result = null;
                    filterContext.Result = new RedirectResult("/Settings/CompanyAccount/");
                    return;
                }
                return;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}

 