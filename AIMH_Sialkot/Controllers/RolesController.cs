using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AIMH_Sialkot.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AIMH_Sialkot.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            var roles = db.Roles.Where(x=>x.Name != "SuperAdmin").ToList();
            return View(roles);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                db.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                db.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            var thisRole = db.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Delete/5
        public async System.Threading.Tasks.Task<ActionResult> Delete(string RoleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var UserList = roleManager.FindByName(RoleName).Users;

            if (UserList != null)
            {
                foreach (var x in UserList)
                {
                    var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users.SingleOrDefault(u => u.Id == x.UserId);
                    var remFromRole = await HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().RemoveFromRoleAsync(x.UserId, RoleName);
                    if (remFromRole.Succeeded)
                    {
                        // Remove user from UserStore
                        var results = await HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().DeleteAsync(user);
                    }
                }
            }

            var thisRole = db.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            db.Roles.Remove(thisRole);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
