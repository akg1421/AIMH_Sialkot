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
    public class PrintsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Prints
        public ActionResult Index()
        {
            var result_set = db.Results.Where(x => x.Value != null && x.Value.Trim() != "").GroupBy(x => x.PatientID).SelectMany(x => x.Select(s => s.PatientID)).Distinct();
            var patient_set = db.PatientInfo.Where(x => result_set.Contains(x.PatientID));

            if (patient_set.Count() == 0)
            {
                ViewBag.message = "No test results are entered. Click to Add Result";
                return View();
            }

            IEnumerable<PatientInfo> SomePatientList;
            if (!string.IsNullOrEmpty(Request.Unvalidated["searchItem"]))
            {
                string searchItem = Request.Unvalidated["searchItem"];
                int Labno = -1;

                if (int.TryParse(searchItem, out Labno)) //searchItem is lab no
                {
                    //Returns records of the "month" in which last record was entered.
                    if (patient_set.Where(x => x.LabNo == Labno).FirstOrDefault() != null)
                    {
                        SomePatientList = patient_set.Where(x => x.LabNo == Labno);
                        SomePatientList = SomePatientList.OrderByDescending(x => x.CreatedOn.Date).ThenByDescending(x => x.CreatedOn.TimeOfDay);
                        return View(SomePatientList);
                    }
                    else
                    {
                        ViewBag.message = "No records found. Click to Add Result";
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
                        ViewBag.message = "No records found. Click to Add Result";
                        return View();
                    }
                }

                else //searchItem is bad
                {
                    ViewBag.message = "No records found. Click to Add Result";
                    return View();
                }
            }

            //IEnumerable<PatientInfo> Pt = patient_set;
            //Pt = Pt.OrderByDescending(x => x.CreatedOn.Date).ThenBy(x => x.CreatedOn.TimeOfDay);
            return View();
        }

        public ActionResult ShowTests(int? PatientID)
        {
            if (PatientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Results.Where(x => x.PatientID == PatientID).FirstOrDefault() == null)
                return HttpNotFound();

            int?[] TestIDs = db.Results.Where(x => x.PatientID == PatientID && (x.Value != null && x.Value.Trim() != "")).Select(x => x.TestID).Distinct().ToArray();
            if (TestIDs.Length == 0)
            {
                return HttpNotFound();
            }

            var testList = new List<TestInfo>();
            foreach (var testid in TestIDs)
            {
                TestInfo test = db.TestInfo.SingleOrDefault(x => x.TestID == testid);
                if (test == null)
                {
                    ViewBag.message = "No record found. Click to go back. ";
                }
                testList.Add(test);
            }

            ViewBag.PatientTests = testList;
            ViewBag.PatientID = PatientID;

            return View();
        }

        public ActionResult Print(int? PatientID, int? TestID)
        {
            if (TestID == null || PatientID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Results.Where(x => x.PatientID == PatientID && x.TestID == TestID).FirstOrDefault() == null || db.TestInfo.Where(x => x.TestID == TestID).FirstOrDefault() == null)
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
            ViewBag.PatientLabno = somepatient.LabNo;
            ViewBag.PatientReceiptNo = somepatient.ReceiptNo;
            ViewBag.PatientBookNo = somepatient.BookNo;
            ViewBag.PatientAge = somepatient.Age;
            ViewBag.PatientSex = somepatient.Sex;
            ViewBag.PatientWard = somepatient.Ward;
            ViewBag.PatientCreatedOn = somepatient.CreatedOn.ToString("d");

            string categoryname = db.Category.Where(x => x.CategoryID == somepatient.CategoryID).Select(x => x.CategoryName).FirstOrDefault().ToString();
            ViewBag.PatientCategory = categoryname;
            return View(ResultValues);
        }

        // GET: Prints/Details/5
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

        // GET: Prints/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Prints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientID,CategoryID,LabNo,ReceiptNo,BookNo,Name,Guardian,Age,Sex,OutdoorNo,ReportingDate,IndoorNo,Ward,ReferenceBy,cnic,TimeIn,ReceivingDate,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] PatientInfo patientInfo)
        {
            if (ModelState.IsValid)
            {
                db.PatientInfo.Add(patientInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patientInfo);
        }

        // GET: Prints/Edit/5
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
            return View(patientInfo);
        }

        // POST: Prints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,CategoryID,LabNo,ReceiptNo,BookNo,Name,Guardian,Age,Sex,OutdoorNo,ReportingDate,IndoorNo,Ward,ReferenceBy,cnic,TimeIn,ReceivingDate,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] PatientInfo patientInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patientInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patientInfo);
        }

        // GET: Prints/Delete/5
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

        // POST: Prints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
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
