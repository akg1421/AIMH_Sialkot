using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class ViewModel_TestOptionsResults
    {
        [Required]
        public string Value { get; set; }
        public Nullable<int> OptionID { get; set; }

        public int ResultID { get; set; }

        [Display(Name = "Name")]
        public string OptionName { get; set; }

        [Display(Name = "Unit")]
        public string OptionUnit { get; set; }

        [Display(Name = "Range")]
        public string Range { get; set; }
    }
}