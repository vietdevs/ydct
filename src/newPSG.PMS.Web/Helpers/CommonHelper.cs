using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using newPSG.PMS.EntityDB;
using newPSG.PMS.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace newPSG.PMS.Web.Helpers
{
    public static class CommonHelper
    {
        public static string Template(string path)
        {
            string strTemplate = "";
            string strFile = System.Web.HttpContext.Current.Server.MapPath(path);
            if (File.Exists(strFile))
            {
                TextReader txtreader = new StreamReader(strFile);
                strTemplate = txtreader.ReadToEnd();
                txtreader.Close();
            }
            return strTemplate;
        }

        public static bool SendMail(string name, string subject, string content,
            string toMail)
        {
            bool rs = false;
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com"; //host name
                    smtp.Port = 587; //port number
                    smtp.EnableSsl = true; //whether your smtp server requires SSL
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("noreply.odaweb@gmail.com", "OdaWeb123@#");
                    smtp.Timeout = 20000;
                }
                MailAddress fromAddress = new MailAddress("noreply.odaweb@gmail.com", name);
                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Send(message);
                rs = true;
            }
            catch (Exception)
            {
                rs = false;
            }
            return rs;
        }

        private static Random rand = new Random();
        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateString(int size)
        {
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }
    }
}