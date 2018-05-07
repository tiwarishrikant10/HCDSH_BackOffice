using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;
using System.Linq.Expressions;
using System.Web.Security;
using System.Data;
using System.Collections;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ST_BO.Models;
 
using context = System.Web.HttpContext;  

namespace ST_BO.Models
{
    #region Enum & Classes
    public enum datetypes
    {
        ThisWeek = 1,
        LastWeek = 2,
        ThisMonth = 3,
        LastMonth = 4,
        ThisYear = 5,
        LastYear = 6
    }
    public enum UserInfo
    {
        UserID, LoginID, FullName, EmailID, Mobile, RoleName, Last_Login_Date, UserPhoto, Gender, RoleID, RoleBit, CompanyID, CompanyName, CompanyFolderName,
        IsCompanySetup, IsCompanyAddUpdate,
        time_zone, currency_id, currency_name, currency_symbol,
        date_format_id, date_format_name, date_format_code_csharp, date_format_code_js,
        time_format_id, time_format_name, time_format_code_csharp, time_format_code_js,
        application_type_id, application_type_name,
        SERVER_TIME_ZONE,
        MenuList,
        SuperAdmin,
        //, Admin, CRMExecutive, Account, Manager, Agent
        system_company_folder, default_contact_id,
        CompanyLogo,
        company_doc_file_name, is_account_access, is_account_add_payment,
        dld_payment_per,
        admin_fees_price,
        is_property_edit

    }

    public class BaseEntities : ST_BASE_DATAEntities
    {
        public override int SaveChanges()
        {

            //Add/New...
            var entitiesNew = this.ChangeTracker.Entries().Where(e => e.State == System.Data.Entity.EntityState.Added);
            try
            {
                foreach (var entry in entitiesNew)
                {
                    var entity = entry.Entity;
                    if (entity != null)
                    {
                        Int32 systemID = Convert.ToInt32(BaseUtil.GetWebConfigValue("SYSTEM_ID"));
                        Int32 companyID = Convert.ToInt32(BaseUtil.GetWebConfigValue("COMPANY_ID"));

                        if (BaseUtil.IsAuthenticated())
                        {
                            if (BaseUtil.GetSessionValue(UserInfo.IsCompanyAddUpdate.ToString()) == "1")
                            {

                                BaseUtil.SetProperty(entity, "CompanyId", BaseUtil.IsAuthenticated() ? SessionUtil.GetCompanyID() : companyID);

                            }
                            BaseUtil.SetProperty(entity, "IsActive", true);
                            BaseUtil.SetProperty(entity, "IsLoginEnable", true);
                            BaseUtil.SetProperty(entity, "CreatedBy", BaseUtil.IsAuthenticated() ? SessionUtil.GetUserID() : systemID);
                            BaseUtil.SetProperty(entity, "CreatedDate", BaseUtil.GetCurrentDateTime());

                        }
                    }
                }
                //Update/Modified...
                var entitiesModified = this.ChangeTracker.Entries().Where(e => e.State == System.Data.Entity.EntityState.Modified);
                foreach (var entry in entitiesModified)
                {
                    var entity = entry.Entity;
                    if (entity != null)
                    {
                        Int32 systemID = Convert.ToInt32(BaseUtil.GetWebConfigValue("SYSTEM_ID"));
                        Int32 companyID = Convert.ToInt32(BaseUtil.GetWebConfigValue("COMPANY_ID"));
                        if (BaseUtil.IsAuthenticated())
                        {
                            if (BaseUtil.GetSessionValue(UserInfo.IsCompanyAddUpdate.ToString()) == "1")
                            {
                                BaseUtil.SetProperty(entity, "CompanyId", BaseUtil.IsAuthenticated() ? SessionUtil.GetCompanyID() : companyID);
                            }
                            BaseUtil.SetProperty(entity, "UpdatedBy", BaseUtil.IsAuthenticated() ? SessionUtil.GetUserID() : systemID);
                            BaseUtil.SetProperty(entity, "UpdatedDate", BaseUtil.GetCurrentDateTime());
                        }
                    }
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);

                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
        static string GetName<T>(T item) where T : class
        {
            var properties = typeof(T).GetProperties();
            return properties[0].Name;
        }

    }
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
    }
    public static class DbContextExt
    {
        public static void AddEntity<T>(this DbContext db, T obj) where T : class
        {
            //db.Set<T>().Add(obj);
            db.Entry<T>(obj).State = EntityState.Added;
        }
        public static T GetEntity<T>(this DbContext db, T obj, object id) where T : class
        {
            //db.Set<T>().Add(obj);
            //return db.Set<T>().Find(id);
            //var t = obj.GetType();
            return db.Set<T>().Find(id);
        }
        //public static T GetObjectById<T>(this DbContext db, int id) where T : class
        //{
        //    return db.Set<T>().Find(id);
        //}
    }
    public static class EnumUtil
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static string GetJsEnum<T>()
        {
            StringBuilder jsStr = new StringBuilder();
            string enumName = typeof(T).Name.ToString();
            var enumNo = Enum.GetValues(typeof(T)).Cast<int>().Select(x => x.ToString()).ToArray();
            var enumVal = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            for (int i = 0; i < enumNo.Length; i++)
            {
                if (i == 0 && enumNo.Length > 1)
                {
                    jsStr.Append(string.Format("var {0} ={{", enumName));
                }
                jsStr.Append(string.Format("{0}: {1},", enumVal[i], enumNo[i]));
            }
            jsStr.Append("}");
            return jsStr.ToString();
            //"var enum1 ={Success: 1,Info: 2,}"
        }
        public static string ParseName<T>(T value)
        {
            return Convert.ToString(Enum.GetName(typeof(T), value));
        }
        public static string GetJsConst(Type type)
        {
            StringBuilder jsStr = new StringBuilder();
            string constName = type.Name.ToString();
            List<string> list = new List<string>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy);

