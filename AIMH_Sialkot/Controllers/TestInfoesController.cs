using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AIMH_Sialkot.Models;
using Microsoft.AspNet.Identity;
using System.Web.Routing;

namespace AIMH_Sialkot.Controllers
{
    public class TestInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestInfoes
        public ActionResult Index()
        {
            Session["TestID"] = null;
            if (!string.IsNullOrEmpty(Request.Unvalidated["searchItem"]))
            {
                string searchItem = Request.Unvalidated["searchItem"];
                int someno = -1;
                if (int.TryParse(searchItem, out someno)) //searchItem is numeric
                {
                    ViewBag.message = "No record found. Click to go back";
                    return View();
                }
                else if (searchItem.All(c => Char.IsLetter(c) || c == ' ')) //searchItem is alphabetic
                {
                    IEnumerable<TestInfo> test_set = db.TestInfo.Where(x => x.TestName.Contains(searchItem)).OrderByDescending(x => x.CreatedOn);
                    if (test_set != null && test_set.Any() == true)
                    {
                        return View(test_set);
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

            return View(db.TestInfo.OrderByDescending(x => x.CreatedOn).ToList());
        }

        // GET: TestInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestInfo testInfo = db.TestInfo.Find(id);
            if (testInfo == null)
            {
                return HttpNotFound();
            }
            return View(testInfo);
        }

        // GET: TestInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestID,TestName,Abbreviation,Details,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] TestInfo testInfo)
        {
            if (ModelState.IsValid)
            {
                testInfo.CreatedBy = User.Identity.GetUserName();
                testInfo.CreatedOn = DateTime.Now;
                db.TestInfo.Add(testInfo);
                db.SaveChanges();
                Session["TestID"] = testInfo.TestID;
                return RedirectToAction("AddTestOption", new { thistestid = testInfo.TestID });
            }

            return View(testInfo);
        }

        public ActionResult AddTestOption([Bind(Include = "OptionID,TestID,OptionName,OptionUnit,Range")] TestOptions testOptions)
        {
            if (Session["TestID"] == null)
                return RedirectToAction("Index", "Home");

            int TestID = (int)Session["TestID"];
            ViewBag.ShowTest = db.TestInfo.First(x => x.TestID == TestID);
            ViewBag.ShowTestOptions = db.TestOptions.Where(x => x.TestID == TestID);
            if (ModelState.IsValid)
            {
                testOptions.TestID = TestID;
                db.TestOptions.Add(testOptions);
                db.SaveChanges();
                return RedirectToAction("AddTestOption");
            }
            return View(testOptions);
        }

        // GET: TestInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestInfo testInfo = db.TestInfo.Find(id);
            if (testInfo == null)
            {
                return HttpNotFound();
            }
            return View(testInfo);
        }

        // POST: TestInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestID,TestName,Price,Abbreviation,Details,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] TestInfo testInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testInfo).State = EntityState.Modified;
                testInfo.ModifiedBy = User.Identity.Name;
                testInfo.ModifiedOn = DateTime.Now.ToShortDateString();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testInfo);
        }

        // GET: TestInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestInfo testInfo = db.TestInfo.Find(id);
            if (testInfo == null)
            {
                return HttpNotFound();
            }
            return View(testInfo);
        }

        // POST: TestInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IEnumerable<TestOptions> testoption_set = db.TestOptions.Where(x => x.TestID == id);
            foreach (var x in testoption_set)
                db.TestOptions.Remove(x);
            db.SaveChanges();

            TestInfo testInfo = db.TestInfo.Find(id);
            db.TestInfo.Remove(testInfo);
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
