using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class PatientInfo
    {
        [Key]
        public int PatientID { get; set; }

        [Display(Name = "Lab no")]
        public Nullable<int> LabNo { get; set; }

        [Required(ErrorMessage = "Receipt no is required.")]
        [Display(Name = "Receipt no")]
        public Nullable<int> ReceiptNo { get; set; }

        [Required(ErrorMessage = "Book no is required.")]
        [Display(Name = "Book no")]
        public Nullable<int> BookNo { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Enter a valid name")]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Enter a valid name")]
        [StringLength(50, ErrorMessage = "Guardian name is too long")]
        public string Guardian { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 150, ErrorMessage = "Enter a valid age.")]
        public Nullable<int> Age { get; set; }

        //[Required(ErrorMessage = "Sex is required.")]
        public string Sex { get; set; }
       
        [Required(ErrorMessage = "Outdoor no is required.")]
        [Display(Name = "Outdoor no")]
        public Nullable<int> OutdoorNo { get; set; }
        public string ReportingDate { get; set; }

        [Required(ErrorMessage = "Indoor no is required.")]
        [Display(Name = "Indoor no")]
        public Nullable<int> IndoorNo { get; set; }
        public string Ward { get; set; }

        [Display(Name = "Referenced by")]
        public string ReferenceBy { get; set; }

        [Display(Name = "CNIC")]
        [MinLength(13, ErrorMessage = "Enter 13 digit CNIC without '-'"), MaxLength(13, ErrorMessage = "Enter 13 digit CNIC without '-'")]
        public string cnic { get; set; }

        [Display(Name = "Time in")]
        public string TimeIn { get; set; }

        [Display(Name = "Receiving date")]
        public string ReceivingDate { get; set; }

        [Display(Name = "Creation date")]
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name = "Modification date")]
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }



        public virtual ICollection<PatientTestInfo> PatientTestInfos { get; set; }
    }
}