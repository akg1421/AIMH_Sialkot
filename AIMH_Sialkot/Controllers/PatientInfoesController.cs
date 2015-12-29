using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AIMH_Sialkot.Models;

namespace AIMH_Sialkot.Controllers
{
    public class PatientInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PatientInfoes
        public ActionResult Index()
        {
            IEnumerable<PatientInfo> PatientList;
            if (!string.IsNullOrEmpty(Request.Unvalidated["searchItem"]))
            {
                string searchItem = Request.Unvalidated["searchItem"];
                int Labno = -1;
                if (int.TryParse(searchItem, out Labno)) //searchItem is lab no
                {
                    if (db.PatientInfo.Where(x => x.LabNo == Labno).SingleOrDefault() != null)
                    {
                        PatientList = db.PatientInfo.Where(x => x.LabNo == Labno);
                        PatientList = PatientList.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
                        return View(PatientList);
                    }
                    else
                    {
                        ViewBag.message = "No record found. Click to go back";
                        return View();
                    }
                }
                else if (searchItem.All(c => Char.IsLetter(c) || c == ' ')) //searchItem is name
                {
                    PatientList = db.PatientInfo.Where(x => x.Name.Contains(searchItem));
                    PatientList = PatientList.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
                    if (PatientList != null && PatientList.Any() == true)
                    {
                        return View(PatientList);
                    }
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

            PatientList = db.PatientInfo;
            PatientList = PatientList.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
            return View(PatientList);
        }

        // GET: PatientInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientInfo patientInfo = db.PatientInfo.Find(id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }
            return View(patientInfo);
        }

        // GET: PatientInfoes/Create
        public ActionResult Create()
        {
            int[] EmptyTests = db.TestInfo.Select(x => x.TestID).ToArray();
            foreach(var id in EmptyTests)
            {
                if(db.TestOptions.Where(x=>x.TestID == id).FirstOrDefault() == null)
                {
                    db.TestInfo.Remove(db.TestInfo.Find(id));
                    db.SaveChanges();
                }
            }

            var TestInfo = db.TestInfo.Select(x => new { TestID = x.TestID, Abbreviation = x.Abbreviation });
            ViewBag.TestInfo = new SelectList(TestInfo, "TestID", "Abbreviation");

            var CategoryInfo = db.Category.Select(x => new { CategoryID = x.CategoryID, CategoryName = x.CategoryName });
            ViewBag.CategoryInfo = new SelectList(CategoryInfo, "CategoryID", "CategoryName");

            return View();
        }

        // POST: PatientInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID, ReceiptNo,BookNo,Name,Guardian,Age,Sex,OutdoorNo,IndoorNo,Ward,ReferenceBy")] PatientInfo patientInfo, IEnumerable<ViewModel_TestCheckBoxes> Testlist)
        {
            var TestInfo = db.TestInfo.Select(x => new { TestID = x.TestID, Abbreviation = x.Abbreviation });
            ViewBag.TestInfo = new SelectList(TestInfo, "TestID", "Abbreviation");

            var CategoryInfo = db.Category.Select(x => new { CategoryID = x.CategoryID, CategoryName = x.CategoryName });
            ViewBag.CategoryInfo = new SelectList(CategoryInfo, "CategoryID", "CategoryName");

            int tests_selected = 0;
            foreach (var x in Testlist)
                if (x.CheckBoxVal == true)
                    tests_selected++;


            if (ModelState.IsValid && tests_selected > 0)
            {
                PatientInfo LastPatient = db.PatientInfo.AsEnumerable().LastOrDefault();
                int? currentLabno;

                if (LastPatient == null)
                    currentLabno = 1;

                else
                {
                    if (Convert.ToDateTime(LastPatient.CreatedOn).Month < DateTime.Now.Month)
                        currentLabno = 1;
                    else
                        currentLabno = LastPatient.LabNo + 1;
                }

                //Random r = new Random();
                //int randlabno;
                //int?[] Labnos = db.PatientInfo.Select(x => x.LabNo).ToArray();
                //for (randlabno = r.Next(100, 40000); db.PatientInfo.Where(x => x.LabNo == randlabno).Select(x => x.LabNo).FirstOrDefault() != null; randlabno = r.Next(100, 40000)) { }
                //for (randlabno = r.Next(100, 40000); Labnos.Contains(randlabno) == true; randlabno = r.Next(100, 40000)) ;

                PatientInfo patient = new PatientInfo()
                {
                    LabNo = currentLabno,
                    CategoryID = patientInfo.CategoryID,
                    ReceiptNo = patientInfo.ReceiptNo,
                    BookNo = patientInfo.BookNo,
                    Name = patientInfo.Name,
                    Guardian = patientInfo.Guardian,
                    Age = patientInfo.Age,
                    Sex = patientInfo.Sex,
                    OutdoorNo = patientInfo.OutdoorNo,
                    IndoorNo = patientInfo.IndoorNo,
                    Ward = patientInfo.Ward,
                    ReferenceBy = patientInfo.ReferenceBy,
                    TimeIn = DateTime.Now.TimeOfDay.ToString(),
                    ReceivingDate = DateTime.Now.ToShortDateString(),
                    ReportingDate = DateTime.Now.ToShortDateString(),
                    CreatedOn = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };
                db.PatientInfo.Add(patient);
                db.SaveChanges();

                List<ViewModel_TestCheckBoxes> chkboxlist = Testlist.ToList();
                for (int i = 0; i < chkboxlist.Count(); i++)
                {
                    if (chkboxlist[i].CheckBoxVal)
                    {
                        int checkboxval = chkboxlist[i].TestID;
                        int[] optionIDs = db.TestOptions.Where(x => x.TestID == checkboxval).Select(x => x.OptionID).ToArray();
                        for (int j = 0; j < optionIDs.Length; j++)
                        {
                            Results result = new Results { PatientID = patient.PatientID, TestID = chkboxlist[i].TestID, OptionID = optionIDs[j], Value = null };
                            db.Results.Add(result);
                        }
                    }
                }

                //foreach (var item in Testlist)
                //{
                //    var testOptions = db.TestOptions.Where(x => x.TestID == item.TestID);

                //    foreach (var option in testOptions)
                //    {
                //        db.Results.Add(new Results()
                //        {
                //            PatientID = patient.PatientID,
                //            TestID = item.TestID,
                //            OptionID = option.OptionID
                //        });
                //    }
                //}

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.message = "Tests must be selected";
            return View(patientInfo);
        }

        // GET: PatientInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientInfo patientInfo = db.PatientInfo.Find(id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }

            var CategoryInfo = db.Category.Select(x => new { CategoryID = x.CategoryID, CategoryName = x.CategoryName });
            ViewBag.CategoryInfo = new SelectList(CategoryInfo, "CategoryID", "CategoryName");
            return View(patientInfo);
        }

        // POST: PatientInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,LabNo,ReceiptNo,BookNo,Name,Guardian,Age,Sex,TimeIn,ReceivingDate,OutdoorNo,ReportingDate,IndoorNo,Ward,ReferenceBy,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] PatientInfo patientInfo)
        {
            var CategoryInfo = db.Category.Select(x => new { CategoryID = x.CategoryID, CategoryName = x.CategoryName });
            ViewBag.CategoryInfo = new SelectList(CategoryInfo, "CategoryID", "CategoryName");

            if (ModelState.IsValid)
            {
                db.Entry(patientInfo).State = EntityState.Modified;
                patientInfo.ModifiedBy = User.Identity.Name;
                patientInfo.ModifiedOn = DateTime.Now.ToShortDateString();
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(patientInfo);
        }

        // GET: PatientInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientInfo patientInfo = db.PatientInfo.Find(id);
            if (patientInfo == null)
            {
                return HttpNotFound();
            }
            return View(patientInfo);
        }

        // POST: PatientInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IEnumerable<Results> result_set = db.Results.Where(x => x.PatientID == id);
            foreach (var x in result_set)
                db.Results.Remove(x);
            db.SaveChanges();

            PatientInfo patientInfo = db.PatientInfo.Find(id);
            db.PatientInfo.Remove(patientInfo);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
