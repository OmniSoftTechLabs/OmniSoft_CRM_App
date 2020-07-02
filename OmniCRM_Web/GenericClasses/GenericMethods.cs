using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.GenericClasses
{
    public class GenericMethods
    {
        public static TimeZoneInfo Indian_Zone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public static IHostingEnvironment _hostingEnvironment;


        public static void Log(string logType, string logMessage)
        {
            try
            {
                if (!Directory.Exists(_hostingEnvironment.ContentRootPath + "//OmniCRMLogs//" + logType))
                {
                    Directory.CreateDirectory(_hostingEnvironment.ContentRootPath + "//OmniCRMLogs//" + logType);
                }

                string logPath = _hostingEnvironment.ContentRootPath + "//OmniCRMLogs//" + logType;
                //if (!File.Exists(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt"))
                //{
                //    File.Create(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt");
                //}


                FileStream fs = new FileStream(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt", FileMode.OpenOrCreate);
                StreamWriter str = new StreamWriter(fs);
                str.BaseStream.Seek(0, SeekOrigin.End);
                str.Write("\r\n=============== Log Entry : " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + " ===============\r\n");
                //str.WriteLine(DateTime.Now.ToLongTimeString() + " " +
                //              DateTime.Now.ToLongDateString());
                string addtext = logMessage + Environment.NewLine;
                str.Flush();
                str.Close();
                fs.Close();


                File.AppendAllText(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt", addtext);


                //using (StreamWriter w = File.AppendText(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt"))
                ////using (StreamWriter w = new StreamWriter(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt", true))
                //{
                //    w.Write("\r\nLog Entry : ");
                //    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                //    //w.WriteLine("  Error Message :");
                //    w.WriteLine($" {logMessage}");
                //    w.WriteLine("==============================================================");
                //}
            }
            catch (Exception ex)
            {
                // throw;
            }
        }

        public static string GenerateSalt()
        {
            var saltBytes = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            return salt;

        }

        public static HashSalt GenerateHashSalt(string password)
        {
            var saltBytes = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            HashSalt hashSalt = new HashSalt { hashPassword = hashPassword, saltPassword = salt };
            //string hashSalt = hashPassword;
            return hashSalt;
        }

        public class HashSalt
        {
            public string hashPassword;
            public string saltPassword;
        }
        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        public static void SendEmailNotification(string toEmailId, string subject, string messageBody)
        {
            SmtpClient smtpClient = new SmtpClient("mail.ostechlabs.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential("admin@ostechlabs.com", "admin@123");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mailMessage = new MailMessage("admin@ostechlabs.com", toEmailId);
            mailMessage.Subject = subject;
            mailMessage.Body = messageBody;
            mailMessage.BodyEncoding = Encoding.ASCII;
            mailMessage.IsBodyHtml = true;
            try
            {
                smtpClient.Send(mailMessage);
                GenericMethods.Log(LogType.ActivityLog.ToString(), "SendEmailNotification: " + "Email send successfully");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "SendEmailNotification: " + ex.ToString());
            }

        }
    }
}
