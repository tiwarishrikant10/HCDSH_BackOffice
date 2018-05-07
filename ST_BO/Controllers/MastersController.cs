using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;
using ST_BO.Controllers;
using System.IO;
using System.Reflection;

namespace ST_BO.Controllers
{
    public class MastersController : BaseController
    {
        private Result result;
        private CountryUtil countryUtil;
        private StateUtil stateUtil;
        private CityUtil cityUtil;
 

        public MastersController()
        {
            result = new Result();
            countryUtil = new CountryUtil();
            stateUtil = new StateUtil();
            cityUtil = new CityUtil();
           
        }
   

        #region COUNTRY
        public ActionResult CountryIndex(string id)
        {
            ViewBag.Title = "Country List";
            COUNTRY c = new COUNTRY();
            if (id != null && id != "")
            {

                c = db.COUNTRies.Find(Convert.ToInt32(id));
            }

            return View(c);
        }
        public ActionResult CountryList(int id)
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
        // GET: /Country/Create
        public ActionResult CountryEdit(string id)
        {
            var data = (from c in db.COUNTRies.AsEnumerable()
                        where c.CountryId == Convert.ToInt32(id)
                        select new
                        {
                            country_id = c.CountryId,
                            country_name = c.CountryName,
                            is_active = c.IsActive,
                        }).FirstOrDefault();

            return Json(data);
        }
        // POST: /Country/Create
        [HttpPost]
        public ActionResult CountryCreateEdit(COUNTRY country)
        {
            result = countryUtil.PostCountryCreate(country);
            ViewBag.Title = country == null ? "Country Create" : "Country Edit";
            ViewBag.action_name = BaseUtil.GetListAllActionByController("");
            return Json(result);
        }

        #endregion

        #region STATE
        public ActionResult StateIndex(string id)
        {
            STATE s = new STATE();
            ViewBag.Title = "State List";
            ViewBag.country_id = new SelectList(countryUtil.GetCountry(), "Value", "Text");
            return View(s);
        }
        public ActionResult StateList(int id)
        {
            var list = db.STATEs.AsEnumerable().ToList();
            var data = (from li in list
                        select new
                        {
                            country_name = li.COUNTRY.CountryName,
                            state_id = li.StateId,
                            state_name = li.StateName,
                            is_active = li.IsActive,
                        }).ToList();
            return Json(data);
        }
        // GET: /Country/Create
        public ActionResult StateEdit(string id)
        {
            var data = (from c in db.STATEs.AsEnumerable()
                        where c.StateId == Convert.ToInt32(id)
                        select new
                        {
                            country_id = c.CountryId,
                            state_name = c.StateName,
                            state_id = c.StateId,
                            is_active = c.IsActive,
                        }).FirstOrDefault();

            return Json(data);
        }
        // POST: /Country/Create
        [HttpPost]
        public ActionResult StateCreateEdit(STATE state)
        {
            result = stateUtil.CreateEditState(state);
            return Json(result);
        }

        #endregion

        #region CITY
        public ActionResult CityIndex(string id)
        {
            CITY s = new CITY();
            ViewBag.Title = "City List";
            List<SelectList> list = new List<SelectList>();
            ViewBag.country_id = new SelectList(countryUtil.GetCountry(), "Value", "Text");
            ViewBag.state_id = list;
            return View(s);
        }
        public ActionResult CityList(int id)
        {
            var list = db.CITies.AsEnumerable().ToList();
            var data = (from li in list
                        select new
                        {
                            country_name = li.STATE.COUNTRY.CountryName,
                            state_id = li.StateId,
                            state_name = li.STATE.StateName,
                            city_name = li.CityName,
                            city_id = li.CityId,
                            is_active = li.IsActive,
                        }).ToList();
            return Json(data);
        }
        // GET: /Country/Create
        public ActionResult CityEdit(string id)
        {
            var data = (from c in db.CITies.AsEnumerable()
                        where c.CityId == Convert.ToInt32(id)
                        select new
                        {
                            country_id = c.STATE.CountryId,
                            state_idList = stateUtil.GetStateSelectList(c.STATE.CountryId),
                            city_name = c.CityName,
                            state_id = c.StateId,
                            CityId = c.CityId,
                            is_active = c.IsActive,
                        }).FirstOrDefault();

            return Json(data);
        }
        // POST: /Country/Create
        [HttpPost]
        public ActionResult CityCreateEdit(CITY city)
        {
            result = cityUtil.CreateEditCity(city);
            return Json(result);
        }
        #endregion

        #region CATEGORY

        public ActionResult MenuCategory()
        {
            return View();
        }
        #endregion
    }
}



