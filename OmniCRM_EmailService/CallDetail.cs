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
    
    public partial class CallDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CallDetail()
        {
            this.AppointmentDetails = new HashSet<AppointmentDetail>();
        }
    
        public int CallID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.Guid CreatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
        public System.DateTime LastChangedDate { get; set; }
        public int OutComeID { get; set; }
        public string Remark { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppointmentDetail> AppointmentDetails { get; set; }
        public virtual UserMaster UserMaster { get; set; }
    }
}
