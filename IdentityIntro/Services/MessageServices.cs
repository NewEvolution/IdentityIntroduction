using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace IdentityIntro.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(email);
            myMessage.From = new System.Net.Mail.MailAddress("ryantanay@newevolution.org", "Ryan Tanay");
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;
            var credentials = new System.Net.NetworkCredential(
                Options.SendGridUser,
                Options.SendGridKey);
            // Create web transport for sending email
            var transportWeb = new SendGrid.Web(credentials);
            return transportWeb.DeliverAsync(myMessage);
        }

        public async Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init(Options.SID, Options.AuthToken);
            var restClient = new TwilioRestClient(Options.SID, Options.AuthToken);

            var sms = await
                new MessageCreator(
                    Options.SID,
                    new PhoneNumber(number),
                    new PhoneNumber(Options.SendNumber),
                    message
                ).ExecuteAsync(restClient);
        }
    }
}
