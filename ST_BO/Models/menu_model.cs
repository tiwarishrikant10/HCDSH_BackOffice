using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;

namespace ST_BO.Models
{
    #region Business
    #region Menu
    public class MenuUtil
    {
        private Result result;
        private BaseEntities db;
        public MenuUtil()
        {
            this.result = new Result();
            this.db = new BaseEntities();
        }
        public List<SelectListItem> GetMenu(bool isSearch)
        {
            if (isSearch)
            {
                return (from m in db.MENUs.AsEnumerable().Where(x => x.IsActive)
                        where m.MenuParentId == null //controller_name == null && m.action_name == null && m.param_id == null
                        orderby m.SequenceOrder
                        select new SelectListItem
                        {
                            Value = m.MenuId.ToString(),
                            Text = m.MenuName + " - " + m.SequenceOrder
                        }).ToList();
            }
            else
            {
                return (from m in db.MENUs.AsEnumerable().Where(x => x.IsActive)
                        orderby m.MenuName
                        select new SelectListItem
                        {
                            Value = m.MenuId.ToString(),
                            Text = m.MenuName
                        }).ToList();

            }

        }
        public List<SelectListItem> GetController()
        {
            var list = (from m in BaseUtil.GetControllerNames()

                        select new SelectListItem
                        {
                            Value = m.Replace("Controller", ""),
                            Text = m.Replace("Controller", "")
                        }).ToList();

            return list;
        }
        public Result ListMenu(string searchString)
        {
            try
            {
                var menuList = (from m in db.MENUs.AsEnumerable()
                                orderby m.SequenceOrder
                                select new
                                {
                                    menu_parent_name = m.MenuParentId != null ? m.MENU2.MenuName : "",
                                    menu_name = m.MenuName,
                                    controller_name = m.ControllerName,
                                    action_name = m.ActionName,
                                    icon = m.Icon,
                                    sequence_order = m.SequenceOrder,
                                    menu_parent_id = m.MenuParentId,
                                    menu_id = m.MenuId,
                                    is_active = m.IsActive
                                }).ToList();
                if (!string.IsNullOrEmpty(searchString))
                {

                    var info = searchString.Split(',');
                    int menu_ddl_id = info[0] != "" ? Convert.ToInt32(info[0]) : 0;
                    var txtController = info[1];
                    var txtActionName = info[2];

                    menuList = menu_ddl_id > 0 ? menuList.Where(x => x.menu_id == menu_ddl_id || x.menu_parent_id == menu_ddl_id).OrderByDescending(x => x.menu_id).ToList() : menuList;
                    if (txtController != "")
                    {
                        menuList = menuList.Where(x => x.controller_name != null && x.controller_name.ToUpper().Trim().Contains(txtController.ToUpper().Trim())).ToList();
                    }
                    if (txtActionName != "")
                    {
                        menuList = menuList.Where(x => x.action_name != null && x.action_name.ToUpper().Trim().Contains(txtActionName.ToUpper().Trim())).ToList();

                    }
                }
                result.Object = menuList;
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }
        public Result PostMenuCreate(MENU menu)
        {
            try
            {
                db = new BaseEntities();
                int menu_id = Convert.ToInt32(menu.MenuId);
                if (menu_id > 0)
                {
                    //menu.is_active = menu.is_active;
                    db.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "Menu");
                }
                else
                {
                    db.MENUs.Add(menu);
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "Menu");
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
    }
    #endregion
    #endregion
}