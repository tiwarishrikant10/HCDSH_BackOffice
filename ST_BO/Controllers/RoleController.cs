using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using System.Threading;
using ST_BO.Controllers;

namespace ST_BO.Controllers
{
    public class RoleController : BaseController
    {
        //
        // GET: /Role/
        private Result result;
        public ActionResult Index(string id = "")
        {
            ViewBag.Title = "Role List";
            ViewBag.SubTitle = "";
            return View();
        }
        public ActionResult CompanyRoleIndex(string id = "")
        {
            ViewBag.Title = "Role List";
            ViewBag.SubTitle = "";
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ListRoles(string id = "")
        {

            result = RoleUtil.GetRoleList(id);
            return new JsonResult()
            {
                Data = result.Object,
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public ActionResult CreateEditRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PostCreateEditRole(ROLE role)
        {
            try
            {
                result = new Result();
                if (role.RoleId > 0)
                {
                    ROLE find = db.ROLEs.Find(role.RoleId);
                 //   find.hierarchy_level = role.hierarchy_level;
                  //  find.parent_id = role.parent_id;
                    find.IsActive = role.IsActive;
                    find.IsPublic = role.IsPublic;
                    find.RoleName = role.RoleName;
                    db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "Role");
                }
                else
                {
                 ////   role.application_type_id = SessionUtil.GetApplicationTypeID();
                    long prevRoleBit = db.ROLEs.Where(x => x.CompanyId == role.CompanyId).Max(x => x.RoleBit);
                    role.RoleBit = prevRoleBit * 2;
                    role.CompanyId = role.CompanyId;
                    db.ROLEs.Add(role);
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "Role");
                }
                BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "0");
                db.SaveChanges();
                BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "1");
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }

        #region Role Action

