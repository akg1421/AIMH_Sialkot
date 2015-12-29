using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class PatientTestInfo
    {
        [Key]
        public int PatientTestID { get; set; }

        public int PatientID { get; set; }

        public int CategoryID{ get; set; }

        public int TestID { get; set; }

        public virtual PatientInfo PatientInfo { get; set; }
        public virtual TestInfo TestInfo { get; set; }
        public virtual CategoryInfo Category { get; set; }
    }
}