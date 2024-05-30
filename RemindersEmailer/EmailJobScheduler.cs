using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using NLog;
//using NLog.Extensions.Logging;
using MimeKit;
using RemindersEmailer.Data;
using RemindersEmailer.LoggerService;
using static System.Formats.Asn1.AsnWriter;

namespace RemindersEmailer
{
    public class EmailJobScheduler: BackgroundService

    {
        private readonly ILoggerManager _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public EmailJobScheduler(IConfiguration configuration, ILoggerManager logger, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _scopeFactory = serviceScopeFactory;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

                while (!stoppingToken.IsCancellationRequested)
                {
                    int interval = new int();

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ReminderModuleDbContext>();
                        _logger.LogInfo("Got dbcontext inside Emailjobscheduler");

                        var pendingReminders = await dbContext.Reminder
                            .Where(e => !e.EmailSent && e.ReminderDateTime <= DateTime.Now)
                            .ToListAsync();
                        var emailsetup = dbContext.Emailsetup.FirstOrDefault();
                        
                        if (emailsetup != null && pendingReminders != null)
                        {
                            interval = emailsetup.Interval;

                            _logger.LogInfo($"processing are {pendingReminders.Count} alerts at {DateTime.Now}");

                            foreach (var reminder in pendingReminders)
                            {
                                try
                                {
                                    await SendEmailAsync(emailsetup);
                                    reminder.EmailSent = true;
                                    await dbContext.SaveChangesAsync();
                                    _logger.LogInfo($"Email alert sent to {emailsetup.EmailTo} and alert was : {reminder.Title} @ {reminder.ReminderDateTime}");

                                }
                                catch (Exception ex)
                                {
                                    //_logger.LogError($"Failed to send email to {email.To}: {ex.Message}");
                                    _logger.LogError($"Failed to send email alert for {reminder.Title} : {ex.Message}");
                                }
                            }
                        }
                        _logger.LogInfo("Checking for pending alerts...");
                
                    }                
                    await Task.Delay(TimeSpan.FromMinutes(interval) , stoppingToken); // Check every 30 seconds
                }
        }

        private async Task SendEmailAsync(EmailModel email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(email.SenderName, email.EmailFrom));
            message.To.Add(MailboxAddress.Parse(email.EmailTo));
            message.Subject = email.Subject;
            message.Body = new TextPart("plain")
            {
                Text = email.Body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(email.EmailHost, email.Port, email.EnableSSL);
                await client.AuthenticateAsync(email.HUName, email.HUPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
