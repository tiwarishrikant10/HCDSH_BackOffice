using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ST_BO.Models;

namespace  ST_BO.Models
{
    #region Classes & Enum

    public class DashboardStatusCount
    {
        public string StatusName { get; set; }
        public string styleColor { get; set; }
        public int StatusID { get; set; }
        public int StatusCount { get; set; }
        public string DateName { get; set; }

    }

    #endregion

    #region Business

    public class DashboardUtil
    {


        private Result result;
        private BaseEntities db;
        public DashboardUtil()
        {
            this.result = new Result();
            this.db = new BaseEntities();
        }
 
    }


    #endregion
}