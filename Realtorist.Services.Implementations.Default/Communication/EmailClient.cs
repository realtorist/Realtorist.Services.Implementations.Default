using Microsoft.Extensions.Logging;
using Realtorist.DataAccess.Abstractions;
using Realtorist.Models.Helpers;
using Realtorist.Models.Listings;
using Realtorist.Models.Settings;
using Realtorist.Services.Abstractions.Communication;
using Realtorist.Services.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Realtorist.Services.Implementations.Default.Communication
{
    /// <summary>
    /// Describes basic email client
    /// </summary>
    public class EmailClient : IEmailClient
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates new instance of <see cref="EmailClient"/>
        /// </summary>
        /// <param name="settingsProvider">Settings provider</param>
        /// <param name="encryptionProvider">Encryption provider</param>
        /// <param name="logger">Loggerr</param>
        public EmailClient(ISettingsProvider settingsProvider, IEncryptionProvider encryptionProvider, ILogger<EmailClient> logger)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _encryptionProvider = encryptionProvider;
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync(MailMessage message)
        {
            var settings = await _settingsProvider.GetSettingAsync<SmtpSettings>(SettingTypes.Smtp);
            await SendEmailAsync(message, settings);
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync(MailMessage message, SmtpSettings smtpSettings)
        {
            var password = _encryptionProvider.Decrypt(smtpSettings.Password);

            using (var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(smtpSettings.Username, password),
                EnableSsl = smtpSettings.EnableSsl                
            })
            {
                await client.SendMailAsync(message);

                _logger.LogInformation($"Sent email to '{message.To.Select(x => x.Address).Join(", ")}' with subject '{message.Subject}'");
            }
        }
    }
}
