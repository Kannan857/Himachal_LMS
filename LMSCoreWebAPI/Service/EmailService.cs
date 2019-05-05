using LMSCoreWebAPI.lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LMSCoreWebAPI.Service
{
    public class EmailService
    {
        public static void SendUserActivationEmail(string emailFrom, string emailPassword, User user)
        {
            SmtpClient smtp = GetSMTPClient(emailFrom, emailPassword);

            MailAddress from = new MailAddress(emailFrom);

            MailAddress to = new MailAddress(user.UserName);

            using (var mail = new MailMessage(from, to))
            {
                string body = "<b>***This is an automatically generated email, please do not reply ***" + "</b><br><br>"

                             + "<b>Congratulations &nbsp;&nbsp;" + user.UserName + "!!!<b><br><br>"

                             + "<b>Please use the link below to authenticate and complete your Profile<b><br><br>"

                             + "http://localhost:4000/user/activate?uniqueId=" + user.UniqueId + "<br><br><br>"

                             + "Regards" + "<br>"

                             + "LMS Team";


                mail.Subject = "LMS Account Activation Mail";

                mail.IsBodyHtml = true;

                mail.Body = body;

                smtp.Send(mail);
            }
        }

        private static SmtpClient GetSMTPClient(string emailFrom, string emailPassword)
        {
            var smtp = new SmtpClient("smtp.gmail.com");

            smtp.Host = "smtp.gmail.com";

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);

            smtp.Port = 587;
            return smtp;
        }

        public static void SendUserResetPasswordEmail(string emailFrom, string emailPassword, User user)
        { 
            SmtpClient smtp = GetSMTPClient(emailFrom, emailPassword);

            MailAddress from = new MailAddress(emailFrom);

            MailAddress to = new MailAddress(user.UserName);

            using (var mail = new MailMessage(from, to))
            {
                string body = "<b>***This is an automatically generated email, please do not reply ***" + "</b><br><br>"

                             + "<b>Please use the link below to reset you password<b><br><br>"

                             + "http://localhost:4000/user/resetpassword?token=" + user.ResetPasswordToken + "<br><br><br>"

                             + "Regards" + "<br>"

                             + "LMS Team";


                mail.Subject = "LMS Reset Password Mail";

                mail.IsBodyHtml = true;

                mail.Body = body;

                smtp.Send(mail);
            }
        }
    }
}
