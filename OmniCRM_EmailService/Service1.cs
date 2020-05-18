using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OmniCRM_EmailService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        OmniCRMEntities _context;
        private double interval;
        public Service1()
        {
            InitializeComponent();
            _context = new OmniCRMEntities();
            timer = new Timer();
            timer.Interval = GetNextInterval();
            //CreateSendEmailBody();
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //===================================================================

            CreateSendEmailBody();

            //===================================================================

            timer.Stop();
            System.Threading.Thread.Sleep(1000000);
            SetTimer();
        }

        private void SetTimer()
        {
            try
            {
                double inter = (double)GetNextInterval();
                timer.Interval = inter;
                timer.Start();
            }
            catch (Exception ex)
            {
                GenericMethods.Log("Error : " + ex.Message);
            }
        }
        private double GetNextInterval()
        {
            int lastSettingId = _context.AdminSettings.Max(p => p.SettingId);
            var adminSetting = _context.AdminSettings.Find(lastSettingId);
            var Time = Convert.ToDateTime(adminSetting.DailyEmailTime).TimeOfDay.ToString();

            DateTime dateTime = DateTime.Parse(Time);
            TimeSpan emailTime = new TimeSpan();

            emailTime = dateTime - DateTime.Now;
            if (emailTime.TotalMilliseconds < 0)
            {
                emailTime = dateTime.AddDays(1) - DateTime.Now;
            }
            return interval = emailTime.TotalMilliseconds;
        }
        protected override void OnStart(string[] args)
        {
            timer.AutoReset = true;
            timer.Enabled = true;
            GenericMethods.Log("----------OmniCRM Email Service Started----------");
        }

        protected override void OnStop()
        {
            timer.AutoReset = false;
            timer.Enabled = false;
            GenericMethods.Log("----------OmniCRM Email Service Stopped----------");
        }

        private void CreateSendEmailBody()
        {
            try
            {
                List<AppointmentDetail> appointListToday = new List<AppointmentDetail>();

                appointListToday = _context.AppointmentDetails.Where(p => p.AppointmentDateTime != null).ToList();
                appointListToday = appointListToday.Where(p => Convert.ToDateTime(p.AppointmentDateTime).Date == DateTime.Now.Date).ToList();
                List<UserMaster> managerList = _context.UserMasters.Where(p => p.Status == true && p.RoleId == 3).ToList();
                managerList = managerList.Where(p => appointListToday.Any(r => r.RelationshipManagerID == p.UserID)).ToList();

                if (managerList.Count > 0)
                    foreach (var item in managerList)
                    {
                        var managerAppoints = appointListToday.Where(p => p.RelationshipManagerID == item.UserID).ToList();
                        var callDetail = _context.CallDetails.ToList().Where(p => managerAppoints.Any(r => r.CallID == p.CallID));

                        var callDetailView = (from call in callDetail
                                              join appoin in managerAppoints on call.CallID equals appoin.CallID
                                              select new
                                              {
                                                  call.CallID,
                                                  call.FirstName,
                                                  call.LastName,
                                                  call.MobileNumber,
                                                  call.Address,
                                                  AppointTime = appoin.AppointmentDateTime.Value.ToString("HH:mm tt"),
                                                  CreatedByName = _context.UserMasters.ToList().FirstOrDefault(p => p.UserID == appoin.CreatedBy).FirstName,
                                                  CreatedDate = appoin.CreatedDate.ToShortDateString(),
                                                  appoin.Remarks
                                              }).ToList();


                        string FilePath = ConfigurationManager.AppSettings["HTMLPath"] + "//DailyAppointRM.html";
                        string domainName = ConfigurationManager.AppSettings["DomainName"];
                        StreamReader str = new StreamReader(FilePath);
                        string MailText = str.ReadToEnd();

                        MailText = MailText.Replace("#MANAGER#", item.FirstName + " " + item.LastName);
                        MailText = MailText.Replace("#DOMAINNAME#", domainName);

                        string strHtmlTable = "";

                        StringBuilder sb = new StringBuilder();
                        //Table start.
                        //sb.Append("<table cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>");
                        sb.Append("<table style='border-color: #84b5e1; margin-left: auto; margin-right: auto;' width='100%' border='1'>");
                        sb.Append("<tbody>");

                        //Adding HeaderRow.
                        sb.Append("<tr style='border-color: #84b5e1; background-color: #185f9e; text-align: center; color: #ffffff;'>");
                        //sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + "First Name" + "</th>");
                        sb.Append("<td><strong>#</strong></td>");
                        sb.Append("<td><strong>Client</strong></td>");
                        sb.Append("<td><strong>Appointment Time</strong></td>");
                        sb.Append("<td><strong>Mobile No.</strong></td>");
                        sb.Append("<td><strong>Address</strong></td>");
                        sb.Append("<td><strong>Remarks</strong></td>");
                        sb.Append("</tr>");

                        //Adding DataRow.
                        int cnt = 1;
                        foreach (var row in callDetailView)
                        {
                            if (cnt % 2 == 0)
                                sb.Append("<tr style='text-align: center; border-color: #84b5e1;'>");
                            else
                                sb.Append("<tr style='text-align: center; border-color: #84b5e1; background-color: #c5ddf3;'>");
                            sb.Append("<td>" + cnt + "</td>");
                            sb.Append("<td><a href='" + domainName + "/lead-followup/" + row.CallID + "'>" + row.FirstName + " " + row.LastName + "</a></td>");
                            sb.Append("<td>" + row.AppointTime + "</td>");
                            sb.Append("<td>" + row.MobileNumber + "</td>");
                            sb.Append("<td>" + row.Address + "</td>");
                            sb.Append("<td>" + row.Remarks + "</td>");
                            sb.Append("</tr>");
                            cnt++;
                        }

                        //Table end.
                        sb.Append("</table>");
                        strHtmlTable = sb.ToString();
                        MailText = MailText.Replace("#APPOINTMENTSTABLE#", strHtmlTable);

                        GenericMethods.Log("Sending email to : " + item.Email);
                        GenericMethods.SendEmailNotification(item.Email, "OmniCRM: Today's Appointments (" + DateTime.Now.ToString("dd-MMM-yyyy") + ")", MailText);
                    }
            }
            catch (Exception ex)
            {
                GenericMethods.Log("CreateSendEmailBody: " + ex.ToString());
            }
        }
    }
}
