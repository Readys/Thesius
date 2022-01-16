using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thesius_001.Models;
using System.Web.Mvc;
using System.Threading;
using Microsoft.AspNet.Identity;
using Thesius_001.Tools;

namespace Thesius_001.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Publications()
        {
            return View();
        }

        public ActionResult Books()
        {

            var tree = db.Book.ToList();

            var catTree = tree.GenerateTree(c => c.BookId, c => c.ParentId);

            List<ViewBook> viewBook = new List<ViewBook>();

            foreach (var root in catTree)
            {
                List<ViewBook> viewBookChild = new List<ViewBook>();

                var thisBooks = tree.Where(w => w.ParentId == root.Item.BookId).ToList();

                foreach (var book in thisBooks)
                {
                    List<ViewBook> viewBookChild2 = new List<ViewBook>();

                    var thisArticlesId = db.BookArticle.Where(w => w.BookId == book.BookId).OrderBy(o => o.Order).Select(s=> s.ArticleId).ToArray();
                    var thisArticles = db.Article.Where(w => thisArticlesId.Contains(w.ArticleId)).ToList();

                    foreach (var art in thisArticles)
                    {
                        viewBookChild2.Add(new ViewBook() { BookId = art.ArticleId, Title = art.Title, ParentId = book.BookId, UserId = art.UserId });
                    }

                    viewBookChild.Add(new ViewBook() { BookId = book.BookId, Title = book.Title, ParentId = root.Item.BookId, children = viewBookChild2,
                     UserId = book.UserId });
                }

                viewBook.Add(new ViewBook() { BookId = root.Item.BookId, Title = root.Item.Title, children = viewBookChild, UserId = root.Item.UserId, });

            }

            ViewBag.Tree = viewBook;
            return View();
        }

        [Authorize]
        public ActionResult InfoThesis(int id)
        {
            var appUserId = User.Identity.GetUserId();
            var thisThesis = db.Thesis.SingleOrDefault(s=> s.ThesisId == id);


           
            return View(thisThesis);
        }

        [Authorize]
        public string ThesisDa(int id)
        {
            var appUserId = User.Identity.GetUserId();
            var thisThesis = db.Thesis.SingleOrDefault(s => s.ThesisId == id);
            var checkThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == id);

            string message = "";

            if (checkThesis != null)
            {
                message = "";
                if (checkThesis.Follow == true)
                {
                    message = message + "Вы уже голосовали За";
                    return message + ": " + thisThesis.Description;
                }
                if (checkThesis.Trash == true)
                {
                    checkThesis.Follow = true;
                    checkThesis.Trash = false;
                    db.SaveChanges();
                    message = message + "Вы поддержали тезис";
                }
                if (checkThesis.Сonsidered == true && checkThesis.Trash != true && checkThesis.Follow != true)
                {
                    checkThesis.Follow = true;
                    checkThesis.Trash = false;
                    db.SaveChanges();
                    message = message + "Вы поддержали тезис";
                }
            }
            else
            {
                db.UserThesises.Add(new UserThesises() { Follow = true, CreateDate = DateTime.Now, ModifedDate = DateTime.Now, Userid = appUserId,
                 Сonsidered = true, ThesisId = id});
                db.SaveChanges();
                message = message + "Вы поддержали тезис";
            }

            return message + ": " + thisThesis.Description;
        }

        [Authorize]
        public string ThesisNet(int id)
        {
            var appUserId = User.Identity.GetUserId();
            var thisThesis = db.Thesis.SingleOrDefault(s => s.ThesisId == id);
            var checkThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == id);

            string message = "";

            if (checkThesis != null)
            {

                if (checkThesis.Trash == true)
                {
                    message = message + "Вы уже голосовали Против";
                    return message + ": " + thisThesis.Description;
                }
                if (checkThesis.Follow == true)
                {
                    checkThesis.Follow = false;
                    checkThesis.Trash = true;
                    db.SaveChanges();
                    message = message + " Вы проголосовали против";
                }
                if (checkThesis.Сonsidered == true && checkThesis.Trash != true && checkThesis.Follow != true)
                {
                    checkThesis.Follow = false;
                    checkThesis.Trash = true;
                    db.SaveChanges();
                    message = message + " Вы проголосовали против";
                }
            }
            else
            {
                db.UserThesises.Add(new UserThesises()
                {
                    Trash = true,
                    CreateDate = DateTime.Now,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    Сonsidered = true,
                    ThesisId = id
                });
                db.SaveChanges();
                message = message + "Вы проголосовали против тезиса";
            }

            return message + ": " + thisThesis.Description;
        }

        [Authorize]
        public string ThesisNePonyatno(int id)
        {
            var appUserId = User.Identity.GetUserId();
            var thisThesis = db.Thesis.SingleOrDefault(s => s.ThesisId == id);
            var checkThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == id);

            string message = "";

            if (checkThesis != null)
            {
                if (checkThesis.Сonsidered == true && checkThesis.Trash != true && checkThesis.Follow != true)
                {
                    message = message + "Вы уже воздержавались по этому вопросу";
                    return message + ": " + thisThesis.Description;
                }
                if (checkThesis.Follow == true)
                {
                    checkThesis.Follow = false;
                    checkThesis.Trash = false;
                    db.SaveChanges();
                    message = message + " Не понятен смысл тезиса";
                }
                if (checkThesis.Trash == true)
                {
                    checkThesis.Follow = false;
                    checkThesis.Trash = false;
                    db.SaveChanges();
                    message = message + " Не понятен смысл тезиса";
                }

            }
            else
            {
                db.UserThesises.Add(new UserThesises()
                {
                    CreateDate = DateTime.Now,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    Сonsidered = true,
                    ThesisId = id
                });
                db.SaveChanges();
                message = message + "Не понятен смысл тезиса";
            }

            return message + ": " + thisThesis.Description;
        }

        public ActionResult About()
        {

            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Donate()
        {

            return View();
        }

        [Authorize]
        public ActionResult Cabinet()
        {

            return View();
        }

        public ActionResult RoadMap()
        {

            return View();
        }

        public ActionResult PrivacyPolicy()
        {

            return View();
        }

        [Authorize]
        public ActionResult Add(int id = 0)
        {
            var appUserId = User.Identity.GetUserId();

            if (id != 0) {
                return RedirectToAction("AddEdit", new { id });
            }

            var article = db.Article.Add(new Article() { CreateDate = DateTime.Now, ModifedDate = DateTime.Now, UserId = appUserId });
            db.SaveChanges();

            db.ArticleBlock.Add(new ArticleBlock() {ArticleId = article.ArticleId, CreateDate = DateTime.Now, ModifedDate = DateTime.Now,
                Text_ = "Введите текст", Type = 1   });
            db.SaveChanges();

            return RedirectToAction("AddEdit", new { id = article.ArticleId });
        }


        [Authorize]
        //[HttpPost]
        public ActionResult AddEdit(int id = 0)
        {
            var appUserId = User.Identity.GetUserId();

            var allUserThesis = db.UserThesises.ToList();
            var article = db.Article.SingleOrDefault(s => s.ArticleId == id);
            var blocks = db.ArticleBlock.Where(w=> w.ArticleId == id).OrderBy(o => o.Order).ToList(); //

            List<ViewArticleBlock> viewArticleBlock = new List<ViewArticleBlock>();

            foreach (var item in blocks)
            {
                var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == item.ThesisId);
                ViewThesis viewThesis = new ViewThesis() { };
                Thesis Thesis0 = new Thesis() { };

                if (thesis != null)
                {

                    var categoryTagId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 0).Select(s => s.TagId).ToArray();
                    var categoryTag = db.Tags.Where(w => categoryTagId.Contains(w.TagId)).ToList();

                    var thesisTagsId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.TagId).ToArray();
                    var thesisTags = db.Tags.Where(w => thesisTagsId.Contains(w.TagId)).ToList();

                    var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.LinkId).ToArray();
                    var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

                    //viewThesis.CategoryName = thesis.Description;
                    viewThesis.CategoryId = categoryTag.First().TagId;
                    viewThesis.CategoryName = categoryTag.First().text;
                    viewThesis.Tags = thesisTags;
                    viewThesis.Links = thesisLinks;
                    viewThesis.Thesis = thesis;
                    viewThesis.Thesis.Description = thesis.Description;

                    var countThesisPlus = allUserThesis.Where(w => w.ThesisId == thesis.ThesisId && w.Follow == true).Count();
                    var countDnotNow = allUserThesis.Where(w => w.ThesisId == thesis.ThesisId && w.Follow == false && w.Сonsidered == true && w.Trash != true).Count();
                    var countThesisMinus = allUserThesis.Where(w => w.ThesisId == thesis.ThesisId && w.Follow == false && w.Сonsidered == true
                    && w.Trash == true).Count();
                    var stoprocentov = countThesisPlus + countThesisMinus;
                    double plusProcent = 0.0;

                    if (countThesisPlus > 0)
                    {
                        plusProcent = countThesisPlus * 100 / stoprocentov;
                    }

                    viewThesis.CountPlus = countThesisPlus;
                    viewThesis.CountDnotNow = countDnotNow;
                    viewThesis.CountMinus = countThesisMinus;
                    viewThesis.Procent = plusProcent;

                }

                viewArticleBlock.Add(new ViewArticleBlock
                {
                    ArticleId = item.ArticleId,
                    ArticleBlockId = item.ArticleBlockId,
                    ImgPath = item.ImgPath,
                    Order = item.Order,
                    Text_ = item.Text_,
                    Type = item.Type,
                    UserId = item.UserId,
                    Video = item.Video, ViewThesis = viewThesis
                });

            }

            ViewBag.ViewArticleBlock = viewArticleBlock.OrderBy(o=> o.Order).ToList();

            return View(article);
        }

        [Authorize]
        //[HttpPost]
        public ActionResult AddEditBook(int id = 0)
        {
            var appUserId = User.Identity.GetUserId();

            var articles = db.Article.ToList();
            var book = db.Book.SingleOrDefault(s => s.BookId == id);
            var thisArticles = db.BookArticle.Where(w => w.BookId == id).OrderBy(o => o.Order).ToList(); //

            List<ViewArticles> viewArticleBook = new List<ViewArticles>();

            foreach (var item in articles)
            {
                var thisArticle = articles.SingleOrDefault(s => s.ArticleId == item.ArticleId);

                viewArticleBook.Add(new ViewArticles() { ArticleId = item.ArticleId, Title = thisArticle.Title, });


            }

            ViewBag.ViewArticleBook = viewArticleBook.ToList();

            return View(book);
        }


        [Authorize]
        public ActionResult AddBook(int id = 0)
        {
            var appUserId = User.Identity.GetUserId();

            if (id != 0)
            {
                return RedirectToAction("AddEditBook", new { id });
            }

            var book = db.Book.Add(new Book() { CreateDate = DateTime.Now, ModifedDate = DateTime.Now, UserId = appUserId });
            db.SaveChanges();

            db.Book.Add(new Book()
            {
                BookId = book.BookId,
                CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now, Title = "Название книги", UserId = appUserId,

            });
            db.SaveChanges();

            return RedirectToAction("AddEditBook", new { id = book.BookId });
        }

        [HttpPost]
        public int addCategory(int id,  string name )
        {
            var appUserId = User.Identity.GetUserId();

            var book = db.Book.Add(new Book() { CreateDate = DateTime.Now, ModifedDate = DateTime.Now, UserId = appUserId, Title = "Новая книга",
             ParentId = id});
            db.SaveChanges();

            return book.BookId;
        }

        [HttpPost]
        public int addArticle(int id, string name)
        {
            var appUserId = User.Identity.GetUserId();

            var art = db.Article.Add(new Article()
            {
                CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now,
                UserId = appUserId,
                Title = "Новая статья",
            });
            db.SaveChanges();

            db.BookArticle.Add(new BookArticle()
            {
                ArticleId = art.ArticleId,
                BookId = id,
                CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now, UserId = appUserId, Order = 0,
            });
            db.SaveChanges();

            return art.ArticleId;
        }

        public ActionResult Test(int id = 0)
        {

            return View();
        }

        [Authorize]
        public ActionResult Invites()
        {
            var appUserId = User.Identity.GetUserId();

            var myInvites = db.InviteCode.Where(w=> w.UserId == appUserId).ToList();
            var activeInvite = myInvites.Where(w => w.Used == false).ToList();

            var invitedCode = myInvites.Where(w => w.Used == true).Select(s=> s.Code).ToArray();
            var myInvitedUser = db.Users.Where(w => invitedCode.Contains(w.InviteCode));

            List<ViewInviteUser> viewInviteUser = new List<ViewInviteUser>();

            foreach (var item in myInvitedUser)
            {
                var thisInvite = myInvites.SingleOrDefault(s => s.Code == item.InviteCode);
                viewInviteUser.Add(new ViewInviteUser()
                {
                  UserName = item.NickName,  Code = item.InviteCode,  CreateDate = thisInvite.CreateDate, ModifedDate = thisInvite.ModifedDate,
                });
            }

            ViewBag.ViewInviteUser = viewInviteUser;
            ViewBag.ActiveInvite = activeInvite;

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

                        Thread.Sleep(300);

                        db.InviteCode.Add(new InviteCode()
                        {
                            Code = randomCode,
                            Type = 0,
                            Used = false,
                            UserId = user.Id,
                            CreateDate = DateTime.Now,
                            ModifedDate = DateTime.Now,
                        });

                        db.SaveChanges();

                    }

                }

            }

            return "Ok";
        }

        public string delTest()
        {
            var testThesisId = db.ThesisTags.Where(w => w.TagId == 33 || w.TagId == 34 || w.TagId == 35).Distinct().Select(s => s.ThesisId).ToArray();
            var testThesis = db.Thesis.Where(w => testThesisId.Contains(w.ThesisId)).ToList();

            var thesisTags = db.ThesisTags.Where(w => testThesisId.Contains(w.ThesisId)).ToList();
            var thesisLink = db.ThesisLinks.Where(w => testThesisId.Contains(w.ThesisId)).ToList();
            var thesisUser = db.UserThesises.Where(w => testThesisId.Contains(w.ThesisId)).ToList();

            db.Thesis.RemoveRange(testThesis);
            db.ThesisTags.RemoveRange(thesisTags);
            db.ThesisLinks.RemoveRange(thesisLink);
            db.UserThesises.RemoveRange(thesisUser);

            db.SaveChanges();

            return "Ок";
        }

        public string TestError()
        {
            var single = db.Thesis.SingleOrDefault(s=> s.Userid == "c109f3f4-8549-42a2-b025-284a27480828");

            return "OK";
        }

    }
}