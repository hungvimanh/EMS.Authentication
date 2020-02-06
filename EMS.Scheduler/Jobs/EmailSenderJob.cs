using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TF.Authentication.Commons;
using TF.Authentication.Entities;

namespace EMS.Scheduler.Jobs
{
    public class EmailSenderJob : IJob
    {
        private User user;
        public EmailSenderJob(User _user)
        {
            user = _user;
        }
        public Task Execute(IJobExecutionContext context)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                string SendEmail = "12finalteam@gmail.com";
                string SendEmailPassword = "TF123456a@";

                var loginInfo = new NetworkCredential(SendEmail, SendEmailPassword);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);

                string body = "Mật khẩu của bạn đã được khôi phục!\n";
                body += "Password: " + user.Password;
                try
                {
                    msg.From = new MailAddress(SendEmail);
                    msg.To.Add(new MailAddress(user.Email));
                    msg.Subject = "Khôi phục mật khẩu Twelve Final!";
                    msg.Body = body;
                    msg.IsBodyHtml = true;

                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = loginInfo;
                    smtpClient.SendMailAsync(msg);
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("Error!");
                }
            }
            return Task.CompletedTask;
        }
    }
}
