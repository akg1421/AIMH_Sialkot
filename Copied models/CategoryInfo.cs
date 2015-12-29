using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AIMH_Sialkot.Models
{
    public class CategoryInfo
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [StringLength(100, ErrorMessage = "Category name is too long")]
        public string CategoryName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage ="Enter a valid rate")]
        public Nullable<int> Rate { get; set; }

        public virtual ICollection<PatientTestInfo> PatientTestInfos { get; set; }
    }
}