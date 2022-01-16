using Thesius_001.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Thesius_001.Tools;
using System.Text.RegularExpressions;

namespace Thesius_001.Controllers
{
    public class JSONController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Сборщик консенсуса
        [HttpGet]
        public JsonResult GetConsensus()
        {
            List<ViewConsensus> viewConsensusList = new List<ViewConsensus>();

            ViewConsensus viewConsensus10095 = new ViewConsensus() {  ConsensusId = 1 , ConsensusName = "Сильный Консенсус", ConsensusValueMax = 100,
             ConsensusValueMin = 95};
            ViewConsensus viewConsensus9580 = new ViewConsensus()
            {
                ConsensusId = 2,
                ConsensusName = "Приблизительный Консенсус",
                ConsensusValueMax = 95,
                ConsensusValueMin = 80
            };
            ViewConsensus viewConsensus8060 = new ViewConsensus()
            {
                ConsensusId = 3,
                ConsensusName = "Слабый Консенсус",
                ConsensusValueMax = 80,
                ConsensusValueMin = 60
            };
            ViewConsensus viewConsensus6050 = new ViewConsensus()
            {
                ConsensusId = 4,
                ConsensusName = "Раскол",
                ConsensusValueMax = 60,
                ConsensusValueMin = 50
            };
            ViewConsensus viewConsensus50 = new ViewConsensus()
            {
                ConsensusId = 5,
                ConsensusName = "Не принято",
                ConsensusValueMax = 50,
                ConsensusValueMin = 0
            };

            List<ViewThesis> thesis1 = new List<ViewThesis>();
            List<ViewThesis> thesis2 = new List<ViewThesis>();
            List<ViewThesis> thesis3 = new List<ViewThesis>();
            List<ViewThesis> thesis4 = new List<ViewThesis>();
            List<ViewThesis> thesis5 = new List<ViewThesis>();

            var allThesis = db.Thesis.ToList();
            var allUserThesis = db.UserThesises.ToList();
            var allTags = db.Tags.ToList();

            foreach (var item in allThesis)
            {
                var countThesisPlus = allUserThesis.Where(w => w.ThesisId == item.ThesisId && w.Follow == true).Count();
                var countDnotNow = allUserThesis.Where(w => w.ThesisId == item.ThesisId && w.Follow == false && w.Сonsidered == true && w.Trash != true).Count();
                var countThesisMinus = allUserThesis.Where(w => w.ThesisId == item.ThesisId && w.Follow == false && w.Сonsidered == true
                && w.Trash == true).Count();
                var stoprocentov = countThesisPlus + countThesisMinus;
                double plusProcent = 0.0;

                if (countThesisPlus > 0)
                {
                    plusProcent = countThesisPlus * 100 / stoprocentov;
                }

                var thesisTagsId = db.ThesisTags.Where(w => w.ThesisId == item.ThesisId).Select(s => s.TagId).ToArray();
                var thesisTags = allTags.Where(w => w.Type == 1 && thesisTagsId.Contains(w.TagId)).ToList();

                var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == item.ThesisId).Select(s => s.LinkId).ToArray();
                var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

                var categoryName = allTags.FirstOrDefault(f => f.Type == 0 && f.TagId == item.CategoryId);

                var viewThesis = new ViewThesis
                {
                    Thesis = item,
                    CategoryId = item.CategoryId,
                    Tags = thesisTags,
                    CategoryName = categoryName.text,
                    Links = thesisLinks,
                    CountPlus = countThesisPlus,
                    CountMinus = countThesisMinus,
                    CountDnotNow = countDnotNow, Procent = plusProcent,
                };

                if (plusProcent >= 95)
                {
                    thesis1.Add(viewThesis);
                }
                if (plusProcent <= 95 && plusProcent > 80)
                {
                    thesis2.Add(viewThesis);
                }
                if (plusProcent <= 80 && plusProcent > 60)
                {
                    thesis3.Add(viewThesis);
                }
                if (plusProcent <= 60 && plusProcent > 50)
                {
                    thesis4.Add(viewThesis);
                }
                if (plusProcent <= 50)
                {
                    thesis5.Add(viewThesis);
                }
            }

            viewConsensus10095.ViewThesis = thesis1;
            viewConsensus9580.ViewThesis = thesis2;
            viewConsensus8060.ViewThesis = thesis3;
            viewConsensus6050.ViewThesis = thesis4;
            viewConsensus50.ViewThesis = thesis5;

            viewConsensusList.Add(viewConsensus10095);
            viewConsensusList.Add(viewConsensus9580);
            viewConsensusList.Add(viewConsensus8060);
            viewConsensusList.Add(viewConsensus6050);
            viewConsensusList.Add(viewConsensus50);

