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
    public class ResultsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Results
        public ActionResult Index()
        {
            //var patient_set = db.PatientInfo.GroupJoin(db.Results.Where(x => x.Value == null || x.Value.Trim() == ""), x => x.PatientID, y => y.PatientID, (x, y) => new 
            //{
            //    x
            //});

            var result_set = db.Results.Where(x => x.Value == null || x.Value.Trim() == "").GroupBy(x => x.PatientID).SelectMany(x => x.Select(s => s.PatientID));

            var patient_set = db.PatientInfo.Where(x => result_set.Contains(x.PatientID));
            

            if(patient_set.Count() == 0)
            {
                ViewBag.message = "All test results are entered. Click to Add Patient";
                return View();
            }

            //----------------------------------------
            IEnumerable<PatientInfo> SomePatientList;
            if (!string.IsNullOrEmpty(Request.Unvalidated["searchItem"]))
            {
                //int LastRecordDateMonth = LastRecordDate.Month;

                string searchItem = Request.Unvalidated["searchItem"];
                int Labno = -1;

                if (int.TryParse(searchItem, out Labno)) //searchItem is lab no
                {
                    //Returns records of the "month" in which last record was entered.
                    if (patient_set.Where(x => x.LabNo == Labno).FirstOrDefault() != null)
                    {
                        SomePatientList = patient_set.Where(x => x.LabNo == Labno);
                        SomePatientList = SomePatientList.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x=>x.CreatedOn.TimeOfDay);
                        return View(SomePatientList);
                    }
                    else
                    {
                        ViewBag.message = "All test results are entered. Click to Add Patient";
                        return View();
                    }
                }

                else if (searchItem.All(c => Char.IsLetter(c) || c == ' ')) //searchItem is name
                {
                    SomePatientList = patient_set.Where(x => x.Name.Contains(searchItem));
                    SomePatientList = SomePatientList.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
                    if (SomePatientList.Count() != 0)
                        return View(SomePatientList);
                    else
                    {
                        ViewBag.message = "All test results are entered. Click to Add Patient";
                        return View();
                    }
                }

                else //searchItem is bad
                {
                    ViewBag.message = "All test results are entered. Click to Add Patient";
                    return View();
                }
            }

      
            //var patients = new List<PatientInfo>();

            //foreach (var item in patient_set)
            //{
            //    patients.Add(item.x);
            //}
            //foreach (var item in patient_set)
            //{
            //    patients.Add(new PatientInfo(){Age = item.x.Age,LabNo = item.x.LabNo,ReceiptNo = item.x.ReceiptNo,Name = item.x.Name,Guardian = item.x.Guardian,Sex = item.x.Sex,OutdoorNo = item.x.OutdoorNo});
            //}

            //return View(patients.OrderByDescending(x=>x.CreatedOn));
            IEnumerable<PatientInfo> Pt = patient_set;
            Pt = Pt.OrderByDescending(x => x.CreatedOn.Date).ThenBy(x => x.CreatedOn.TimeOfDay);
            return View(Pt);

        }

        public ActionResult ShowTests(int? PatientID)
        {
            if (PatientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int?[] TestIDs = db.Results.Where(x => x.PatientID == PatientID).Select(x => x.TestID).Distinct().ToArray();
            if (TestIDs.Length == 0)
            {
                return HttpNotFound();
            }

            var testList = new List<TestInfo>();
            foreach (var id in TestIDs)
            {
                if (db.Results.Where(x => (x.Value == null || x.Value.Trim() == "") && x.TestID == id && x.PatientID == PatientID).Count() != 0)
                {
                    TestInfo test = db.TestInfo.SingleOrDefault(x => x.TestID == id);
                    testList.Add(test);
                }
            }

            if(testList.Count() == 0)
                ViewBag.message = "All test results are entered. Click to Add Patient";

            ViewBag.PatientTests = testList;
            ViewBag.PatientID = PatientID;

            return View();
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            return View(results);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ResultID,PatientID,TestID,OptionID,Value")] Results results)
        {
            if (ModelState.IsValid)
            {
                db.Results.Add(results);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(results);
        }

        // GET: Results/Edit/5
        public ActionResult ResultEntry(int? TestID, int? PatientID)
        {
            if (TestID == null || PatientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(db.Results.Where(x=>x.PatientID == PatientID).FirstOrDefault() == null || db.TestInfo.Where(x=>x.TestID == TestID).FirstOrDefault() == null)
                return HttpNotFound();

            var ResultValues = db.Results.Where(x => x.PatientID == PatientID).Join(db.TestOptions.Where(x => x.TestID == TestID), a => a.OptionID, b => b.OptionID,
                (a, b) => new ViewModel_TestOptionsResults
                {
                    ResultID = a.ResultID,
                    OptionID = a.OptionID,
                    OptionName = b.OptionName,
                    OptionUnit = b.OptionUnit,
                    Range = b.Range,
                    Value = a.Value
                })
                .ToList();

            TestInfo sometest = db.TestInfo.Where(x => x.TestID == TestID).FirstOrDefault();
            ViewBag.TestName = sometest.TestName;
            PatientInfo somepatient = db.PatientInfo.Where(x => x.PatientID == PatientID).FirstOrDefault();
            ViewBag.PatientName = somepatient.Name;
            ViewBag.Labno = somepatient.LabNo;
            return View(ResultValues);

            //int?[] OptionIDs = db.Results.Where(x => x.TestID == TestID).Select(x => x.TestID).ToArray();
            //int[] ResultIDs = db.Results.Where(x=>x.TestID == TestID).Select(x=>x.ResultID).ToArray();
            //if (OptionIDs.Length == 0)
            //{
            //    return HttpNotFound();
            //}


            //var testoptionList = new List<ViewModel_TestOptionInfo>();
            //foreach (var optionID in OptionIDs)
            //{
            //    ViewModel_TestOptionInfo testoptionInfo = db.TestOptions.Select(x=> new { x.OptionID, x.OptionName, x.OptionUnit, x.Range} ).SingleOrDefault(x=>x.OptionID == optionID);
            //    testoptionList.Add(testoptionInfo);
            //}

            //-----------------

            //var testInformation = new List<TestOptions>();
            //foreach (var item in )
            //{
            //    patients.Add(item.x);
            //}

        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResultEntry([Bind(Include = "ResultID, OptionID, Value")] Results result, IEnumerable<ViewModel_TestOptionsResults> viewmodelList)
        {
            if (ModelState.IsValid)
            {
                foreach(var item in viewmodelList)
                {
                    var testresult = db.Results.Find(item.ResultID);
                    testresult.Value = item.Value;
                    db.SaveChanges();
                }
                //db.Entry(results).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Results");
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            return View(results);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Results results = db.Results.Find(id);
            db.Results.Remove(results);
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
