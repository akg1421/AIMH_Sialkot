using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class TestInfo
    {
        [Key]
        public int TestID { get; set; }

        [Required(ErrorMessage = "Test name is required.")]
        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Test name is too long")]
        public string TestName { get; set; }

        [Required(ErrorMessage = "If no abbreviation exists give test name as abbreviation.")]
        [Display(Name = "Abbreviation")]
        [StringLength(100, ErrorMessage = "Test abbreviation is too long")]
        public string Abbreviation { get; set; }

        [StringLength(500, ErrorMessage = "Test details are too long.")]
        public string Details { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        [Required(ErrorMessage = "Test price is required.")]
        public int Price { get; set; }
    }
}