        public ActionResult RoleAction(int id)
        {
            ViewBag.Title = "Role Action";
            int roleID = id;
            IList<ControllerAction> list = new List<ControllerAction>();
            var listController = BaseUtil.GetListController();
            ViewBag.action_name = new List<SelectListItem>();
            ViewBag.role_id = id;
            ViewBag.RoleName = db.ROLEs.Where(r => r.RoleId == id).FirstOrDefault().RoleName;

            foreach (var c in listController)
            {
                string controllerName = c.Value;
                var listAllAction = BaseUtil.GetListAllActionByController(controllerName);
                var listAssignedAction = BaseUtil.GetListActionAssignedByRoleAndController(roleID, controllerName);
                foreach (var aa in listAllAction)
                {
                    ControllerAction ca = new ControllerAction();
                    ca.ControllerName = controllerName;
                    ca.ActionName = aa.Text;
                    ca.IsAssigned = listAssignedAction.Where(a => a.Text == aa.Text).Count() > 0 ? true : false;
                    list.Add(ca);
                }
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult RoleAction(string id, int roleID)
        {

            string[] str = id.Split(','); //Chk_CityController_Create_False,
            for (int i = 0; i < str.Length; i++)
            {
                string[] info = str[i].Split('_');
                string controllerName = info[1];
                string actionName = info[2];
                bool isAssigned = Convert.ToBoolean(info[3]);
                ROLE_ACTION roleAction = db.ROLE_ACTION.Where(ra => ra.ControllerName == controllerName && ra.ActionName == actionName && ra.RoleId == roleID).SingleOrDefault();
                if (roleAction == null) // new
                {
                    roleAction = new ROLE_ACTION();
                    roleAction.RoleId = roleID;
                    roleAction.ControllerName = controllerName;
                    roleAction.ActionName = actionName;
                    db.ROLE_ACTION.Add(roleAction);
                }
                else
                {
                    db.ROLE_ACTION.Remove(roleAction);
                }
                db.SaveChanges();

            }
            return RedirectToAction("Index");


        }

        // POST: /Role/Delete/5
        public ActionResult RoleActionDelete(int id)
        {
            ROLE_ACTION roleAction = db.ROLE_ACTION.Find(id);
            db.ROLE_ACTION.Remove(roleAction);
            db.SaveChanges();
            return RedirectToAction("RoleAction", new { id = roleAction.RoleId });
        }
        #endregion

        #region Company RoleAction
        public ActionResult CompanyRoleAction(int id, string companyId = "")
        {
            ViewBag.Title = "Role Action";
            int roleID = id;
            int company_id = Convert.ToInt32(companyId);
            IList<ControllerAction> list = new List<ControllerAction>();
            var listController = BaseUtil.GetListController(); //Nedd to check data in respwct of
            ViewBag.action_name = new List<SelectListItem>();
            ViewBag.role_id = id;
            ViewBag.RoleName = db.ROLEs.Where(r => r.RoleId == id && r.CompanyId== company_id).FirstOrDefault().RoleName;

            foreach (var c in listController)
            {
                string controllerName = c.Value;
                var listAllAction = BaseUtil.GetListAllActionByController(controllerName);
                var listAssignedAction = BaseUtil.GetListActionAssignedByRoleAndController(roleID, controllerName);
                foreach (var aa in listAllAction)
                {
                    ControllerAction ca = new ControllerAction();
                    ca.ControllerName = controllerName;
                    ca.ActionName = aa.Text;
                    ca.IsAssigned = listAssignedAction.Where(a => a.Text == aa.Text).Count() > 0 ? true : false;
                    list.Add(ca);
                }
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult PostCompanyRoleAction(string id, int roleID)
        {

            string[] str = id.Split(','); //Chk_CityController_Create_False,
            for (int i = 0; i < str.Length; i++)
            {
                string[] info = str[i].Split('_');
                string controllerName = info[1];
                string actionName = info[2];
                bool isAssigned = Convert.ToBoolean(info[3]);
                ROLE_ACTION roleAction = db.ROLE_ACTION.Where(ra => ra.ControllerName == controllerName && ra.ActionName == actionName && ra.RoleId == roleID).SingleOrDefault();
                if (roleAction == null) // new
                {
                    roleAction = new ROLE_ACTION();
                    roleAction.RoleId = roleID;
                    roleAction.ControllerName = controllerName;
                    roleAction.ActionName = actionName;
                    db.ROLE_ACTION.Add(roleAction);
                }
                else
                {
                    db.ROLE_ACTION.Remove(roleAction);
                }
                db.SaveChanges();

            }
            return RedirectToAction("Index");


        }
        #endregion

        #region Role Menu
        public ActionResult RoleMenu(int id)
        {
            ViewBag.Title = "Menu Tree List";
            ViewBag.role_id = id;
            return View();
        }

        public ActionResult SaveRoleMenu(string id, int roleID)
        {

            string[] menuIds = id.Split(',');
            var listToRemove = db.ROLE_MENU.AsEnumerable().Where(r => r.RoleId == roleID);

            db.ROLE_MENU.RemoveRange(listToRemove);
            db.SaveChanges();

            for (int i = 0; i < menuIds.Length; i++)
            {
                int menuId = Convert.ToInt32(menuIds[i]);
                ROLE_MENU rm = new ROLE_MENU();
                rm.RoleId = roleID;
                rm.MenuId = menuId;
                rm.IsActive = true;

                db.ROLE_MENU.Add(rm);
                db.SaveChanges();

            }
            ViewBag.role_id = id;
            return Json(id);
            //return View();
        }
        #endregion

        #region Company RoleMenu
        public ActionResult CompanyRoleMenu(int roleID, string companyId = "")
        {
            ViewBag.Title = "Menu Tree List";
            ViewBag.role_id = roleID;
            ViewBag.CompanyId = companyId;
            return View();
        }
        public ActionResult SaveCompanyRoleMenu(string id, string ids = "")
        {

            string[] menuIds = id.Split(',');
            string[] info = ids.Split(',');
            int roleID = Convert.ToInt32(info[0]);
            int companyID = Convert.ToInt32(info[1]);

            var listToRemove = db.ROLE_MENU.AsEnumerable().Where(r => r.RoleId == roleID && r.CompanyId == companyID);

            db.ROLE_MENU.RemoveRange(listToRemove);
            db.SaveChanges();

            for (int i = 0; i < menuIds.Length; i++)
            {
                int menuId = Convert.ToInt32(menuIds[i]);
                ROLE_MENU rm = new ROLE_MENU();
                rm.RoleId = roleID;
                rm.MenuId = menuId;
                rm.IsActive = true;
                rm.CompanyId = companyID;
                db.ROLE_MENU.Add(rm);
                BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "0");
                db.SaveChanges();
                BaseUtil.SetSessionValue(UserInfo.IsCompanyAddUpdate.ToString(), "1");
            }
            ViewBag.role_id = id;
            return Json(id);
            //return View();
        }
        public ActionResult GetCompanyMenuByRoleId(string ids = "")
        {
            string[] info = ids.Split(',');
            int roleId = Convert.ToInt32(info[0]);
            int companyID = Convert.ToInt32(info[1]);
            var list = (from me in db.MENUs.AsEnumerable()
                        join rm in db.ROLE_MENU.AsEnumerable() on me.MenuId equals rm.MenuId into rmTemp
                        from rmLj in rmTemp.Where(f => f.RoleId == roleId && f.CompanyId == companyID).DefaultIfEmpty()
                        orderby me.SequenceOrder
                        select new
                        {
                            id = me.MenuId,
                            parent = (me.MenuParentId != null ? Convert.ToString(me.MenuParentId) : "#"),
                            text = me.MenuName,
                            selected = rmLj == null ? false : true,
                        }).ToList();
            return Json(list);
        }
        #endregion

        #region Tree
        public ActionResult GetTree(string id)
        {
            string[] treeobj = { "id", "parent", "Text" };
            var list = (from me in db.MENUs.AsEnumerable()
                        join rm in db.ROLE_MENU.AsEnumerable() on me.MenuId equals rm.MenuId into rmTemp
                        from rmLj in rmTemp.DefaultIfEmpty()
                        select new
                        {
                            id = me.MenuId,
                            parent = (me.MenuParentId != null ? Convert.ToString(me.MenuParentId) : "#"),
                            text = me.MenuName,
                            selected = rmLj == null ? false : true,
                        }).ToList();

            List<object> Arraylist = treeobj.ToList<object>();
            return Json(list);
        }
        public ActionResult GetAllMenuByRoleId(Int32 id)
        {
            int roleId = Convert.ToInt32(id);
            var list = (from me in db.MENUs.AsEnumerable()
                        join rm in db.ROLE_MENU.AsEnumerable() on me.MenuId equals rm.MenuId into rmTemp
                        from rmLj in rmTemp.Where(f => f.RoleId == roleId && f.CompanyId == SessionUtil.GetCompanyID()).DefaultIfEmpty()
                        orderby me.SequenceOrder
                        select new
                        {
                            id = me.MenuId,
                            parent = (me.MenuParentId != null ? Convert.ToString(me.MenuParentId) : "#"),
                            text = me.MenuName,
                            selected = rmLj == null ? false : true,
                        }).ToList();
            return Json(list);
        }


        //public ActionResult GetMenuOfRoleId(string id)
        //{
        //    return Json(RoleUtil.GetMenusOfRoleId(Convert.ToInt32(id)));
        //}
        #endregion

        #region Assign Controller
        public ActionResult AssignView(string companyId = "")
        {
            ViewBag.Title = "Assign View";
            int company_id = Convert.ToInt32(companyId);
            ViewBag.CompanyId = company_id;
            IList<AssignController> list = new List<AssignController>();
            var listController = BaseUtil.GetListController(); //Nedd to check data in respwct of
            var listRoles = db.ROLEs.AsEnumerable().Where(x => x.CompanyId == company_id).ToList();
            foreach (var rr in listRoles)
            {
                foreach (var c in listController)
                {
                    string controllerName = c.Value;

                    if (controllerName.ToUpper() != "CompanyController".ToUpper()
                        && controllerName.ToUpper() != "EmailController".ToUpper()
                        && controllerName.ToUpper() != "JsTree3Controller".ToUpper()
                        && controllerName.ToUpper() != "ListingXmlController".ToUpper()
                        && controllerName.ToUpper() != "MastersController".ToUpper()
                        && controllerName.ToUpper() != "MenuController".ToUpper()
                        && controllerName.ToUpper() != "RoleController".ToUpper()
                        )
                    {
                        // var listAllAction = BaseUtil.GetListAllActionByController(controllerName);
                        var listAssignedAction = BaseUtil.GetListActionAssignedByRoleAndController(rr.RoleId, controllerName);
                        AssignController ac = new AssignController();
                        ac.ControllerName = controllerName;
                        ac.RoleName = rr.RoleName;
                        ac.RoleId = rr.RoleId;
                        ac.IsAssigned = db.ROLE_ACTION.AsEnumerable().Where(a => a.ControllerName == controllerName && a.RoleId == rr.RoleId).Count() > 0 ? true : false;
                        list.Add(ac);
                    }
                }
            }

            return View(list);
        }
        [HttpPost]
        public ActionResult PostAssignViewAction(string id, int companyID)
        {
            result = new Result();
            try
            {
                string[] str = id.Split(','); //Chk_CityController_Create_False,
                for (int i = 0; i < str.Length; i++)
                {
                    if (!String.IsNullOrEmpty(str[i]))
                    {
                        string[] info = str[i].Split('_');
                        if (!String.IsNullOrEmpty(info[1]))
                        {
                            string roleName = info[1];
                            string controllerName = info[2];
                            bool isAssigned = Convert.ToBoolean(info[3]);
                            var roleID = db.ROLEs.AsEnumerable().Where(x => x.RoleName == roleName && x.CompanyId == companyID).Select(x => x.RoleId).FirstOrDefault();
                            var listAllAction = BaseUtil.GetListAllActionByController(controllerName);
                            //var listController = BaseUtil.GetListController();
                            var roleaction = db.ROLE_ACTION.Where(ra => ra.ControllerName == controllerName && ra.RoleId == roleID).ToList();
                            if (roleaction.Count == 0) // new
                            {
                                foreach (var item in listAllAction)
                                {
                                    ROLE_ACTION roleAction = new ROLE_ACTION();
                                    roleAction.RoleId = roleID;
                                    roleAction.ControllerName = controllerName;
                                    roleAction.ActionName = item.Value;
                                    db.ROLE_ACTION.Add(roleAction);
                                }
                            }
                            else
                            {
                                db.ROLE_ACTION.RemoveRange(roleaction);

                            }
                            db.SaveChanges();
                            Thread.Sleep(4000);

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return RedirectToAction("Index", result);
        }
        #endregion
    }
}