            for (int i = 0; i < fields.Length; i++)
            {
                var fieldInfo = fields[i];
                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(string))
                {
                    if (i == 0 && fields.Length > 1)
                    {
                        jsStr.Append(string.Format("{0} ={{}};\n", constName));
                    }
                    string str = constName + "." + fieldInfo.Name + "=\"" + fieldInfo.GetRawConstantValue() + "\";\n";
                    jsStr.Append(str);
                }
            }
            return jsStr.ToString();
        }
        public static string GetJs(object obj, string name)
        {
            StringBuilder Js = new StringBuilder();
            var obj_js = JsonConvert.SerializeObject((obj), Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = BaseConst.DATE });

            Js.Append("<script>\r\n");
            Js.Append(string.Format("js_{0} = {1}", name, obj_js));
            Js.Append("</script>\r\n");
            string line = Convert.ToString(Js);
            return (string.IsNullOrEmpty(line) ? "" : line);
        }
    }
    public class GoogleResponse
    {
        public string Kind { get; set; }
        public string Id { get; set; }
        public string LongUrl { get; set; }
    }
    public static class BaseConst
    {
        public const string VALIDATION_ISREQUIRED = "data-valc-isrequired";
        public const string VALIDATION_REQ_MSG = "data-valc-required-msg";
        public const string VALIDATION_ID = "data-valc-validation-id";
        public const string HTML_PREFIX = "dmx";
        public const string DATE = "dd-MMM-yyyy";
        public const string DATE_NUM = "dd-MM-yyyy";
        public const string MONTH_year = "MMM-yyyy";
        public const string MONTH_YEAR = "MMM-YYYY";
        public const string year_MONTH = "yyyy-MMM";
        public const string TIME = "hh:mm:ss";
        public const string TIME24 = "HH:mm";
        public const string DATETIME = "dd-MMM-yyyy hh:mm";
        public const string DATETIME24 = "dd-MMM-yyyy HH:mm";
        public const string MSG_SUCCESS_CREATE = "{0} created successfully";
        public const string MSG_SUCCESS_UPDATE = "{0} updated successfully";
        public const string MSG_SUCCESS_DELETE = "{0} deleted successfully";
        public const string MSG_SUCCESS_UNLOCK = "{0} unlocked successfully";
        public const string MSG_SUCCESS_EMAIL = "{0} sent successfully";
        public const string MSG_INVALID_OLD_PASSWORD = "Old {0} is invalid";
        public const string MSG_FEEDBACK_SENT = "{0} sent successfully";
        public const string MSG_INVALID_OPERATION = "{0} You dont have permission to perform the action.";
    }
    public static class DateFormat
    {
        public const string DATE = "dd-MMM-yyyy";
        public const string DATE_START_WITH_MONTH = "MM/dd/yyyy";
        public const string DATE_NUM = "dd-MM-yyyy";
        public const string MONTH_year = "MMM-yyyy";
        public const string MONTH_YEAR = "MMM-YYYY";
        public const string year_MONTH = "yyyy-MMM";
    }
    public static class DateTimeFormat
    {
        public const string TIME = "hh:mm:ss";
        public const string TIME24 = "HH:mm";
        public const string DATETIME = "dd-MMM-yyyy hh:mm";
        public const string DATETIME24 = "dd-MMM-yyyy HH:mm";
    }
    public static class ValueConst
    {
        public const string CLEAR = "CLEAR";
        public const string TODAY = "TODAY";
    }
    public class DataDashConst
    {

    }
    public enum QRCodeErrorCorrectionLevel
    {
        /// <summary>Recovers from up to 7% erroneous data.</summary>
        Low,
        /// <summary>Recovers from up to 15% erroneous data.</summary>
        Medium,
        /// <summary>Recovers from up to 25% erroneous data.</summary>
        QuiteGood,
        /// <summary>Recovers from up to 30% erroneous data.</summary>
        High
    }
    public enum AutoCompleteMode
    {
        AutoCompleteOnly,
        AutoCompleteWithEdit
    }

    public enum Gender
    {
        M = 1,
        F = 2
    }
    #endregion



    #region ConvertUtil
    public static class ConvertUtil
    {
        public static string NumberToWords(double doubleNumber)
        {
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = NumberToWords(beforeFloatingPoint);
            double after_point_value = 0;
            var ponit_value = doubleNumber - (double)beforeFloatingPoint;
            if (ponit_value > 0)
            {
                var afterpont_str_value = Convert.ToString(doubleNumber).Split('.')[1];
                after_point_value = Convert.ToDouble("." + afterpont_str_value);
                var afterFloatingPointWord = SmallNumberToWord((int)((after_point_value) * 100), "");
                return beforeFloatingPointWord + " point " + afterFloatingPointWord;

            }
            else
            {
                return beforeFloatingPointWord;
            }
            //var afterFloatingPointWord = SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * 100), "");
            //var afterFloatingPointWord = SmallNumberToWord((int)after_point_value, "");

            //string t3 = beforeFloatingPointWord + " point " + afterFloatingPointWord;

            //return beforeFloatingPointWord + " point " + afterFloatingPointWord;
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }

        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
            return words;
        }
    }
    #endregion

    #region BaseUtil
    public static class BaseUtil
    {
        private static BaseEntities db = new BaseEntities();
        // private static CommonUtil commonUtil = new CommonUtil();

        #region Get Start And End date Acordingly
        public static void getstartenddate(int type, ref DateTime startdate, ref DateTime enddate)
        {
            if (type == 1)
            {
                startdate = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                enddate = DateTime.Today.AddDays(1);
            }
            if (type == 2)
            {
                startdate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 6);
                enddate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 1);
            }
            if (type == 3)
            {
                DateTime now = DateTime.Now;
                startdate = new DateTime(now.Year, now.Month, 1);
                enddate = startdate.AddMonths(1).AddDays(-1);
            }
            if (type == 4)
            {
                var today = DateTime.Today;
                var month = new DateTime(today.Year, today.Month, 1);
                startdate = month.AddMonths(-1);
                enddate = month.AddDays(-1);
            }
            if (type == 5)
            {
                int year = DateTime.Now.Year;
                startdate = new DateTime(year, 1, 1);
                enddate = new DateTime(year, 12, 31);
            }
            if (type == 6)
            {
                int year = DateTime.Now.Year - 1;
                startdate = new DateTime(year, 1, 1);
                enddate = new DateTime(year, 12, 31);
            }


        }


        public static string GetDateActivity(DateTime date)
        {
            var todayDate = DateTime.Now;
            var dateDiff = (todayDate - date).Days;
            var monthAppart = 12 * (date.Year - todayDate.Year) + date.Month - todayDate.Month;
            var appartYear = (date.Year - todayDate.Year);


            DateTime dt1 = Convert.ToDateTime(date);//mm/dd/yyyy
            DateTime dt = Convert.ToDateTime(todayDate); //mm/dd/yyyy
            TimeSpan ts = dt.Subtract(dt1);



            string result = "";
            if (date.ToString("dd-MMM-yyyy") == todayDate.ToString("dd-MMM-yyyy"))
            {
                result = date.ToString(BaseConst.TIME24);
            }
            if (dateDiff < 30 && dateDiff > 0)
            {
                result = dateDiff + " days ago";
            }
            else if (dateDiff >= 30 && dateDiff < 365)
            {
                result = monthAppart + " Month ago";
            }
            else if (dateDiff > 365)
            {

                result = appartYear + " Year ago";
            }
            return result;

        }
        #endregion

        #region Date & Time
        public static String WeekNumber(DateTime dt)
        {

            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);

            return "Week-" + CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
        }

        public static String WeekNumberStartDate(DateTime dt)
        {

            DayOfWeek day = DayOfWeek.Sunday;
            int diff = dt.DayOfWeek - day;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date.ToString(BaseConst.DATE);

        }

        public static DateTime GetCurrentDateTime()
        {
            Int32 diffMinutes = Convert.ToInt32(GetWebConfigValue("MIN_DIFF"));
            return System.DateTime.Now.AddMinutes(diffMinutes);
        }
        public static DateTime GetTodayDate()
        {
            Int32 diffMinutes = Convert.ToInt32(GetWebConfigValue("MIN_DIFF"));
            return System.DateTime.Today.AddMinutes(diffMinutes).Date;
        }

        public static DateTime? GetNullDate()
        {
            return (DateTime?)null;
        }

        public static TimeSpan? GetCurrentTimeSpan()
        {
            DateTime myDate1 = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 0, 0, 0);
            DateTime myDate2 = GetCurrentDateTime();

            TimeSpan? myDateResult;
            myDateResult = myDate2 - myDate1;

            TimeSpan? myDateResult1 = new TimeSpan(myDateResult.Value.Hours, myDateResult.Value.Minutes, 0);
            return myDateResult1;
        }
        public static String GetHours(double minutes)
        {
            var timeSpan = TimeSpan.FromMinutes(minutes);
            return timeSpan.ToString(@"hh\:mm");
        }


        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        #endregion

        #region Configuration
        public static string GetWebConfigValue(string Name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[Name].ToString(); ;
        }

        public static string GetImagePath(string img, int? gender = 0)
        {
            gender = gender == null ? 0 : gender;
            int companyid = SessionUtil.GetCompanyID();
            var SNAG_PATH = BaseUtil.GetWebConfigValue("SNAG_AWS_S3");
            var CompanyFolderName = BaseUtil.GetSessionValue(UserInfo.CompanyFolderName.ToString());
            var NA_IMAGE = BaseUtil.GetWebConfigValue("NA_IMAGE");
            var DefaultImgs = "";
            if (string.IsNullOrEmpty(img))
            {
                switch ((Gender)(gender))
                {
                    case Gender.M:
                        DefaultImgs = NA_IMAGE.Split(',').Where(a => a.Contains(Convert.ToString(Gender.M).ToString())).FirstOrDefault().ToString();

                        break;
                    case Gender.F:
                        DefaultImgs = NA_IMAGE.Split(',').Where(a => a.Contains(Convert.ToString(Gender.F).ToString())).FirstOrDefault().ToString();

                        break;
                    default:
                        DefaultImgs = NA_IMAGE.Split(',').Where(a => a.Contains("NA")).FirstOrDefault().ToString();

                        break;
                }
            }
            else
            {
                DefaultImgs = img;
                if (DefaultImgs.Trim() == "NA.JPG" || DefaultImgs.Trim() == "M.JPG" || DefaultImgs.Trim() == "F.JPG")
                {
                    DefaultImgs = SNAG_PATH + DefaultImgs;
                }
                else
                {
                    DefaultImgs = SNAG_PATH + CompanyFolderName + DefaultImgs;
                }
            }

            DefaultImgs = string.IsNullOrEmpty(img) ? SNAG_PATH + DefaultImgs : DefaultImgs;

            return DefaultImgs;

        }
        #endregion

        #region Send Mail
        //public static void SendMail(String EmailIDTo, String SubjectText, String Body, String[] attachments = null)
        //{
        //    try
        //    {

        //        string EMAIL_SENT = GetWebConfigValue("EMAIL_SENT");

        //        string COMPANY_EMAIL = GetWebConfigValue("COMPANY_EMAIL");
        //        string COMPANY_EMAIL_PWD = GetWebConfigValue("COMPANY_EMAIL_PWD");
        //        string Host = GetWebConfigValue("Host");

        //        string CC = GetWebConfigValue("CC");
        //        string BCC = GetWebConfigValue("BCC");

        //        SmtpClient smtpClient = new SmtpClient();
        //        MailMessage message = new MailMessage();
        //        MailAddress fromAddress = new MailAddress(COMPANY_EMAIL, SubjectText);
        //        smtpClient.Host = Host;
        //        smtpClient.Port = 25;
        //        smtpClient.UseDefaultCredentials = false;
        //        smtpClient.Credentials = new System.Net.NetworkCredential(COMPANY_EMAIL, COMPANY_EMAIL_PWD);

        //        message.From = fromAddress;



        //        message.To.Add(Convert.ToString(EmailIDTo.Trim()));
        //        //message.CC.Add(Convert.ToString(CC));
        //        message.Bcc.Add(Convert.ToString(BCC));

        //        StringBuilder sb = new StringBuilder();
        //        if (attachments != null)
        //        {
        //            foreach (var item in attachments)
        //            {
        //                if (item != null)
        //                    message.Attachments.Add(new Attachment(item));
        //            }
        //        }
        //        message.Subject = SubjectText;
        //        message.IsBodyHtml = true;
        //        message.Body = Body;// +sb.ToString();
        //        if (EMAIL_SENT == "Y")
        //        {
        //            smtpClient.Send(message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseUtil.CaptureErrorValues(ex);
        //        // throw ex;
        //    }

        //}

        public static void SendMail(string EmailIDTo, string SubjectText, string Body, String[] attachments = null)
        {

            try
            {

                string EMAIL_SENT = GetWebConfigValue("EMAIL_SENT");
                string EMAIL_SENT_BCC = GetWebConfigValue("EMAIL_SENT_BCC");
                // Replace sender@example.com with your "From" address. 
                // This address must be verified with Amazon SES.
                String FROM = GetWebConfigValue("FROM");
                String FROM_NAME = GetWebConfigValue("FROM_NAME");

                // Replace recipient@example.com with a "To" address. If your account 
                // is still in the sandbox, this address must be verified.
                string TO = EmailIDTo;

                // Replace smtp_username with your Amazon SES SMTP user name.
                String SMTP_USERNAME = GetWebConfigValue("SMTP_USERNAME");

                // Replace smtp_password with your Amazon SES SMTP user name.
                string SMTP_PASSWORD = GetWebConfigValue("SMTP_PASSWORD");

                // (Optional) the name of a configuration set to use for this message.
                // If you comment out this line, you also need to remove or comment out
                // the "X-SES-CONFIGURATION-SET" header on line 59, below.
                string CONFIGSET = GetWebConfigValue("CONFIG_SET");

                // If you're using Amazon SES in a region other than US West (Oregon), 
                // replace email-smtp.us-west-2.amazonaws.com with the Amazon SES SMTP  
                // endpoint in the appropriate Region.
                string HOST = GetWebConfigValue("HOST");

                // The port you will connect to on the Amazon SES SMTP endpoint. We
                // are choosing port 587 because we will use STARTTLS to encrypt
                // the connection.
                int PORT = Convert.ToInt32(GetWebConfigValue("PORT"));



                // The subject line of the email
                string SUBJECT = SubjectText;
                //"Amazon SES test (SMTP interface accessed using C#)";

                // The body of the email
                string BODY = Body;

                // Create and build a new MailMessage object
                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress(FROM, FROM_NAME);
                message.To.Add(new MailAddress(TO));
                if (EMAIL_SENT_BCC == "Y")
                {
                    string BCC = GetWebConfigValue("BCC");
                    message.Bcc.Add(new MailAddress(BCC));
                }
                // The Attachment
                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {
                        if (item != null)
                            message.Attachments.Add(new Attachment(item));
                    }
                }
                message.Subject = SUBJECT;
                message.Body = BODY;
                // Comment or delete the next line if you are not using a configuration set
                // message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);

                // Create and configure a new SmtpClient
                SmtpClient client =
                    new SmtpClient(HOST, PORT);
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
                // Enable SSL encryption
                client.EnableSsl = true;

                // Send the email. 
                try
                {
                    if (EMAIL_SENT == "Y")
                    {
                        client.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    BaseUtil.CaptureErrorValues(ex);
                }

                //// Wait for a key press so that you can see the console output
                //Console.Write("Press any key to continue...");
                //Console.ReadKey();
            }
            catch (System.Exception ex)
            {
                BaseUtil.CaptureErrorValues(ex);
                // throw ex;
            }
            finally
            {

            }
        }
        #endregion

        #region Set Property
        public static void SetProperty(object p, string propName, object value)
        {
            Type t = p.GetType();
            PropertyInfo info = t.GetProperty(propName);
            if (info == null)
                return;
            if (!info.CanWrite)
                return;
            info.SetValue(p, value, null);
        }
        public static object GetValue(object p, string propName)
        {
            return p.GetType().GetProperty(propName).GetValue(p, null);
        }
        public static void CopyObject(object from, object to, string properties)
        {
            if (from.GetType() == to.GetType())
            {
                string[] pi = properties.Split(',');

                Type t = from.GetType();
                for (int i = 0; i < pi.Count(); i++)
                {
                    string propName = pi[i];
                    PropertyInfo info = t.GetProperty(propName);
                    if (info == null)
                        return;
                    if (!info.CanWrite)
                        return;
                    object value = info.GetValue(from);
                    info.SetValue(to, value, null);
                }
            }
        }
        public static string LowerCase(object p)
        {
            return Convert.ToString(p).Trim().ToLower();
        }
        public static string UpperCase(object p)
        {
            return Convert.ToString(p).Trim().ToUpper();
        }
        #endregion

        #region SMS API
        public static string apicall(string url)
        {
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);

            try
            {

                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();

                StreamReader sr = new StreamReader(httpres.GetResponseStream());

                string results = sr.ReadToEnd();

                sr.Close();
                return results;



            }
            catch
            {
                return "0";
            }
        }
        #endregion

        #region Capture Error
        public static int CaptureErrorValues(Exception exception)
        {
            int errorLogID = 0;
            #region Capture the Values
            StringBuilder errrorMessage = new StringBuilder();

            string FormValues = String.Empty;

            string SessionValues = String.Empty;
            if (HttpContext.Current.Session != null)
            {
                foreach (var item in HttpContext.Current.Session.Keys)
                {
                    SessionValues = SessionValues + item.ToString() + " : " + HttpContext.Current.Session[item.ToString()] + "<br>";
                }
            }

            string RawUrl = HttpContext.Current.Request.RawUrl;
            string RequestType = HttpContext.Current.Request.RequestType;
            string UserAgent = HttpContext.Current.Request.UserAgent;
            string BrowserName = ((System.Web.Configuration.HttpCapabilitiesBase)(HttpContext.Current.Request.Browser)).Browser;


            errrorMessage.Append("<b>" + GetWebConfigValue("APPLICATION_NAME") + "</b><br><br>");

            errrorMessage.Append("<b>Error Date  :</b>" + System.DateTime.Now + "<br>");
            errrorMessage.Append("<b>Server Name  :</b>" + System.Environment.MachineName + "<br>");
            errrorMessage.Append("<b>Application Url :</b>" + RawUrl + "<br>");
            errrorMessage.Append("<b>Request Type :</b>" + RequestType + "<br>");
            errrorMessage.Append("<b>User Agent :</b>" + UserAgent + "<br><br>");
            errrorMessage.Append("<b>BrowserName :</b>" + BrowserName + "<br><br>");

            errrorMessage.Append("<b>Exception InnerException :</b>" + exception.InnerException + "<br><br>");
            errrorMessage.Append("<b>Exception Message :</b>" + exception.Message + "<br><br>");
            errrorMessage.Append("<b>Exception Source :</b>" + exception.Source + "<br><br>");

            errrorMessage.Append("<u><b>Posted Form Values</b></u><br>");
            errrorMessage.Append(FormValues);

            errrorMessage.Append("<br><br><u><b>Session Values Values</b></u><br>");
            errrorMessage.Append(SessionValues);

            errrorMessage.Append("<br><br><u><b>Exception Stack Trace:</b></u><br>" + exception.StackTrace);

            #endregion

            try
            {
                Int32 userID = 0;
                if (HttpContext.Current.Session != null)
                {
                    userID = HttpContext.Current.Session[UserInfo.UserID.ToString()] != null ? Convert.ToInt32(HttpContext.Current.Session[UserInfo.UserID.ToString()].ToString()) : 0;
                }
                DateTime currDateTime = GetCurrentDateTime();

                #region Insert Into APP_ERROR_LOG

                APP_ERROR_LOG errorlog = new APP_ERROR_LOG();
                errorlog.ErrorMessage = errrorMessage.ToString();
                if (userID > 0)
                {
                    errorlog.UserId = userID;
                }
                else
                {
                    errorlog.UserId = null;

                }
                errorlog.Created_Date = currDateTime;

                db.APP_ERROR_LOG.Add(errorlog);
                db.SaveChanges();

                errorLogID = errorlog.ErrorLogId;
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return errorLogID;

        }
        public static string GetValidationMessage(ModelStateDictionary modelState)
        {
            string msgResult = "";
            for (int i = 0; i < modelState.ToList().Count; i++)
            {
                if (modelState.ToList()[i].Value.Errors.Count > 0)
                {
                    msgResult += string.Format("{0}~{1};", modelState.ToList()[i].Key.ToString(), modelState.ToList()[i].Value.Errors[0].ErrorMessage);
                }
            }
            return msgResult;
        }
        #endregion

        #region Session
        public static void SetSessionValue(String Key, String Value)
        {
            HttpContext.Current.Session[Key] = Value;
        }
        public static String GetSessionValue(String Key)
        {
            return HttpContext.Current.Session[Key] != null ? HttpContext.Current.Session[Key].ToString() : string.Empty;
        }

        #endregion

        #region Accesible Pages
        public static List<string> AccesiblePages()
        {
            List<Int64> authlevels = new List<Int64>();
            List<string> list = new List<string>();
            try
            {
                Int64 Value = Convert.ToInt32(GetSessionValue(UserInfo.RoleBit.ToString()));
                Int64 result = 0;
                for (Int64 i = 0; Value >= (Int64)Math.Pow(2, i); i++)
                {
                    result = Value & (Int64)Math.Pow(2, i);
                    authlevels.Add(result);
                }

                foreach (var item in authlevels)
                {
                    var au = (from R in db.ROLEs.AsEnumerable()
                              join RA in db.ROLE_ACTION.AsEnumerable() on R.RoleId equals RA.RoleId
                              where R.RoleBit == item  
                              select new
                              {
                                  ACTION = (RA.ControllerName + RA.ActionName).ToUpper()
                              }
                              ).ToList();

                    foreach (var item1 in au)
                    {
                        list.Add(item1.ACTION.ToUpper());
                    }

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public static List<string> ListControllerExcluded()
        {
            List<string> list = new List<string>() { "BASE", "JSON", "ACCOUNT", "API" };
            return list;
        }
        public static bool CheckAuthentication(ActionExecutingContext filterContext)
        {
            bool result = false;

            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            String Action = string.Format("{0}Controller{1}", controllerName, actionName).ToUpper();

            List<string> accesiblePages = AccesiblePages();

            foreach (var item in accesiblePages)
            {
                if (Action == item)
                {
                    result = true;
                    break;
                }

            }

            return result;
        }

        #endregion

        #region Login and PWD
        public static bool IsAuthenticated()
        {
            return string.IsNullOrEmpty(GetSessionValue(UserInfo.UserID.ToString())) ? false : true;
        }
        public static String UpdatePassword(String OldPassword, String NewPassword)
        {
            int UserID = SessionUtil.GetUserID();
            String result = String.Empty;
            try
            {
                string CheckOldPwd = db.USP_Decrypt_TEXT(db.USERs.Find(UserID).Password).ToString();

                if (OldPassword == CheckOldPwd)
                {

                    USER objTable = (from t in db.USERs
                                     where t.UserId == UserID
                                     select t
                                             ).First();

                    objTable.Password = (byte[])db.USP_Encrypt_TEXT(NewPassword).FirstOrDefault();
                    db.Entry(objTable).State = EntityState.Modified;
                    db.SaveChanges();
                    result = "SUCCESS";
                }
                else
                {
                    result = "INVALID_OLD_PASSWORD";
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public static string UserLoginPolicy(int UserID, Boolean isFaild)
        {
            String result = String.Empty;
            try
            {
                Int32 No_failed_attempt = Convert.ToInt32(GetWebConfigValue("No_failed_attempt"));
                USER objTable = (from t in db.USERs
                                 where t.UserId == UserID
                                 select t
                                         ).First();
                if (isFaild)
                {
                    objTable.PasswordFailedAttempt = objTable.PasswordFailedAttempt + 1;
                    Int32 No_failed_attempt_left = Convert.ToInt32(No_failed_attempt - objTable.PasswordFailedAttempt);

                    result = "Invalid Login ID or Password. You have only  " + No_failed_attempt_left + " attempt left.";

                    if (objTable.PasswordFailedAttempt > No_failed_attempt)
                    {
                        objTable.IsAccountLocked = true;
                        result = "your account has been locked due to continues " + No_failed_attempt + " time wrong password ";
                    }

                }
                else
                {
                    objTable.PasswordFailedAttempt = 0;
                    objTable.IsAccountLocked = false;
                    objTable.LastLoginDate = GetCurrentDateTime();
                }
                db.Entry(objTable).State = EntityState.Modified;
                db.SaveChanges();


            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public static string GetRandomPasswordString(int pwdLength)
        {
            int asciiZero;
            int asciiNine;
            int asciiA;
            int asciiZ;
            int count = 0;
            int randNum;
            string randomString;

            System.Random rRandom = new System.Random(System.DateTime.Now.AddMinutes(0).Millisecond);

            asciiZero = 48;
            asciiNine = 57;
            asciiA = 64;
            asciiZ = 90;

            randomString = "";
            while ((count < pwdLength))
            {
                if (count % 2 == 0)
                {
                    randNum = rRandom.Next(asciiZero, asciiNine);
                }
                else
                {
                    randNum = rRandom.Next(asciiA, asciiZ);
                }
                if (((randNum >= asciiZero) && (randNum <= asciiNine)) || ((randNum >= asciiA) && (randNum <= asciiZ)))
                {
                    randomString = (randomString + ((char)(randNum)));
                    count = (count + 1);
                }
            }
            return randomString;
        }
        public static string GetRandomPasswordNumber(int pwdLength)
        {
            int asciiZero;
            int asciiNine;
            int asciiA;
            int asciiZ;
            int count = 0;
            int randNum;
            string randomString;

            System.Random rRandom = new System.Random(System.DateTime.Now.AddMinutes(0).Millisecond);

            asciiZero = 48;
            asciiNine = 57;
            asciiA = 64;
            asciiZ = 90;

            randomString = "";
            while ((count < pwdLength))
            {
                //if (count % 2 == 0)
                //{
                randNum = rRandom.Next(asciiZero, asciiNine);
                //}
                //else
                //{
                //    randNum = rRandom.Next(asciiA, asciiZ);
                //}
                if (((randNum >= asciiZero) && (randNum <= asciiNine)))// || ((randNum >= asciiA) && (randNum <= asciiZ)))
                {
                    randomString = (randomString + ((char)(randNum)));
                    count = (count + 1);
                }
            }
            return randomString;
        }
        public static int CheckLoginIDExist(String LoginID)
        {
            var info = LoginID.Split(',');
            var login_id = info[0];
            var user_id = info[1];
            Int32 companyID = SessionUtil.GetCompanyID();
            return info.Length == 2 ? db.USERs.AsEnumerable().Where(x => (x.LoginId ?? "").Trim().ToUpper() == login_id.Trim().ToUpper() && x.UserId != Convert.ToInt32(user_id)).Count() :
                db.USERs.AsEnumerable().Where(x => x.LoginId.Trim().ToUpper() == login_id.Trim().ToUpper()).Count();
        }
        public static int CheckMobileExist(String id)
        {
            var info = id.Split(',');
            var mobileNo = info[0];
            var userId = info[1];
            Int32 companyID = SessionUtil.GetCompanyID();
            return info.Length == 2 ? db.USERs.AsEnumerable().Where(x => x.Mobile.Trim().ToUpper() == mobileNo.Trim().ToUpper() && x.UserId != Convert.ToInt32(userId) && x.CompanyId == companyID).Count() :
                db.USERs.AsEnumerable().Where(x => x.Mobile.Trim().ToUpper() == mobileNo.Trim().ToUpper()).Count();
        }
        //public static int CheckEmailIDExist(String id)
        //{
        //    var info = id.Split(',');
        //    var emailId = info[0];
        //    var employeeId = Convert.ToInt32(info[1]);
        //    return info.Length == 2 ? db.employees.AsEnumerable().Where(x => x.user.email_id.Trim().ToUpper() == emailId.Trim().ToUpper() && x.employee_id != employeeId).Count() :
        //        db.employees.AsEnumerable().Where(x => x.user.mobile.Trim().ToUpper() == emailId.Trim().ToUpper()).Count();
        //}
        public static int CheckEmailIDExist(String id)
        {
            var info = id.Split(',');
            var email_id = info[0];
            var user_id = Convert.ToInt32(info[1]);
            Int32 companyID = SessionUtil.GetCompanyID();
            return info.Length == 2 ? db.USERs.AsEnumerable().Where(x => x.EmailId.Trim().ToUpper() == email_id.Trim().ToUpper() && x.UserId != user_id && x.CompanyId == companyID).Count() :
                db.USERs.AsEnumerable().Where(x => x.EmailId.Trim().ToUpper() == email_id.Trim().ToUpper()).Count();
        }

        public static int CheckEmployeeCodeExist(string login_id)
        {
            return (from M in db.USERs.AsEnumerable()
                    where (M.LoginId.Trim().ToUpper() == login_id.Trim().ToUpper())
                    select M).Count();
        }
        #endregion

        #region Application Path

        static HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        static HttpRequest Request
        {
            get { return Context.Request; }
        }
        public static AjaxOptions ConfigAjaxOptions()
        {
            AjaxOptions options = new AjaxOptions();
            options.HttpMethod = "Post";
            options.OnSuccess = "OnSuccess";
            options.OnBegin = "LockUIOnCallback('true')";
            options.LoadingElementId = "PleaseWait";
            options.OnComplete = "LockUIOnCallback('false')";
            options.OnFailure = "OnFail";
            return options;
        }

        public static UrlHelper GetUrlHelper()
        {
            return new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public static string GetActionPath(string controllerActionName) //Ex: "Category/Edit"
        {
            return GetApplicationPath(Request.RequestContext) + string.Format("/{0}", controllerActionName);
        }

        //public static string GetActionPath(string controllerName, string actionName)
        //{
        //    return GetApplicationPath(Request.RequestContext) + string.Format("/{0}/{1}", controllerName, actionName);
        //}
        public static string GetActionPath(string controllerName, string actionName, string id = null)
        {
            id = string.IsNullOrEmpty(id) ? "" : "/" + id;
            return GetApplicationPath(Request.RequestContext) + string.Format("/{0}/{1}{2}", controllerName, actionName, id);
        }

        public static string GetApplicationPath(RequestContext requestContext)
        {
            string retPath;
            string httpOrigin = Request.ServerVariables["HTTP_ORIGIN"];
            string applicationPath = Request.ApplicationPath;
            //Approach #1: OK:Post
            //retPath = (applicationPath == "/" ? httpOrigin : httpOrigin + applicationPath);
            //Approach #2 OK:All
            retPath = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority) + (applicationPath == "/" ? "" : applicationPath);
            return retPath;
        }

        public static string GetApplicationPath()
        {
            string retPath;
            string applicationPath = Request.ApplicationPath;
            retPath = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority) + (applicationPath == "/" ? "" : applicationPath);
            return retPath;
        }


        public static string GetCurrentController()
        {
            return Convert.ToString(Request.RequestContext.RouteData.Values["controller"]);
        }
        public static string GetCurrentAction()
        {
            return Convert.ToString(Request.RequestContext.RouteData.Values["action"]);
        }

        public static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        public static List<string> GetControllerActionNames(Type t)
        {
            List<string> list = new List<string>();
            if (t != null)
            {
                list = t.GetMethods().Where(m => m.ReturnType == typeof(ActionResult))
                    .Select(m => m.Name).Distinct().ToList();
            }
            return list;
        }
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
        #endregion

        #region Dropdown
        public static List<SelectListItem> GetListController()
        {
            List<SelectListItem> listAllControler = (from M in GetControllerNames().AsEnumerable()
                                                     orderby M
                                                     select new SelectListItem
                                                     {
                                                         Value = M,
                                                         Text = M.Replace("Controller", ""),
                                                     }).ToList();


            List<SelectListItem> listActual = (from la in listAllControler
                                               where !(from exc in ListControllerExcluded() select exc).Contains(la.Text.ToUpper())
                                               select la).ToList();
            return listActual;
        }
        public static List<SelectListItem> GetListAllActionByController(string controllerName)
        {
            var listAll = (from M in GetControllerActionNames(GetType("ST_BO.Controllers." + controllerName)).AsEnumerable()
                           orderby M
                           select new SelectListItem
                           {
                               Value = M,
                               Text = M,
                           }).ToList();
            return listAll;
        }
        public static List<SelectListItem> GetListActionUnassignByRoleAndController(int roleID, string controllerName)
        {
            return (from la in GetListAllActionByController(controllerName)
                    where !(from asg in GetListActionAssignedByRoleAndController(roleID, controllerName) select asg.Value).Contains(la.Value)
                    select la).ToList();

        }
        public static List<SelectListItem> GetListActionAssignedByRoleAndController(int roleID, string controllerName)
        {
            var listAssigned = (from r in db.ROLE_ACTION
                                where r.RoleId == roleID && r.ControllerName == controllerName
                                orderby r.ActionName
                                select new SelectListItem
                                {
                                    Value = r.ActionName,
                                    Text = r.ActionName,
                                }).ToList();
            return listAssigned;
        }
        #endregion

        #region Misc/Others
        public static string FetchDefaultActionName(string actionName)
        {
            string defaultActionName = "INDEX";
            if (actionName.Contains(defaultActionName))
            {
                return defaultActionName;
            }
            defaultActionName = "DETAILS";
            if (actionName.Contains(defaultActionName))
            {
                return defaultActionName;
            }
            defaultActionName = "CREATE";
            if (actionName.Contains(defaultActionName))
            {
                return defaultActionName;
            }
            defaultActionName = "EDIT";
            if (actionName.Contains(defaultActionName))
            {
                return defaultActionName;
            }
            defaultActionName = "DELETE";
            if (actionName.Contains(defaultActionName))
            {
                return defaultActionName;
            }
            return string.Empty;
        }

        public static String getUserByRoleBit(int roleBit = 0)
        {
            string userName = "";

            if (roleBit > 0)
            {
                var roleList = db.ROLEs.AsEnumerable().Where(x => x.RoleBit == roleBit).ToList();

                userName = roleList.Count > 0 ? roleList.FirstOrDefault().RoleName : "";
            }
            return userName;
        }
        #endregion

        #region Send SMS
        public static void SendSms(String MemberPhoneNumber, String Message, String smsType)
        {
            try
            {

                string SMS_SENT = GetWebConfigValue("SMS_SENT");
                string domain = GetWebConfigValue("SMS_DOMAIN");
                string SMS_USERNAME = GetWebConfigValue("SMS_USERNAME");
                string SMS_PASSWORD = GetWebConfigValue("SMS_PASSWORD");
                string SENDER = GetWebConfigValue("SENDER");
                string Notify = GetWebConfigValue("Notify");
                string CompanyFooterName = GetWebConfigValue("CompanyFooterName");

                if (MemberPhoneNumber.StartsWith("0755"))
                {
                    SMS_SENT = "N";
                }
                if (MemberPhoneNumber.Length == 11)
                {
                    MemberPhoneNumber = MemberPhoneNumber.Substring(1, 10);
                }

                // Message = Message + Environment.NewLine + "Thanks and Regards " + Environment.NewLine + CompanyFooterName;
                MemberPhoneNumber = MemberPhoneNumber + Notify;

                int l = Message.Length / 160 + 1;
                int m = MemberPhoneNumber.Split(',').Count();

                if (SMS_SENT == "Y")
                {
                    string result = string.Empty;
                    //http://login.bulksmsdeal.in/vendorsms/pushsms.aspx?user=digimindtechnology&password=digimind1234&msisdn=9754266388,9826528745&sid=CCFPIL&msg=CCFPIL TEST MESSAGE&fl=0&gwid=2
                    result = apicall("http://" + domain + "/vendorsms/pushsms.aspx?user=" + SMS_USERNAME + "&password=" + SMS_PASSWORD + "&msisdn=" + MemberPhoneNumber + "&sid=" + SENDER + "&msg=" + Message + "&fl=0&gwid=2");
                    //string result = apicall("http://" + domian + "/pushsms.php?username=" + SMS_USERNAME + "&api_password=" + SMS_PASSWORD + "&sender=" + SENDER + "&to=" + MemberPhoneNumber + "&message=" + Message + "&priority=2");
                    if (!result.StartsWith("Wrong Username or Password"))
                    {
                        #region Insert Into sms_tracking initialize

                        //sms_tracking track = new sms_tracking();
                        //track.sms_numbers = MemberPhoneNumber;
                        //track.sms_message = Message;
                        //track.api_message = result;
                        //track.sms_count = l * m;
                        //track.smsType = smsType;


                        //db.sms_tracking.Add(track);
                        //db.SaveChanges();

                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        #endregion

        #region Get Tiny URL
        public static String GetTinyUrl(String URL)
        {
            String tinyURL = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=AIzaSyD53jrR-hUl5R7Tw8dc6_kIdvQ2AWQdaAs");
                request.Method = "POST";
                request.ContentType = "application/json";
                string requestData = string.Format(@"{{""longUrl"": ""{0}""}}", URL);
                byte[] requestRawData = Encoding.ASCII.GetBytes(requestData);
                request.ContentLength = requestRawData.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(requestRawData, 0, requestRawData.Length);
                requestStream.Close();

                WebResponse response = request.GetResponse();
                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                string responseData = responseReader.ReadToEnd();
                responseReader.Close();

                var deserializer = new JavaScriptSerializer();
                var results = deserializer.Deserialize<GoogleResponse>(responseData);
                tinyURL = results.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return tinyURL;

        }
        #endregion

        #region QR Code
        public static string GetQRCode(string data, int size = 80, int margin = 4, QRCodeErrorCorrectionLevel errorCorrectionLevel = QRCodeErrorCorrectionLevel.Low, object htmlAttributes = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (size < 1)
                throw new ArgumentOutOfRangeException("size", size, "Must be greater than zero.");
            if (margin < 0)
                throw new ArgumentOutOfRangeException("margin", margin, "Must be greater than or equal to zero.");
            if (!Enum.IsDefined(typeof(QRCodeErrorCorrectionLevel), errorCorrectionLevel))
                throw new InvalidEnumArgumentException("errorCorrectionLevel", (int)errorCorrectionLevel, typeof(QRCodeErrorCorrectionLevel));

            var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chld={2}|{3}&chs={0}x{0}&chl={1}", size, HttpUtility.UrlEncode(data), errorCorrectionLevel.ToString()[0], margin);

            var tag = new TagBuilder("img");
            if (htmlAttributes != null)
                tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tag.Attributes.Add("src", url);
            tag.Attributes.Add("width", size.ToString());
            tag.Attributes.Add("height", size.ToString());

            return new MvcHtmlString(tag.ToString(TagRenderMode.SelfClosing)).ToString();
        }
        #endregion

        #region Calculate Distance
        public static double distance(double lat1, double lon1, double lat2, double lon2, String sr)
        {


            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (sr.Equals("K"))
            {
                dist = dist * 1.609344;
            }
            else if (sr.Equals("N"))
            {
                dist = dist * 0.8684;
            }
            return (dist);

            //double theta = lon1 - lon2;
            //double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            //dist = Math.Abs(Math.Round(rad2deg(Math.Acos(dist)) * 60 * 1.1515 * 1.609344 * 1000, 0));
            //return (dist);
        }
        public static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        public static double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
        #endregion
        public static string ListToJson(IList<System.Web.WebPages.Html.SelectListItem> listSelected)
        {
            StringBuilder selected = new StringBuilder();
            if (listSelected != null)
            {
                if (listSelected.Count() > 0)
                {
                    //["9", "4"]
                    selected.Append("[");
                    for (int i = 0; i < listSelected.Count(); i++)
                    {
                        string str = "'{0}'" + (i < listSelected.Count() - 1 ? "," : "]");
                        selected.Append(string.Format(str, listSelected[i].Value));
                    }
                }
            }
            return selected.ToString();
        }

        public static List<SelectListItem> GetTimeZoneInfo()
        {

            return (from c in TimeZoneInfo.GetSystemTimeZones()
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.DisplayName
                    }).ToList();
        }
    }
    #endregion

    #region Misc
    public static class SessionUtil
    {
        public static int GetUserID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.UserID.ToString()));
        }
        public static string GetFullName()
        {
            return BaseUtil.GetSessionValue(UserInfo.FullName.ToString());
        }

        public static string GetLoginID()
        {
            return BaseUtil.GetSessionValue(UserInfo.LoginID.ToString());
        }

        public static int GetRoleID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.RoleID.ToString()));
        }

        public static int GetRoleBit()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.RoleBit.ToString()));
        }

        public static int GetRoleName()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.RoleName.ToString()));
        }
        public static int GetCompanyID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.CompanyID.ToString()));
        }

        public static string GetCompanyFolderName()
        {
            return BaseUtil.GetSessionValue(UserInfo.CompanyFolderName.ToString());
        }

        public static string GetTimeZone()
        {
            return BaseUtil.GetSessionValue(UserInfo.time_zone.ToString());
        }
        public static string GetCurrencySymbol()
        {
            return BaseUtil.GetSessionValue(UserInfo.currency_symbol.ToString());
        }
        public static string GetDateFormatCSharp()
        {
            return BaseUtil.GetSessionValue(UserInfo.date_format_code_csharp.ToString());
        }
        public static string GetTimeFormatCSharp()
        {
            return BaseUtil.GetSessionValue(UserInfo.time_format_code_csharp.ToString());
        }
        public static string GetDateFormatJS()
        {
            return BaseUtil.GetSessionValue(UserInfo.date_format_code_js.ToString());
        }
        public static string GetTimeFormatJS()
        {
            return BaseUtil.GetSessionValue(UserInfo.time_format_code_js.ToString());
        }

        public static string GetServerTimeZone()
        {
            return BaseUtil.GetSessionValue(UserInfo.SERVER_TIME_ZONE.ToString());
        }

        public static int GetApplicationTypeID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.application_type_id.ToString()));
        }

        public static string GetApplicationTypeName()
        {
            return (BaseUtil.GetSessionValue(UserInfo.application_type_name.ToString()));
        }

        public static string GetSystemCompanyFolder()
        {
            return (BaseUtil.GetSessionValue(UserInfo.system_company_folder.ToString()));
        }
        public static string GetDefaultContactID()
        {
            return (BaseUtil.GetSessionValue(UserInfo.default_contact_id.ToString()));
        }
        public static string GetCompanyContractDocFileName()
        {
            return (BaseUtil.GetSessionValue(UserInfo.company_doc_file_name.ToString()));
        }

        public static Boolean IsAccountAccess()
        {
            return Convert.ToBoolean(BaseUtil.GetSessionValue(UserInfo.is_account_access.ToString()));
        }

        public static Boolean IsAccountAddPayment()
        {
            return Convert.ToBoolean(BaseUtil.GetSessionValue(UserInfo.is_account_add_payment.ToString()));
        }

        public static Boolean IsPropertyEdit()
        {
            return Convert.ToBoolean(BaseUtil.GetSessionValue(UserInfo.is_property_edit.ToString()));
        }

        public static decimal GetDLDPaymentPercentage()
        {
            return Convert.ToDecimal(BaseUtil.GetSessionValue(UserInfo.dld_payment_per.ToString()));
        }
        public static decimal GetAdminFee()
        {
            return Convert.ToDecimal(BaseUtil.GetSessionValue(UserInfo.admin_fees_price.ToString()));
        }
    }
    public static class ExUtil
    {
        public static string GetMessage(DbEntityValidationException dbEx)
        {
            string errors = string.Empty;
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    errors += string.Format("{0}-{1} ", validationError.PropertyName, validationError.ErrorMessage);
                }
            }
            return errors;
        }
    }
    #endregion

    #region Custom Html Helper
    public enum EditorType
    {
        None,
        Input,
        Button,
        Select,
        Textarea,
        Image,
        File,
        MultiSelect,
        AutoComplete,
        Custom,
        Div,
        DropdownMenu,
    }
    public enum RequiredType
    {
        None,
        Standard,
        Custom
    }
    public enum ControlType
    {
        none,
        button,
        checkbox,
        color,
        date,
        datetime,
        datetime_local,
        div,
        email,
        file,
        hidden,
        image,
        month,
        number,
        password,
        radio,
        range,
        reset,
        search,
        submit,
        tel,
        text,
        time,
        url,
        week,
        textarea,
        select,
        autoComplete,
        graph,
        tree,
        menu,
        menu_horizontal,
        dropdownMenu,
        textCaptcha
    }
    public class ServerTask
    {
        public ServerTask()
        {
            this.ServerAction = ServersideAction.None;
            this.AjaxInJsonName = "";
            this.Async = false;
            this.ControllerName = "Json";
            this.ActionName = "";
            this.LoadInClient = false;
            this.ParentId = "";
            this.ParmId = "";
            this.OnChangeIds = "";
            this.OnClickIds = "";
            this.ClearDataIds = "";
            this.SkipValidation = false;
            this.AjaxInJsonName = "";

        }
        public string Id { get; set; }
        public string Name { get; set; }
        public ServersideAction ServerAction { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ParentId { get; set; }
        public string ParmId { get; set; }
        public string OnClickIds { get; set; }
        public string OnChangeIds { get; set; }
        public string ClearDataIds { get; set; }
        public bool Async { get; set; }
        public bool LoadInClient { get; set; }
        #region Post
        public bool SkipValidation { get; set; }
        public string AjaxInJsonName { get; set; } //country or any custom model user_employee etc.
        public MimeType MimeType { get; set; } //html/json etc.  
        #endregion
    }
    public enum MimeType
    {
        Json,
        Html,
    }
    public enum ServersideAction
    {
        None,
        IsExist,
        LoadParitally,
        LoadGraph,
        LoadTree,
        Casecade,
        GoTo,
        Post,
    }
    #region Graph
    public class Graph
    {
        public Graph()
        {
            this.legend = new legend();
            //this.series = new series();
            this.hAxis = new axis();
            this.vAxis = new axis();
            this.chartArea = new chartArea();
            this.titleTextStyle = new textStyle();
        }
        //var options = {
        //    title: TITLE,
        //    titleTextStyle: { color: '#9f8b8b', fontName: 'Arial', fontSize: 12 },
        //    is3D: true,
        //    pointSize: 6,
        //    isStacked: true,
        //    width: WIDTH,
        //    height: HEIGHT,
        //    hAxis: { title: "Date" },
        //    vAxis: { title: "Count" },
        //    vAxis: { textPosition: 'none', gridlines: { color: 'transparent' } },
        //    hAxis: { title: "" }, //{ title: "Date" },
        //    legend: { position: 'none', textStyle: { color: '#666', fontSize: 12 } },
        //    chartArea: { left: LeftMargin, top: 0, width: AREA_WIDHT, height: AREA_HEIGHT },
        //    colors: ['#3366cc', '#dc3912', '#ff9900', '#990099', '#109618', '#018ab4', '#71180c', '#FFFF00', '#93ACE7'],
        //    displayAnnotations: false,
        //    legend: { position: 'in', textStyle: { color: '#666', fontSize: 12 } },
        //    series: { 1: { type: "line" } }
        //};
        public ChartType chartType { get; set; }
        public string title { get; set; }
        public textStyle titleTextStyle { get; set; }
        public bool is3D { get; set; }
        public int pointSize { get; set; }
        public bool isStacked { get; set; }
        public string width { get; set; }
        public string height { get; set; }

        public axis hAxis { get; set; }
        public axis vAxis { get; set; }
        public chartArea chartArea { get; set; }
        public string[] colors { get; set; }
        public bool displayAnnotations { get; set; }
        public legend legend { get; set; }
        public string className { get; set; }
        //public series series { get; set; }
    }
    public class Tree
    {
        public Tree()
        {
            this.List = new List<TreeNode>();
        }
        public List<TreeNode> List { get; set; }
    }
    [Serializable]
    public class TreeNode
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int? Sequence { get; set; }
    }
    public enum ChartType
    {
        GeoChart,
        ScatterChart,
        ColumnChart,
        Histogram,
        BarChart,
        ComboChart,
        AreaChart,
        SteppedAreaChart,
        LineChart,
        PieChart,
        BubbleChart,
        DonutChart,
        OrgChart,
        Treemap,
        Table,
        Timeline,
        Gauge,
        CandlestickChart,
    }
    public class legend
    {
        public legend()
        {
            this.textStyle = new textStyle();
        }
        public string position { get; set; }
        public textStyle textStyle { get; set; }
        public string maxLines { get; set; }

    }
    public class chartArea
    {
        public string left { get; set; }
        public string top { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }
    public class series
    {
        public string name { get; set; }
    }
    public class axis
    {
        public string title { get; set; }
        public string textPosition { get; set; }
    }
    public class textStyle
    {
        //color: '#9f8b8b', fontName: 'Arial', fontSize: 12 
        public string color { get; set; }
        public string fontName { get; set; }
        public string fontSize { get; set; }
    }

    #endregion
    public enum Alignment
    {
        Left,
        Right
    }
    public static class Glyphicon
    {
        #region Web Application Icons
        public const string Globe = "fa fa-globe";
        public const string Envelope = "fa fa-envelope";
        public const string User = "fa fa-user";
        public const string Users = "fa fa-users";
        public const string UserPlus = "fa fa-user-plus";
        public const string Phone = "glyphicon glyphicon-phone";
        public const string Percent = "fa fa-percent";
        public const string Inr = "fa fa-inr";
        public const string GooglePlus = "fa fa-google-plus-official";
        public const string Bars = "fa fa-bars";
        public const string Camera = "fa fa-camera";
        public const string Comment = "fa fa-comment";
        public const string SquareO = "fa fa-pencil-square-o";
        public const string EnvelopeO = "fa fa-envelope-o";
        public const string Exclamation = "fa fa-exclamation";
        public const string ImageO = "fa fa-file-image-o";
        public const string Pencil = "fa fa-pencil";
        public const string TelePhone = "fa fa-phone";
        public const string Print = "fa fa-print";
        public const string QuestionCircle = "fa fa-question-circle";
        public const string Remove = "fa fa-times";
        public const string Send = "fa fa-paper-plane";
        public const string Bell = "fa fa-bell"; // having BLACK backround
        public const string BellO = "fa fa-bell-o"; // having WHITE backround
        public const string BellSlash = "fa fa-bell-slash";
        public const string Calculator = "fa fa-calculator";
        public const string CalendarO = "fa fa-calendar-o";
        public const string ClockO = "fa fa-clock-o";
        public const string Eye = "fa fa-eye";
        public const string Asterisk = "fa fa-asterisk";
        public const string BalanceScale = "fa fa-balance-scale";
        public const string BarChart = "fa fa-bar-chart";
        public const string Bookmark = "fa fa-bookmark";     //having BLACK backround
        public const string BookmarkO = "fa fa-bookmark-o";  // having WHITE backround
        public const string PdfO = "fa fa-file-pdf-o";
        public const string Film = "fa fa-film";
        public const string ShoppingBag = "fa fa-shopping-bag";
        public const string Download = "fa fa-download";
        public const string ExcelO = "fa fa-file-excel-o";
        public const string Home = "fa fa-home";
        public const string ShoppingBasket = "fa fa-shopping-basket";
        public const string Trash = "fa fa-trash";
        public const string Truck = "fa fa-truck";

        #endregion

        #region Hands Icon
        public const string ThumbsDown = "fa fa-thumbs-down";    //having BLACK backround
        public const string ThumbsDownO = "fa fa-thumbs-o-down"; // having WHITE backround
        public const string ThumbsUp = "fa fa-thumbs-up";    //having BLACK backround
        public const string ThumbsUpO = "fa fa-thumbs-o-up"; // having WHITE backround
        #endregion
    }
    public class HtmlModel
    {
        public StringBuilder htmlString { get; set; }
        public StringBuilder Js { get; set; }
        public string JsOnSelect { get; set; }
        public bool IncludeValidationMsg { get; set; }
        public bool IncludeLabel { get; set; }
        public string About { get; set; }
        public string Glyphicon { get; set; }
        public Alignment GlyphiconAlignment { get; set; }
        public ControlType ControlType { get; set; }
        public ServersideAction ServerAction { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsDefaultButton { get; set; }
        public bool IsDefaultFocus { get; set; }
        public string DefaultText { get; set; }
        public bool IsAutoComplete { get; set; }
        public bool SkipValidation { get; set; }
        public string PlaceHolder { get; set; }
        public bool IsTitle { get; set; }
        public Int16 TimerInterval { get; set; }
        public List<ServerTask> ServerTasks { get; set; }
        public bool IsMultiSelect { get; set; }
        public int MultiSelectSize { get; set; }
        public bool IsButton { get; set; }
        public bool IsEditor { get; set; }
        public Graph Graph { get; set; }
        public Tree Tree { get; set; }

        #region Common Attributes
        public string Id { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string FieldName { get; set; }

        public bool IsRequired { get; set; }
        public string ValidationId { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ParmId { get; set; }
        public string ClearDataIds { get; set; }
        public bool IsLoad { get; set; }
        public bool IsLoadInClient { get; set; }
        public string OnChangeIds { get; set; }
        public string OnClickIds { get; set; }
        public string Message { get; set; }
        public string Regex { get; set; }
        public int? Minlength { get; set; }
        public int? Maxlength { get; set; }

        public bool RemoveCheckBox { get; set; }
        public List<System.Web.WebPages.Html.SelectListItem> List { get; set; } //Select, MultiSelect, AutoComplete etc.
        public List<System.Web.WebPages.Html.SelectListItem> ListSelected { get; set; } //Select, MultiSelect, AutoComplete etc.
        public Dictionary<string, string> Attr { get; set; } //custom html attributes like class, placeholder, onchange, style etc.

        #endregion

        #region File Upload
        public string FileName { get; set; }
        public long MinFileSize { get; set; }
        public long MaxFileSize { get; set; }
        public FileType[] FileTypes { get; set; }
        #endregion

        #region Text Area
        public int Cols { get; set; }
        public int Rows { get; set; }
        #endregion

        #region Auto Complete
        public string fillCtrlId { get; set; }
        public AutoCompleteMode Mode { get; set; }
        public bool isValid { get; set; }
        public string DbId { get; set; }
        public string dbValue { get; set; }
        #endregion

        #region Ajax
        public bool Async { get; set; }
        public string AjaxInJsonName { get; set; } //country or any custom model user_employee etc.
        public string MimeType { get; set; } //html/json etc.
        #endregion

        public HtmlModel()
        {
            this.Attr = new Dictionary<string, string>();
            this.htmlString = new StringBuilder();
            this.ControllerName = "Json";
            this.List = new List<System.Web.WebPages.Html.SelectListItem>();
            this.IncludeValidationMsg = true;
            this.IncludeLabel = false;
            this.Url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            this.ActionName = "";
            this.ParmId = "";
            this.OnChangeIds = "";
            this.OnClickIds = "";
            this.Message = "";
            this.IsLoad = true;
            this.IsButton = this.IsButtonType();
            this.IsEditor = this.IsEditorType();
            this.Graph = new Graph();
            this.Tree = new Tree();
        }

        #region Internal Property & Logic
        private string ReadOnlyProp { get; set; }
        private RequiredType RequiredType { get; set; }
        private UrlHelper Url { get; set; }
        private EditorType EditorType { get; set; }
        private bool IsLengthApplicable { get; set; }
        private bool IsRegExApplicable { get; set; }
        private bool IsInputType()
        {
            bool inputType = true;
            if (this.ControlType == ControlType.textarea || this.ControlType == ControlType.select)
            {
                inputType = false;
            }
            return inputType;
        }
        private bool IsButtonType()
        {
            bool isButton = false;
            switch (this.ControlType)
            {
                case ControlType.button:
                    isButton = true;
                    break;
                case ControlType.submit:
                    isButton = true;
                    break;
                case ControlType.reset:
                    isButton = true;
                    break;
                default:
                    break;
            }
            return isButton;
        }
        private bool IsEditorType()
        {
            bool isEditor = true;
            switch (this.ControlType)
            {
                case ControlType.hidden:
                    isEditor = false;
                    break;
                case ControlType.button:
                    isEditor = false;
                    break;
                case ControlType.submit:
                    isEditor = false;
                    break;
                case ControlType.reset:
                    isEditor = false;
                    break;
                case ControlType.div:
                    isEditor = false;
                    break;
                case ControlType.graph:
                    isEditor = false;
                    break;
                case ControlType.tree:
                    isEditor = false;
                    break;
                case ControlType.menu:
                    isEditor = false;
                    break;
                case ControlType.menu_horizontal:
                    isEditor = false;
                    break;
                default:
                    break;
            }
            return isEditor;
        }
        private ControlType GetInputType()
        {
            ControlType ct = Models.ControlType.none;
            string inputType = this.ControlType.ToString();
            string inputType1 = null;
            this.Attr.TryGetValue("data-input-type", out inputType1);
            if (!string.IsNullOrEmpty(inputType1))
            {
                inputType = inputType1;
                //do nothing...
            }
            try
            {
                ct = EnumUtil.ParseEnum<ControlType>(inputType);
            }
            catch (Exception ex)
            {
                ct = ControlType.none;
            }
            //else
            //{
            //    ct = EnumUtil.ParseEnum<ControlType>(inputType);
            //}
            return ct;
        }
        private string GetDisplayName()
        {
            return string.IsNullOrEmpty(this.Name) ? this.Id : this.Name;
        }
        private string GetEditorId()
        {
            string id = string.Format("{0}", this.Id);
            return (id);
        }
        private string GetRequiredCustom()
        {
            var vs = "";
            string msg = string.Format("The {0} field is required. ", this.GetDisplayName());
            if (this.ValidationId != null)
            {
                string ret = " " + BaseConst.VALIDATION_ISREQUIRED + "='{0}' " + BaseConst.VALIDATION_REQ_MSG + "='{1}' " + BaseConst.VALIDATION_ID + "='{2}' ";
                vs = string.Format(ret, Convert.ToString(this.IsRequired).ToLower(), msg, this.ValidationId);
            }
            else
            {
                string ret = " " + BaseConst.VALIDATION_ISREQUIRED + "='{0}' " + BaseConst.VALIDATION_REQ_MSG + "='{1}' ";
                vs = string.Format(ret, Convert.ToString(this.IsRequired).ToLower(), msg);
            }
            return vs;
        }
        #endregion

        #region Html Creation
        string[] attrDuplicacyAllow = { "style" };
        private void SetRequireAttr(Dictionary<string, string> reqAttr)
        {
            foreach (var req in reqAttr)
            {
                this.AddAttribute(req.Key, req.Value);
            }
        }
        private bool AddAttribute(string key, string value)
        {
            bool alreadyExist = false;
            Dictionary<string, string> exist = new Dictionary<string, string>();
            foreach (var attr in this.Attr)
            {
                if (attr.Key.ToLower() == key.ToLower())// && attrDuplicacyAllow.Contains(key))
                {
                    //if (!attrDuplicacyAllow.Contains(key))
                    alreadyExist = true;
                    exist.Add(attr.Key, attr.Value);//store current key,values
                    break;
                }
            }
            if (alreadyExist)
            {
                this.Attr.Remove(key);
                value += string.Format(" {0}", exist[key]);
            }
            this.Attr.Add(key, value);
            return !alreadyExist;
        }
        private string GetAllValuesOfKeyAttr(string key)
        {
            string values = "";
            foreach (var attr in this.Attr)
            {
                if (attr.Key.ToLower() == key.ToLower() && attrDuplicacyAllow.Contains(key))
                {
                    values += attr.Value + " ";
                }
            }
            return values;
        }
        //private Dictionary<string, string> InitGraph()
        //{

        //    //<script>
        //    //var company = @Html.Raw(JsonConvert.SerializeObject((new company()), Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = BaseConst.DATE }))
        //    //</script>
        //    var x = JsonConvert.SerializeObject((new company()), Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = BaseConst.DATE });
        //    #region Graph Settings
        //    this.Graph.type = ChartType.AreaChart;
        //    this.Graph.chartArea.height = "10";
        //    this.Graph.colors = new string[2] { "", "" };
        //    this.Graph.height = "100px";
        //    this.Graph.is3D = true;
        //    this.Graph.displayAnnotations = true;
        //    this.Graph.isStacked = true;
        //    this.Graph.legend.position = "In";
        //    this.Graph.legend.textStyle.color = "red";
        //    this.Graph.legend.textStyle.fontName = "arial";
        //    this.Graph.legend.textStyle.fontSize = "12px";
        //    this.Graph.series = new series();
        //    this.Graph.title = "graph title";
        //    this.Graph.type = ChartType.AreaChart;
        //    this.Graph.width = "200px";
        //    this.Graph.hAxis.title = "Units";
        //    this.Graph.vAxis.title = "Month";
        //    #endregion

        //    #region Graph Settings
        //    Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
        //    requiredAttr.Add("data-graph-type", Convert.ToInt16(this.Graph.type).ToString());
        //    if (this.Graph.chartArea != null)
        //    {
        //        if (!string.IsNullOrEmpty(this.Graph.chartArea.height))
        //        {
        //            requiredAttr.Add("data-graph-chartarea-height", this.Graph.chartArea.height);
        //        }
        //        if (!string.IsNullOrEmpty(this.Graph.chartArea.left))
        //        {
        //            requiredAttr.Add("data-graph-chartarea-left", this.Graph.chartArea.left);
        //        }
        //        if (!string.IsNullOrEmpty(this.Graph.chartArea.top))
        //        {
        //            requiredAttr.Add("data-graph-chartarea-top", this.Graph.chartArea.top);
        //        }
        //        if (!string.IsNullOrEmpty(this.Graph.chartArea.width))
        //        {
        //            requiredAttr.Add("data-graph-chartarea-width", this.Graph.chartArea.width);
        //        }
        //    }
        //    this.Graph.colors = new string[2] { "", "" };
        //    this.Graph.height = "100px";
        //    this.Graph.is3D = true;
        //    this.Graph.displayAnnotations = true;
        //    this.Graph.isStacked = true;
        //    if (this.Graph.legend != null)
        //    {
        //        if (!string.IsNullOrEmpty(this.Graph.legend.position))
        //        {
        //            requiredAttr.Add("data-graph-legend-position", this.Graph.legend.position);
        //        }
        //        if (!string.IsNullOrEmpty(this.Graph.legend.position))
        //        {
        //            requiredAttr.Add("data-graph-legend-position", this.Graph.legend.position);
        //        }
        //        if (!string.IsNullOrEmpty(this.Graph.legend.position))
        //        {
        //            requiredAttr.Add("data-graph-legend-position", this.Graph.legend.position);
        //        }
        //        if (!string.IsNullOrEmpty(this.Graph.legend.position))
        //        {
        //            requiredAttr.Add("data-graph-legend-position", this.Graph.legend.position);
        //        }

        //    }

        //    this.Graph.legend.position = "In";
        //    this.Graph.legend.textStyle.color = "red";
        //    this.Graph.legend.textStyle.fontName = "arial";
        //    this.Graph.legend.textStyle.fontSize = "12px";
        //    this.Graph.series = new series();
        //    this.Graph.title = "graph title";
        //    this.Graph.type = ChartType.AreaChart;
        //    this.Graph.width = "200px";
        //    this.Graph.hAxis.title = "Units";
        //    this.Graph.vAxis.title = "Month";
        //    #endregion


        //    if (IsReadOnly)
        //    {
        //        this.ControlType = Models.ControlType.text;
        //    }
        //    else
        //    {
        //        requiredAttr.Add("readonly", "");
        //        requiredAttr.Add("style", "background-color: transparent;"); //Revisit[COD]: not working from here imeplemnt in Base.js
        //    }
        //    string valueString = Convert.ToString(this.Value);
        //    switch (valueString.ToUpper())
        //    {
        //        case "CLEAR":
        //            this.Value = null;
        //            //do nothing, let it go null
        //            break;
        //        case "TODAY":
        //            this.Value = DateTime.Now.ToString(BaseConst.DATE);
        //            break;
        //        default:
        //            this.Value = string.IsNullOrEmpty(valueString) ? "" : Convert.ToDateTime(valueString).ToString(BaseConst.DATE);
        //            if (string.IsNullOrEmpty(valueString) || valueString == "01-Jan-0001")
        //            {
        //                this.Value = DateTime.Now.ToString(BaseConst.DATE);
        //            }
        //            break;
        //    }
        //    return requiredAttr;
        //}
        private Dictionary<string, string> InitDate()
        {
            Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
            requiredAttr.Add("data-input-type", "datetime");
            requiredAttr.Add("class", "form-control input-sm");
            if (IsReadOnly)
            {
                this.ControlType = Models.ControlType.text;
            }
            else
            {
                requiredAttr.Add("readonly", "");
                requiredAttr.Add("style", "background-color: transparent;"); //Revisit[COD]: not working from here imeplemnt in Base.js
            }
            string valueString = Convert.ToString(this.Value);
            switch (valueString.ToUpper())
            {
                case "CLEAR":
                    this.Value = null;
                    //do nothing, let it go null
                    break;
                case "TODAY":
                    this.Value = DateTime.Now.ToString(BaseConst.DATE);
                    break;
                default:
                    this.Value = string.IsNullOrEmpty(valueString) ? "" : Convert.ToDateTime(valueString).ToString(BaseConst.DATE);
                    if (string.IsNullOrEmpty(valueString) || valueString == "01-Jan-0001")
                    {
                        this.Value = DateTime.Now.ToString(BaseConst.DATE);
                    }
                    break;
            }
            return requiredAttr;
        }
        private Dictionary<string, string> InitMonth()
        {
            Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
            requiredAttr.Add("data-input-type", "month");
            requiredAttr.Add("class", "form-control input-sm");
            this.ControlType = Models.ControlType.text; //always
            if (IsReadOnly)
            {
                //this.ControlType = Models.ControlType.text;
            }
            else
            {
                requiredAttr.Add("readonly", "");
                requiredAttr.Add("style", "background-color: transparent;"); //Revisit[COD]: not working from here imeplemnt in Base.js
            }
            string valueString = Convert.ToString(this.Value);
            switch (valueString.ToUpper())
            {
                case "CLEAR":
                    this.Value = null;
                    //do nothing, let it go null
                    break;
                case "TODAY":
                    this.Value = DateTime.Now.ToString(BaseConst.MONTH_year);
                    break;
                default:
                    this.Value = string.IsNullOrEmpty(valueString) ? "" : Convert.ToDateTime(valueString).ToString(BaseConst.MONTH_year);
                    if (string.IsNullOrEmpty(valueString) || valueString == "01-Jan-0001")
                    {
                        this.Value = DateTime.Now.ToString(BaseConst.MONTH_year);
                    }
                    break;
            }
            return requiredAttr;
        }
        private Dictionary<string, string> InitTime()
        {
            Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
            requiredAttr.Add("data-input-type", "time");
            requiredAttr.Add("class", "form-control input-sm");
            this.ControlType = Models.ControlType.text; //always
            if (this.TimerInterval > 0)
            {
                requiredAttr.Add("data-timer-interval", Convert.ToString(this.TimerInterval));
            }
            if (IsReadOnly)
            {
                //this.ControlType = Models.ControlType.text;
            }
            else
            {
                requiredAttr.Add("readonly", "");
                requiredAttr.Add("style", "background-color: transparent;"); //Revisit[COD]: not working from here imeplemnt in Base.js
            }
            string valueString = Convert.ToString(this.Value);
            switch (valueString.ToUpper())
            {
                case "CLEAR":
                    this.Value = null;
                    //do nothing, let it go null
                    break;
                case "TODAY":
                    this.Value = DateTime.Now.ToString(BaseConst.TIME24);
                    break;
                default:
                    this.Value = string.IsNullOrEmpty(valueString) ? "" : Convert.ToDateTime(valueString).ToString(BaseConst.TIME24);

                    //if (string.IsNullOrEmpty(valueString) || valueString == "01-Jan-0001")
                    //{
                    //    this.Value = DateTime.Now.ToString(BaseConst.TIME24);
                    //}
                    break;
            }
            return requiredAttr;
        }
        private void Init()
        {
            Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
            this.EditorType = EditorType.Input;
            this.RequiredType = RequiredType.Standard;
            this.ReadOnlyProp = IsReadOnly ? "readonly" : "";

            switch (this.ControlType)
            {
                case ControlType.button:
                    this.EditorType = EditorType.Button;
                    this.Value = string.IsNullOrEmpty(Convert.ToString(this.Value)) ? "Name It" : this.Value;
                    this.RequiredType = RequiredType.None;
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    this.RequiredType = RequiredType.None;
                    requiredAttr.Add("class", "btn btn-sm btn-primary");
                    requiredAttr.Add("data-is-default-button", Convert.ToString(this.IsDefaultButton).ToLower());
                    break;
                case ControlType.checkbox:
                    this.RequiredType = RequiredType.None;
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    string isChecked = Convert.ToString(this.Value).ToLower();
                    this.Value = isChecked;
                    if (isChecked == "true")
                    {
                        requiredAttr.Add("checked", "");
                    }
                    break;
                case ControlType.color:
                    requiredAttr.Add("class", "form-control input-sm");
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    break;
                case ControlType.date:
                    requiredAttr = InitDate();
                    break;
                case ControlType.datetime:
                    requiredAttr = InitDate();
                    break;
                case ControlType.datetime_local:
                    requiredAttr = InitDate();
                    break;
                case ControlType.div:
                    this.RequiredType = RequiredType.None;
                    this.EditorType = EditorType.Div;
                    break;
                case ControlType.graph:
                    this.RequiredType = RequiredType.None;
                    this.EditorType = EditorType.Div;
                    break;
                case ControlType.tree:
                    this.RequiredType = RequiredType.None;
                    this.EditorType = EditorType.Div;
                    requiredAttr.Add("remove-check-box", this.RemoveCheckBox ? "true" : "false");
                    break;
                case ControlType.menu:
                    this.RequiredType = RequiredType.None;
                    this.EditorType = EditorType.Div;
                    break;
                case ControlType.menu_horizontal:
                    this.RequiredType = RequiredType.None;
                    this.EditorType = EditorType.Div;
                    break;
                case ControlType.email:
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.file:
                    requiredAttr.Add("class", "form-control input-sm");
                    if (this.IsMultiSelect)
                    {
                        requiredAttr.Add(this.IsMultiSelect ? "multiple" : "", "");
                    }
                    this.EditorType = EditorType.File;
                    this.RequiredType = RequiredType.Custom;
                    break;
                case ControlType.dropdownMenu:
                    this.EditorType = EditorType.DropdownMenu;
                    this.RequiredType = RequiredType.Custom;
                    break;
                case ControlType.hidden:
                    this.ReadOnlyProp = "";
                    requiredAttr.Add("data-is-title", BaseUtil.LowerCase(this.IsTitle));
                    break;
                case ControlType.image:
                    requiredAttr.Add("class", "form-control input-sm");
                    this.EditorType = EditorType.Image;
                    break;
                case ControlType.month:
                    requiredAttr = InitMonth();
                    break;
                case ControlType.number:
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.password:
                    requiredAttr.Add("class", "form-control input-sm");
                    this.IsRegExApplicable = true;
                    this.IsLengthApplicable = true;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    break;
                //case ControlType.radio:
                //    break;
                case ControlType.radio:
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    break;
                case ControlType.range:
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.reset:
                    this.EditorType = EditorType.Button;
                    this.Value = string.IsNullOrEmpty(Convert.ToString(this.Value)) ? "Reset" : this.Value;
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    requiredAttr.Add("class", "btn btn-sm btn-primary"); //Revisit[DMO]:
                    requiredAttr.Add("data-is-default-button", Convert.ToString(this.IsDefaultButton).ToLower());
                    break;
                case ControlType.search:
                    break;
                case ControlType.submit:
                    this.EditorType = EditorType.Button;
                    this.Value = string.IsNullOrEmpty(Convert.ToString(this.Value)) ? "Submit" : this.Value;
                    this.RequiredType = RequiredType.None;
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    requiredAttr.Add("class", "btn btn-sm btn-primary"); //Revisit[DMO]:
                    requiredAttr.Add("data-is-default-button", Convert.ToString(this.IsDefaultButton));

                    requiredAttr.Add("data-click-type", "post");
                    requiredAttr.Add("onclick", "return formUtil.buttonClickById(this.id);");

                    break;
                case ControlType.tel:
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.text:
                    this.IsRegExApplicable = true;
                    this.IsLengthApplicable = true;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    requiredAttr.Add(this.IsDefaultFocus ? "autofocus" : "", "");
                    requiredAttr.Add("autocomplete", this.IsAutoComplete ? "on" : "off");
                    requiredAttr.Add("class", "form-control input-sm");
                    requiredAttr.Add("data-is-title", BaseUtil.LowerCase(this.IsTitle));
                    break;
                case ControlType.time:
                    requiredAttr = InitTime();
                    break;
                case ControlType.url:
                    this.IsRegExApplicable = true;
                    this.IsLengthApplicable = true;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    requiredAttr.Add(this.IsDefaultFocus ? "autofocus" : "", "");
                    requiredAttr.Add("class", "form-control input-sm");
                    requiredAttr.Add("data-is-title", BaseUtil.LowerCase(this.IsTitle));
                    break;
                case ControlType.week:
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.textarea:
                    this.IsRegExApplicable = true;
                    this.IsLengthApplicable = true;
                    this.EditorType = EditorType.Textarea;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    requiredAttr.Add(this.IsDefaultFocus ? "autofocus" : "", "");
                    requiredAttr.Add("autocomplete", this.IsAutoComplete ? "on" : "off");
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.select:
                    this.ReadOnlyProp = IsReadOnly ? "disabled" : "";
                    this.EditorType = EditorType.Select;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    requiredAttr.Add(this.IsDefaultFocus ? "autofocus" : "", "");
                    if (this.IsMultiSelect)
                    {
                        requiredAttr.Add(this.IsMultiSelect ? "multiple" : "", "");
                        if (this.MultiSelectSize > 0)
                        {
                            requiredAttr.Add("size", Convert.ToString(this.MultiSelectSize));
                        }
                        requiredAttr.Add("data-values", BaseUtil.ListToJson(this.ListSelected));

                        //StringBuilder selected = new StringBuilder();
                        //if (this.ListSelected != null)
                        //{
                        //    if (this.ListSelected.Count() > 0)
                        //    {
                        //        //["9", "4"]
                        //        selected.Append("[");
                        //        for (int i = 0; i < this.ListSelected.Count(); i++)
                        //        {
                        //            string str = "'{0}'" + (i < this.ListSelected.Count() - 1 ? "," : "]");
                        //            selected.Append(string.Format(str, this.ListSelected[i].Value));
                        //        }
                        //        requiredAttr.Add("data-values", selected.ToString());
                        //    }
                        //}
                    }
                    requiredAttr.Add("autocomplete", this.IsAutoComplete ? "on" : "off");
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.autoComplete:
                    this.EditorType = EditorType.AutoComplete;
                    this.RequiredType = RequiredType.Custom;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    requiredAttr.Add(this.IsDefaultFocus ? "autofocus" : "", "");
                    requiredAttr.Add("class", "form-control input-sm");
                    break;
                case ControlType.textCaptcha:
                    this.IsRegExApplicable = true;
                    this.IsLengthApplicable = true;
                    requiredAttr.Add("placeholder", string.IsNullOrEmpty(this.PlaceHolder) ? "" : this.PlaceHolder);
                    requiredAttr.Add(this.IsDefaultFocus ? "autofocus" : "", "");
                    requiredAttr.Add("autocomplete", this.IsAutoComplete ? "on" : "off");
                    requiredAttr.Add("class", "form-control input-sm");
                    requiredAttr.Add("data-is-title", BaseUtil.LowerCase(this.IsTitle));
                    break;
                default:
                    break;
            }
            this.SetRequireAttr(requiredAttr);
        }
        private string Start()
        {
            HtmlTextWriterTag htmlTag = HtmlTextWriterTag.Input;
            ControlType type = this.ControlType;
            string buttonType = "";
            switch (this.ControlType)
            {
                case ControlType.button:
                    htmlTag = HtmlTextWriterTag.Button;
                    buttonType = this.ControlType.ToString();
                    break;
                case ControlType.checkbox:
                    //do nothing...
                    break;
                case ControlType.color:
                    break;
                case ControlType.date:
                    //do nothing...
                    break;
                case ControlType.datetime:
                    //do nothing...
                    break;
                case ControlType.datetime_local:
                    break;
                case ControlType.email:
                    break;
                case ControlType.file:
                    //do nothing...
                    break;
                case ControlType.hidden:
                    break;
                case ControlType.image:
                    break;
                case ControlType.month:
                    break;
                case ControlType.number:
                    break;
                case ControlType.password:
                    //do nothing...
                    break;
                case ControlType.radio:
                    break;
                case ControlType.range:
                    break;
                case ControlType.reset:
                    htmlTag = HtmlTextWriterTag.Button;
                    buttonType = this.ControlType.ToString();
                    break;
                case ControlType.textCaptcha:
                    //do nothing...
                    break;
                case ControlType.search:
                    break;
                case ControlType.submit:
                    htmlTag = HtmlTextWriterTag.Button;
                    buttonType = this.ControlType.ToString();
                    break;
                case ControlType.tel:
                    break;
                case ControlType.text:
                    //do nothing...
                    break;
                case ControlType.time:
                    //do nothing...
                    break;
                case ControlType.url:
                    break;
                case ControlType.week:
                    break;
                case ControlType.div:
                    htmlTag = HtmlTextWriterTag.Div;
                    break;
                case ControlType.graph:
                    htmlTag = HtmlTextWriterTag.Div;
                    break;
                case ControlType.tree:
                    htmlTag = HtmlTextWriterTag.Div;
                    break;
                case ControlType.menu:
                    htmlTag = HtmlTextWriterTag.Div;
                    break;
                case ControlType.menu_horizontal:
                    htmlTag = HtmlTextWriterTag.Div;
                    break;
                case ControlType.textarea:
                    htmlTag = HtmlTextWriterTag.Textarea;
                    break;
                case ControlType.select:
                    htmlTag = HtmlTextWriterTag.Select;
                    break;
                case ControlType.autoComplete:
                    break;
                default:
                    break;
            }
            StringBuilder line = new StringBuilder();
            string styleName = string.IsNullOrEmpty(this.ReadOnlyProp) ? "" : " background-color: lightgray;cursor: no-drop; ";
            styleName += this.IsRequired ? "border-left: 2px solid red; " : "";
            //styleName = string.IsNullOrEmpty(styleName) ? "" : string.Format("style='{0}' ", styleName.Trim());
            Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
            requiredAttr.Add("style", styleName);
            this.SetRequireAttr(requiredAttr);
            styleName = "";
            switch (htmlTag)
            {
                case HtmlTextWriterTag.Input:
                    if (ControlType == ControlType.radio)
                    {
                        foreach (var item in this.List)
                        {
                            string isSelected = item.Value.ToLower() == Convert.ToString(this.Value).ToLower() ? "checked" : "";
                            string isReq = this.IsRequired ? "required" : "";
                            StringBuilder attributes = new StringBuilder();
                            foreach (var attr in this.Attr)
                            {
                                if (!string.IsNullOrEmpty(attr.Key))
                                {
                                    attributes.Append(string.Format("{0} =\"{1}\" ", attr.Key, attr.Value));
                                }
                            }
                            line.Append(string.Format("<input type='{0}' {7} id='{1}' name='{1}' value='{2}' {3} {4} {6} {8}>{5} ", this.ControlType, this.GetEditorId(), item.Value, isSelected, isReq, item.Text, this.ReadOnlyProp, styleName, attributes.ToString()));
                        }
                    }
                    else if (ControlType == ControlType.image)
                    {
                        //line.Append(string.Format("<img id='{1}' src='{0}' ", this.Source, this.GetEditorId()));
                    }
                    else
                    {
                        line.Append(string.Format("<input type='{0}' {3} id='{1}' name='{1}' {2} ", this.ControlType.ToString().Replace("_", "-"), this.GetEditorId(), this.ReadOnlyProp, styleName));
                    }
                    break;
                case HtmlTextWriterTag.Select:
                    line.Append(string.Format("<{0} id='{1}' {3} name='{1}' {2} ", this.ControlType, this.GetEditorId(), this.ReadOnlyProp, styleName));
                    break;
                case HtmlTextWriterTag.Textarea:
                    line.Append(string.Format("<{0} {5} id='{1}' name='{1}'  cols='{2}' rows='{3}' {4} ", this.ControlType, this.GetEditorId(), this.Cols, this.Rows, this.ReadOnlyProp, styleName));
                    break;
                case HtmlTextWriterTag.Button:
                    line.Append(string.Format("<button type='{0}' id='{1}' name='{1}' {2} {3} ", this.ControlType, this.GetEditorId(), this.ReadOnlyProp, styleName));
                    break;
                case HtmlTextWriterTag.Div:
                    line.Append(string.Format("<div id='{1}'  ", this.ControlType, this.GetEditorId()));
                    break;
            }
            return Convert.ToString(line);
        }
        private string GetValue()
        {
            string line = "";
            if (this.EditorType == EditorType.Input)
            {
                ControlType ct = this.GetInputType();
                if (!string.IsNullOrEmpty(Convert.ToString(this.Value)))
                {
                    if (ct == ControlType.checkbox)
                    {
                        if (Convert.ToString(this.Value).ToLower() == "true")
                        {
                            line = string.Format("checked='checked' ");
                        }
                        line += string.Format(" value='true' ");
                    }
                    else if (ct == ControlType.radio) // || this.IsButton) //Revisit[COD]
                    {
                        //do nothing...
                    }
                    else
                    {
                        line = string.Format("value='{0}' ", this.Value);
                    }
                }
            }
            //this.htmlString.Append(line);
            return line;
        }
        private string GetRequired()
        {
            string line = "";
            if (this.IsRequired)
            {
                switch (this.RequiredType)
                {
                    case RequiredType.Standard:
                        if (this.ControlType == ControlType.radio)
                        {
                            //do nothing...
                        }
                        else
                        {
                            line = string.Format(" class='form-control input-sm base-required' data-val ='true' data-val-required='The {0} field is required.' ", this.GetDisplayName());
                        }
                        //this.htmlString.Append(line);
                        break;
                    case RequiredType.Custom:
                        line = this.GetRequiredCustom();
                        break;
                    default:
                        break;
                }
            }
            return line;
        }
        private string GetLength()
        {
            StringBuilder line = new StringBuilder();
            if (this.IsLengthApplicable)
            {
                string minMaxMsg = "";
                if (this.Minlength > 0)
                {
                    line.Append(string.Format("data-val-length-min ='{0}' ", this.Minlength));
                    //this.htmlString.Append(line);
                    //minMaxMsg = string.Format("The field {0} must be a string with a minimum length of {1}", this.GetDisplayName(), this.Minlength);
                    minMaxMsg = string.Format("Please enter the currect  {0} ", this.GetDisplayName());

                }
                if (this.Maxlength > 0)
                {
                    line.Append(string.Format("data-val-length-max ='{0}' ", this.Maxlength));
                    //this.htmlString.Append(line);
                    minMaxMsg = string.Format("Please enter the currect {0}", this.GetDisplayName(), this.Maxlength);
                    //  minMaxMsg = string.Format("The field {0} must be a string with a maximum length of {1}", this.GetDisplayName(), this.Maxlength);
                }
                if (this.Minlength > 0 && this.Maxlength > 0)
                {
                    // minMaxMsg = string.Format("The field {0} must be a string with a minimum length of {1} and a maximum length of {2}", this.GetDisplayName(), this.Minlength, this.Maxlength);
                    minMaxMsg = string.Format("Please enter the currect {0} ", this.GetDisplayName());
                }
                if (!string.IsNullOrEmpty(minMaxMsg))
                {
                    line.Append(string.Format("data-val-length ='{0}' ", minMaxMsg));
                    //this.htmlString.Append(line);
                }
            }
            return Convert.ToString(line);
        }
        private string GetRegEx()
        {
            StringBuilder line = new StringBuilder();
            if (this.IsRegExApplicable)
            {
                if (!string.IsNullOrEmpty(this.Regex))
                {
                    line.Append("data-val-regex-pattern ='" + this.Regex + "' ");
                    // line.Append("data-val-regex ='The field " + this.GetDisplayName() + " must match the regular expression " + this.Regex + "' ");
                    line.Append("data-val-regex ='Please enter the currect " + this.GetDisplayName() + " ' ");
                }
            }
            return Convert.ToString(line);
        }
        private string GetAttr()
        {
            StringBuilder line = new StringBuilder();
            if (this.ControlType == Models.ControlType.radio)
            {
                line.Append("");
            }
            else
            {
                foreach (var item in this.Attr)
                {
                    if (!string.IsNullOrEmpty(item.Key))
                    {
                        line.Append(string.Format("{0} =\"{1}\" ", item.Key, item.Value));
                    }
                }
            }
            //this.htmlString.Append(line);
            return (line.ToString());
        }
        private string GetServerTasks()
        {
            string lines = "";
            if (this.ServerTasks != null)
            {
                foreach (var item in this.ServerTasks)
                {
                    lines += this.GetServerTask(item);
                }
            }
            return lines;
        }
        private string GetServerTask(ServerTask item)
        {
            string lines = "";
            string line = "";
            StringBuilder temp = new StringBuilder();
            Js = new StringBuilder();
            if (item == null) return lines;
            string taskType = "";
            string async = BaseUtil.LowerCase(item.Async);
            string skipValidation = BaseUtil.LowerCase(item.SkipValidation);

            string caption = this.GetDisplayName();
            string mimeType = EnumUtil.ParseName<MimeType>(item.MimeType).ToLower();
            switch (item.ServerAction)
            {
                case ServersideAction.Casecade:
                    taskType = "casecade";
                    temp.Append("data-is-casecade='true' ");
                    temp.Append("data-load-dropdown='true' ");

                    temp.Append(string.Format("data-on-change-ids-{0}='{1}' ", taskType, item.OnChangeIds));
                    temp.Append(string.Format("data-on-click-ids-{0}='{1}' ", taskType, item.OnClickIds));
                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));

                    temp.Append(string.Format("data-load-in-client-{0}='{1}' ", taskType, item.LoadInClient));
                    line = temp.ToString();
                    break;
                case ServersideAction.IsExist:
                    taskType = "is-exist";
                    temp.Append("data-val-isExist='true' ");

                    temp.Append(string.Format("data-on-change-ids-{0}='{1}' ", taskType, item.OnChangeIds));
                    temp.Append(string.Format("data-on-click-ids-{0}='{1}' ", taskType, item.OnClickIds));
                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));

                    temp.Append(string.Format("data-caption='{0}' ", caption));
                    line = temp.ToString();
                    break;
                case ServersideAction.LoadParitally:
                    taskType = "load-partially";
                    temp.Append("data-load-partially='true' ");

                    temp.Append(string.Format("data-on-change-ids-{0}='{1}' ", taskType, item.OnChangeIds));
                    temp.Append(string.Format("data-on-click-ids-{0}='{1}' ", taskType, item.OnClickIds));
                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));

                    temp.Append(string.Format("data-load-in-client-{0}='{1}' ", taskType, item.LoadInClient));
                    line = temp.ToString();
                    break;
                case ServersideAction.LoadGraph:
                    taskType = "load-graph";
                    temp.Append("data-is-graph='true' ");
                    temp.Append("data-load-graph='true' ");

                    temp.Append(string.Format("data-on-change-ids-{0}='{1}' ", taskType, item.OnChangeIds));
                    temp.Append(string.Format("data-on-click-ids-{0}='{1}' ", taskType, item.OnClickIds));
                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));

                    temp.Append(string.Format("data-load-in-client-{0}='{1}' ", taskType, item.LoadInClient));
                    line = temp.ToString();
                    break;
                case ServersideAction.LoadTree:
                    taskType = "load-tree";
                    temp.Append("data-is-tree='true' ");
                    temp.Append("data-load-tree='true' ");

                    temp.Append(string.Format("data-on-change-ids-{0}='{1}' ", taskType, item.OnChangeIds));
                    temp.Append(string.Format("data-on-click-ids-{0}='{1}' ", taskType, item.OnClickIds));
                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));

                    temp.Append(string.Format("data-load-in-client-{0}='{1}' ", taskType, item.LoadInClient));
                    line = temp.ToString();
                    break;
                case ServersideAction.GoTo:
                    taskType = "goto";
                    temp.Append("data-click-type='goto' ");
                    temp.Append("onclick='return formUtil.buttonClickById(this.id);' ");

                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));
                    line = temp.ToString();
                    break;
                case ServersideAction.Post:
                    taskType = "post";
                    temp.Append("data-click-type='post' ");
                    temp.Append("onclick='return formUtil.buttonClickById(this.id);' ");

                    temp.Append(string.Format("data-skip-validation-{0}='{1}' ", taskType, skipValidation));
                    temp.Append(string.Format("data-on-change-ids-{0}='{1}' ", taskType, item.OnChangeIds));
                    temp.Append(string.Format("data-on-click-ids-{0}='{1}' ", taskType, item.OnClickIds));
                    temp.Append(string.Format("data-clear-data-ids-{0}='{1}' ", taskType, item.ClearDataIds));

                    temp.Append(string.Format("data-ajax-in-json-name-{0}='{1}' ", taskType, item.AjaxInJsonName));
                    temp.Append(string.Format("data-ajax-return-type-{0}='{1}' ", taskType, mimeType));
                    line = temp.ToString();
                    break;
                default:
                    break;
            }

            temp.Append(string.Format("data-task-type-{0}='{0}' ", taskType));
            temp.Append(string.Format("data-async-{0}='{1}' ", taskType, async));
            temp.Append(string.Format("data-controller-name-{0}='{1}' ", taskType, item.ControllerName));
            temp.Append(string.Format("data-action-name-{0}='{1}' ", taskType, item.ActionName));
            temp.Append(string.Format("data-parent-id-{0}='{1}' ", taskType, item.ParentId));
            temp.Append(string.Format("data-parm-id-{0}='{1}' ", taskType, item.ParmId));

            line += temp.ToString();
            lines += line;
            return lines;
        }
        private string GetCustomValidation()
        {
            StringBuilder line = new StringBuilder();
            if (string.IsNullOrEmpty(this.Message))
            {
                //do nothing..
                //return lines;
            }
            else
            {
                line.Append(string.Format("data-val='true' "));
                line.Append(string.Format("data-val-cv='true' "));
                line.Append(string.Format("data-val-cv-message='{0}' ", this.Message));
            }
            return line.ToString();
        }
        private string End()
        {
            StringBuilder line = new StringBuilder();

            switch (this.EditorType)
            {
                case EditorType.Input:
                    if (ControlType == ControlType.checkbox)
                    {
                        line.Append(">");
                        //line.Append(string.Format("><input name='{0}' type='hidden' value='false'>", this.GetEditorId()));
                    }
                    else if (ControlType == ControlType.radio)
                    {
                        //do nothing
                    }
                    else
                    {
                        line.Append("/>");
                    }
                    break;
                case EditorType.Select:
                    line.Append(">");
                    if (!this.IsMultiSelect)
                    {
                        line.Append(string.Format(" <option value={0} >{1} </option>", "", string.IsNullOrEmpty(this.DefaultText) ? "Please Select" : this.DefaultText));
                    }
                    if (this.List != null)
                    {
                        foreach (var item in this.List)
                        {
                            string isSelected = item.Value.ToLower() == Convert.ToString(this.Value).ToLower() ? "selected" : "";
                            line.Append(string.Format(" <option value={0} {1}>{2} </option>", item.Value, isSelected, item.Text));
                        }
                    }
                    line.Append("</select>");
                    break;
                case EditorType.Textarea:
                    line.Append(">" + (string.IsNullOrEmpty(Convert.ToString(this.Value)) ? "" : this.Value) + "</textarea>");
                    break;
                case EditorType.Button:
                    line.Append(">" + (string.IsNullOrEmpty(Convert.ToString(this.Value)) ? "" : this.Value) + "</button>");
                    break;
                case EditorType.Div:
                    line.Append("></div>");
                    break;
                case EditorType.Image:
                    line.Append(">");
                    break;
                case EditorType.File:
                    break;
                case EditorType.DropdownMenu:
                    break;
                case EditorType.MultiSelect:
                    break;
                case EditorType.AutoComplete:
                    break;
                case EditorType.Custom:
                    break;
                default:
                    break;
            }
            //this.htmlString.Append(line);
            bool isMsg = false;
            if (this.IncludeValidationMsg)
            {
                if (this.RequiredType == RequiredType.Standard)
                {
                    if (this.IsRequired)
                    {
                        isMsg = true;
                    }
                }
                if (!string.IsNullOrEmpty(this.Message))
                {
                    isMsg = true;
                }
            }
            if (isMsg)
            {
                line.Append("<span class ='field-validation-valid' ");
                line.Append("data-valmsg-for ='" + this.Id + "' ");
                line.Append("data-valmsg-replace ='true' ");
                line.Append("></span>");
            }


            return Convert.ToString(line);
        }
        private string GetJs()
        {
            if (this.ControlType == Models.ControlType.graph)
            {
                var config = JsonConvert.SerializeObject((this.Graph), Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = BaseConst.DATE });
                this.Js.Append("<script>\r\n");
                this.Js.Append(string.Format("{0}Config = {1}", this.GetEditorId(), config));
                this.Js.Append("</script>\r\n");
            }
            string line = Convert.ToString(this.Js);
            return (string.IsNullOrEmpty(line) ? "" : line);
        }
        private MvcHtmlString GetHtmlStandard()
        {
            this.htmlString.Append(Start());
            this.htmlString.Append(GetValue());
            this.htmlString.Append(GetRequired());
            this.htmlString.Append(GetLength());
            this.htmlString.Append(GetRegEx());
            this.htmlString.Append(GetServerTasks());
            this.htmlString.Append(GetCustomValidation());
            this.htmlString.Append(GetAttr());
            this.htmlString.Append(End());
            this.htmlString.Append(GetJs());

            if (this.IsEditorType())
            {
                if (this.IncludeLabel)
                {
                    string editorClass = string.IsNullOrEmpty(this.Glyphicon) ? "col-md-10" : string.Format("col-md-10 {0}-inner-addon", this.GlyphiconAlignment == Alignment.Left ? "left" : "right");
                    string editor = string.IsNullOrEmpty(this.Glyphicon) ? this.htmlString.ToString() : string.Format("<i class='{0}'></i>{1}", this.Glyphicon, this.htmlString.ToString());
                    string label = string.IsNullOrEmpty(this.About) ? this.GetDisplayName() : string.Format("{0}&nbsp;<span class='glyphicon glyphicon-question-sign' title='{1}'></span>", this.GetDisplayName(), this.About);
                    string str = "<div class='form-group'><label class='col-md-2 control-label' for='{0}' data-orig-caption='{1}'>{2}</label><div class='{3}'>{4}</div></div>";
                    this.htmlString = new StringBuilder(string.Format(str, this.GetEditorId(), this.GetDisplayName(), label, editorClass, editor));
                }
                else
                {
                    //preceded by <div col-md-10>editor</div>
                    string editor = string.IsNullOrEmpty(this.Glyphicon) ? this.htmlString.ToString() : string.Format("<i class='{0}'></i>{1}", this.Glyphicon, this.htmlString.ToString());
                    this.htmlString = new StringBuilder(editor);
                }
            }
            else if (this.ControlType == ControlType.graph)
            {
                StringBuilder sb = new StringBuilder();
                string className = string.IsNullOrEmpty(this.Graph.className) ? "col-md-6" : this.Graph.className;
                sb.Append("<div class='" + className + "'>");
                sb.Append("     <div class='panel panel-success'>");
                sb.Append("         <div class='panel-heading'>");
                sb.Append("             <h4 class='panel-title'>" + this.Graph.title + "</h4>");
                sb.Append("         </div>");
                sb.Append("         <div class='panel-body '>");
                sb.Append("             <ul class='list-group' >");
                sb.Append("                 " + this.htmlString.ToString());
                sb.Append("             </ul>");
                sb.Append("         </div>");
                sb.Append("     </div>");
                sb.Append("</div>");
                this.htmlString = sb;
            }
            else if (this.ControlType == ControlType.menu)
            {
                StringBuilder sb = new StringBuilder();
                if (this.Tree != null && this.Tree.List.Count > 0)
                {
                    List<TreeNode> listParent = this.Tree.List.Where(tn => string.IsNullOrEmpty(tn.ParentId)).ToList();

                    sb.Append("<nav id='leftNav' class='navbar-aside navbar-static-side no-print' role='navigation'>");
                    sb.Append("	<div class='sidebar-collapse nano'>");
                    sb.Append("		<div class='nano-content'>");
                    sb.Append("			<ul class='nav metismenu' id='side-menu'>");
                    foreach (var p in listParent)
                    {
                        List<TreeNode> listChild = this.Tree.List.Where(tn => tn.ParentId == p.Id).ToList();
                        string end = listChild.Count() > 0 ? "</span><span class='fa arrow'></span></a>" : "</span></a></li>";
                        string url = string.IsNullOrEmpty(p.Url) ? "#" : p.Url;
                        sb.Append("         <li id='parent_" + p.Id + "'>");
                        sb.Append("			    <a id='parent_" + p.Id + "_a' href='" + url + "'><i class='" + p.Icon + "'></i> <span class='nav-label'>" + p.Name + end);
                        foreach (var c in listChild)
                        {
                            List<TreeNode> listChildOfChild = this.Tree.List.Where(tn => tn.ParentId == c.Id).ToList();
                            end = listChildOfChild.Count() > 0 ? "</span><span class='fa arrow'></span></a>" : "</span></a></li>";
                            sb.Append("         <ul class='nav nav-second-level collapse'>");
                            //sb.Append("			    <li><a href='#' data-url='" + c.Url + "'><i class='" + c.Icon + "'></i> <span class='nav-label'>" + c.Name + end);#0e96ec
                            sb.Append("             <li id='child_" + c.Id + "'>");
                            sb.Append("			    <a id='child_" + c.Id + "_a' href='" + c.Url + "'><i class='" + c.Icon + "'></i> <span class='nav-label'>" + c.Name + end);
                            foreach (var cc in listChildOfChild)
                            {
                                sb.Append("         <ul class='nav nav-third-level collapse'>");
                                sb.Append("             <li id='childOfChild_" + cc.Id + "'>");
                                sb.Append("			    <a id='childOfChild_" + cc.Id + "_a' href='" + cc.Url + "'><i class='" + cc.Icon + "'></i> <span class='nav-label'>" + cc.Name + "</span></a></li>");
                                sb.Append("			</ul>");
                            }
                            sb.Append("			</ul>");
                        }
                        sb.Append("			</li>");
                    }
                    sb.Append("			</ul>");
                    sb.Append("		</div>");
                    sb.Append("	</div>");
                    sb.Append("</nav>");
                }
                this.htmlString = sb;
            }
            else if (this.ControlType == ControlType.menu_horizontal)
            {
                StringBuilder sb = new StringBuilder();
                if (this.Tree != null && this.Tree.List.Count > 0)
                {
                    List<TreeNode> listParent = this.Tree.List.Where(tn => string.IsNullOrEmpty(tn.ParentId)).ToList();

                    sb.Append("<nav id='leftNav' class='navbar navbar-default no-print'>");
                    sb.Append("	<div class='container'>");
                    sb.Append("		 <div class='navbar-header'> <button type='button' class='navbar-toggle collapsed' data-toggle='collapse' data-target='#navbar-main' aria-expanded='false' aria-controls='navbar'>   <span class='sr-only'>Toggle navigation</span> <span class='icon-bar'></span> <span class='icon-bar'></span> <span class='icon-bar'></span> </button> </div>");
                    sb.Append("		 <div id='navbar-main' class='navbar-collapse collapse'> ");
                    sb.Append("			<ul class='nav navbar-nav'>");
                    foreach (var p in listParent)
                    {
                        List<TreeNode> listChild = this.Tree.List.Where(tn => tn.ParentId == p.Id).ToList();
                        string url = string.IsNullOrEmpty(p.Url) ? "#" : p.Url;
                        if (listChild.Count() > 0)
                        {
                            sb.Append("<li class='dropdown'> <a href='#' class='dropdown-toggle' data-toggle='dropdown' role='button' aria-haspopup='true' aria-expanded='false'> " + p.Name + "<span class='caret'></span></a>");
                            sb.Append(" <ul class='dropdown-menu'>");
                            foreach (var c in listChild)
                            {
                                sb.Append("<li id='child_" + p.Id + "'><a id='child_" + c.Id + "_a' href='" + c.Url + "'>" + c.Name + "</a></li>");

                                List<TreeNode> listChildOfChild = this.Tree.List.Where(tn => tn.ParentId == c.Id).ToList();

                                foreach (var cc in listChildOfChild)
                                {

                                }
                            }

                            sb.Append(" </ul>");
                            sb.Append(" </li>");

                        }
                        else
                        {
                            sb.Append("         <li id='parent_" + p.Id + "'><a id='parent_" + p.Id + "_a' href='" + url + "'>" + p.Name + "</a></li>");

                        }


                        //string end = listChild.Count() > 0 ? "</a></li>" : "</a></li>";
                        //string url = string.IsNullOrEmpty(p.Url) ? "#" : p.Url;
                        //sb.Append("         <li id='parent_" + p.Id + "'>");
                        //sb.Append("			    <a id='parent_" + p.Id + "_a' href='" + url + "'>" + p.Name + end);
                        //foreach (var c in listChild)
                        //{
                        //    List<TreeNode> listChildOfChild = this.Tree.List.Where(tn => tn.ParentId == c.Id).ToList();
                        //    end = listChildOfChild.Count() > 0 ? "</span><span class='fa arrow'></span></a>" : "</span></a></li>";
                        //    sb.Append("         <ul class='nav nav-second-level collapse'>");
                        //    //sb.Append("			    <li><a href='#' data-url='" + c.Url + "'><i class='" + c.Icon + "'></i> <span class='nav-label'>" + c.Name + end);#0e96ec
                        //    sb.Append("             <li id='child_" + c.Id + "'>");
                        //    sb.Append("			    <a id='child_" + c.Id + "_a' href='" + c.Url + "'><i class='" + c.Icon + "'></i> <span class='nav-label'>" + c.Name + end);
                        //    foreach (var cc in listChildOfChild)
                        //    {
                        //        sb.Append("         <ul class='nav nav-third-level collapse'>");
                        //        sb.Append("             <li id='childOfChild_" + cc.Id + "'>");
                        //        sb.Append("			    <a id='childOfChild_" + cc.Id + "_a' href='" + cc.Url + "'><i class='" + cc.Icon + "'></i> <span class='nav-label'>" + cc.Name + "</span></a></li>");
                        //        sb.Append("			</ul>");
                        //    }
                        //    sb.Append("			</ul>");
                        //}
                        //sb.Append("			</li>");
                    }
                    sb.Append("			</ul>");
                    // sb.Append("<ul class='nav navbar-nav navbar-right'> <li> <form class='mainnav-form' role='search'> <input type='text' class='form-control input-md' placeholder='Search'> <button class='btn btn-sm mainnav-form-btn' type='button'><i class='fa fa-search''></i></button> </form> </li> </ul>");
                    sb.Append("		</div>");
                    sb.Append("	</div>");
                    sb.Append("</nav>");
                }
                this.htmlString = sb;
            }
            return new MvcHtmlString(Convert.ToString(this.htmlString));
        }
        private MvcHtmlString GetHtmlFile()
        {
            UrlHelper url = this.Url;
            if (!String.IsNullOrEmpty(this.Id))
            {
                string allowFileTypes = "";
                for (int i = 0; i < this.FileTypes.Length; i++)
                {
                    allowFileTypes += this.FileTypes.GetValue(i) + ((this.FileTypes.Length == 1 || i == this.FileTypes.Length - 1) ? "" : ", ");
                }
                StringBuilder fileTag = new StringBuilder();
                string msRef = url.Content("~/Base/Content/js/fileUtil.js");
                fileTag.Append("<script src='" + msRef + "' type='text/javascript'></script>");
                fileTag.Append("<input type='file' multiple id='" + this.Id + "' ");
                fileTag.Append(" name ='" + this.Id + "' ");
                if (!string.IsNullOrEmpty(Convert.ToString(this.Value)))
                {
                    fileTag.Append(" value='" + this.Value + "' ");
                }
                fileTag.Append("data-file-types='" + allowFileTypes + "' ");
                fileTag.Append("data-file-min-size='" + this.MinFileSize + "' ");
                fileTag.Append("data-file-max-size='" + this.MaxFileSize + "' ");
                fileTag.Append("onchange= 'fileUtil.fileUploadChanged(this);' ");

                #region add style border-left: 2px solid red
                StringBuilder line = new StringBuilder();
                string styleName = string.IsNullOrEmpty(this.ReadOnlyProp) ? "" : " background-color: lightgray;cursor: no-drop; ";
                styleName += this.IsRequired ? "border-left: 2px solid red; " : "";
                //styleName = string.IsNullOrEmpty(styleName) ? "" : string.Format("style='{0}' ", styleName.Trim());
                Dictionary<string, string> requiredAttr = new Dictionary<string, string>();
                requiredAttr.Add("style", styleName);
                this.SetRequireAttr(requiredAttr);
                #endregion

                fileTag.Append(this.GetRequired());
                //if (this.BaseValidation != null)
                //{
                //    fileTag.Append(this.BaseValidation.GetValidationString());
                //}
                fileTag.Append(this.GetAttr());
                fileTag.Append("></input>");

                var imgdelete = url.Content(string.Format("~/Base/Content/images/{0}.png", "cross-delete"));
                var imgDelClick = "fileUtil.imageDeleteClick(this);";
                fileTag.Append("<img id='imgDel" + this.Id + "' style='width:20px;height:20px;display:none;' src='" + imgdelete + "' onclick='" + imgDelClick + "'> </img>");
                string fileImg = "";
                if (!string.IsNullOrEmpty(this.FileName))
                {
                    string fileType = this.FileName.Split('.')[this.FileName.Split('.').Length - 1];
                    if (!string.IsNullOrEmpty(this.FileName))
                    {
                        if (FileUtil.GetIsImgType(fileType))
                        {
                            fileImg = url.Content(string.Format("~/Files/Images/{0}", this.FileName));
                        }
                        else
                        {
                            fileImg = url.Content(string.Format("~/Base/Content/images/{0}.png", fileType));
                        }
                        fileTag.Append("<br/><img id='img" + this.Id + "' style='width:50px;height:50px;' src='" + fileImg + "' </img>");
                    }
                }
                return MvcHtmlString.Create(fileTag.ToString());
            }
            return null;
        }
        private MvcHtmlString GetHtmlMultiSelect()
        {
            UrlHelper url = this.Url;
            if (!String.IsNullOrEmpty(this.Id))
            {
                StringBuilder MultiselectTag = new StringBuilder();
                string msId = string.Format(this.Id);
                string hdnId = string.Format("hdn{0}", this.Id);
                string hdnVal = string.Format("txt{0}", this.Id);
                string divMsId = string.Format("divMs{0}", this.Id);
                string chkAllId = string.Format("chkAll{0}", this.Id);
                //string cssRef = string.Format("../Content/multiSelect.css");
                //string msRef = string.Format("../Scripts/baseMultiSelect.js");

                string cssRef = string.Format("../Base/Content/css/multiSelect.css");
                string msRef = string.Format("../Base/Content/js/baseMultiSelect.js");

                HtmlString valString = new HtmlString(this.GetAttr());
                //if (this.BaseValidation != null)
                //{
                //    this.BaseValidation.ValidationId = hdnId;
                //    valString = this.BaseValidation.GetValidationString();
                //}
                MultiselectTag.Append(valString);

                MultiselectTag.Append("<link href='" + cssRef + "' rel='stylesheet' />");
                MultiselectTag.Append("<script src='" + msRef + "'></script>");

                MultiselectTag.Append("<script type='text/javascript'>");
                MultiselectTag.Append("$(document).ready(function () { var settings = {};");
                MultiselectTag.Append("settings = { msId: '" + msId + "', caption: '" + this.GetDisplayName() + "', selectedValues: '" + this.Value + "'};");
                MultiselectTag.Append("multiSelectUtil.init(settings)");
                MultiselectTag.Append("});</script>");
                MultiselectTag.Append("<input type='hidden' id='" + hdnId + "' name='" + hdnId + "' />");
                MultiselectTag.Append("<input type='hidden' id='" + hdnVal + "' name='" + hdnVal + "' />");
                MultiselectTag.Append("<dl id='" + divMsId + "' class='dropdown' " + valString + " >");
                MultiselectTag.Append("<dt>");
                MultiselectTag.Append("<a href='#'>");
                MultiselectTag.Append("<p class='multiSel'></p>");
                MultiselectTag.Append("</a>");
                MultiselectTag.Append("</dt>");
                MultiselectTag.Append("<dd>");
                MultiselectTag.Append("<div class='mutliSelect'>");
                MultiselectTag.Append("<ul>");
                if (this.List != null)
                {
                    MultiselectTag.Append("<li>");
                    MultiselectTag.Append("<input type='checkbox' value='Select All' id='" + chkAllId + "' class='chkclass' />Select All");
                    MultiselectTag.Append("</li>");
                    foreach (var item in this.List)
                    {
                        string itemId = string.Format("chk{0}_{1}", msId, item.Value.Trim());
                        MultiselectTag.Append("<li>");
                        MultiselectTag.Append("<input type='checkbox' value='" + item.Text + "' id='" + itemId + "' class='chkclass' />" + item.Text);
                        MultiselectTag.Append("</li> ");

                    }
                }
                MultiselectTag.Append("</ul>");
                MultiselectTag.Append("</div>");
                MultiselectTag.Append("</dd>");
                MultiselectTag.Append("</dl>");
                return MvcHtmlString.Create(MultiselectTag.ToString());
            }
            return null;
        }
        private MvcHtmlString GetHtmlAutoComplete()
        {
            UrlHelper url = this.Url;
            //<input id="txtJobSkill" name="txtJobSkill" type="text" placeholder="Skills" class="job" data-auto-html-id="hdnskillId" data-auto-db-id="skill_set_id" data-auto-db-val="skill_set_name">

            if (!String.IsNullOrEmpty(this.Id))
            {
                StringBuilder inputTag = new StringBuilder();
                string txtId = string.Format("txt{0}", this.Id);
                string hdnId = string.Format("hdn{0}", this.Id);
                inputTag.Append("<input type='text' id='" + txtId + "' name='" + txtId + "'");

                inputTag.Append(" value='" + this.Value + "' ");
                inputTag.Append(this.GetAttr());

                //inputTag.Append(" data-auto-html-id='" + hdnId + "'");

                inputTag.Append(" data-auto-controller-name='" + this.ControllerName + "'");
                inputTag.Append(" data-auto-action-name='" + this.ActionName + "'");
                inputTag.Append(" data-auto-parm-id='" + this.ParmId + "'");
                inputTag.Append(" data-auto-fillCtrl-id='" + this.fillCtrlId + "'");
                inputTag.Append(" data-auto-isValid='" + this.isValid + "'");
                inputTag.Append(" data-auto-db-id='" + this.DbId + "'");
                inputTag.Append(" data-auto-db-val='" + this.dbValue + "'");
                if (!string.IsNullOrEmpty(this.JsOnSelect))
                {
                    inputTag.Append(" data-auto-js-onselect='" + this.JsOnSelect + "'");
                }

                inputTag.Append(this.GetAttr());
                if (this.Mode == AutoCompleteMode.AutoCompleteWithEdit)
                {
                    inputTag.Append(" data-auto-with-edit='true'");
                }
                else
                {
                    inputTag.Append(" data-auto-with-edit='false'");
                }
                inputTag.Append(" ></input>");
                inputTag.Append(" <input type='hidden' id='" + hdnId + "' name='" + hdnId + "'");
                inputTag.Append(" ></input>");
                return MvcHtmlString.Create(inputTag.ToString());
            }
            return null;
        }
        private MvcHtmlString GetHtmlDropdownMenu()
        {
            UrlHelper url = this.Url;
            if (!String.IsNullOrEmpty(this.Id) && this.ServerTasks != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("	<div class='input-group-btn'>");
                sb.Append("		<button data-toggle='dropdown' class='btn btn-primary btn-xs dropdown-toggle' type='button' aria-expanded='false'>" + this.Value + "<span class='caret'></span></button>");
                sb.Append("		<ul class='dropdown-menu pull-right'>");
                foreach (var item in this.ServerTasks)
                {
                    sb.Append(string.Format("			<li><a href='#' type='button' id='{0}' name='{0}' {1} >{2}</a></li>", item.Id, this.GetServerTask(item), item.Name));
                }
                sb.Append("		</ul>");
                sb.Append("	</div>");
                return MvcHtmlString.Create(sb.ToString());
            }
            return null;
        }

        #endregion

        public MvcHtmlString
            GetHtml()
        {
            MvcHtmlString html = null;
            if (!String.IsNullOrEmpty(this.Id))
            {
                this.Init();
                bool IsStandard = true;
                switch (this.EditorType)
                {
                    case EditorType.Input:
                        break;
                    case EditorType.Button:
                        break;
                    case EditorType.Select:
                        break;
                    case EditorType.Textarea:
                        break;
                    case EditorType.Image:
                        break;
                    case EditorType.File:
                        IsStandard = false;
                        this.htmlString.Append(this.GetHtmlFile());
                        break;
                    case EditorType.DropdownMenu:
                        IsStandard = false;
                        this.htmlString.Append(this.GetHtmlDropdownMenu());
                        break;
                    case EditorType.MultiSelect:
                        IsStandard = false;
                        this.htmlString.Append(this.GetHtmlMultiSelect());
                        break;
                    case EditorType.AutoComplete:
                        IsStandard = false;
                        this.htmlString.Append(this.GetHtmlAutoComplete());
                        break;
                    case EditorType.Custom:
                        IsStandard = false;
                        break;
                    default:
                        break;
                }
                if (IsStandard)
                {
                    html = GetHtmlStandard();
                }
                html = new MvcHtmlString(Convert.ToString(this.htmlString));
            }
            return html;
        }

    }
    public enum FileType
    {
        PDF,
        DOC,
        DOCX,
        TXT,
        CSV,
        XLS,
        XLSX,
        JPG,
        PNG,
        BMP,
        dat,
        JPEG,
        GIF
    }
    public static class FileUtil
    {
        public static bool GetIsImgType(string type)
        {
            bool isImgType = false;
            FileType ft = EnumUtil.ParseEnum<FileType>(type);
            switch (ft)
            {
                case FileType.PDF:
                    break;
                case FileType.DOC:
                    break;
                case FileType.TXT:
                    break;
                case FileType.CSV:
                    break;
                case FileType.XLS:
                    break;
                case FileType.XLSX:
                    break;
                case FileType.JPG:
                    isImgType = true;
                    break;
                case FileType.PNG:
                    isImgType = true;
                    break;
                case FileType.BMP:
                    isImgType = true;
                    break;
                case FileType.dat:
                    isImgType = true;
                    break;
                default:
                    break;
            }
            return isImgType;
        }
    }
    public class RegExConst
    {
        public const string LoginID = "[a-zA-Z0-9_-]{4,20}";
        public const string Password = "^.{4,12}$";
        public const string General = @"^[ A-Za-z0-9_@./,#&+-]{1,500}";
        public const string GeneralWithOutSpace = @"^[A-Za-z0-9_@./,#&+-]{4,500}";
        public const string GeneralWithSpace = @"^[ A-Za-z0-9_@./,#&+-]{4,500}";

        public const string CharacterAndSpaceOnly = "^[a-zA-Z ]*$";
        public const string CharactersAndParanthisisOnly = "^[a-zA-Z ()'.-]+$";
        public const string CharactersParanthisisAndSpecialCharOnly = "^[a-zA-Z() ]*$";
        public const string CharactersAndCommaOnly = "^[a-zA-Z, ]{2,}$";
        public const string CharactersOnly = "^[a-zA-Z]*$";

        public const string NumbersOnly = "^[0-9-+]+$";
        public const string NumbersAnddecimalOnly = "^[0-9]{0,6}(.[0-9]{1,2})?$";
        public const string NumbersWithPlusOnly = "^[0-9+/]+$";
        public const string NumbersWithPlusAndSpaceOnly = "^[0-9+/ ]+$";
        public const string AlphaNumericOnly = "^[a-zA-Z0-9]*$";
        public const string AlphaNumericAndSpaceOnly = "^[a-z A-Z 0-9 ]*$";//"(?=.*\\S)[a-z A-Z 0-9 \\s]*"; ////////// need to check
        public const string IntegerGreaterthanZero = "^[1-9][0-9]*$";
        public const string DecimalGreaterthanZero = "^\\s*(?=.*[1-9])\\d*(?:\\.\\d{1,2})?\\s*$";

        public const string EmailAddress = "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$";
        public const string EmailAddressMulti = @"^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[,]{0,1}\s*)+$";
        public const string WebAddress = @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9]\.[^\s]{2,})";
        public const string IpAddress = @"^([\d]{1,3}\.){3}[\d]{1,3}$";

        public const string SkypeId = @"[a-zA-Z][a-zA-Z0-9\.,\-_]{5,31}";



    }
    public static class HtmlHelperNameSpaces
    {
        public static HelperExtensionDmtFactory Dmt(this HtmlHelper helper)
        {
            return new HelperExtensionDmtFactory(helper);
        }
    }
    public class HelperExtensionDmtFactory
    {
        private HtmlHelper HtmlHelper { get; set; }
        public HelperExtensionDmtFactory(HtmlHelper htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }
        public MvcHtmlString HtmlFor(HtmlModel hm)
        {
            return hm.GetHtml();
        }
        //public MvcHtmlString HtmlFor(USP_GetProductAttr_Result emDb)
        //{
        //    HtmlModel hm = new HtmlModel();
        //    hm.Id = string.Format("attr_{0}", emDb.attribute_id);
        //    hm.DisplayName = emDb.name;
        //    hm.Value = emDb.productDetailValue;
        //    //hm.PlaceHolder = emDb.name;
        //    hm.Regex = emDb.regExp;
        //    hm.IsRequired = emDb.deis_required;

        //    ControlType ct = EnumUtil.ParseEnum<ControlType>(emDb.controlType.ToLower());
        //    hm.ControlType = ct;
        //    switch (ct)
        //    {
        //        case ControlType.select:
        //            //select specific
        //            hm.List = AttributeUtil.getAttributeData(emDb.attribute_id).ToList();
        //            break;
        //        case ControlType.text:
        //            hm.Minlength = emDb.de_regex_min_length;
        //            hm.Maxlength = emDb.de_regex_max_length;
        //            //text specific
        //            break;
        //        case ControlType.textarea:
        //            hm.Minlength = emDb.de_regex_min_length;
        //            hm.Maxlength = emDb.de_regex_max_length;
        //            //textarea specific
        //            break;
        //    }

        //    return HtmlHelper.Dmt().HtmlFor(hm);
        //}
    }

    public static class HtmlHelpers
    {
        //public static MvcHtmlString HtmlFor(this HtmlHelper helper, HtmlModel hm)
        //{
        //    return hm.GetHtml();
        //}

        #region Get Something Out Of It
        public static MvcHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, object routeValues, int width, int height)
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);

            if (!String.IsNullOrEmpty(imageUrl))
            {
                TagBuilder imgBuilder = new TagBuilder("img");
                imgBuilder.MergeAttribute("src", Url.Content("~/Content/images/" + imageUrl));

                imgBuilder.MergeAttribute("alt", altText);
                imgBuilder.MergeAttribute("width", width.ToString());
                imgBuilder.MergeAttribute("height", height.ToString());
                var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
                var linkBuilder = new TagBuilder("a");
                linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, routeValues));
                var text = linkBuilder.ToString(TagRenderMode.StartTag);
                text += imgBuilder.ToString(TagRenderMode.SelfClosing);
                text += linkBuilder.ToString(TagRenderMode.EndTag);
                return MvcHtmlString.Create(text);
            }
            return null;
        }
        public static MvcHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, string controllerName, object routeValues, int width, int height)
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);

            if (!String.IsNullOrEmpty(imageUrl))
            {
                TagBuilder imgBuilder = new TagBuilder("img");
                imgBuilder.MergeAttribute("src", Url.Content("~/Content/images/" + imageUrl));

                imgBuilder.MergeAttribute("alt", altText);
                imgBuilder.MergeAttribute("width", width.ToString());
                imgBuilder.MergeAttribute("height", height.ToString());
                var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
                var linkBuilder = new TagBuilder("a");
                linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routeValues));
                var text = linkBuilder.ToString(TagRenderMode.StartTag);
                text += imgBuilder.ToString(TagRenderMode.SelfClosing);
                text += linkBuilder.ToString(TagRenderMode.EndTag);
                return MvcHtmlString.Create(text);
            }
            return null;
        }
        public static MvcHtmlString ButtonSubmit(this HtmlHelper helper, string value)
        {
            //<input type="submit" value="Create" class="btn btn-md btn-primary" />
            var Url = new UrlHelper(helper.ViewContext.RequestContext);
            if (!String.IsNullOrEmpty(value))
            {
                TagBuilder inputTag = new TagBuilder("input");
                inputTag.MergeAttribute("type", "submit");
                inputTag.MergeAttribute("class", "btn btn-sm btn-primary");
                inputTag.MergeAttribute("value", value);
                return MvcHtmlString.Create(inputTag.ToString());
            }
            return null;
        }
        public static MvcHtmlString EditorForDateTime(this HtmlHelper helper, string dbName, DateTime? dateTime, bool isReadOnly = false)
        {
            //<input type="submit" value="Create" class="btn btn-sm btn-primary"  />
            var Url = new UrlHelper(helper.ViewContext.RequestContext);

            TagBuilder inputTag = new TagBuilder("input");
            inputTag.MergeAttribute("id", dbName);
            inputTag.MergeAttribute("name", dbName);
            inputTag.MergeAttribute("type", "datetime");
            inputTag.MergeAttribute("style", "text-align:left");
            inputTag.MergeAttribute("class", "btn btn-sm btn-primary");
            //inputTag.MergeAttribute("data-val", "true");
            //inputTag.MergeAttribute("data-val-required='The " + dbName + " field is required.' ", dbName); 
            if (isReadOnly)
            {
                inputTag.MergeAttribute("readonly", "readonly");
                inputTag.MergeAttribute("disabled", "");
            }
            if (dateTime != null)
            {
                inputTag.MergeAttribute("value", dateTime.Value.ToString(BaseConst.DATE));
            }
            return MvcHtmlString.Create(inputTag.ToString());
        }
        public static MvcHtmlString ButtonLink(this HtmlHelper helper, string value, string controllerName, string actionName, string id = null, string className = "btn btn-sm btn-primary")
        {
            //<input type="button" value="Back To List" class="btn btn-primary" onclick="base.goTo('User','Index')" />
            var Url = new UrlHelper(helper.ViewContext.RequestContext);
            if (!String.IsNullOrEmpty(value))
            {
                TagBuilder inputTag = new TagBuilder("input");
                inputTag.MergeAttribute("type", "button");
                inputTag.MergeAttribute("class", className);
                inputTag.MergeAttribute("value", value);
                inputTag.MergeAttribute("onclick", string.Format("base.goTo('{0}','{1}','{2}')", controllerName, actionName, id == null ? "" : id));
                return MvcHtmlString.Create(inputTag.ToString());
            }
            return null;
        }
        public static MvcHtmlString GetStyleSheet(this HtmlHelper helper, string fileName)
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);

            if (!String.IsNullOrEmpty(fileName))
            {
                TagBuilder linkTag = new TagBuilder("link");
                linkTag.MergeAttribute("href", Url.Content("~/Content/Css/" + fileName));
                linkTag.MergeAttribute("rel", "stylesheet");
                linkTag.MergeAttribute("type", "text/css");
                return MvcHtmlString.Create(linkTag.ToString());
            }
            return null;
        }
        public static MvcHtmlString GetStyleSheetMedia(this HtmlHelper helper, string fileName, string mediaName)
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);

            if (!String.IsNullOrEmpty(fileName))
            {
                TagBuilder linkTag = new TagBuilder("link");
                linkTag.MergeAttribute("href", Url.Content("~/Content/Css/" + fileName));
                linkTag.MergeAttribute("rel", "stylesheet");
                linkTag.MergeAttribute("type", "text/css");
                linkTag.MergeAttribute("media", mediaName);
                return MvcHtmlString.Create(linkTag.ToString());
            }
            return null;
        }
        public static MvcHtmlString GetJavaScript(this HtmlHelper helper, string fileName)
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);

            if (!String.IsNullOrEmpty(fileName))
            {
                TagBuilder scriptTag = new TagBuilder("script");
                scriptTag.MergeAttribute("src", Url.Content("~/Scripts/" + fileName));
                scriptTag.MergeAttribute("type", "text/javascript");
                return MvcHtmlString.Create(scriptTag.ToString());
            }
            return null;
        }
        public static MvcHtmlString RadioButtonForSelectList<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list 
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers 
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();

                    // Create the html string that will be returned to the client 
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat("<div class=\"RadioButton\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString WebPage(string serverPath)
        {
            var filePath = HttpContext.Current.Server.MapPath(serverPath);
            return MvcHtmlString.Create(new WebClient().DownloadString(filePath));
        }
        public static MvcHtmlString BreadCrumbs(this HtmlHelper helper, String glyphiconIconClass, string Menu, String SubMenu, String Headings = "")
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Headings))
            {
                Headings = SubMenu;
            }
            if (!String.IsNullOrEmpty(Menu))
            {
                sb.Append("<div class='row'><div class='col-sm-12'><div class='page-title'>");
                //sb.Append(" <h1>" + Headings + "<small></small></h1>");
                sb.Append("<ol class='breadcrumb'> <li><a href='#'><i class='" + glyphiconIconClass + "'> " + Menu + "</i></a></li><li class='active'>" + SubMenu + "</li></ol>");
                sb.Append("</div></div></div>");
                //sb.Append("<div class='breadcrumbs' id='breadcrumbs'>");
                //sb.Append("<script type='text/javascript'>");
                //sb.Append(" try { ace.settings.check('breadcrumbs', 'fixed') } catch (e) { }");
                //sb.Append("</script>");

                //sb.Append(" <ul class='breadcrumb'>");
                //sb.Append(" <li>");
                //sb.Append("<i class='" + glyphiconIconClass + "'></i>");

                //sb.Append("   <a href='#'>" + Menu + "</a>");



                //sb.Append(" </li>");
                //if (!String.IsNullOrEmpty(SubMenu))
                //{
                //    sb.Append(" <li class='active'>" + SubMenu + "</li>");
                //}
                //sb.Append(" </ul>");

                //sb.Append("</div>");



                return MvcHtmlString.Create(sb.ToString());
            }
            return null;
        }
        public static MvcHtmlString PageHading(this HtmlHelper helper, string Heading, string SubHeading)
        {
            var Url = new UrlHelper(helper.ViewContext.RequestContext);
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(Heading))
            {
                //sb.Append("<div class='page-content' >");
                //sb.Append(" <div class='page-header'>");
                //sb.Append(" <h1>");
                //sb.Append(Heading);
                //if (SubHeading != string.Empty)
                //{
                //    sb.Append("<small> <i class='ace-icon glyphicon1 glyphicon-chevron-right'></i><i class='ace-icon glyphicon1 glyphicon-chevron-right'></i> " + SubHeading + "</small>");
                //}
                //sb.Append(" </h1>");
                //sb.Append("</div></div>");
                return MvcHtmlString.Create(sb.ToString());
            }
            return null;
        }
        #endregion
    }
    #endregion

    #region Cookies
    public static class CookieStore
    {
        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            HttpCookie encodedCookie = HttpSecureCookie.Encode(new HttpCookie(key, value), CookieProtection.None);

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                // For security purpose, we need to encrypt the value.
                HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie, CookieProtection.None);
                value = decodedCookie.Value;
            }
            return value;
        }

    }

    public static class HttpSecureCookie
    {

        public static HttpCookie Encode(HttpCookie cookie)
        {
            return Encode(cookie, CookieProtection.All);
        }

        public static HttpCookie Encode(HttpCookie cookie,
                      CookieProtection cookieProtection)
        {
            HttpCookie encodedCookie = CloneCookie(cookie);
            encodedCookie.Value = MachineKeyCryptography.Encode(cookie.Value, cookieProtection);
            return encodedCookie;
        }

        public static HttpCookie Decode(HttpCookie cookie)
        {
            return Decode(cookie, CookieProtection.All);
        }

        public static HttpCookie Decode(HttpCookie cookie,
                      CookieProtection cookieProtection)
        {
            HttpCookie decodedCookie = CloneCookie(cookie);
            decodedCookie.Value =
              MachineKeyCryptography.Decode(cookie.Value, cookieProtection);
            return decodedCookie;
        }

        public static HttpCookie CloneCookie(HttpCookie cookie)
        {
            HttpCookie clonedCookie = new HttpCookie(cookie.Name, cookie.Value);
            clonedCookie.Domain = cookie.Domain;
            clonedCookie.Expires = cookie.Expires;
            clonedCookie.HttpOnly = cookie.HttpOnly;
            clonedCookie.Path = cookie.Path;
            clonedCookie.Secure = cookie.Secure;

            return clonedCookie;
        }
    }

    public static class MachineKeyCryptography
    {

        /// <summary>
        /// Encodes a string and protects it from tampering
        /// </summary>
        /// <param name="text">String to encode</param>
        /// <returns>Encoded string</returns>
        public static string Encode(string text)
        {
            return Encode(text, CookieProtection.All);
        }

        /// <summary>
        /// Encodes a string
        /// </summary>
        /// <param name="text">String to encode</param>
        /// <param name="cookieProtection">The method in which the string is protected</param>
        /// <returns></returns>
        public static string Encode(string text, CookieProtection cookieProtection)
        {
            if (string.IsNullOrEmpty(text) || cookieProtection == CookieProtection.None)
            {
                return text;
            }
            byte[] buf = Encoding.UTF8.GetBytes(text);
            return CookieProtectionHelperWrapper.Encode(cookieProtection, buf, buf.Length);
        }

        /// <summary>
        /// Decodes a string and returns null if the string is tampered
        /// </summary>
        /// <param name="text">String to decode</param>
        /// <returns>The decoded string or throws InvalidCypherTextException if tampered with</returns>
        public static string Decode(string text)
        {
            return Decode(text, CookieProtection.All);
        }

        /// <summary>
        /// Decodes a string
        /// </summary>
        /// <param name="text">String to decode</param>
        /// <param name="cookieProtection">The method in which the string is protected</param>
        /// <returns>The decoded string or throws InvalidCypherTextException if tampered with</returns>
        public static string Decode(string text, CookieProtection cookieProtection)
        {
            if (string.IsNullOrEmpty(text) || cookieProtection == CookieProtection.None)
            {
                return text;
            }
            byte[] buf;
            try
            {
                buf = CookieProtectionHelperWrapper.Decode(cookieProtection, text);
            }
            catch (Exception ex)
            {
                throw new InvalidCypherTextException("Unable to decode the text", ex.InnerException);
            }
            if (buf == null || buf.Length == 0)
            {
                throw new InvalidCypherTextException("Unable to decode the text");
            }
            return Encoding.UTF8.GetString(buf, 0, buf.Length);
        }
    }

    public class InvalidCypherTextException : Exception
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public static class CookieProtectionHelperWrapper
    {

        private static MethodInfo _encode;
        private static MethodInfo _decode;

        /// <summary>
        /// Constructor
        /// </summary>
        static CookieProtectionHelperWrapper()
        {
            // obtaining a reference to System.Web assembly
            Assembly systemWeb = typeof(HttpContext).Assembly;
            if (systemWeb == null)
            {
                throw new InvalidOperationException("Unable to load System.Web.");
            }
            // obtaining a reference to the internal class CookieProtectionHelper
            Type cookieProtectionHelper = systemWeb.GetType("System.Web.Security.CookieProtectionHelper");
            if (cookieProtectionHelper == null)
            {
                throw new InvalidOperationException("Unable to get the internal class System.Web.Security.CookieProtectionHelper.");
            }
            // obtaining references to the methods of CookieProtectionHelper class
            _encode = cookieProtectionHelper.GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Static);
            _decode = cookieProtectionHelper.GetMethod("Decode", BindingFlags.NonPublic | BindingFlags.Static);

            if (_encode == null || _decode == null)
            {
                throw new InvalidOperationException("Unable to get the methods to invoke.");
            }
        }

        /// <summary>
        /// Wrapper arround CookieProtectionHelper.Encode
        /// </summary>
        /// <param name="cookieProtection">Protection Type</param>
        /// <param name="buf">Bytes buffer to encode</param>
        /// <param name="count">The number of bytes in the buffer</param>
        /// <returns>Encoded text</returns>
        public static string Encode(CookieProtection cookieProtection, byte[] buf, int count)
        {
            return (string)_encode.Invoke(null, new object[] { cookieProtection, buf, count });
        }

        /// <summary>
        /// Wrapper arround CookieProtectionHelper.Decode
        /// </summary>
        /// <param name="cookieProtection">Protection Type</param>
        /// <param name="data">String to decode</param>
        /// <returns>Decoded bytes</returns>
        public static byte[] Decode(CookieProtection cookieProtection, string data)
        {
            return (byte[])_decode.Invoke(null, new object[] { cookieProtection, data });
        }

    }
    #endregion

    #region WebGrid
    public static class GridExtensions
    {
        public static WebGridColumn[] DynamicColumns(this HtmlHelper htmlHelper, WebGrid grid)
        {
            var columns = new List<WebGridColumn>();

            columns.Add(grid.Column("Property1", "Header", style: "record"));
            columns.Add(grid.Column("Property2", "Header", style: "record"));
            columns.Add(grid.Column("Actions", format: (item) => { return new HtmlString(string.Format("<a target='_blank' href= {0}>Edit </a>", "/Edit/" + item.Id) + string.Format("<a target='_blank' href= {0}> Delete</a>", "/Delete/" + item.Id)); }));
            return columns.ToArray();
        }
    }
    public static class WebGridBase
    {
        public static WebGrid Init(ParmInGridInit parmIn)
        {
            WebGrid grid = new WebGrid(null, canPage: parmIn.CanPage, canSort: parmIn.CanSort, rowsPerPage: 100,//parmIn.RowPerPage,
            selectionFieldName: "selectedRow", ajaxUpdateContainerId: parmIn.AjaxContainerID, ajaxUpdateCallback: string.Format("gridUtil.gridUpdateCallback('{0}')", parmIn.AjaxContainerID));
            grid.Pager(WebGridPagerModes.Numeric);
            grid.Bind(parmIn.Source);
            return grid;
        }
        public static IHtmlString GetWebGridHtml(WebGrid grid, IList<WebGridColumn> columns = null)
        {
            WebGridColumn columnSrNo = grid.Column(header: "SNo.", format: item => item.WebGrid.Rows.IndexOf(item) + 1 + Math.Round(Convert.ToDouble(grid.TotalRowCount / grid.PageCount) / grid.RowsPerPage) * grid.RowsPerPage * grid.PageIndex);
            columns.Insert(0, columnSrNo);
            return grid.GetHtml(tableStyle: "table table-bordered",
            headerStyle: "DataGridHeader",
            alternatingRowStyle: "DataGridrowb",
            rowStyle: "DataGridrowa",
            selectedRowStyle: "DataGridSelection",

             mode: WebGridPagerModes.FirstLast | WebGridPagerModes.NextPrevious | WebGridPagerModes.Numeric,
            firstText: "First", lastText: "Last",
            previousText: "Prev", nextText: "Next",
            columns: columns);
        }
    }

    public class ParmInGridInit
    {
        public string AjaxContainerID { get; set; }
        public IEnumerable<dynamic> Source { get; set; }
        public int RowPerPage { get; set; }
        public bool CanPage { get; set; }
        public bool CanSort { get; set; }
        public ParmInGridInit()
        {
            RowPerPage = 20;
            CanPage = true;
            CanSort = true;
        }
    }
    #endregion

    #region CSV Utility
    public static class CSVUtility
    {
        public static MemoryStream GetCSV(DataTable data)
        {
            string[] fieldsToExpose = new string[data.Columns.Count];
            for (int i = 0; i < data.Columns.Count; i++)
            {
                fieldsToExpose[i] = data.Columns[i].ColumnName;
            }

            return GetCSV(fieldsToExpose, data);
        }

        public static MemoryStream GetCSV(string[] fieldsToExpose, DataTable data)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                for (int i = 0; i < fieldsToExpose.Length; i++)
                {
                    if (i != 0) { writer.Write(","); }
                    writer.Write("\"");
                    writer.Write(fieldsToExpose[i].Replace("\"", "\"\""));
                    writer.Write("\"");
                }
                writer.Write("\n");

                foreach (DataRow row in data.Rows)
                {
                    for (int i = 0; i < fieldsToExpose.Length; i++)
                    {
                        if (i != 0) { writer.Write(","); }
                        writer.Write("\"");
                        writer.Write(row[fieldsToExpose[i]].ToString()
                            .Replace("\"", "\"\""));
                        writer.Write("\"");
                    }

                    writer.Write("\n");
                }
            }

            return stream;
        }
    }
    #endregion

    #region Excel Utility
    //public class ExcelUtility
    //{
    //    // Get the excel column letter by index
    //    public static string ColumnLetter(int intCol)
    //    {
    //        int intFirstLetter = ((intCol) / 676) + 64;
    //        int intSecondLetter = ((intCol % 676) / 26) + 64;
    //        int intThirdLetter = (intCol % 26) + 65;

    //        char FirstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
    //        char SecondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
    //        char ThirdLetter = (char)intThirdLetter;

    //        return string.Concat(FirstLetter, SecondLetter, ThirdLetter).Trim();
    //    }

    //    // Create a text cell
    //    private static Cell CreateTextCell(string header, UInt32 index, string text)
    //    {
    //        var cell = new Cell
    //        {
    //            DataType = CellValues.InlineString,
    //            CellReference = header + index
    //        };
    //        var istring = new InlineString();
    //        var t = new Text { Text = text };
    //        istring.Append(t);
    //        cell.Append(istring);
    //        return cell;
    //    }

    //    public static MemoryStream GetExcel(DataTable data)
    //    {
    //        string[] fieldsToExpose = new string[data.Columns.Count];
    //        for (int i = 0; i < data.Columns.Count; i++)
    //        {
    //            fieldsToExpose[i] = data.Columns[i].ColumnName;
    //        }

    //        return GetExcel(fieldsToExpose, data);
    //    }

    //    public static MemoryStream GetExcel(string[] fieldsToExpose, DataTable data)
    //    {
    //        MemoryStream stream = new MemoryStream();
    //        UInt32 rowcount = 0;

    //        // Create the Excel document
    //        var document = SpreadsheetDocument.Create
    //        (stream, SpreadsheetDocumentType.Workbook);
    //        var workbookPart = document.AddWorkbookPart();
    //        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
    //        var relId = workbookPart.GetIdOfPart(worksheetPart);

    //        var workbook = new Workbook();
    //        var fileVersion = new FileVersion { ApplicationName = "Microsoft Office Excel" };
    //        var worksheet = new Worksheet();
    //        var sheetData = new SheetData();
    //        worksheet.Append(sheetData);
    //        worksheetPart.Worksheet = worksheet;

    //        var sheets = new Sheets();
    //        var sheet = new Sheet { Name = "Sheet1", SheetId = 1, Id = relId };
    //        sheets.Append(sheet);
    //        workbook.Append(fileVersion);
    //        workbook.Append(sheets);
    //        document.WorkbookPart.Workbook = workbook;
    //        document.WorkbookPart.Workbook.Save();

    //        // Add header to the sheet
    //        var row = new Row { RowIndex = ++rowcount };
    //        for (int i = 0; i < fieldsToExpose.Length; i++)
    //        {
    //            row.Append(CreateTextCell(ColumnLetter(i), rowcount, fieldsToExpose[i]));
    //        }
    //        sheetData.AppendChild(row);
    //        worksheetPart.Worksheet.Save();

    //        // Add data to the sheet
    //        foreach (DataRow dataRow in data.Rows)
    //        {
    //            row = new Row { RowIndex = ++rowcount };
    //            for (int i = 0; i < fieldsToExpose.Length; i++)
    //            {
    //                row.Append(CreateTextCell(ColumnLetter(i), rowcount,
    //                    dataRow[fieldsToExpose[i]].ToString()));
    //            }
    //            sheetData.AppendChild(row);
    //        }
    //        worksheetPart.Worksheet.Save();

    //        document.Close();
    //        return stream;
    //    }
    //}
    #endregion

    #region BaseModel
    public class BaseModel
    {
        public string ControllerName { get; set; }
        public string GetPartialViewPath(string viewName)
        {
            return string.Format("~/Views/{0}/{1}.cshtml", this.ControllerName.Replace("Controller", ""), viewName);

        }
        public string GetViewPath(string controllerName, string viewName)
        {
            return string.Format("~/Views/{0}/{1}.cshtml", controllerName, viewName);
        }
    }

    public class Result
    {
        public Result()
        {
            this.MessageType = MessageType.Success;
        }
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
        public ActionResult ActionResult { get; set; }
        public PartialViewResult PartialViewResult { get; set; }
        public string Redirect { get; set; }
        public object Id { get; set; }
        public object Info { get; set; }
        public object Object { get; set; }
        public int TabNo { get; set; }
        public MvcHtmlString htmlString { get; set; }
    }

    public enum MessageType
    {
        Success,
        Error,
        Info,
        Warning,
        InvalidPassword
    }
    public enum DesignationType
    {
        SuperAdmin = 1,
        Admin = 2,

    }

    #endregion

    #region My Profile
    //public class UserUtil
    //{
    //    private BaseEntities db;
    //    public Result Result { get; set; }
    //    public UserUtil()
    //    {
    //        this.db = new BaseEntities();
    //        this.Result = new Result();
    //    }
    //    public Result UpdateProfile(user u, string parmIds)
    //    {
    //        try
    //        {
    //            int user_id = Convert.ToInt32(u.user_id);
    //            db.Configuration.ProxyCreationEnabled = false;
    //            if (user_id > 0)
    //            {
    //                user find = db.users.Find(user_id);
    //                BaseUtil.CopyObject(u, find, parmIds);
    //                db.Entry(find).State = EntityState.Modified;
    //            }
    //            else
    //            {
    //                u.role_bit = (int)Role.Admin;
    //                // u.city_id = 1;//Revisit[COD]
    //                u.password = u.password != "" ? u.password : BaseUtil.GetRandomPasswordNumber(6);
    //                db.users.Add(u);

    //            }
    //            db.SaveChanges();
    //            Result.MessageType = MessageType.Success;
    //            Result.Id = u.company_id;
    //        }
    //        catch (Exception ex)
    //        {
    //            Result.MessageType = MessageType.Error;
    //            Result.Message = ex.Message;
    //        }
    //        return Result;
    //    }
    //    public Result UpdatePassword(String OldPassword, String NewPassword)
    //    {
    //        int UserID = SessionUtil.GetUserID();
    //        try
    //        {

    //            var data = db.users.Find(UserID);
    //            string CheckOldPwd = data.password;


    //            if (OldPassword == CheckOldPwd)
    //            {
    //                user objTable = (from t in db.users
    //                                 where t.user_id == UserID
    //                                 select t
    //                                         ).First();
    //                objTable.password = NewPassword;
    //                db.Entry(objTable).State = EntityState.Modified;
    //                db.SaveChanges();
    //                Result.Message = string.Format(BaseConst.MSG_SUCCESS_UPDATE, "Password");
    //            }
    //            else
    //            {
    //                Result.MessageType = MessageType.Error;
    //                Result.Message = string.Format(BaseConst.MSG_INVALID_OLD_PASSWORD, "Password");
    //            }



    //        }
    //        catch (Exception ex)
    //        {
    //            Result.MessageType = MessageType.Error;
    //            Result.Message = ex.Message;
    //        }
    //        return Result;
    //    }

    //}
    #endregion


    public static class ExceptionLogging
    {

        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

        public static void SendErrorToText(Exception ex)
        {
            var line = Environment.NewLine + Environment.NewLine;

            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            exurl = context.Current.Request.Url.ToString();
            ErrorLocation = ex.Message.ToString();

            try
            {
                string filepath = context.Current.Server.MapPath("~/ExceptionDetailsFile/");  //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);

                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {


                    File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }

    }  


}