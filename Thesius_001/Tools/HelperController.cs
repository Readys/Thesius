using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Thesius_001.Models;

namespace Thesius_001.Tools
{

    public class Helper
    {
        const string Digits = "0123456789";
        const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        const string Symbols = "@#$&";

        public static void SendMail(string from, List<string> toList, string subject, string messageText)
        {
            //// наш email с заголовком письма
            //MailAddress from2 = new MailAddress("admin@thesius.ru", "Web Registration");
            //// кому отправляем
            ////MailAddress to = new MailAddress(user.Email);
            //// создаем объект сообщения
            //MailMessage m = new MailMessage("admin@thesius.ru", "armyideas@gmail.com"); //from, to
            //// тема письма
            //m.Subject = "Thesius: Подтверждение Email";
            //// текст письма - включаем в него ссылку
            //m.Body = "test";
            //m.IsBodyHtml = true;
            //// адрес smtp-сервера, с которого мы и будем отправлять письмо
            //SmtpClient smtp = new SmtpClient("mail.hosting.reg.ru", 25);
            //// логин и пароль
            //smtp.Credentials = new System.Net.NetworkCredential("admin@thesius.ru", "6xjJZRymgmJLpF9");
            //smtp.Send(m);


            var configMail = ConfigurationManager.GetSection("MailBox") as NameValueCollection;

            if (from == "")
            {
                from = configMail["From"];
            }

            var msg = new MailMessage { From = new MailAddress(from) };

            //var to = "armyideas@gmail.com";
            var to = toList.FirstOrDefault();
            msg.To.Add(new MailAddress(to));
            msg.Subject = subject;

            foreach (var item in toList)
            {
                if (to != item.Trim())
                {
                    msg.To.Add(new MailAddress(item.Trim()));
                    //msg.CC.Add(new MailAddress(item.Trim()));
                }
            }
            msg.Body = messageText;
            msg.Priority = MailPriority.High;
            msg.IsBodyHtml = true;

            if (configMail != null)
            {
                var smtpClient = new SmtpClient(configMail["SMTP"], Int32.Parse(configMail["SMTPPort"])) { EnableSsl = false };
                smtpClient.Credentials = new System.Net.NetworkCredential("admin@thesius.ru", "6xjJZRymgmJLpF9");

                try
                {
                    smtpClient.Send(msg);
                }
                catch (Exception ex)
                {
                    var cc = ex.Message;
                }
            }


        }

        [Flags]
        public enum PasswordChars
        {
            Digits = 0b0001,
            Alphabet = 0b0010,
            Symbols = 0b0100
        }

        public static string GeneratePassword(PasswordChars passwordChars, int length = 16)
        {
            var random = new Random();
            var resultPassword = new StringBuilder(length);
            var passwordCharSet = string.Empty;
            if (passwordChars.HasFlag(PasswordChars.Alphabet))
            {
                passwordCharSet += Alphabet + Alphabet.ToUpper();
            }
            if (!passwordChars.HasFlag(PasswordChars.Digits))
            {
                passwordCharSet += Digits;
            }
            if (!passwordChars.HasFlag(PasswordChars.Symbols))
            {
                passwordCharSet += Symbols;
            }
            for (var i = 0; i < length; i++)
            {
                resultPassword.Append(passwordCharSet[random.Next(0, passwordCharSet.Length)]);
            }
            return resultPassword.ToString();
        }

        public static void SaveInvite(string userId, string inviteCode)
        {
            using (var db = new ApplicationDbContext())
            {
                var findThisCode = db.InviteCode.SingleOrDefault(s=> s.Code == inviteCode && s.Used == false && s.Type == 0);

                if (findThisCode != null)
                {
                    findThisCode.Used = true;
                    findThisCode.ModifedDate = DateTime.Now;

                    db.InviteUser.Add(new InviteUser()
                    {
                        UserId = userId,
                        Code = inviteCode,
                        Closed = true,
                        InviterUserId = findThisCode.UserId,
                        CreateDate = DateTime.Now,
                        ModifedDate = DateTime.Now,
                    });
                }

                db.SaveChanges();

            }
        }

        public static bool CheckInvite(string inviteCode)
        {
            using (var db = new ApplicationDbContext())
            {
                var findThisCode = db.InviteCode.SingleOrDefault(s => s.Code == inviteCode && s.Used == false && s.Type == 0);
                if (findThisCode != null) {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }



    }

}