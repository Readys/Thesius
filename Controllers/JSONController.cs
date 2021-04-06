using Thesius_001.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Thesius_001.Controllers
{
    public class JSONController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public int CreateThesis(List<Links> links, List<Tags> tags, string userid = "", string description = "Test", int categoryId = 0, string categoryName = "")
        {
            Tags category = new Tags() { };

            var checkEqual = db.Tags.Any(a => a.Type == 0 && a.text.ToLower() == categoryName.ToLower());

            if (categoryName != "" && !checkEqual)
            {
                category.Userid = userid;
                category.text = categoryName;
                category.CreateDate = DateTime.Now;
                category.ModifedDate = DateTime.Now;
                db.Tags.Add(category);
                db.SaveChanges();
                categoryId = category.TagId;
            }

            var thesis = db.Thesis.Add(new Thesis()
            {
                Userid = userid, Description = description, CategoryId = categoryId, CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now,
            });

            //Добавление одобрения тезиса
            var thesisFollow = db.UserThesises.Add(new UserThesises()
            {
                Userid = userid, ThesisId = thesis.ThesisId, Сonsidered = true,  Follow = true, Trash = false, 
                CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now, Deleted = 0,
            });

            db.SaveChanges();

            //Добавление тэгов
            tags.RemoveAt(1);
            int t = 1;
            foreach (var tag in tags)
            {
                db.ThesisTags.Add(new ThesisTags()
                {
                    ThesisId = thesis.ThesisId, TagId = tag.TagId, Order = t, Type = tag.Type,

                });
                db.SaveChanges();
                t++;
            }

            //Добавление ссылок
            if (links != null) {
                int l = 1;
                foreach (var link in links)
                {
                    db.ThesisLinks.Add(new ThesisLinks()
                    {
                        ThesisId = thesis.ThesisId,
                        LinkId = link.LinkId,
                        Order = l,

                    });
                    db.SaveChanges();
                    l++;
                }
            }
            return thesis.ThesisId;
        }

        [HttpPost]
        public int CreateTag(string userid = "", string name = "Test", int typeTag = 1)
        {
            var checkEqual = db.Tags.Any(a => a.Type == typeTag && a.text.ToLower() == name.ToLower());
         
            if (!checkEqual)
            {
                var tag = db.Tags.Add(new Tags()
                {
                    Userid = userid,
                    text = name,
                    CreateDate = DateTime.Now,
                    ModifedDate = DateTime.Now,
                    Type = typeTag,
                });

                db.SaveChanges();
                return tag.TagId;
            }
            else
            {
                return -1;
            }

        }

        [HttpPost]
        public JsonResult GetSimularTag(string search, int typeTag = 1)
        {
            var tagArr = db.Tags.Where(w => w.Type == typeTag && w.text.ToLower().Contains(search.ToLower())).ToList();
            var tags = tagArr.Select(s => new { TagId = s.TagId, Name = s.text }).ToList();
            db.SaveChanges();
            return Json(new { tags }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetThesisForMe()
        {
            var appUserId = User.Identity.GetUserId();

            var allCategoryTags = db.Tags.Where(w => w.Type == 0).ToList();

            var thesisIdForMe = db.Thesis.Where(w => w.Userid == appUserId).Select(s=> s.ThesisId).ToArray();
            var tagsIdForMe = db.ThesisTags.Where(w => thesisIdForMe.Contains(w.ThesisId)).Select(s=> s.TagId).ToArray();
            var categoryForMe = allCategoryTags.Where(w => tagsIdForMe.Contains(w.TagId)).ToList();

            List<ViewCategoryThesis> categoryList = new List<ViewCategoryThesis>();

            foreach (var item in categoryForMe)
            {
                var thesisForCategoryId = db.ThesisTags.Where(w => w.TagId == item.TagId).Select(s => s.ThesisId).ToArray();
                var thesisForCategory = db.Thesis.Where(w => thesisForCategoryId.Contains(w.ThesisId)).ToList();

                var thesisFollowId = db.UserThesises.Where(w => w.Follow == true).Select(s => s.ThesisId).ToArray();
                thesisForCategory = thesisForCategory.Where(w => thesisFollowId.Contains(w.ThesisId)).ToList();

                List<ViewThesis> viewThesisList = new List<ViewThesis>();

                foreach (var thesis in thesisForCategory)
                {
                    var thesisTagsId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 1).Select(s => s.TagId).ToArray();
                    var thesisTags = db.Tags.Where(w=> thesisTagsId.Contains(w.TagId)).ToList();

                    var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.LinkId).ToArray();
                    var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

                    viewThesisList.Add(new ViewThesis()
                    {
                         Thesis = thesis,  Tags = thesisTags, Links = thesisLinks
                    });

                }

                categoryList.Add(new ViewCategoryThesis()
                {
                      CategoryId = item.TagId, CategoryName = item.text, ViewThesis = viewThesisList
                });

            }
            return Json(new { categoryList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string MinusThesis(string userid, int thesisId)
        {
            userid = User.Identity.GetUserId();

            var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == userid && w.ThesisId == thesisId);
            thisUserFollowThesis.Follow = false;
            thisUserFollowThesis.Сonsidered = true;
            thisUserFollowThesis.Trash = false;
            db.SaveChanges();
            return "Ok";
        }

        [HttpPost]
        public string TrashThesis(string userid, int thesisId)
        {
            userid = User.Identity.GetUserId();

            var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == userid && w.ThesisId == thesisId);
            thisUserFollowThesis.Follow = false;
            thisUserFollowThesis.Сonsidered = true;
            thisUserFollowThesis.Trash = true;
            db.SaveChanges();
            return "Ok";
        }

        [HttpGet]
        public string TestMail()
        {
            List<string> toList = new List<string>();
            toList.Add("armyideas@gmail.com");
            Tools.Helper.SendMail("", toList, "Подтверждение электронной почты", "Тест с локалки");
            return "Ok";
        }





        //[HttpPost]
        //public JsonResult CreateThesis(string userid = "", string description = "Test", string links = "test", int categoryId = 0, int firstTagId = 1)
        //{
        //    var thesis = db.Thesis.Add(new Thesis()
        //    {
        //        Userid = userid,
        //        Description = description,
        //        Links = links,
        //        CategoryId = categoryId,
        //        FirstTagId = firstTagId
        //    });

        //    db.SaveChanges();

        //    return Json(thesis.ThesisId, JsonRequestBehavior.AllowGet);
        //}

    }
}