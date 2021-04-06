using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thesius_001.Models;
using System.Web.Mvc;

namespace Thesius_001.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {

            //Tools.Helper.GeneratePassword(Tools.Helper.PasswordChars.Alphabet, 16 );
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string GenerateToken(int count = 10)
        {
            var userList = db.Users.Where(w => w.EmailConfirmed == true).ToList();

            foreach (var user in userList)
            {
                var UserInviteList = db.InviteCode.Where(w=> w.UserId == user.Id).ToList();

                var thisFreeIventCount = count - UserInviteList.Where(w => w.Used == false).Count(); 

                if (thisFreeIventCount > 0)
                {
                    for (int i = 0; i < thisFreeIventCount; i++)
                    {
                        var randomCode = Tools.Helper.GeneratePassword(Tools.Helper.PasswordChars.Alphabet, 16);

                        db.InviteCode.Add(new InviteCode()
                        {
                            Code = randomCode, Type = 0, Used = false, UserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifedDate = DateTime.Now,
                        });

                        db.SaveChanges();

                    }

                }

            }

            return "Ok";
        }
    }
}