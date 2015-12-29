using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class TestOptions
    {
        [Key]
        public int OptionID { get; set; }

        public int TestID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string OptionName { get; set; }

        [Display(Name = "Unit")]
        public string OptionUnit { get; set; }

        [Display(Name = "Range")]
        public string Range{ get; set; }
    }
}