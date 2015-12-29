using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class Results
    {
        [Key]
        public int ResultID { get; set; }

        public Nullable<int> PatientID { get; set; }

        public Nullable<int> TestID { get; set; }

        public Nullable<int> OptionID { get; set; }

        public string Value{ get; set; }
    }
}