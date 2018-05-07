using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ST_BO.Models;

namespace ST_BO.Models
{
    public enum ApprovedStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
    }
    public enum PaymentStatus
    {

        Pending = 1,
        Paid = 2,
        Cancel = 3,
        Invalid = 4

    }

    #region Country
    public class CountryUtil
    {
        private Result result;
        private BaseEntities db;
        public CountryUtil()
        {
            this.result = new Result();
            this.db = new BaseEntities();
        }

        public List<SelectListItem> GetCountry()
        {
            return (from c in db.COUNTRies.AsEnumerable().Where(x => x.IsActive).AsEnumerable()
                    orderby c.CountryName
                    select new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();
        }

        public COUNTRY GetCountryByName(int countryId, string countryName)
        {
            return countryId > 0 ?
                db.COUNTRies.AsEnumerable().Where(x => x.CountryName.ToUpper() == countryName.ToUpper() && x.CountryId != countryId).FirstOrDefault()
                : db.COUNTRies.AsEnumerable().Where(x => x.CountryName.ToUpper() == countryName.ToUpper()).FirstOrDefault();
        }


        internal object GetCompanyByName(int companyId, string companyName)
        {
            throw new NotImplementedException();
        }

        public Result ListCountry()
        {
            try
            {
                var menuList = (from c in db.COUNTRies.AsEnumerable()
                                select new
                                {
                                    country_id = c.CountryId,
                                    country_name = c.CountryName,
                                    is_active = c.IsActive
                                }).ToList();

                result.Object = menuList;
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result PostCountryCreate(COUNTRY country)
        {
            try
            {
                db = new BaseEntities();
                if (country.CountryId > 0)
                {
                    //menu.is_active = menu.is_active;
                    db.Entry(country).State = System.Data.Entity.EntityState.Modified;
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "Country");
                }
                else
                {
                    db.COUNTRies.Add(country);
                    result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "Country");
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

        public COUNTRY GetCountryByCountryId(int countryId, string countryName)
        {
            return countryId > 0 ?
                db.COUNTRies.AsEnumerable().Where(x => x.CountryName.ToUpper() == countryName.ToUpper() && x.CountryId != countryId).FirstOrDefault()
                : db.COUNTRies.AsEnumerable().Where(x => x.CountryName.ToUpper() == countryName.ToUpper()).FirstOrDefault();
        }
    }
    #endregion

    #region State
    public class StateUtil
    {
        private Result result;
        private BaseEntities db;
        public StateUtil()
        {
            result = new Result();
            db = new BaseEntities();
        }
        public Result CreateEditState(STATE stateObj)
        {

            try
            {
                result = new Result();
                if (stateObj.StateId > 0)
                {
                    STATE tempState = db.STATEs.Where(s => s.StateId == stateObj.StateId).SingleOrDefault();
                    tempState.StateName = stateObj.StateName;
                    tempState.IsActive = stateObj.IsActive;
                    db.Entry(tempState).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "state");

                }
                else
                {
                    db.STATEs.Add(stateObj);
                    db.SaveChanges();

                    result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "state");
                }
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }

            return result;

        }
        public List<SelectListItem> GetStateSelectList(int countryId)
        {

            var list = (from c in db.STATEs.AsEnumerable().Where(c => c.IsActive && c.CountryId == countryId)
                        orderby c.StateName
                        select new SelectListItem
                        {
                            Text = c.StateName,
                            Value = c.StateId.ToString(),
                        }).ToList();

            return list;
        }
        public STATE GetStateByName(int StateID, string stateName, int CountryId)
        {
            return StateID > 0 ?
                db.STATEs.AsEnumerable().Where(x => x.StateName.ToUpper() == stateName.ToUpper() && x.CountryId == CountryId && x.StateId != StateID).FirstOrDefault()
                : db.STATEs.AsEnumerable().Where(x => x.StateName.ToUpper() == stateName.ToUpper() && x.CountryId == CountryId).FirstOrDefault();
        }
        public List<SelectListItem> GetStateName()
        {

            return (from s in db.STATEs
                    select new SelectListItem
                    {
                        Value = s.StateId.ToString(),
                        Text = s.StateName
                    }).ToList();
        }

        public string GetStateNameById(int StateId)
        {
            return db.STATEs.Find(StateId).StateName.ToString();
        }

    }

    #endregion

    #region City
    public class CityUtil
    {

        private Result result;
        private BaseEntities db;
        public CityUtil()
        {
            result = new Result();
            db = new BaseEntities();
        }
        public Result CreateEditCity(CITY cityObj)
        {

            try
            {
                result = new Result();
                if (cityObj.CityId > 0)
                {
                    CITY tempCity = db.CITies.Where(s => s.CityId == cityObj.CityId).SingleOrDefault();
                    tempCity.CityName = cityObj.CityName;
                    tempCity.IsActive = cityObj.IsActive;
                    db.Entry(tempCity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "city");

                }
                else
                {
                    db.CITies.Add(cityObj);
                    db.SaveChanges();

                    result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "city");
                }
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }

            return result;

        }
        public CITY GetCityByName(int StateID, string cityName, int CountryId,int cityId)
        {
            return cityId > 0 ?
                db.CITies.AsEnumerable().Where(x => x.CityName.ToUpper() == cityName.ToUpper() && x.StateId == StateID && x.STATE.COUNTRY.CountryId == CountryId && x.CityId != cityId && x.IsActive).FirstOrDefault()
                : db.CITies.AsEnumerable().Where(x => x.CityName.ToUpper() == cityName.ToUpper() && x.StateId == StateID && x.STATE.COUNTRY.CountryId == CountryId && x.IsActive).FirstOrDefault();

        }
        public Result GetCityById(int cityId)
        {
            result = new Result();
            try
            {
                if (cityId > 0)
                {

                    result.Object = db.CITies.Where(s => s.CityId == cityId).SingleOrDefault();

                }
                else
                {
                    result.MessageType = MessageType.Error;
                    result.Message = "State Not Found";
                }
            }
            catch (Exception ex)
            {
                result.MessageType = MessageType.Error;
                result.Message = ex.Message;
            }
            return result;
        }
        public List<SelectListItem> GetCitySelectList(int stateId)
        {

            var list = (from c in db.CITies.AsEnumerable().Where(c => c.IsActive && c.StateId == stateId)
                        orderby c.CityName
                        select new SelectListItem
                        {
                            Text = c.CityName,
                            Value = c.CityId.ToString(),
                        }).ToList();

            return list;
        }
        public Result GetCityList()
        {
            result = new Result();
            try
            {
                result.Object = db.CITies.AsEnumerable().ToList();
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

    //#region Area
    //public class AreaUtil
    //{
    //    private Result result;
    //    private BaseEntities db;
    //    public AreaUtil()
    //    {
    //        result = new Result();
    //        db = new BaseEntities();
    //    }
    //    public Result CreateEditArea(area areaObj)
    //    {

    //        try
    //        {
    //            result = new Result();
    //            if (areaObj.area_id > 0)
    //            {
    //                db.Entry(areaObj).State = System.Data.Entity.EntityState.Modified;
    //                db.SaveChanges();
    //                result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "Area");

    //            }
    //            else
    //            {
    //                db.areas.Add(areaObj);
    //                db.SaveChanges();
    //                result.Message = string.Format(BaseConst.MSG_SUCCESS_CREATE, "Area");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result.MessageType = MessageType.Error;
    //            result.Message = ex.Message;
    //        }

    //        return result;
    //    }

    //    public Result GetAreaById(int AreaId)
    //    {
    //        result = new Result();
    //        try
    //        {
    //            if (AreaId > 0)
    //            {

    //                result.Object = db.areas.Where(s => s.area_id == AreaId).SingleOrDefault();

    //            }
    //            else
    //            {
    //                result.MessageType = MessageType.Error;
    //                result.Message = "Area Not Found";
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result.MessageType = MessageType.Error;
    //            result.Message = ex.Message;
    //        }
    //        return result;
    //    }
    //    public List<SelectListItem> GetAreaSelectList(int cityId)
    //    {

    //        var list = (from c in db.areas.Where(c => c.is_active && c.city_id == cityId)
    //                    orderby c.display_order
    //                    select new SelectListItem
    //                    {
    //                        Text = c.area_name,
    //                        Value = c.area_id.ToString(),
    //                    }).ToList();

    //        return list;
    //    }
    //    public Result GetAreaList()
    //    {
    //        result = new Result();
    //        try
    //        {
    //            result.Object = db.areas.AsEnumerable().ToList();
    //        }
    //        catch (Exception ex)
    //        {
    //            result.MessageType = MessageType.Error;
    //            result.Message = ex.Message;
    //        }

    //        return result;
    //    }
    //    public area GetAreaByName(int areaId, string areaName, int countryId, int stateId, int cityId)
    //    {
    //        return areaId > 0 ?
    //            db.areas.AsEnumerable().Where(x => x.area_name.ToUpper() == areaName.ToUpper() && x.city.state.country_id == countryId && x.city.state_id == stateId && x.city_id == cityId && x.area_id != areaId).FirstOrDefault()
    //            : db.areas.AsEnumerable().Where(x => x.area_name.ToUpper() == areaName.ToUpper() && x.city.state.country_id == countryId && x.city.state_id == stateId && x.city_id == cityId).FirstOrDefault();
    //    }

    //}

    //#endregion

    
}