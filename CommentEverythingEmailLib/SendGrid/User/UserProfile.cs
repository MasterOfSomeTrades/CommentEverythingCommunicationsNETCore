using System;
using System.Collections.Generic;
using System.Text;

namespace CommentEverythingEmailLib.SendGrid.User {
    public class UserProfile {
        public UserProfile() {
            FromEmailCipher = DEFAULT_FROM_EMAIL_CIPHER;
            AdminEmailCipher = DEFAULT_ADMIN_EMAIL_CIPHER;
        }

        private string API_KEY_CIPHER = Environment.GetEnvironmentVariable("sendgrid");

        private string DEFAULT_FROM_EMAIL_CIPHER = Environment.GetEnvironmentVariable("DEFAULT_FROM_EMAIL");

        private string DEFAULT_ADMIN_EMAIL_CIPHER = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_EMAIL");

        public string APIKeyCipher {
            get {
                if (API_KEY_CIPHER is null) {
                    throw new ApplicationException("API_KEY_CIPHER value not found");
                }

                return API_KEY_CIPHER;
            }
        }
        public string FromEmailCipher { get; set; }
        public string AdminEmailCipher { get; set; }


        public List<string> RecipientListCiphers = new List<string>(new string[] {  });

        public void AddToRecipientCiphers(string recipientEmailCipher) {
            RecipientListCiphers.Add(recipientEmailCipher);
        }
    }
}
