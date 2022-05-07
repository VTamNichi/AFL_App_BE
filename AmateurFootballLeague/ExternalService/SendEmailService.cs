
using AmateurFootballLeague.ViewModels.Requests;
using System.Net;
using System.Net.Mail;

namespace AmateurFootballLeague.ExternalService
{
    public interface ISendEmailService
    {
        public Task<bool> SendEmail(EmailForm model);
    }
    public class SendEmailService : ISendEmailService
    {
        public async Task<bool> SendEmail(EmailForm model)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "in-v3.mailjet.com";
            client.Port = 587;

            NetworkCredential credentials =
                new NetworkCredential("6c66e5d6bcdadebccbc50630514264b6", "cdcd97d3e12bcace128be5cf6336b11e");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("mailclone@gmail.com", "A-Football-League");
            msg.To.Add(new MailAddress(model.ToEmail));
            msg.Priority = MailPriority.High;

            msg.Subject = model.Subject;
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body><h3>" + model.Message + "</h3></body>");

            try
            {
                await client.SendMailAsync(msg);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return false;

        }
    }
}