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
    public class CategoryInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CategoryInfoes
        public ActionResult Index()
        {
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
                    IEnumerable<CategoryInfo> category_set = db.Category.Where(x => x.CategoryName.Contains(searchItem));
                    if (category_set != null && category_set.Any() == true)
                    {
                        return View(category_set);
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

            return View(db.Category.ToList());
        }

        // GET: CategoryInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryInfo categoryInfo = db.Category.Find(id);
            if (categoryInfo == null)
            {
                return HttpNotFound();
            }
            return View(categoryInfo);
        }

        // GET: CategoryInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName,Rate")] CategoryInfo categoryInfo)
        {
            if (ModelState.IsValid)
            {
                db.Category.Add(categoryInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoryInfo);
        }

        // GET: CategoryInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryInfo categoryInfo = db.Category.Find(id);
            if (categoryInfo == null)
            {
                return HttpNotFound();
            }
            return View(categoryInfo);
        }

        // POST: CategoryInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName,Rate")] CategoryInfo categoryInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoryInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoryInfo);
        }

        // GET: CategoryInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryInfo categoryInfo = db.Category.Find(id);
            if (categoryInfo == null)
            {
                return HttpNotFound();
            }
            return View(categoryInfo);
        }

        // POST: CategoryInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryInfo categoryInfo = db.Category.Find(id);
            db.Category.Remove(categoryInfo);
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
