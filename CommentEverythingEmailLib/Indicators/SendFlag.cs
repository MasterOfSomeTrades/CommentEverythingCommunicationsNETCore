using System;
using System.Collections.Generic;
using System.Text;

namespace CommentEverythingEmailLib.Indicators {
    public class SendFlag {
        public SendFlag() {
            AppSettingName = "SEND_NOTIFICATIONS";
        }

        public enum SendOption {
            ALWAYS,
            IF_ENV_FLAG_ON
        }

        public string AppSettingName { get; set; }

        public bool ShouldSend() {
            string appSettingValue = Environment.GetEnvironmentVariable(AppSettingName);

            if (appSettingValue is null) { // will send out notification by default
                return true;
            } else {
                return bool.Parse(appSettingValue);
            }
        }
    }
}
