using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using ST_BO.Models;
using ST_BO.Controllers;

namespace ST_BO.Controllers
{
    public class MenuController : BaseController
    {
        private Result result;
        private MenuUtil menuUtil;
        public MenuController()
        {
            result = new Result();
            menuUtil = new MenuUtil();
        }
        // GET: /Menu/
        public ActionResult Index()
        {
            ViewBag.Title = "Menu List";

            return View();

        }
        public ActionResult ListMenu()
        {
            result = menuUtil.ListMenu("");
            return new JsonResult()
            {
                Data = result.Object,
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }
        public ActionResult CreateEdit(Int32 Id = 0)
        {
            MENU menu = db.MENUs.Find(Id);

            ViewBag.Title = menu == null ? "Menu Create" : "Menu Edit";

            ViewBag.ControllerName = new SelectList(menuUtil.GetController(), "Value", "Text", menu != null ? menu.ControllerName : "");
            ViewBag.MenuParentId = new SelectList(menuUtil.GetMenu(true), "Value", "Text", menu != null ? menu.MenuParentId : 0);
            ViewBag.ActionName = new SelectList(BaseUtil.GetListAllActionByController(menu != null ? menu.ControllerName : ""), "Value", "Text", menu != null ? menu.ActionName : "");


            ViewBag.SelectedMenuAction = menu != null ? menu.ActionName : "";

            return View(menu);
        }
        [HttpPost]
        public ActionResult CreateEdit(MENU menu)
        {
            result = menuUtil.PostMenuCreate(menu);
            ViewBag.Title = menu == null ? "Menu Create" : "Menu Edit";

            ViewBag.controller_name = menuUtil.GetController();
            ViewBag.menu_parent_id = menuUtil.GetMenu(true);
            ViewBag.menu_ddl_id = menuUtil.GetMenu(true);
            ViewBag.action_name = BaseUtil.GetListAllActionByController("");
            switch (result.MessageType)
            {
                case MessageType.Success:
                    return RedirectToAction("Index", "Menu", new { result = result.Message, MessageType = result.MessageType });

                default:
                    return RedirectToAction("CreateEdit", "Menu", new { result = result.Message, MessageType = result.MessageType });

            }

            return View(menu);
        }
        public ActionResult GetActionNameByController(string id)
        {
            string controllerName = string.Format("{0}Controller", id);
            var list = BaseUtil.GetListAllActionByController(controllerName);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
	}
}