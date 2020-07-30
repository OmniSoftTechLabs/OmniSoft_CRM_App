using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OmniCRM_Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.GenericClasses
{
    public static class DailyEmailService
    {
        private static OmniCRMContext _context;
        public static IHostingEnvironment _hostingEnvironment;
        public static IConfiguration _configuration;
        private static Timer timer;


        //public static DailyEmailService(OmniCRMContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        //{
        //    _context = context;
        //    _hostingEnvironment = hostingEnvironment;
        //    _configuration = configuration;
        //    timer = new Timer();
        //    timer.Interval = GetNextInterval();
        //    timer.Elapsed += Timer_Elapsed;
        //}

        public static void StartTimer()
        {
            GenericMethods.Log(LogType.ActivityLog.ToString(), "DailyEmailService StartTimer");
            timer = new Timer();
            _context = new OmniCRMContext();
            timer.Interval = GetNextInterval();
            timer.Start();
            timer.Elapsed += Timer_Elapsed;

        }

        private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SendEmailDailyAppointmentToRM();
            SetTimer();
        }

        public static void SetTimer()
        {
            try
            {
                GenericMethods.Log(LogType.ActivityLog.ToString(), "DailyEmailService SetTimer");
                timer.Stop();
                //System.Threading.Thread.Sleep(300000);
                double inter = (double)GetNextInterval();
                timer.Interval = inter;
                timer.Start();

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "Error : " + ex.Message);
            }
        }

        private static double GetNextInterval()
        {
            GenericMethods.Log(LogType.ActivityLog.ToString(), "DailyEmailService GetNextInterval");
            if (_context.AdminSetting.Count() > 0)
            {
                int lastSettingId = _context.AdminSetting.Max(p => p.SettingId);
                var adminSetting = _context.AdminSetting.Find(lastSettingId);
                var Time = Convert.ToDateTime(adminSetting.DailyEmailTime).TimeOfDay.ToString();

                DateTime dateTime = DateTime.Parse(Time);
                TimeSpan emailTime = new TimeSpan();

                emailTime = dateTime - DateTime.Now;
                if (emailTime.TotalMilliseconds < 0)
                {
                    emailTime = dateTime.AddDays(1) - DateTime.Now;
                }
                return emailTime.TotalMilliseconds;
            }
            return 3600000;
        }


        public static async Task SendEmailDailyAppointmentToRM()
        {
            try
            {
                GenericMethods.Log(LogType.ActivityLog.ToString(), "DailyEmailService SendEmailDailyAppointmentToRM");

                List<AppointmentDetail> appointListToday = new List<AppointmentDetail>();

                appointListToday = await _context.AppointmentDetail.Where(p => p.AppointmentDateTime != null).ToListAsync();
                appointListToday = appointListToday.Where(p => Convert.ToDateTime(p.AppointmentDateTime).Date == DateTime.Now.Date).ToList();
                List<UserMaster> managerList = await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.RelationshipManager).ToListAsync();
                managerList = managerList.Where(p => appointListToday.Any(r => r.RelationshipManagerId == p.UserId)).ToList();

                if (managerList.Count > 0)
                    foreach (var item in managerList)
                    {
                        var managerAppoints = appointListToday.Where(p => p.RelationshipManagerId == item.UserId).ToList();
                        var callDetail = _context.CallDetail.ToList().Where(p => managerAppoints.Any(r => r.CallId == p.CallId));

                        var callDetailView = (from call in callDetail
                                              join appoin in managerAppoints on call.CallId equals appoin.CallId
                                              select new
                                              {
                                                  call.CallId,
                                                  call.FirstName,
                                                  call.LastName,
                                                  call.MobileNumber,
                                                  call.Address,
                                                  AppointTime = appoin.AppointmentDateTime.Value.ToString("hh:mm tt"),
                                                  CreatedByName = _context.UserMaster.ToList().FirstOrDefault(p => p.UserId == appoin.CreatedBy).FirstName,
                                                  CreatedDate = appoin.CreatedDate.ToShortDateString(),
                                                  appoin.Remarks
                                              }).ToList();


                        string FilePath = _hostingEnvironment.ContentRootPath + "//HTMLTemplate//DailyAppointRM.html";
                        //string FilePath = ConfigurationManager.AppSettings["HTMLPath"] + "//DailyAppointRM.html";
                        //string domainName = ConfigurationManager.AppSettings["DomainName"];
                        //string domainName = _configuration["TokenSettings:Client_URL"].ToString();
                        string domainName = _configuration.GetSection("Domains").GetSection("CurrentDomain").Value;
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
                            sb.Append("<td><a href='" + domainName + "/lead-followup/" + row.CallId + "'>" + row.FirstName + " " + row.LastName + "</a></td>");
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

                        GenericMethods.Log(LogType.ActivityLog.ToString(), "Sending email to : " + item.Email);
                        GenericMethods.SendEmailNotification(item.Email, "OmniCRM: Today's Appointments (" + DateTime.Now.ToString("dd-MMM-yyyy") + ")", MailText);
                    }
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "SendEmailDailyAppointmentToRM: " + ex.ToString());
            }
        }

        public static void RestartEmailService()
        {
            string serviceName = _configuration.GetSection("EmailService").GetSection("ServiceName").Value;
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(1000);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(1000 - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch(Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "RestartEmailService: " + ex.ToString());
            }
        }
    }
}
