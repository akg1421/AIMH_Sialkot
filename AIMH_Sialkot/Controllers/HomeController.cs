using AIMH_Sialkot.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

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
            if (patientNameSet.Count() > 0)
                ViewBag.message = null;
            else if(patientNameSet.Count() <= 0)
                ViewBag.message = "No records entered in last 15 minutes";
            
            return View(patientNameSet);


            
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

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



            var recDaily = db.PatientInfo.Where(x => EntityFunctions.TruncateTime(x.CreatedOn) == DateTime.Today).Select(x => x.LumpSum);
            if (recDaily == null || recDaily.Count() == 0)
                ViewBag.dailyCash = "(No record entered today)";
            else
            {
                int dailyCash = db.PatientInfo.Where(x => EntityFunctions.TruncateTime(x.CreatedOn) == DateTime.Today).Select(x => x.LumpSum).Sum();
                ViewBag.dailyCash = dailyCash;
            }

            var recMonthly = db.PatientInfo.Where(x => x.CreatedOn.Month.Equals(DateTime.Now.Month) && x.CreatedOn.Year.Equals(DateTime.Now.Year)).Select(x => x.LumpSum);
            if (recMonthly == null || recMonthly.Count() == 0)
                ViewBag.monthlyCash = "(No record entered this month)";
            else
            {
                int monthlyCash = db.PatientInfo.Where(x => x.CreatedOn.Month.Equals(DateTime.Now.Month) && x.CreatedOn.Year.Equals(DateTime.Now.Year)).Select(x => x.LumpSum).Sum();
                ViewBag.monthlyCash = monthlyCash;
            }




            int? no_of_patients = db.PatientInfo.Count();
            if (no_of_patients == null)
                ViewBag.no_of_patients = "(No patients exist)";
            else
                ViewBag.no_of_patients = no_of_patients;



            var no_of_testsSET = db.PatientInfo.Select(x => x.TestTotal);
            if (no_of_testsSET.Count() == 0 || no_of_testsSET == null)
                ViewBag.no_of_tests = "(No test exist)";
            else
            {
                int no_of_tests = db.PatientInfo.Select(x => x.TestTotal).Sum();
                ViewBag.no_of_tests = no_of_tests;
            }
                
            

            return View();
        }
    }
}