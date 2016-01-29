using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twilio;
using System.Configuration;
using System.Net.Mail;

    public class SmsMgr
    {
        public class SmsResult
        {
            public bool IsSent { get; set; }
            public string Sid { get; set; }
            public string AccountSid { get; set; }
            public string ExceptionMsg { get; set; }
            public string Status { get; set; }
        }

        static readonly SmsMgr _Instance = new SmsMgr();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SmsMgr()
        {
        }

        SmsMgr()
        {
        }

        public static SmsMgr Instance
        {
            get
            {   
                return _Instance;
            }
        }


        private TwilioRestClient _SmsClient = null;

        private TwilioRestClient SmsClient
        {
            get
            {
                if (_SmsClient == null)
                {
                    string acctSid = ConfigurationManager.AppSettings["twilio-acct-sid"];
                    string authToken = ConfigurationManager.AppSettings["twilio-auth-token"];

                    _SmsClient = new TwilioRestClient(acctSid, authToken);
                }

                return _SmsClient;
            }
        }
        //public SmsMgr.SmsResult SendSms(string to, string text, int objectId)
        //{
        //    SmsMgr.SmsResult result = null;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(to))
        //        {
        //            throw new Exception("A 'to' phone number was not specified");
        //        }
        //        if (string.IsNullOrEmpty(text))
        //        {
        //            throw new Exception("A blank text message cannot be sent");
        //        }

        //        result = new SmsMgr.SmsResult();

        //        string from = ConfigurationManager.AppSettings["twilio-from-no"];
               
        //        var msg = this.SmsClient.SendSmsMessage(from, to, text);

        //        result.AccountSid = msg.AccountSid;
        //        result.Sid = msg.Sid;
        //        result.Status = msg.Status;

        //        if (msg.RestException != null)
        //        {
        //            result.ExceptionMsg = msg.RestException.Message;
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        HeadSpin.App.Core.MGR.Instance.LogDebugMsg("error sending sms: " + e.Message);
        //    }

        //    return result;
        //}

        public SmsMgr.SmsResult SendSms(string to, string text, string fromPhoneNum = "")
        {
            SmsMgr.SmsResult result = null;

            try
            {
                if (string.IsNullOrEmpty(to))
                {
                    throw new Exception("A 'to' phone number was not specified");
                }
                if (string.IsNullOrEmpty(text))
                {
                    throw new Exception("A blank text message cannot be sent");
                }

                result = new SmsMgr.SmsResult();

                string from = fromPhoneNum;

                if (string.IsNullOrWhiteSpace(fromPhoneNum))
                {
                    from = ConfigurationManager.AppSettings["twilio-from-no"];
                }

                HeadSpin.App.Core.MGR.Instance.LogDebugMsg(string.Format("try send twillio client from {0} to {1} txt {2}", from, to, text));

                SMSMessage msg = null;

                Boolean sendTexts = false;
                var send = System.Configuration.ConfigurationManager.AppSettings["send-text"];

                if (send != null)
                {
                    sendTexts = Boolean.Parse(send);
                }

                if (sendTexts == true)
                {
                    msg = this.SmsClient.SendSmsMessage(from, to, text);

                    result.AccountSid = msg.AccountSid;
                    result.Sid = msg.Sid;
                    result.Status = msg.Status;

                    if (msg.RestException != null)
                    {
                        result.ExceptionMsg = msg.RestException.Message;
                    }

                    HeadSpin.App.Core.MGR.Instance.LogDebugMsg("result status " + result.Status);

                }
            }
            catch (Exception e)
            {
                HeadSpin.App.Core.MGR.Instance.LogException(e);
                throw;
            }

            return result;
        }

    }
