using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Quartz;

namespace NetworkHairdressing.Jobs
{
    public class EmailSender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (MailMessage message = new MailMessage("admin@yandex.ru", "user@yandex.ru"))
            {
                message.Subject = "Новостная рассылка";
                message.Body = "Новости сайта: бла бла бла";
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.yandex.ru",
                    Port = 25,
                    Credentials = new NetworkCredential("admin@yandex.ru", "password")
                })
                {
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}