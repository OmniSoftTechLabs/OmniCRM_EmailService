//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OmniCRM_EmailService
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppointmentDetail
    {
        public int AppintmentID { get; set; }
        public int CallID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.DateTime> AppointmentDateTime { get; set; }
        public System.Guid RelationshipManagerID { get; set; }
        public int AppoinStatusID { get; set; }
        public string Remarks { get; set; }
    
        public virtual AppoinmentStatusMaster AppoinmentStatusMaster { get; set; }
        public virtual CallDetail CallDetail { get; set; }
        public virtual UserMaster UserMaster { get; set; }
    }
}