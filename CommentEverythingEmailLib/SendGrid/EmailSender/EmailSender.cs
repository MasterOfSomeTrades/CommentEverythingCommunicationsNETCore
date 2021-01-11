using CommentEverythingCryptographyNETCore.Encryption;
using CommentEverythingEmailLib.Indicators;
using CommentEverythingEmailLib.SendGrid.User;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommentEverythingEmailLib.SendGrid.EmailSender {
    public class EmailSender {
        public EmailSender() {
            MaxSend = 20;
            CharLimit = 1000000;
        }

        public int MaxSend { get; set; }
        public int CharLimit { get; set; }
        private int _numberSent = 0;
        public SendFlag SendFlag = new SendFlag();
        public UserProfile User = new UserProfile();

        public Task SendEmail(string subject, string msg) {
            Task ret = Task.CompletedTask;

            if (SendFlag.ShouldSend()) {
                IEncryptionProvider decryptor = EncryptionProviderFactory.CreateInstance(EncryptionProviderFactory.CryptographyMethod.AES);

                SendGridClient sg = new SendGridClient(decryptor.Decrypt(User.APIKeyCipher));
                EmailAddress from = new EmailAddress(decryptor.Decrypt(User.FromEmailCipher));
                EmailAddress admin = new EmailAddress(decryptor.Decrypt(User.AdminEmailCipher));

                foreach (string addrCipher in User.RecipientListCiphers) {
                    EmailAddress to = new EmailAddress(decryptor.Decrypt(addrCipher));

                    if (_numberSent < MaxSend) {
                        if (msg.Length <= CharLimit) {
                            Content content = new Content("text/plain", msg);
                            SendGridMessage mail = new SendGridMessage();
                            mail.From = from;
                            mail.Subject = subject;
                            mail.AddTo(to);
                            mail.HtmlContent = msg;
                            mail.PlainTextContent = msg;
                            Task sendTask = sg.SendEmailAsync(mail);
                            ret = Task.WhenAll(new Task[] { sendTask });
                        } else {
                            subject = "Undeliverable Mail";
                            Content content = new Content("text/plain", "Email not delivered to " + decryptor.Decrypt(addrCipher) + ". Email message length greater than maximum number of characters of " + CharLimit.ToString());
                            SendGridMessage mail = new SendGridMessage();
                            mail.From = from;
                            mail.Subject = subject;
                            mail.AddTo(admin);
                            mail.HtmlContent = content.Value;
                            mail.PlainTextContent = content.Value;
                            Task sendTask = sg.SendEmailAsync(mail);
                            ret = Task.WhenAll(new Task[] { sendTask });
                        }
                    } else if (_numberSent == MaxSend) {
                        subject = "Undeliverable Mail";
                        Content content = new Content("text/plain", "Email not delivered to " + decryptor.Decrypt(addrCipher) + ". Number of email messages sent in specified time period has reached the limit of " + MaxSend.ToString());
                        SendGridMessage mail = new SendGridMessage();
                        mail.From = from;
                        mail.Subject = subject;
                        mail.AddTo(admin);
                        mail.HtmlContent = content.Value;
                        mail.PlainTextContent = content.Value;
                        Task sendTask = sg.SendEmailAsync(mail);
                        ret = Task.WhenAll(new Task[] { sendTask });
                    }
                    _numberSent++;
                }
            }

            return ret;
        }

        // TODO: Counter reset as function of this class using time
        // TODO: Count message segments in counter (e.g. using message length)
        public void ResetCounter() {
            _numberSent = 0;
        }
    }
}