            return Json(new { viewConsensusList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int CreateThesis(List<Links> links, List<Tags> tags, string userid = "", string description = "Test", int categoryId = 0, 
            string categoryName = "", int id = 0, int articleBlockId = 0)//, int articleId = 0, int order = 0
        {
            var appUserId = User.Identity.GetUserId();
            Tags category = new Tags() { };

            var checkEqual = db.Tags.Any(a => a.Type == 0 && a.text.ToLower() == categoryName.ToLower());

            Thesis thesis = new Thesis() { };

            if (userid == "") { userid = appUserId; }

            if (id == 0) {

                if (categoryId > 0) {
                    thesis = db.Thesis.Add(new Thesis()
                    {
                        Userid = userid,
                        Description = description,
                        CategoryId = categoryId,
                        CreateDate = DateTime.Now,
                        ModifedDate = DateTime.Now,
                    });

                    db.SaveChanges();
                    id = thesis.ThesisId;

                }
                else
                {
                    return -2;
                }

                if(articleBlockId != 0)
                {
                    var articleBlock = db.ArticleBlock.SingleOrDefault(s => s.ArticleBlockId == articleBlockId);
                    articleBlock.ThesisId = id;
                    articleBlock.Text_ = thesis.Description;
                    db.SaveChanges();
                    //db.ArticleBlock.Add(new ArticleBlock() { ThesisId = id, ArticleBlockId = articleBlockId, ArticleId = articleId, Order = order,
                    //  Text_ = thesis.Description,  CreateDate = DateTime.Now, ModifedDate = DateTime.Now,  Type = 2, UserId = appUserId, 
                    //});
                }

                //Добавление одобрения тезиса
                var thesisFollow = db.UserThesises.Add(new UserThesises()
                {
                    Userid = userid,
                    ThesisId = thesis.ThesisId,
                    Сonsidered = true,
                    Follow = true,
                    Trash = false,
                    CreateDate = DateTime.Now,
                    ModifedDate = DateTime.Now,
                    Deleted = 0,
                    My = true,
                });

                db.SaveChanges();

                //Добавление тэгов
                //tags.RemoveAt(0);
                int t = 0;
                var tagtype = 0;
                foreach (var tag in tags)
                {
                    db.ThesisTags.Add(new ThesisTags()
                    {
                        ThesisId = thesis.ThesisId,
                        TagId = tag.TagId,
                        Order = t,
                        Type = tagtype,
                    });
                    db.SaveChanges();
                    tagtype = 1;
                    t++;
                }

                if (categoryName != "" && !checkEqual)
                {
                    category.Userid = userid;
                    category.text = categoryName;
                    category.CreateDate = DateTime.Now;
                    category.ModifedDate = DateTime.Now;
                    category.Type = 0;
                    db.Tags.Add(category);
                    db.SaveChanges();
                    categoryId = category.TagId;
                }

                //Добавление ссылок
                if (links != null)
                {
                    int l = 1;
                    foreach (var link in links)
                    {

                        var lk = db.Links.Add(new Links()
                            {
                               Name = link.Name, Body = link.Body, Userid = userid, CreateDate = DateTime.Now, Deleted = 0, ModifedDate = DateTime.Now, 
                            });

                        db.SaveChanges();

                        db.ThesisLinks.Add(new ThesisLinks()
                        {
                            ThesisId = thesis.ThesisId,
                            LinkId = lk.LinkId,
                            Order = l,

                        });

                        db.SaveChanges();
                        l++;
                    }
                }

            }
            else
            {
                var thesis2 = db.Thesis.SingleOrDefault(s => s.ThesisId == id);
                var userthesis = db.UserThesises.Where(w => w.ThesisId == id).ToList();

                var oldThesisTags = db.ThesisTags.Where(w => w.ThesisId == id && w.Type != 0).ToList();
                db.ThesisTags.RemoveRange(oldThesisTags);
                var oldThesisLinks = db.ThesisLinks.Where(w => w.ThesisId == id).ToList();
                db.ThesisLinks.RemoveRange(oldThesisLinks);
                db.SaveChanges();

                if (tags != null)
                {

                    //if (categoryName != "" && !checkEqual)
                    //{
                    //    category.Userid = userid;
                    //    category.text = categoryName;
                    //    category.CreateDate = DateTime.Now;
                    //    category.ModifedDate = DateTime.Now;
                    //    category.Type = 0;
                    //    db.Tags.Add(category);
                    //    db.SaveChanges();
                    //    categoryId = category.TagId;
                    //}

                    //Добавление тэгов
                    tags.RemoveAt(0);
                    int t = 1;
                    foreach (var tag in tags)
                    {
                        db.ThesisTags.Add(new ThesisTags()
                        {
                            ThesisId = thesis2.ThesisId,
                            TagId = tag.TagId,
                            Order = t,
                            Type = 1,

                        });
                        db.SaveChanges();
                        t++;
                    }

                    //var oldTagsId = oldThesisTags.Where(w => w.ThesisId == id).Select(s => s.TagId).ToArray();
                    //var oldTags = db.Tags.Where(w => oldTagsId.Contains(w.TagId)).ToList();
                    //var tagOrderMax = oldThesisTags.Max(m => m.Order);


                }

                //Добавление ссылок
                if (links != null)
                {
                    int l = 1;
                    foreach (var link in links)
                    {
                        var lk = db.Links.Add(new Links()
                        {
                            Name = link.Name,
                            Body = link.Body,
                            Userid = userid,
                            CreateDate = DateTime.Now,
                            Deleted = 0,
                            ModifedDate = DateTime.Now,
                        });

                        db.SaveChanges();

                        db.ThesisLinks.Add(new ThesisLinks()
                        {
                            ThesisId = thesis2.ThesisId,
                            LinkId = lk.LinkId,
                            Order = l,

                        });

                        db.SaveChanges();
                        l++;
                    }
                }
                
                if (userthesis.Count() == 0)
                {
                    if (thesis2.Description != description)
                    {
                        thesis2.Description = description;
                        db.SaveChanges();
                    }
                }
                else if(userthesis.Count() == 1)
                {
                    var checkAuthor = userthesis.Any(f => f.Userid == appUserId);
                    if (checkAuthor)
                    {
                        if (thesis2.Description != description)
                        {
                            thesis2.Description = description;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }


            return id;
        }

        [HttpPost]
        public string EditThesis(int id, string description, Tags tags, Links links)
        {
            var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == id);
            var userthesis = db.UserThesises.Where(w => w.ThesisId == id).ToList();
            
            if (userthesis.Count() == 0)
            {
                if (thesis.Description != description) {
                    thesis.Description = description;
                }
            }

            return "Ok";
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

        //[HttpPost]
        //public int ArticleCreateTag(string userid = "", string name = "Test", int typeTag = 1)
        //{
        //    var checkEqual = db.Tags.Any(a => a.Type == typeTag && a.text.ToLower() == name.ToLower());

        //    if (!checkEqual)
        //    {
        //        var tag = db.Tags.Add(new Tags()
        //        {
        //            Userid = userid,
        //            text = name,
        //            CreateDate = DateTime.Now,
        //            ModifedDate = DateTime.Now,
        //            Type = typeTag,
        //        });

        //        db.SaveChanges();
        //        return tag.TagId;
        //    }
        //    else
        //    {
        //        return -1;
        //    }

        //}

        [HttpPost]
        public JsonResult GetSimularTag(string search, int typeTag = 1)
        {
            var tagArr = db.Tags.Where(w => w.Type == typeTag && w.text.ToLower().Contains(search.ToLower())).ToList();
            var tags = tagArr.Select(s => new { TagId = s.TagId, Name = s.text }).ToList();
            db.SaveChanges();
            return Json(new { tags }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThesisTag(int id)
        {
            var tagArr = db.ThesisTags.Where(w => w.ThesisId == id).Select(s=> s.TagId).ToArray();
            var tags = db.Tags.Where(w => tagArr.Contains(w.TagId)).ToList();
            return Json(new { tags }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetThesisForMe()
        {
            var appUserId = User.Identity.GetUserId();

            var allThesisTags = db.ThesisTags.ToList();

            var allCategoryTags = db.Tags.Where(w => w.Type == 0).ToList();

            var allMyUserThesises = db.UserThesises.Where(w => w.Userid == appUserId || w.Follow == true).ToList();
            var thesisId = allMyUserThesises.Where(w=> w.Userid == appUserId && w.Follow == true).Select(s => s.ThesisId).ToArray();

            var tagsIdForMe = allThesisTags.Where(w => thesisId.Contains(w.ThesisId)).Select(s=> s.TagId).ToArray();
            var categoryForMe = allCategoryTags.Where(w => tagsIdForMe.Contains(w.TagId)).OrderByDescending(o=> o.CreateDate).ToList();

            categoryForMe.Distinct();

            List<ViewCategoryThesis> categoryList = new List<ViewCategoryThesis>();

            foreach (var item in categoryForMe)
            {
                var thesisForCategoryId = allThesisTags.Where(w => w.TagId == item.TagId).Select(s => s.ThesisId).ToArray();
                var thesisForCategory = db.Thesis.Where(w => thesisForCategoryId.Contains(w.ThesisId)).ToList();

                thesisForCategory = thesisForCategory.Where(w => thesisId.Contains(w.ThesisId)).ToList();

                List<ViewThesis> viewThesisList = new List<ViewThesis>();

                foreach (var thesis in thesisForCategory)
                {

                    //var thesisTagsId = allThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 1).OrderBy(o => o.Order).Select(s => s.TagId).ToArray();
                    ////var thesisTags = db.Tags.Where(w=> thesisTagsId.Contains(w.TagId)).ToList();

                    //var thesisTags = allThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 1).OrderBy(o => o.Order).ToList();

                    //List<Tags> tags = new List<Tags>();
                    //foreach (var tt in thesisTags)
                    //{
                    //    var tag = db.Tags.SingleOrDefault(s => s.TagId == tt.TagId);
                    //    tags.Add(new Tags()
                    //    {
                    //         TagId = tag.TagId, CreateDate = tag.CreateDate, Deleted = tag.Deleted, Description = tag.Description, ModifedDate = tag.ModifedDate,
                    //          text = tag.text, Type = 1, Userid = tag.Userid
                    //    });
                    //}

                    var my = false;
                    var checkMy = allMyUserThesises.SingleOrDefault(s=> s.ThesisId == thesis.ThesisId && s.Userid == appUserId);
                    if (checkMy != null && checkMy.My ) {
                        my = true;
                    }

                    //var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.LinkId).ToArray();
                    //var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

                    //viewThesisList.Add(new ViewThesis()
                    //{
                    //     Thesis = thesis,  Tags = tags, Links = thesisLinks, My = my,
                    //});

                }

                categoryList.Add(new ViewCategoryThesis()
                {
                      CategoryId = item.TagId, CategoryName = item.text, OpenClose = false, //ViewThesis = viewThesisList
                });

            }
            return Json(new { categoryList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRandomThesis()
        {
            var appUserId = User.Identity.GetUserId();

            var myThesisId = db.UserThesises.Where(w => w.Userid == appUserId).Select(s => s.ThesisId).ToArray();

            //var thesisId = db.UserThesises.Where(w => w.Userid != appUserId).Select(s => s.ThesisId).ToArray();
            var thesisList = db.Thesis.Where(w => !myThesisId.Contains(w.ThesisId)).ToList();

            thesisList.Shuffle();

            if (thesisList.Count() >= 10)
            {
                var r = thesisList.Count() - 10;
                thesisList.RemoveRange(10 - 1, r);
            }

            List<ViewThesis> viewThesisList = new List<ViewThesis>();

            foreach (var thesis in thesisList)
            {
                var categoryTagId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 0).Select(s => s.TagId).ToArray();
                var categoryTag = db.Tags.Where(w => categoryTagId.Contains(w.TagId)).ToList();

                var thesisTagsId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 1).Select(s => s.TagId).ToArray();
                var thesisTags = db.Tags.Where(w => thesisTagsId.Contains(w.TagId)).ToList();

                var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.LinkId).ToArray();
                var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

                viewThesisList.Add(new ViewThesis()
                {
                    Thesis = thesis,
                    Tags = thesisTags,
                    Links = thesisLinks, CategoryId = categoryTag.First().TagId, CategoryName = categoryTag.First().text
                });
            }


            return Json(new { viewThesisList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetThesisForCategory(int id = 0) {

            var appUserId = User.Identity.GetUserId();

            var thesisList = db.Thesis.Where(w => w.CategoryId == id).ToList();

            List<ViewThesis> viewThesisList = new List<ViewThesis>();

            foreach (var thesis in thesisList)
            {
                var categoryTagId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 0).Select(s => s.TagId).ToArray();
                var categoryTag = db.Tags.Where(w => categoryTagId.Contains(w.TagId)).ToList();

                var thesisTagsId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 1).Select(s => s.TagId).ToArray();
                var thesisTags = db.Tags.Where(w => thesisTagsId.Contains(w.TagId)).ToList();

                var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.LinkId).ToArray();
                var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

                bool my = false;

                if (thesis.Userid == appUserId)
                {
                    my = true;
                }

                viewThesisList.Add(new ViewThesis()
                {
                    Thesis = thesis,
                    Tags = thesisTags,
                    Links = thesisLinks,
                    CategoryId = categoryTag.First().TagId,
                    CategoryName = categoryTag.First().text, My = my,
                });
            }
            return Json(new { viewThesisList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string PlusThesis(int thesisId)
        {
            var thesis = db.Thesis.SingleOrDefault(s=> s.ThesisId == thesisId);
            var userid = thesis.Userid;

            var appUserId = User.Identity.GetUserId();

            if (appUserId == userid)
            {
                var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == userid && w.ThesisId == thesisId);
                thisUserFollowThesis.Follow = true;
                thisUserFollowThesis.Сonsidered = true;
                thisUserFollowThesis.Trash = false;
                db.SaveChanges();
            }
            else
            {
                db.UserThesises.Add(new UserThesises
                {
                     CreateDate = DateTime.Now, Follow = true, ModifedDate = DateTime.Now, Trash = false, Userid = appUserId, ThesisId = thesisId,
                     Сonsidered = true
                });
                db.SaveChanges();
            }


            return "Ok";
        }

        [HttpPost]
        public string MinusThesis(int thesisId)
        {
            var appUserId = User.Identity.GetUserId();

            var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == thesisId);
            var authorUserid = thesis.Userid;
            var followThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == thesisId);
            //Редактирование своего тезиса или ранее расмотренного тезиса
            if (authorUserid == appUserId || followThesis != null)
            {
                if(followThesis != null)
                {
                    authorUserid = followThesis.Userid;
                }
                var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == authorUserid && w.ThesisId == thesisId);
                thisUserFollowThesis.Follow = false;
                thisUserFollowThesis.Сonsidered = true;
                thisUserFollowThesis.Trash = true;
                db.SaveChanges();
            }
            else
            {
                db.UserThesises.Add(new UserThesises
                {
                    CreateDate = DateTime.Now,
                    Сonsidered = true,
                    Follow = false,
                    Trash = true,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    ThesisId = thesisId,

                });
                db.SaveChanges();
            }

            return "Ok";
        }

        [HttpPost]
        public int MinusThesis2(int thesisId)
        {
            var appUserId = User.Identity.GetUserId();

            var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == thesisId);
            var authorUserid = thesis.Userid;
            var followThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == thesisId);
            //Редактирование своего тезиса или ранее расмотренного тезиса
            if (authorUserid == appUserId || followThesis != null)
            {

                if (followThesis.MyMark != -1)
                {
                    followThesis.MyMark--;
                    db.SaveChanges();
                }
                //if (followThesis != null)
                //{
                //    authorUserid = followThesis.Userid;
                //}
                //var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == authorUserid && w.ThesisId == thesisId);

                //if (thisUserFollowThesis.Сonsidered = true && thisUserFollowThesis.Follow == false)
                //{
                //    myMark = -1;
                //}

                //thisUserFollowThesis.Follow = false;
                //thisUserFollowThesis.Сonsidered = true;
                //thisUserFollowThesis.Trash = true;
                //db.SaveChanges();
                return followThesis.MyMark;
            }
            else
            {
                db.UserThesises.Add(new UserThesises
                {
                    CreateDate = DateTime.Now,
                    Сonsidered = true,
                    Follow = false,
                    Trash = true,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    ThesisId = thesisId,
                    MyMark = -1,

                });
                db.SaveChanges();

                return -1;
            }

            return -1;
        }


        [HttpPost]
        public int PlusThesis2(int thesisId)
        {
            var appUserId = User.Identity.GetUserId();

            var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == thesisId);
            var authorUserid = thesis.Userid;
            var followThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == thesisId);
            //Редактирование своего тезиса или ранее расмотренного тезиса
            if (authorUserid == appUserId || followThesis != null)
            {

                if (followThesis.MyMark != 1)
                {
                    followThesis.MyMark++;
                    db.SaveChanges();
                }
                return followThesis.MyMark;
            }
            else
            {
                db.UserThesises.Add(new UserThesises
                {
                    CreateDate = DateTime.Now,
                    Сonsidered = true,
                    Follow = false,
                    Trash = true,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    ThesisId = thesisId,
                    MyMark = 1,

                });
                db.SaveChanges();

                return 1;
            }

        }


        [HttpPost]
        public string IDoNotNowThesis(int thesisId)
        {
            var appUserId = User.Identity.GetUserId();

            var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == thesisId);
            var authorUserid = thesis.Userid;
            var followThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == thesisId);
            //Редактирование своего тезиса или ранее расмотренного тезиса
            if (authorUserid == appUserId || followThesis != null)
            {
                if (followThesis != null)
                {
                    authorUserid = followThesis.Userid;
                }

                var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == authorUserid && w.ThesisId == thesisId);
                thisUserFollowThesis.Сonsidered = true;
                thisUserFollowThesis.Follow = false;
                thisUserFollowThesis.Trash = false;
                db.SaveChanges();
            }
            else
            {
                db.UserThesises.Add(new UserThesises
                {
                    CreateDate = DateTime.Now,
                    Сonsidered = true,
                    Follow = false,
                    Trash = false,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    ThesisId = thesisId,

                });
                db.SaveChanges();
            }

            return "Ok";
        }

        [HttpPost]
        public string TrashThesis(int thesisId)
        {
            var appUserId = User.Identity.GetUserId();

            var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == thesisId);
            var authorUserid = thesis.Userid;
            var followThesis = db.UserThesises.SingleOrDefault(s => s.Userid == appUserId && s.ThesisId == thesisId);
            //Редактирование своего тезиса или ранее расмотренного тезиса
            if (authorUserid == appUserId || followThesis != null)
            {
                if (followThesis != null)
                {
                    authorUserid = followThesis.Userid;
                }

                var thisUserFollowThesis = db.UserThesises.SingleOrDefault(w => w.Userid == authorUserid && w.ThesisId == thesisId);
                thisUserFollowThesis.Сonsidered = true;
                thisUserFollowThesis.Follow = false;
                thisUserFollowThesis.Trash = false;
                db.SaveChanges();
            }
            else
            {
                db.UserThesises.Add(new UserThesises
                {
                    CreateDate = DateTime.Now,
                    Сonsidered = true,
                    Follow = false,
                    Trash = false,
                    ModifedDate = DateTime.Now,
                    Userid = appUserId,
                    ThesisId = thesisId,

                });
                db.SaveChanges();
            }
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

        [HttpPost]
        public bool CheckInvite(string inviteCode)
        {
            using (var db = new ApplicationDbContext())
            {
                var findThisCode = db.InviteCode.SingleOrDefault(s => s.Code == inviteCode && s.Used == false && s.Type == 0);
                if (findThisCode != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [HttpPost]
        public string LinkDelete(int id)
        {
            var link = db.Links.SingleOrDefault(s => s.LinkId == id);
            var linkthesis = db.ThesisLinks.Where(w => w.LinkId == id).ToList();

            db.Links.Remove(link);
            db.ThesisLinks.RemoveRange(linkthesis);

            db.SaveChanges();

            return "Ok";
        }

        [HttpPost]
        public JsonResult SimularThesis(string text)
        {
            var thesises = db.Thesis.ToList();

            List<Thesis> thesisList = new List<Thesis>();

            var textLength = text.Length;

            foreach (var item in thesises)
            {
                if (item.Description.Length < textLength) { textLength = item.Description.Length; }

                var equal = Helper.LevenshteinDistance(text, item.Description.Substring(0, textLength));

                if (equal < 5)
                {
                    thesisList.Add(item);
                }
            }


            return Json(new { thesisList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SelectThesis(int id, int idBlock)
        {
            var allUserThesis = db.UserThesises.ToList();

            var thesis = db.Thesis.SingleOrDefault(s=> s.ThesisId == id);
            var block = db.ArticleBlock.SingleOrDefault(s => s.ArticleBlockId == idBlock);

            ViewThesis viewThesis = new ViewThesis() { };

            if (thesis != null)
            {

                var categoryTagId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 0).Select(s => s.TagId).ToArray();
                var categoryTag = db.Tags.Where(w => categoryTagId.Contains(w.TagId)).ToList();

                var thesisTagsId = db.ThesisTags.Where(w => w.ThesisId == thesis.ThesisId && w.Type == 1).Select(s => s.TagId).ToArray();
                var thesisTags = db.Tags.Where(w => thesisTagsId.Contains(w.TagId)).ToList();

                var thesisLinkId = db.ThesisLinks.Where(w => w.ThesisId == thesis.ThesisId).Select(s => s.LinkId).ToArray();
                var thesisLinks = db.Links.Where(w => thesisLinkId.Contains(w.LinkId)).ToList();

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

                viewThesis.CategoryName = thesis.Description;
                viewThesis.CategoryId = categoryTag.First().TagId;
                viewThesis.CategoryName = categoryTag.First().text;
                viewThesis.Tags = thesisTags;
                viewThesis.Links = thesisLinks;
                viewThesis.Thesis = thesis;
                viewThesis.CountPlus = countThesisPlus;
                viewThesis.CountMinus = countThesisMinus;
                viewThesis.CountDnotNow = countDnotNow;
                viewThesis.Procent = plusProcent;

                block.Text_ = thesis.Description;
                block.ThesisId = thesis.ThesisId;
                db.SaveChanges();

            }


            return Json(new { viewThesis }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string GetNickName()
        {
            var appUserId = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(s => s.Id == appUserId);
            return user.NickName;
        }

        #region Article
        [HttpPost]
        public string AddTitleArticle(int id, string articleTitle)
        {
            var appUserId = User.Identity.GetUserId();

            var article = db.Article.SingleOrDefault(s => s.ArticleId == id);
            article.Title = articleTitle;
            db.SaveChanges();

            return "Ok";
        }

        [HttpGet]
        public JsonResult Publications()
        {
            var appUserId = User.Identity.GetUserId();

            var allUserThesis = db.UserThesises.ToList();

            var articles = db.Article.ToList();
            if (articles.Count() > 10) {
                articles.RemoveRange(10, articles.Count() - 1);
            }

            List<ViewArticles> viewArticles = new List<ViewArticles>();

            foreach (var item in articles)
            {
                var blocks = db.ArticleBlock.Where(f => f.ArticleId == item.ArticleId).OrderBy(o=> o.Order).ToList();
                var userNick = db.Users.FirstOrDefault(f=> f.Id == item.UserId);

                List<ViewArticleBlock> viewArticlesBlock = new List<ViewArticleBlock>();

                foreach (var block in blocks)
                {
                    ViewThesis viewThesis = new ViewThesis() { };

                    var thesis = db.Thesis.SingleOrDefault(s => s.ThesisId == block.ThesisId);

                    if(block.Type == 2)
                    {
                        var thesisMark = db.UserThesises.SingleOrDefault(s=> s.ThesisId == block.ThesisId && s.Userid == appUserId);

                        var countThesisPlus = allUserThesis.Where(w => w.ThesisId == block.ThesisId && w.Follow == true).Sum(s => s.MyMark);
                        var countDnotNow = allUserThesis.Where(w => w.ThesisId == block.ThesisId && w.Follow == false && w.Сonsidered == true && w.Trash != true).Count();
                        var countThesisMinus = allUserThesis.Where(w => w.ThesisId == block.ThesisId && w.Follow == false && w.Сonsidered == true
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
                        viewThesis.Thesis = thesis;
                        //viewThesis.ShowOther = false;

                        if (thesisMark != null)
                        {
                            viewThesis.MyMark = thesisMark.MyMark;
                        }


                        //if (thesisMark.Сonsidered == true && thesisMark.Follow == true) {
                        //    viewThesis.MyMark = 1;
                        //}
                        //else if(thesisMark.Сonsidered == true && thesisMark.Follow == false)
                        //{
                        //    viewThesis.MyMark = 1;
                        //}
                        //else
                        //{
                        //    viewThesis.MyMark = 0;
                        //}



                        if (appUserId == block.UserId)
                        {
                            viewThesis.My = true;
                        }
                        
                    }

                    string noHTML = Regex.Replace(block.Text_, @"<[^>]+>|&nbsp;", "").Trim();

                    viewArticlesBlock.Add(new ViewArticleBlock() {
                         ArticleId = block.ArticleId, CreateDate = block.CreateDate, ImgPath = block.ImgPath, ModifedDate = block.ModifedDate,
                          Order = block.Order, Text_ = noHTML, Type = block.Type, UserId = block.UserId, UserName = block.UserId,
                           Video = block.Video, ViewThesis = viewThesis, 
                    });

                }

                viewArticles.Add(new ViewArticles()
                {
                    ArticleId = item.ArticleId,
                    Text_ = viewArticlesBlock.First().Text_, Title = item.Title, UserId = item.UserId, UserName = userNick.NickName, CreateDate = item.CreateDate,
                     ViewArticleBlock = viewArticlesBlock, ShortView = 0, 
                });


            }

            return Json(new { viewArticles }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Books()
        {
            var appUserId = User.Identity.GetUserId();

            var books = db.Book.ToList();

            var articles = db.Article.ToList();

            List<ViewBook> viewBooks = new List<ViewBook>();

            foreach (var item in books)
            {
                var thisArticles = db.BookArticle.Where(w => w.BookId == item.BookId);

                List<ViewArticles> viewArticles = new List<ViewArticles>();

                foreach (var article in thisArticles)
                {
                    var thisArticle = articles.SingleOrDefault(s => s.ArticleId == article.ArticleId);

                    viewArticles.Add(new ViewArticles() { ArticleId = article.ArticleId, Title = thisArticle.Title });
                }

                viewBooks.Add(new ViewBook() { BookId = item.BookId, Title = item.Title, ViewArticles = viewArticles, UserId = appUserId});
            }

            return Json(new { viewBooks }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ThesisUpdateCount(int id) {

            ViewThesis viewThesis = new ViewThesis() { };

            var allUserThesis = db.UserThesises.ToList();
            var thesisMark = db.UserThesises.SingleOrDefault(s => s.ThesisId == id);

            var allThisThesis = allUserThesis.Where(w => w.ThesisId == id && w.Сonsidered == true && (w.Follow == true || w.Trash == true )).ToList();
            var countThesisPlus = allThisThesis.Where(w => w.ThesisId == id && w.Follow == true).Count();
            var countThesisMinus = allThisThesis.Where(w => w.ThesisId == id && w.Trash == true).Count();
            var countDnotNow = allThisThesis.Where(w => w.ThesisId == id && w.Trash != true && w.Follow != true).Count();


            var stoProcentov = allThisThesis.Count();
            double plusProcent = 0.0;

            if (countThesisPlus > 0)
            {
                plusProcent = countThesisPlus * 100 / stoProcentov;
            }

            viewThesis.CountPlus = countThesisPlus;
            viewThesis.CountDnotNow = countDnotNow;
            viewThesis.CountMinus = countThesisMinus;
            viewThesis.Procent = plusProcent;
            viewThesis.MyMark = thesisMark.MyMark;

            return Json(new { viewThesis }, JsonRequestBehavior.AllowGet);
        }

        public string SaveArticle(int id, List<Tags> tags, int auto = 0)
        {
            var article = db.Article.SingleOrDefault(s => s.ArticleId == id);

            if (auto == 1)
            {
                article.Save = 1;
                db.SaveChanges();
            }
            else
            {
                article.ModifedDate = DateTime.Now;
            }

            if (tags != null)
            {
                //Добавление тэгов
                tags.RemoveAt(0);
                int t = 1;
                foreach (var tag in tags)
                {
                    db.ArticleTag.Add(new ArticleTag()
                    {
                        ArticleId = id,
                        TagId = tag.TagId,
                        Order = t,
                        Type = 1,

                    });
                    db.SaveChanges();
                    t++;
                }
            }

            return "Ok";
        }

        [HttpPost]
        public JsonResult ShowAllArticle(int id)
        {
            var allUserThesis = db.UserThesises.ToList();
            var blocks = db.ArticleBlock.Where(w => w.ArticleId == id).OrderBy(o => o.Order).ToList();

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
                    Video = item.Video,
                    ViewThesis = viewThesis
                });

            }


            return Json(new { viewArticleBlock }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string DelArticle(int id)
        {
            var article = db.Article.SingleOrDefault(s => s.ArticleId == id);
            db.Article.Remove(article);
            db.SaveChanges();

            var blocks = db.ArticleBlock.Where(w => w.ArticleId == id).ToList();
            db.ArticleBlock.RemoveRange(blocks);
            db.SaveChanges();

            return "Ok";
        }

        [HttpPost]
        public int AddBlockText(int id, int type, int order)
        {
            var appUserId = User.Identity.GetUserId();

            var block = db.ArticleBlock.Add(new ArticleBlock() { Text_ = "Введите текст", ArticleId = id, Type = type, CreateDate = DateTime.Now,
             ModifedDate = DateTime.Now, Order = order,
            });

            db.SaveChanges();

            var thisBlocks = db.ArticleBlock.Where(w => w.ArticleId == id).ToList();



            return block.ArticleBlockId;
        }

        [HttpPost]
        public JsonResult AddBlockTextThesis(int id, int type, int order)
        {
            var appUserId = User.Identity.GetUserId();

            var thesis = db.Thesis.Add(new Thesis()
            {
                Description = "Напишите тезис",
                CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now,
            });
            db.SaveChanges();

            var block = db.ArticleBlock.Add(new ArticleBlock()
            {
                Text_ = "Введите текст",
                ArticleId = id,
                Type = type,
                CreateDate = DateTime.Now,
                ModifedDate = DateTime.Now,
                Order = order, ThesisId = thesis.ThesisId,
            });

            db.SaveChanges();

            ViewThesis viewThesis = new ViewThesis() { Thesis = thesis};

            ViewArticleBlock viewArticleBlock = new ViewArticleBlock() { ArticleBlockId = block.ArticleBlockId, ViewThesis = viewThesis, Order = order,
                Type = type };

            return Json(new { viewArticleBlock }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int EditBlockText(int id, int type, string text)
        {
            var appUserId = User.Identity.GetUserId();

            if (type == 2) { }

            var block = db.ArticleBlock.SingleOrDefault(s => s.ArticleBlockId == id);
            block.Text_ = text;

            db.SaveChanges();

            return block.ArticleBlockId;
        }

        [HttpPost]
        public string DelBlock(int id)
        {
            var block = db.ArticleBlock.SingleOrDefault(s => s.ArticleBlockId == id);

            db.ArticleBlock.Remove(block);

            db.SaveChanges();

            return "Ok";
        }

        [HttpPost]
        public string ChangeTypeBlock(int id, int type)
        {
            var appUserId = User.Identity.GetUserId();

            var block = db.ArticleBlock.SingleOrDefault(s => s.ArticleBlockId == id);
            block.Type = type;

            db.SaveChanges();


            return "Ok";
        }

        [HttpPost]
        public string ChangeOrder( List <ViewArticleBlock> viewArticleBlock)
        {
            foreach (var item in viewArticleBlock)
            {
                var thisBlock = db.ArticleBlock.SingleOrDefault(s=> s.ArticleBlockId == item.ArticleBlockId);
                thisBlock.Order = item.Order;
                db.SaveChanges();

            }


            return "Ok";
        }

        #endregion

        #region Tree
        [HttpPost]
        public string SaveTree(int id, string title, int type)
        {
            if (type == 1) {
                var book = db.Book.SingleOrDefault(s => s.BookId == id);
                book.Title = title;
                book.ModifedDate = DateTime.Now;
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if(type == 2)
            {
                var art = db.Article.SingleOrDefault(s => s.ArticleId== id);
                art.Title = title;
                art.ModifedDate = DateTime.Now;
                db.Entry(art).State = EntityState.Modified;
                db.SaveChanges();
            }

            return "Название сохранено";
        }

        #endregion

    }


    public static class Cc
    {
        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}

