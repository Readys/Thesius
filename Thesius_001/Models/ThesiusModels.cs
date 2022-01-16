using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

namespace Thesius_001.Models
{
    [Table("Thesis")]
    public class Thesis
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ThesisId { get; set; }
        public string Userid { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public string Links { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
        public int CategoryId { get; set; }
        public int FirstTagId { get; set; }
        public int Deleted { get; set; }
    }

    [Table("UserThesises")]
    public class UserThesises
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserThesisesId { get; set; }
        public string Userid { get; set; }
        public int ThesisId { get; set; }
        public bool Сonsidered { get; set; }
        public bool Follow { get; set; }
        public bool Trash { get; set; }
        public bool My { get; set; }
        public int MyMark { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
        public int Deleted { get; set; }
    }


    public partial class ViewThesis
    {
        public virtual Thesis Thesis { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Tags> Tags { get; set; }
        public List<Links> Links { get; set; }
        public bool My { get; set; }
        public int CountPlus { get; set; }
        public int CountMinus { get; set; }
        public int CountDnotNow { get; set; }
        public double Procent { get; set; }

        public int MyMark { get; set; }
        public bool ShowOther { get; set; }
    }

    public partial class ViewCategoryThesis
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ViewThesis> ViewThesis { get; set; }
        public bool OpenClose { get; set; }
    }

    public partial class ViewConsensus
    {
        public int ConsensusId { get; set; }
        public string ConsensusName { get; set; }
        public int ConsensusValueMax { get; set; }
        public int ConsensusValueMin { get; set; }
        public List<ViewThesis> ViewThesis { get; set; }
    }

    //[Table("Category")]
    //public class Category
    //{
    //    [Key]
    //    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    //    public int CategoryId { get; set; }
    //    public string Userid { get; set; }
    //    [MaxLength(80)]
    //    public string Name { get; set; }
    //    [MaxLength(512)]
    //    public string Description { get; set; }
    //    public DateTime CreateDate { get; set; }
    //    public DateTime ModifedDate { get; set; }
    //    public int Deleted { get; set; }
    //}

    [Table("Tags")]
    public class Tags
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }
        public string Userid { get; set; }
        [MaxLength(80)]
        //[JsonProperty(PropertyName = "text")]
        public string text { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public int Type { get; set; } //Tag type = 1 Category type = 0

        public int Deleted { get; set; }
    }

    [Table("ThesisTags")]
    public class ThesisTags
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ThesisTagsId { get; set; }
        public int ThesisId { get; set; }
        public int TagId { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
    }

    [Table("Links")]
    public class Links
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LinkId { get; set; }
        public string Userid { get; set; }
        [MaxLength(2048)]
        public string Name { get; set; }
        [MaxLength(2048)]
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
        public int Deleted { get; set; }
    }

    [Table("ThesisLinks")]
    public class ThesisLinks
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ThesisLinkId { get; set; }
        public int ThesisId { get; set; }
        public int LinkId { get; set; }
        public int Order { get; set; }
    }

    [Table("InviteCode")]
    public class InviteCode
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InviteCodeId { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public int Type { get; set; }
        public bool Used { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    [Table("InviteUser")]
    public class InviteUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InviteCodeId { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
        public string InviterUserId { get; set; }
        public bool Closed { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    [Table("Filter")]
    public class Filter
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InviteCodeId { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
        public string InviterUserId { get; set; }
        public bool Closed { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    public class ViewInviteUser
    {
        public int InviteCodeId { get; set; }
        public string Code { get; set; }
        public string Userid { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }

    }

    #region Book

    [Table("Book")]
    public class Book
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        public int ParentId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    public partial class ViewBook
    {
        public int BookId { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }

        public List<ViewBook> children { get; set; }
        public List<ViewArticles> ViewArticles { get; set; }
    }

    [Table("Article")]
    public class Article
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }
        public int MainThesisId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public int Save { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    public partial class ViewArticles
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Text_ { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }

        public int ShortView { get; set; }

        public List<ViewArticleBlock> ViewArticleBlock { get; set; }
    }

    [Table("ArticleBlock")]
    public class ArticleBlock
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ArticleBlockId { get; set; }
        public int ArticleId { get; set; }
        public int Order { get; set; }
        public string UserId { get; set; }
        public string Text_ { get; set; }
        public int ThesisId { get; set; }
        public string ImgPath { get; set; }
        public string Video { get; set; }
        public int Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    public partial class ViewArticleBlock
    {
        public int ArticleBlockId { get; set; }
        public int ArticleId { get; set; }
        public int Order { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Text_ { get; set; }

        public string ImgPath { get; set; }
        public string Video { get; set; }
        public int Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }

        public ViewThesis ViewThesis { get; set; }
    }

    [Table("ArticleTag")]
    public class ArticleTag
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ArticleTagId { get; set; }
        public int ArticleId { get; set; }
        public string UserId { get; set; }
        public int TagId { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
    }




    [Table("BookArticle")]
    public class BookArticle
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BookArticleId { get; set; }
        public int BookId { get; set; }
        public int ArticleId { get; set; }
        public int Order { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    [Table("Text")]
    public class Text
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TextId { get; set; }
        public int ArticleId { get; set; }
        public string UserId { get; set; }
        public string Text_ { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }

    [Table("ArticleText")]
    public class ArticleText
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ArticleTextId { get; set; }
        public int ArticleId { get; set; }
        public int TextId { get; set; }
        public int ThesisId { get; set; }
        public int Order { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifedDate { get; set; }
    }



    #endregion

    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IEnumerable<TreeItem<T>> children { get; set; }
    }
}