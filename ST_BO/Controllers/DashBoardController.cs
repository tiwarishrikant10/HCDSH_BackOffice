using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using ST_BO.Controllers;
namespace ST_BO.Controllers
{
    public class DashBoardController : BaseController
    {
        private Result result;
        private DashboardUtil dashboardUtil;
        public DashBoardController()
        {
            result = new Result();
            dashboardUtil = new DashboardUtil();
        }
        public ActionResult Index()
        { 
            return View();
        }


    }
}