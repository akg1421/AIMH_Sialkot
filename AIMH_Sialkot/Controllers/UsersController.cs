using AIMH_Sialkot.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AIMH_Sialkot.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        public ActionResult Index()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var role1 = roleManager.FindByName("Admin").Users.First();
            ViewBag.AdminUserList = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role1.RoleId)).ToList();

            var role2 = roleManager.FindByName("Operator").Users.First();
            ViewBag.OptUserList = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role2.RoleId)).ToList();

            return View();
        }
        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }

        //        var user = await _userManager.FindByIdAsync(id);
        //        var logins = user.Logins;

        //        foreach (var login in logins.ToList())
        //        {
        //            await _userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
        //        }

        //        var rolesForUser = await _userManager.GetRolesAsync(id);

        //        if (rolesForUser.Count() > 0)
        //        {
        //            foreach (var item in rolesForUser.ToList())
        //            {
        //                // item should be the name of the role
        //                var result = await _userManager.RemoveFromRoleAsync(user.Id, item);
        //            }
        //        }

        //        _await _userManager.DeleteAsync(user);

        //        return RedirectToAction("Index");
        //    }
        //}
    }
}
