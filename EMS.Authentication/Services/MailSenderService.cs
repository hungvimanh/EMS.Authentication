﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TF.Authentication.Entities;

namespace TF.Authentication.Commons
{
    public class MailSenderService
    {
        private User user;
        public MailSenderService(User _user)
        {
            user = _user;
        }
        public void RecoveryPasswordMail()
        {
            if (string.IsNullOrEmpty(user.Email)) return;
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
    }
}
