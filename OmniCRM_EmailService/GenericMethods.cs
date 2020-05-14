using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OmniCRM_EmailService
{
    public class GenericMethods
    {
        public static void Log(string logMessage)
        {
            try
            {
                if (!Directory.Exists(@"C:\OmniCRMLogs\DailyEmailService"))
                {
                    Directory.CreateDirectory(@"C:\OmniCRMLogs\DailyEmailService");
                }

                string logPath = @"C:\OmniCRMLogs\DailyEmailService";
                if (!File.Exists(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt"))
                {
                    File.Create(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt");
                }

                using (StreamWriter w = File.AppendText(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt"))
                //using (StreamWriter w = new StreamWriter(logPath + "\\log_" + DateTime.Now.ToShortDateString() + ".txt", true))
                {
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    //w.WriteLine("  Error Message :");
                    w.WriteLine($" {logMessage}");
                    w.WriteLine("==============================================================");
                }
            }
            catch (Exception ex)
            {
                // throw;
            }
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
                GenericMethods.Log("SendEmailNotification: " + "Email send successfully");

            }
            catch (Exception ex)
            {
                GenericMethods.Log("SendEmailNotification: " + ex.ToString());
            }

        }
    }
}
