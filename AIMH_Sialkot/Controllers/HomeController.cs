using AIMH_Sialkot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIMH_Sialkot.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            IEnumerable<PatientInfo> patientNameSet;
            var resultSet = db.Results.Where(x => x.Value == null || x.Value.Trim() == "").GroupBy(x => x.PatientID).SelectMany(x => x.Select(s => s.PatientID));
            var patientSet = db.PatientInfo.Where(x => resultSet.Contains(x.PatientID));
            DateTime limit_date = DateTime.Now.AddHours(-0.25);
            if (!string.IsNullOrEmpty(Request.Unvalidated["searchItem"]))
            {
                string searchItem = Request.Unvalidated["searchItem"];
                int Labno = -1;

                if (int.TryParse(searchItem, out Labno) && patientSet.Count() != 0) //searchItem is lab no
                {
                    //Returns records of the "month" in which last record was entered.
                    if (patientSet.Where(x => x.LabNo == Labno && x.CreatedOn > limit_date).SingleOrDefault() != null)
                        return View(patientSet.Where(x => x.LabNo == Labno && x.CreatedOn > limit_date).Take(1));
                    else
                    {
                        ViewBag.message = "No record found. Click to go back";
                        return View();
                    }
                }

                else if (searchItem.All(c => Char.IsLetter(c) || c == ' ') && patientSet.Count() != 0) //searchItem is name
                {
                    patientNameSet = patientSet.Where(x => x.Name.Contains(searchItem) && x.CreatedOn > limit_date);
                    patientNameSet = patientNameSet.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
                    if (patientNameSet.Count() != 0)
                        return View(patientNameSet);
                    else
                    {
                        ViewBag.message = "No record found. Click to go back";
                        return View();
                    }
                }

                else //searchItem is bad
                {
                    ViewBag.message = "No record found. Click to go back";
                    return View();
                }
            }

            patientNameSet = patientSet.Where(x => x.CreatedOn > limit_date);
            patientNameSet = patientNameSet.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
            ViewBag.message = "No records entered in last 15 minutes";
            return View(patientNameSet);


            //Agar last record ke month wale tamam records dikhane ho
            //IEnumerable<PatientInfo> patientNameSet;
            //var resultSet = db.Results.Where(x => x.Value == null || x.Value.Trim() == "").GroupBy(x => x.PatientID).SelectMany(x => x.Select(s => s.PatientID));
            //var patientSet = db.PatientInfo.Where(x => resultSet.Contains(x.PatientID));
            //string LastRecordStringDate = db.PatientInfo.AsEnumerable().Select(x => x.CreatedOn).LastOrDefault().ToString();
            //DateTime LastRecordDate = Convert.ToDateTime(LastRecordStringDate);//null when no record exist

            //if (!string.IsNullOrEmpty(Request.Unvalidated["searchItem"]))
            //{    
            //    //int LastRecordDateMonth = LastRecordDate.Month;

            //    string searchItem = Request.Unvalidated["searchItem"];
            //    int Labno = -1;

            //    if (int.TryParse(searchItem, out Labno) && LastRecordDate != null) //searchItem is lab no
            //    {
            //        //Returns records of the "month" in which last record was entered.
            //        if (patientSet.Where(x => x.LabNo == Labno && x.CreatedOn.Month == LastRecordDate.Month).SingleOrDefault() != null)
            //            return View(patientSet.Where(x => x.LabNo == Labno && x.CreatedOn.Month == LastRecordDate.Month).Take(1));
            //        else
            //        {
            //            ViewBag.message = "No record found. Click to go back";
            //            return View();
            //        }
            //    }

            //    else if (searchItem.All(c => Char.IsLetter(c) || c == ' ') && LastRecordDate != null) //searchItem is name
            //    {
            //        patientNameSet = patientSet.Where(x => x.Name.Contains(searchItem) && x.CreatedOn.Month == LastRecordDate.Month);
            //        patientNameSet = patientNameSet.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
            //        if (patientNameSet.Count() != 0)
            //            return View(patientNameSet);
            //        else
            //        {
            //            ViewBag.message = "No record found. Click to go back";
            //            return View();
            //        }
            //    }

            //    else //searchItem is bad
            //    {
            //        ViewBag.message = "No record found. Click to go back";
            //        return View();
            //    }
            //}

            //patientNameSet = patientSet.Where(x => x.CreatedOn.Month == LastRecordDate.Month);
            //patientNameSet = patientNameSet.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);

            //return View(patientNameSet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        public ActionResult Setting()
        {
            ViewBag.PatientCount = db.PatientInfo.Count();
            ViewBag.TestCount = db.TestInfo.Count();
            ViewBag.UserCount = db.Users.Count();
            ViewBag.CategoryCount = db.Category.Count();
            ViewBag.RolesCount = db.Roles.Count();
            return View();
        }

       
        public ActionResult AnalysisReport()
        {
            
            return View();
        }
    }
}