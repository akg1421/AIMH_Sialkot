using AIMH_Sialkot.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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

        public ActionResult Index(string msg)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var RoleCount = roleManager.Roles.Count() - 1;
            if (RoleCount <= 0)
                return View();

            if (string.IsNullOrEmpty(msg))
                TempData["msg"] = null;
            else
                TempData["msg"] = msg;

            string[] RoleNames = roleManager.Roles.Where(x=>x.Name != "SuperAdmin").Select(x => x.Name).ToArray();
            List<ViewModel_ApplicationUser> UserList = new List<ViewModel_ApplicationUser>();
            
            

            for (int i = 0; i < RoleCount; i++)
            {
                var role = roleManager.FindByName(RoleNames[i]).Users.FirstOrDefault();
                var users = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role.RoleId)).ToList();
                foreach(var user in users)
                {
                    UserList.Add(new ViewModel_ApplicationUser { id = user.Id, Username = user.UserName, Rolename = RoleNames[i] });
                }
            }



            //var role1 = roleManager.FindByName(RoleNames[0]).Users.FirstOrDefault();
            //if (role1 == null)
            //    ViewBag.AdminUserList = null;
            //else
            //    ViewBag.AdminUserList = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role1.RoleId)).ToList();


            //var role2 = roleManager.FindByName(RoleNames[1]).Users.FirstOrDefault();
            //if (role2 == null)
            //    ViewBag.OptUserList = null;
            //else
            //    ViewBag.OptUserList = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role2.RoleId)).ToList();

            return View(UserList);
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

        //GET: Users/Delete/5
        
        public async Task<ActionResult> Delete(string id, string role)
        {
            TempData["message"] = null;
            // Check for for both ID and Role and exit if not found
            if (id == null || role == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Look for user in the UserStore
            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users.SingleOrDefault(u => u.Id == id);

            // If not found, exit
            if (user == null)
            {
                return HttpNotFound();
            }

            // Remove user from role first!
            var remFromRole = await HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().RemoveFromRoleAsync(id, role);

            // If successful
            if (remFromRole.Succeeded)
            {
                // Remove user from UserStore
                var results = await HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().DeleteAsync(user);

                // If successful
                if (results.Succeeded)
                {
                    // Redirect to Users page
                    return RedirectToAction("Index", "Users", new { area = "Dashboard" });
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Look for user in the UserStore
            var cUser = db.Users.Where(x => x.Id == id).FirstOrDefault();
            //ApplicationUser cUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            
            // If not found, exit
            if (cUser == null)
            {
                return HttpNotFound();
            }

            
            String newPassword = "admin123";
            
            String hashedNewPassword = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().PasswordHasher.HashPassword(newPassword);

            cUser.PasswordHash = hashedNewPassword;
            db.SaveChanges();
            //UserStore<ApplicationUser> store = new UserStore<ApplicationUser>();
            //store.SetPasswordHashAsync(cUser, hashedNewPassword);


            string message = "New password = admin123 has been assigned to user = " + cUser.UserName;
            return RedirectToAction("Index", "Users", new { msg = message });
        }
    }
}
