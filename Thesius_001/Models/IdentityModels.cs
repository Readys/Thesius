using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Thesius_001.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string NickName { get; set; }
        public string InviteCode { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Thesis> Thesis { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Links> Links { get; set; }
        public DbSet<ThesisLinks> ThesisLinks { get; set; }
        public DbSet<ThesisTags> ThesisTags { get; set; }
        public DbSet<UserThesises> UserThesises { get; set; }
        public DbSet<InviteCode> InviteCode { get; set; }
        public DbSet<InviteUser> InviteUser { get; set; }
        public DbSet<Filter> Filter { get; set; }

        public DbSet<Book> Book { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<BookArticle> BookArticle { get; set; }
        public DbSet<Text> Text { get; set; }
        public DbSet<ArticleText> ArticleText { get; set; }
        public DbSet<ArticleBlock> ArticleBlock { get; set; }
        public DbSet<ArticleTag> ArticleTag { get; set; }

    }
}