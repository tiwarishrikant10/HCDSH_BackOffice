using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ST_BO.Controllers;
using ST_BO.Models;

namespace ST_BO.Controllers
{
    public class CountryController : BaseController
    {
        private CountryUtil countryUtil;
        private Result result;

        public CountryController()
        {
            result = new Result();
            countryUtil = new CountryUtil();
        }
        //
        // GET: /Country/
        public ActionResult Index()
        {
            ViewBag.Title = "Country List";
            return View();
        }

        public ActionResult CountryList(string id)
        {
            var list = db.COUNTRies.AsEnumerable().ToList();
            var data = (from li in list
                        select new
                        {
                            country_name = li.CountryName,
                            country_id = li.CountryId,
                            is_active = li.IsActive,
                        }).ToList();
            return Json(data);
        }
        //
        // GET: /Country/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Country/Create
        public ActionResult CreateEdit(string  id)
        {
            COUNTRY c = new COUNTRY();
            if (id != null && id != "")
            {
                ViewBag.Title = "Country Edit";
                c = db.COUNTRies.Find(Convert.ToInt32(id));
            }
            else
            {
                ViewBag.Title = "Country Create";

            }
            return View(c);
        }
     
        //
        // POST: /Country/Create
        [HttpPost]
        public ActionResult CreateEdit(COUNTRY country)
        {
            try
            {
                result = countryUtil.PostCountryCreate(country);
                ViewBag.Title = country == null ? "Country Create" : "Country Edit";

                //ViewBag.controller_name = menuUtil.GetController();
                //ViewBag.menu_parent_id = menuUtil.GetMenu(false);
                //ViewBag.menu_ddl_id = menuUtil.GetMenu(true);
                ViewBag.action_name = BaseUtil.GetListAllActionByController("");
                switch (result.MessageType)
                {
                    case MessageType.Success:
                        return RedirectToAction("Index", "Country", new { result = result.Message, MessageType = result.MessageType });

                    default:
                        return RedirectToAction("CreateEdit", "Country", new { result = result.Message, MessageType = result.MessageType });

                }

                return View(country);
            }
            catch
            {
                return View(country);
            }
        }

        //
        // GET: /Country/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Country/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Country/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Country/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
