//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ST_BO.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SMS_TRACKING
    {
        public int SmsId { get; set; }
        public string SmsNumbers { get; set; }
        public string SmsMessage { get; set; }
        public string ApiMessage { get; set; }
        public Nullable<int> SmsCount { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Smstype { get; set; }
        public int CompanyId { get; set; }
    
        public virtual COMPANY COMPANY { get; set; }
    }
}
